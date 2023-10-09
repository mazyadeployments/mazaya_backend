using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MMA.Documents.Domain.Helpers;
using MMA.WebApi.DataAccess.Extensions;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.AdnocTermsAndConditions;
using MMA.WebApi.Shared.Interfaces.ApplicationUsers;
using MMA.WebApi.Shared.Interfaces.Companies;
using MMA.WebApi.Shared.Interfaces.Document;
using MMA.WebApi.Shared.Interfaces.Domain.Document;
using MMA.WebApi.Shared.Interfaces.Image;
using MMA.WebApi.Shared.Interfaces.MailStorage;
using MMA.WebApi.Shared.Interfaces.Offers;
using MMA.WebApi.Shared.Interfaces.RoadshowDocuments;
using MMA.WebApi.Shared.Models.ApplicationUser;
using MMA.WebApi.Shared.Models.DefaultLocations;
using MMA.WebApi.Shared.Models.Document;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.Image;
using MMA.WebApi.Shared.Models.MailStorage;
using MMA.WebApi.Shared.Models.Roadshow;
using MMA.WebApi.Shared.Monads;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Declares;
using static MMA.WebApi.Shared.Enums.Document.DocumentDeclares;

namespace MMA.WebApi.Core.Services
{
    public class RoadshowService : IRoadshowService
    {
        private readonly IRoadshowRepository _roadshowRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IRoadshowDocumentRepository _roadshowDocumentRepository;
        private readonly IMailStorageRepository _mailStorageServiceRepository;
        private readonly IDocumentService _documentService;
        private readonly IImageUtilsService _imageUtilsService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IApplicationUserService _applicationUserService;
        private readonly IDocumentRepository _documentRepository;
        private readonly IConfiguration _configuration;
        private readonly IAdnocTermsAndConditionsRepository _adnocTermsAndConditionsRepository;
        private readonly IMailStorageService _mailStorageServiceService;

        public RoadshowService(
            IRoadshowRepository roadshowRepository,
            IRoadshowDocumentRepository roadshowDocumentRepository,
            IApplicationUserService applicationUserService,
            IDocumentService documentService,
            IImageUtilsService imageUtilsService,
            UserManager<ApplicationUser> userManager,
            IMailStorageRepository mailStorageServiceRepository,
            IDocumentRepository documentRepository,
            IConfiguration configuration,
            ICompanyRepository companyRepository,
            IAdnocTermsAndConditionsRepository adnocTermsAndConditionsRepository,
            IMailStorageService mailStorageServiceService
        )
        {
            _roadshowRepository = roadshowRepository;
            _roadshowDocumentRepository = roadshowDocumentRepository;
            _documentService = documentService;
            _imageUtilsService = imageUtilsService;
            _userManager = userManager;
            _applicationUserService = applicationUserService;
            _documentRepository = documentRepository;
            _configuration = configuration;
            _companyRepository = companyRepository;
            _mailStorageServiceRepository = mailStorageServiceRepository;
            _adnocTermsAndConditionsRepository = adnocTermsAndConditionsRepository;
            _mailStorageServiceService = mailStorageServiceService;
        }

        public async Task<IEnumerable<RoadshowModel>> GetRoadshows()
        {
            var roadshows = await _roadshowRepository.Get().ToListAsync();

            return roadshows;
        }

        public async Task<RoadshowModel> GetRoadshow(int id)
        {
            var roadshow = await _roadshowRepository.GetRoadshow(id);

            foreach (var comment in roadshow.Comments)
            {
                if (comment.CreatedOn.HasValue)
                {
                    comment.CreatedOn = DateTime.SpecifyKind(
                        comment.CreatedOn.Value,
                        DateTimeKind.Utc
                    );
                }
            }

            if (roadshow.Status == RoadshowStatus.Submitted)
            {
                var roadshowTermsAndCondition =
                    await _adnocTermsAndConditionsRepository.GetAdnocTermsAndConditions(
                        AdnocTermsAndConditionType.RoadshowType
                    );
                roadshow.InstructionBox =
                    roadshowTermsAndCondition != null ? roadshowTermsAndCondition.Content : "";
            }

            return roadshow;
        }

        public async Task<PaginationListModel<RoadshowModel>> GetAllRoadshows(
            QueryModel queryModel,
            string userId
        )
        {
            var roles = GetUserRoles(userId);
            var roadshows = _roadshowRepository.GetAllRoadshows(userId, roles, queryModel).ToList();

            return roadshows.ToPagedList(
                queryModel.PaginationParameters.PageNumber,
                queryModel.PaginationParameters.PageSize
            );
        }

        public async Task<List<RoadshowEventCalendarCard>> GetAllEventsForCalendar(
            QueryModel queryModel,
            string userId
        )
        {
            var roles = GetUserRoles(userId);
            var roadshowEvents = _roadshowRepository
                .GetAllEventsForCalendar(userId, roles, queryModel)
                .ToList();

            return roadshowEvents;
        }

        public async Task<IEnumerable<RoadshowModel>> GetAllRoadshowsForCalendar(
            CalendarQueryModel queryModel,
            string userId
        )
        {
            var roles = GetUserRoles(userId);
            var roadshows = _roadshowRepository
                .GetAllRoadshowsForCalendar(userId, roles, queryModel)
                .ToList();

            return roadshows;
        }

        public Task<RoadshowModel> EditRoadshowForCalendar(RoadshowModel model, string userId)
        {
            var roles = GetUserRoles(userId);
            var roadshow = _roadshowRepository.EditRoadshowForCalendar(userId, roles, model);

            return roadshow;
        }

        public List<Roles> GetUserRoles(string userId)
        {
            ApplicationUser applicationUser = _userManager.FindByIdAsync(userId).Result;

            List<Roles> roles = new List<Roles>();
            foreach (string userRole in _userManager.GetRolesAsync(applicationUser).Result.ToList())
            {
                Enum.TryParse(userRole, out Roles role);
                roles.Add(role);
            }

            return roles;
        }

        public IEnumerable<DefaultLocationModel> GetAllDefaultLocations()
        {
            return _roadshowRepository.GetAllDefaultLocations();
        }

        public async Task<RoadshowModel> CreateRoadshow(RoadshowModel model, string userId)
        {
            var auditVisitor = new CreateAuditVisitor(userId, DateTime.UtcNow);
            model.Description = DecodeBase64String(model.Description);
            model.Activities = DecodeBase64String(model.Activities);
            var company = await _companyRepository.GetMyCompany(userId);
            if (company == null)
                return null;

            var roadshow = await _roadshowRepository.CreateRoadshowAsync(
                model,
                auditVisitor,
                userId,
                company.Id
            );
            return roadshow;
        }

        public string DecodeBase64String(string encodedString)
        {
            byte[] data = Convert.FromBase64String(encodedString);
            return System.Text.Encoding.UTF8.GetString(data);
        }

        private async Task<bool> ShouldProcessImage(ImageModel image)
        {
            var imageDb = await _roadshowDocumentRepository.GetByDocumentId(new Guid(image.Id));

            if (
                imageDb != null
                && imageDb.X1 == image.CropCoordinates.X1
                && imageDb.X2 == image.CropCoordinates.X2
                && imageDb.Y1 == image.CropCoordinates.Y1
                && imageDb.Y2 == image.CropCoordinates.Y2
            )
            {
                return false;
            }
            else
            {
                //Delete old thumbanail and large image
                var imagesToDelete = _roadshowDocumentRepository
                    .GetRoadshowImages(new Guid(image.Id))
                    .Where(
                        x =>
                            x.Type == OfferDocumentType.Thumbnail
                            || x.Type == OfferDocumentType.Large
                    );
                foreach (RoadshowDocumentModel imageToDelete in imagesToDelete)
                {
                    await _roadshowDocumentRepository.DeleteAsync(imageToDelete.Id);
                }

                return true;
            }
        }

        private async Task<DocumentFileModel> CreateDetailsImage(
            ImageModel image,
            Guid? parentId,
            DocumentFileModel file,
            byte[] cropedImage
        )
        {
            var imageDetails = _imageUtilsService.Resize(cropedImage, 801, 534);
            var resultDetails = await _documentService.Upload(
                imageDetails,
                Guid.Parse(image.Id),
                file.Name,
                file.MimeType,
                parentId
            );

            return resultDetails;
        }

        private async Task<DocumentFileModel> CreateThumbnailImage(
            ImageModel image,
            Guid? parentId,
            DocumentFileModel file,
            byte[] cropedImage
        )
        {
            var imageThumbnail = _imageUtilsService.Resize(cropedImage, 351, 234);
            var resultThumbnail = await _documentService.Upload(
                imageThumbnail,
                Guid.Parse(image.Id),
                file.Name,
                file.MimeType,
                parentId
            );

            return resultThumbnail;
        }

        public async Task DeleteRoadshow(int id)
        {
            var roadshowModel = await _roadshowRepository.DeleteAsync(id);
            if (roadshowModel.Image != null)
            {
                DocumentProvider provider = DocumentProviderFactory.GetDocumentProvider(
                    DocumentProviderFactory.Operator.azureblobstorage,
                    _documentRepository,
                    _configuration
                );
                provider.Delete(new Guid(roadshowModel.Image.Id));
            }
        }

        public async Task CheckExpiredRoadshows(ILogger logger)
        {
            var expired = new List<RoadshowEmailModel>();
            try
            {
                expired = await _roadshowRepository.DoBackgroundJobAsync(logger);
            }
            catch (Exception e)
            {
                logger.LogError(
                    "Error in trying to execute _roadshowRepository.DoBackgroundJobAsync -> "
                        + e.ToString()
                );
            }

            logger.LogInformation("Expired roadshow models -> " + expired.Count);

            foreach (var s in expired)
            {
                var user = await _applicationUserService.GetById(s.UserId);

                logger.LogInformation("Sending mail -> Status:" + s.EmailStatus);
                logger.LogInformation("Sending mail -> UserId:" + user.Id);
                logger.LogInformation("Sending mail -> RoadshowTitle:" + s.RoadshowTitle);
                try
                {
                    await _mailStorageServiceRepository.CreateMail(
                        new EmailDataModel()
                        {
                            User = user,
                            MailTemplateId = s.EmailStatus,
                            RoadshowName = s.RoadshowTitle,
                            RoadshowId = s.RoadshowId
                        }
                    );
                }
                catch (Exception e)
                {
                    logger.LogError("Error while sending mails via service: " + e.ToString());
                }
            }
        }

        public async Task UnpublishRoadshow(int roadshowId)
        {
            var usersInvitedToRoadshow = await _roadshowRepository.UnpublishRoadshow(roadshowId);
            var roadshow = _roadshowRepository
                .Get()
                .Where(r => r.Id == roadshowId)
                .FirstOrDefault();
            var locations = roadshow.Locations.Select(l => l.Title).ToList();
            var loc = locations.Aggregate((current, next) => current + ", " + next);

            foreach (var u in usersInvitedToRoadshow.Item2)
            {
                var user = await _applicationUserService.GetById(u);
                await _mailStorageServiceRepository.CreateMail(
                    new EmailDataModel()
                    {
                        User = user,
                        MailTemplateId =
                            MessageTemplateList.Roadshow_Unpublished_Notify_SupplierAdminOrSupplier,
                        RoadshowName = usersInvitedToRoadshow.Item1,
                        RoadshowLocation = loc
                    }
                );
            }
        }

        public async Task<RoadshowModel> UpdateRoadshow(RoadshowModel model, string userId)
        {
            var auditVisitor = new UpdateAuditVisitor(userId, DateTime.UtcNow);
            model.Description = DecodeBase64String(model.Description);
            model.Activities = DecodeBase64String(model.Activities);

            var roadshow = await _roadshowRepository.UpdateRoadshowAsync(
                model,
                auditVisitor,
                userId
            );
            return roadshow;
        }

        public async Task<IEnumerable<RoadshowModel>> GetConfirmedRoadshows()
        {
            var roadshows = await _roadshowRepository.GetConfirmedRoadshows().ToListAsync();

            return roadshows;
        }

        public async Task<RoadshowStatus> GetRoadshowStatusById(int id)
        {
            return await _roadshowRepository.GetRoadshowStatusById(id);
        }

        public async Task SendMailNotificationIfRoadshowStatusChanged(
            RoadshowStatus currentRoadshowStatus,
            RoadshowStatus newRoadshowStatus,
            RoadshowModel roadshow
        )
        {
            if (
                (
                    currentRoadshowStatus == RoadshowStatus.Draft
                    && newRoadshowStatus == RoadshowStatus.Submitted
                )
                || (
                    currentRoadshowStatus == RoadshowStatus.Submitted
                    && newRoadshowStatus == RoadshowStatus.Submitted
                )
            )
            {
                await NotifyCoordinatorsRoadshowSubmitted(roadshow);
            }
            else if (
                currentRoadshowStatus == RoadshowStatus.Submitted
                && newRoadshowStatus == RoadshowStatus.Draft
            )
            {
                await NotifySupplierRoadshowReturned(roadshow);
            }
            else if (
                currentRoadshowStatus == RoadshowStatus.Submitted
                && newRoadshowStatus == RoadshowStatus.Approved
            )
            {
                await NotifySupplierRoadshowApproved(roadshow);
            }
            else if (
                currentRoadshowStatus == RoadshowStatus.Approved
                && newRoadshowStatus == RoadshowStatus.Submitted
            )
            {
                await NotifyCoordinatorsRoadshowRejected(roadshow);
            }
            if (
                currentRoadshowStatus == RoadshowStatus.Approved
                && newRoadshowStatus == RoadshowStatus.Confirmed
            )
            {
                await NotifyCoordinatorsAndFocalPointsRoadshowConfirmed(roadshow);
            }
        }

        public async Task NotifyCoordinatorsRoadshowSubmitted(RoadshowModel roadshow)
        {
            var coordinators = await _userManager.GetUsersInRoleAsync("ADNOC Coordinator");

            if (!coordinators.Any())
                return;

            var messageTemplate = Declares
                .MessageTemplateList
                .Roadshow_Submitted_Notify_Coordinator;
            foreach (var coordinator in coordinators)
            {
                var emailData = await _mailStorageServiceService.CreateMailDataForRoadshow(
                    coordinator.Id,
                    coordinator.Email,
                    roadshow.Id,
                    messageTemplate,
                    false
                );
                await _mailStorageServiceService.CreateMail(emailData);
            }
        }

        public async Task NotifySupplierRoadshowReturned(RoadshowModel roadshow)
        {
            ApplicationUser roadshowCreator = await _userManager.FindByIdAsync(roadshow.CreatedBy);
            if (roadshowCreator == null)
                return;
            ApplicationUserModel applicationUserModel = new ApplicationUserModel
            {
                Email = roadshowCreator.Email,
                Id = roadshowCreator.Id
            };

            var messageTemplate = Declares
                .MessageTemplateList
                .Roadshow_Returned_To_Supplier_Notify_SupplierAdminOrSupplier;

            var emailData = await _mailStorageServiceService.CreateMailDataForRoadshow(
                applicationUserModel.Id,
                applicationUserModel.Email,
                roadshow.Id,
                messageTemplate,
                false
            );

            await _mailStorageServiceService.CreateMail(emailData);
        }

        public async Task NotifySupplierRoadshowApproved(RoadshowModel roadshow)
        {
            ApplicationUser roadshowCreator = await _userManager.FindByIdAsync(roadshow.CreatedBy);
            if (roadshowCreator == null)
                return;
            ApplicationUserModel applicationUserModel = new ApplicationUserModel
            {
                Email = roadshowCreator.Email,
                Id = roadshowCreator.Id
            };

            var messageTemplate = Declares
                .MessageTemplateList
                .Roadshow_Approved_Notify_SupplierAdminOrSupplier;

            var emailData = await _mailStorageServiceService.CreateMailDataForRoadshow(
                applicationUserModel.Id,
                applicationUserModel.Email,
                roadshow.Id,
                messageTemplate,
                false
            );

            await _mailStorageServiceService.CreateMail(emailData);
        }

        public async Task NotifyCoordinatorsRoadshowRejected(RoadshowModel roadshow)
        {
            var coordinators = await _userManager.GetUsersInRoleAsync("ADNOC Coordinator");

            if (!coordinators.Any())
                return;

            var messageTemplate = Declares
                .MessageTemplateList
                .Roadshow_Reject_Attendance_Notify_Coordinator;
            var emailData = await _mailStorageServiceService.CreateMailDataForRoadshow(
                coordinators.FirstOrDefault().Id,
                coordinators.FirstOrDefault().Email,
                roadshow.Id,
                messageTemplate,
                false
            );
            await _mailStorageServiceService.CreateMail(emailData);
        }

        public async Task NotifyCoordinatorsAndFocalPointsRoadshowConfirmed(RoadshowModel roadshow)
        {
            ICollection<string> roles = new List<string>();
            roles.Add(Roles.RoadshowFocalPoint.ToString());
            roles.Add(Roles.AdnocCoordinator.ToString());
            var users = _applicationUserService.GetAllUsersByRolesForMail(roles);
            if (users.Count > 0)
            {
                var messageTemplate = Declares.MessageTemplateList.Roadshow_Confirmed_Notify_All;
                foreach (var user in users)
                {
                    var emailData = await _mailStorageServiceService.CreateMailDataForRoadshow(
                        user.Id,
                        user.Email,
                        roadshow.Id,
                        messageTemplate,
                        false
                    );
                    await _mailStorageServiceService.CreateMail(emailData);
                }
            }
        }

        public async Task<Maybe<RoadshowOfferModel>> GetSpecificRoadshowOfferById(int id)
        {
            return await _roadshowRepository.GetSpecificRoadshowOfferById(id);
        }
    }
}
