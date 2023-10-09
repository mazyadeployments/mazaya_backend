using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MMA.WebApi.DataAccess.Extensions;
using MMA.WebApi.DataAccess.Helpers;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Extensions;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.Companies;
using MMA.WebApi.Shared.Interfaces.MailStorage;
using MMA.WebApi.Shared.Interfaces.Offers;
using MMA.WebApi.Shared.Interfaces.RoadshowDocuments;
using MMA.WebApi.Shared.Interfaces.RoadshowInvite;
using MMA.WebApi.Shared.Interfaces.Roles;
using MMA.WebApi.Shared.Models.ApplicationUser;
using MMA.WebApi.Shared.Models.Attachment;
using MMA.WebApi.Shared.Models.Companies;
using MMA.WebApi.Shared.Models.DefaultLocations;
using MMA.WebApi.Shared.Models.Document;
using MMA.WebApi.Shared.Models.Image;
using MMA.WebApi.Shared.Models.MailStorage;
using MMA.WebApi.Shared.Models.Roadshow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Declares;
using static MMA.WebApi.Shared.Enums.Document.DocumentDeclares;

namespace MMA.WebApi.DataAccess.Repository.RoadshowInvites
{
    public class RoadshowInviteRepository
        : BaseRepository<RoadshowInviteModel>,
            IRoadshowInviteRepository
    {
        private readonly IRoadshowDocumentRepository _roadshowDocumentRepository;
        private readonly IRoadshowRepository _roadshowRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IConfiguration _configuration;
        private readonly IMailStorageService _mailStorageServiceService;
        private readonly IRoleService _roleService;
        private readonly UserManager<ApplicationUser> userManager;

        public RoadshowInviteRepository(
            IRoadshowDocumentRepository roadshowDocumentRepository,
            ICompanyRepository companyRepository,
            IRoadshowRepository roadshowRepository,
            Func<MMADbContext> contexFactory,
            IConfiguration configuration,
            IRoleService roleService,
            IMailStorageService mailStorageServiceService,
            UserManager<ApplicationUser> userManager
        )
            : base(contexFactory)
        {
            _roadshowDocumentRepository = roadshowDocumentRepository;
            _roadshowRepository = roadshowRepository;
            _configuration = configuration;
            _companyRepository = companyRepository;
            _mailStorageServiceService = mailStorageServiceService;
            _roleService = roleService;
            this.userManager = userManager;
        }

        public async Task<RoadshowInviteModel> Update(RoadshowInviteModel model, string userId)
        {
            var context = ContextFactory();

            var roadshowInvite = PopulateEntityModel(model, userId);
            context.Update(roadshowInvite);
            await context.SaveChangesAsync();

            return projectToRoadshowInviteCardModel.Compile().Invoke(roadshowInvite);
        }

        private RoadshowInvite PopulateEntityModel(RoadshowInviteModel model, string userId)
        {
            RoadshowInvite roadshowInvite = new RoadshowInvite();
            roadshowInvite.Id = model.Id;
            roadshowInvite.CreatedOn = model.CreatedOn;
            roadshowInvite.CreatedBy = model.CreatedBy;
            roadshowInvite.CompanyId = model.CompanyId;
            roadshowInvite.UpdatedOn = DateTime.UtcNow;
            roadshowInvite.UpdatedBy = userId;
            roadshowInvite.Status = model.Status;

            return roadshowInvite;
        }

        public IQueryable<RoadshowInviteModel> GetAllRoadshowInvitesForRoadshow(
            QueryModel queryModel,
            string userId,
            int roadshowId
        )
        {
            var context = ContextFactory();
            IQueryable<RoadshowInvite> roadshowInvites = context.RoadshowInvite
                .Include(ri => ri.Company)
                .Include(ri => ri.RoadshowEvents)
                .Where(ri => ri.RoadshowId == roadshowId)
                .AsNoTracking();

            var filteredRoadshowInvites = Filter(roadshowInvites, queryModel);
            var roadshowInviteModels = filteredRoadshowInvites.Select(
                projectToRoadshowInviteCardModel
            );

            return roadshowInviteModels.OrderByDescending(x => x.UpdatedOn);
        }

        public async Task<RoadshowInviteDetailsModel> GetRoadshowInviteDetails(
            int roadshowId,
            int inviteId,
            List<Roles> roles,
            string userId
        )
        {
            var context = ContextFactory();

            var inviteModel = context.RoadshowInvite
                .AsNoTracking()
                .Include(x => x.Roadshow)
                .Include(x => x.RoadshowEvents)
                .ThenInclude(x => x.DefaultLocation)
                .Include(x => x.RoadshowEvents)
                .ThenInclude(x => x.RoadshowEventOffers)
                .ThenInclude(x => x.RoadshowOffer)
                .ThenInclude(x => x.RoadshowProposal)
                .Select(projectToRoadshowInviteDetailsModel);

            if (roles.Contains(Roles.Supplier) || roles.Contains(Roles.SupplierAdmin))
            {
                var companyId = await (
                    from cs in context.CompanySuppliers
                    where cs.SupplierId == userId
                    select cs.CompanyId
                ).FirstOrDefaultAsync();

                return await inviteModel
                    .Where(x => x.CompanyId == companyId && x.RoadshowId == roadshowId)
                    .FirstOrDefaultAsync();
            }
            else if (roles.Contains(Roles.Admin) || roles.Contains(Roles.AdnocCoordinator))
            {
                return await inviteModel.FirstOrDefaultAsync(x => x.Id == inviteId);
            }
            else
            {
                return null;
            }
        }

        public async Task SendRoadshowInvitesFromModal(
            RoadshowInvitesQueryModel roadshowInvitesQueryModel,
            int id,
            string userId
        )
        {
            var companies = GetCompanies(roadshowInvitesQueryModel, id);

            if (companies.Any())
            {
                var roadshowInvites = await CreateRoadshowInvites(
                    roadshowInvitesQueryModel,
                    id,
                    userId,
                    companies
                );

                var context = ContextFactory();
                context.AddRange(roadshowInvites);
                context.SaveChanges();
            }
        }

        public async Task SendRoadshowInvitesFromForm(
            RoadshowInvitesQueryModel roadshowInvitesQueryModel,
            int id,
            string userId
        )
        {
            var context = ContextFactory();
            IQueryable<RoadshowInvite> roadshowInvites = context.RoadshowInvite
                .Where(ri => ri.Status == RoadshowInviteStatus.Draft)
                .Include(ri => ri.Company)
                .Include(ri => ri.Roadshow)
                .Include(ri => ri.RoadshowEvents);

            if (
                roadshowInvitesQueryModel.InviteIds != null
                && roadshowInvitesQueryModel.InviteIds.Count() > 0
            )
            {
                roadshowInvites = roadshowInvites.Where(
                    invite =>
                        roadshowInvitesQueryModel.InviteIds.Any(inviteId => inviteId == invite.Id)
                );
            }
            else
            {
                roadshowInvites = Filter(roadshowInvites, roadshowInvitesQueryModel.QueryModel);
            }

            var listRoadshowInvites = await roadshowInvites.ToListAsync();

            foreach (var roadshowInvite in listRoadshowInvites)
            {
                roadshowInvite.Status = RoadshowInviteStatus.Invited;
                var supplierIdsForCompany = context.CompanySuppliers
                    .Where(x => x.CompanyId == roadshowInvite.CompanyId)
                    .Select(x => x.SupplierId);
                var supplierUsers = await context.Users
                    .Where(x => supplierIdsForCompany.Contains(x.Id))
                    .OrderByDescending(x => x.CreatedOn)
                    .ToListAsync();
                var userForNotification = supplierUsers.FirstOrDefault();

                if (userForNotification != null)
                    await CreateAndSendMail(userForNotification, roadshowInvite);
            }

            context.RoadshowInvite.UpdateRange(roadshowInvites);
            await context.SaveChangesAsync();
        }

        private async Task CreateAndSendMail(
            ApplicationUser userForNotification,
            RoadshowInvite roadshowInvite
        )
        {
            ApplicationUserModel supplierAdminModel = new ApplicationUserModel
            {
                Id = userForNotification.Id,
                Email = userForNotification.Email
            };

            var emailData = new EmailDataModel()
            {
                User = supplierAdminModel,
                MailTemplateId = Declares
                    .MessageTemplateList
                    .Company_Invited_To_Roadshow_Notify_SupplierAdminOrSupplier,
                CompanyName = roadshowInvite.Company.NameEnglish,
                RoadshowLocation = "",
                RoadshowName = roadshowInvite.Roadshow.Title
            };

            await _mailStorageServiceService.CreateMail(emailData);
        }

        public void DeleteRoadshowInvites(RoadshowInvitesQueryModel roadshowInvitesQueryModel)
        {
            var context = ContextFactory();
            IQueryable<RoadshowInvite> roadshowInvites = context.RoadshowInvite
                .Include(ri => ri.Company)
                .Include(ri => ri.RoadshowEvents)
                .ThenInclude(ri => ri.RoadshowEventOffers)
                .ThenInclude(ri => ri.RoadshowOffer);

            if (
                roadshowInvitesQueryModel.InviteIds != null
                && roadshowInvitesQueryModel.InviteIds.Count() > 0
            )
            {
                roadshowInvites = roadshowInvites.Where(
                    invite =>
                        roadshowInvitesQueryModel.InviteIds.Any(inviteId => inviteId == invite.Id)
                );
            }
            else
            {
                roadshowInvites = Filter(roadshowInvites, roadshowInvitesQueryModel.QueryModel);
            }

            context.RoadshowInvite.RemoveRange(roadshowInvites);
            context.SaveChanges();
        }

        public void UpdateRoadshowInvites(RoadshowInvitesQueryModel roadshowInvitesQueryModel)
        {
            var context = ContextFactory();
            IQueryable<RoadshowInvite> roadshowInvites = context.RoadshowInvite
                .Include(ri => ri.Company)
                .Include(ri => ri.RoadshowEvents)
                .ThenInclude(ri => ri.DefaultLocation)
                .Where(
                    ri =>
                        ri.Status == RoadshowInviteStatus.Draft
                        || ri.Status == RoadshowInviteStatus.Renegotiation
                );

            var roadshowInvitesList = FilterInvites(roadshowInvitesQueryModel, roadshowInvites);
            var roadshowInvitesListWithNewEvents = AddEventsToInvite(
                roadshowInvitesQueryModel,
                roadshowInvitesList
            );

            context.RoadshowInvite.UpdateRange(roadshowInvitesListWithNewEvents);
            context.SaveChanges();
        }

        private static List<RoadshowInvite> AddEventsToInvite(
            RoadshowInvitesQueryModel roadshowInvitesQueryModel,
            List<RoadshowInvite> roadshowInvitesList
        )
        {
            foreach (var roadshowInvite in roadshowInvitesList)
            {
                if (!roadshowInvitesQueryModel.KeepEvents)
                {
                    roadshowInvite.RoadshowEvents.Clear();
                }

                var eventsToAdd = roadshowInvitesQueryModel.Events
                    .Select(
                        re =>
                            new RoadshowEvent
                            {
                                Id = re.Id,
                                DateFrom = re.DateFrom,
                                DateTo = re.DateTo,
                                RoadshowInviteId = roadshowInvite.Id,
                            }
                    )
                    .ToList();

                roadshowInvite.RoadshowEvents.AddRange(eventsToAdd);
            }

            return roadshowInvitesList;
        }

        private static List<RoadshowInvite> FilterInvites(
            RoadshowInvitesQueryModel roadshowInvitesQueryModel,
            IQueryable<RoadshowInvite> roadshowInvites
        )
        {
            bool invitesSelected =
                roadshowInvitesQueryModel.InviteIds != null
                && roadshowInvitesQueryModel.InviteIds.Count() > 0;

            if (invitesSelected)
            {
                roadshowInvites = roadshowInvites.Where(
                    invite =>
                        roadshowInvitesQueryModel.InviteIds.Any(inviteId => inviteId == invite.Id)
                );
            }
            else
            {
                roadshowInvites = Filter(roadshowInvites, roadshowInvitesQueryModel.QueryModel);
            }

            return roadshowInvites.ToList();
        }

        public int AddOrUpdateRoadshowEventToRoadshowInvite(
            RoadshowEventModel roadshowEventModel,
            int id,
            int idinvite
        )
        {
            RoadshowEvent roadshowEvent = new RoadshowEvent
            {
                Id = roadshowEventModel.Id,
                DateFrom = roadshowEventModel.DateFrom,
                DateTo = roadshowEventModel.DateTo,
                RoadshowInviteId = idinvite,
            };

            var context = ContextFactory();

            if (roadshowEventModel.Id == 0)
            {
                context.RoadshowEvent.Add(roadshowEvent);
            }
            else
            {
                context.RoadshowEvent.Update(roadshowEvent);
            }

            context.SaveChanges();

            return roadshowEvent.Id;
        }

        private async Task<List<RoadshowInvite>> CreateRoadshowInvites(
            RoadshowInvitesQueryModel roadshowInvitesQueryModel,
            int id,
            string userId,
            List<CompanyCardModel> companies
        )
        {
            List<RoadshowInvite> roadshowInvites = new List<RoadshowInvite>();

            foreach (var company in companies)
            {
                RoadshowInvite roadshowInvite = new RoadshowInvite { CompanyId = company.Id };
                if (roadshowInvitesQueryModel.SendInvite)
                {
                    roadshowInvite.Status = RoadshowInviteStatus.Invited;
                    var supplierAdmin = await userManager.FindByIdAsync(company.CreatedBy);

                    if (supplierAdmin != null)
                    {
                        ApplicationUserModel supplierAdminModel = new ApplicationUserModel
                        {
                            Id = supplierAdmin.Id,
                            Email = supplierAdmin.Email
                        };

                        var roles = await _roleService.GetUserRoles(supplierAdminModel.Id);
                        if (!roles.Contains(Roles.Admin) && !roles.Contains(Roles.AdnocCoordinator))
                        {
                            var roadshow = _roadshowRepository
                                .Get()
                                .Where(r => r.Id == id)
                                .FirstOrDefault();
                            var roadshowTitle = roadshow.Title;

                            var locations = roadshow.Locations.Select(l => l.Title).ToList();
                            var loc = locations.Aggregate((current, next) => current + ", " + next);

                            var emailData = new EmailDataModel()
                            {
                                User = supplierAdminModel,
                                MailTemplateId = Declares
                                    .MessageTemplateList
                                    .Company_Invited_To_Roadshow_Notify_SupplierAdminOrSupplier,
                                CompanyName = company.NameEnglish,
                                RoadshowLocation = loc,
                                RoadshowName = roadshowTitle
                            };

                            await _mailStorageServiceService.CreateMail(emailData);
                        }
                    }
                }
                else
                {
                    roadshowInvite.Status = RoadshowInviteStatus.Draft;
                }
                roadshowInvite.CreatedOn = DateTime.UtcNow;
                roadshowInvite.CreatedBy = userId;
                roadshowInvite.UpdatedOn = DateTime.UtcNow;
                roadshowInvite.UpdatedBy = userId;
                roadshowInvite.RoadshowId = id;
                roadshowInvite.RoadshowEvents = roadshowInvitesQueryModel.Events
                    .Select(
                        roadshowEventModel =>
                            new RoadshowEvent
                            {
                                DateFrom = roadshowEventModel.DateFrom,
                                DateTo = roadshowEventModel.DateTo,
                                RoadshowInviteId = roadshowInvite.Id,
                            }
                    )
                    .ToList();

                roadshowInvites.Add(roadshowInvite);
            }

            return roadshowInvites;
        }

        private List<CompanyCardModel> GetCompanies(
            RoadshowInvitesQueryModel roadshowInvitesQueryModel,
            int id
        )
        {
            bool noCompanyWereSelected =
                roadshowInvitesQueryModel.CompanyIds == null
                || roadshowInvitesQueryModel.CompanyIds.Count() == 0;

            if (noCompanyWereSelected)
            {
                return _companyRepository
                    .GetSuppliersForRoadshowInviteModal(roadshowInvitesQueryModel.QueryModel, id)
                    .AsNoTracking()
                    .ToList();
            }
            else
            {
                return _companyRepository
                    .GetAllCompaniesCard()
                    .Where(i => roadshowInvitesQueryModel.CompanyIds.Contains(i.Id))
                    .ToList();
            }
        }

        private static IQueryable<RoadshowInvite> Filter(
            IQueryable<RoadshowInvite> roadshowInvite,
            QueryModel queryModel
        )
        {
            var filteredInvites = roadshowInvite.Where(
                ri =>
                    ri.Company.NameEnglish
                        .ToLower()
                        .Contains(queryModel.Filter.Keyword.Trim().ToLower())
            );

            if (queryModel.Filter.Categories?.Any() == true)
            {
                filteredInvites = filteredInvites.Where(
                    ri =>
                        ri.Company.CompanyCategories.Any(
                            cc => queryModel.Filter.Categories.Contains(cc.CategoryId)
                        )
                );
            }

            return filteredInvites;
        }

        private readonly Expression<
            Func<RoadshowInvite, RoadshowInviteModel>
        > projectToRoadshowInviteCardModel = data =>
            new RoadshowInviteModel()
            {
                Status = data.Status,
                Id = data.Id,
                Company = new CompanyModel
                {
                    Logo =
                        data.Company.Logo != null
                            ? data.Company.Logo.DocumentId.ToString()
                            : string.Empty,
                    Mobile = new PhoneNumberModel
                    {
                        CountryCode = data.Company.MobileCountryCode,
                        E164Number = data.Company.MobileE164Number,
                        InternationalNumber = data.Company.Mobile,
                        Number = data.Company.MobileNumber,
                    },
                    NameEnglish = data.Company.NameEnglish,
                    OfficialEmail = data.Company.OfficialEmail,
                    Id = data.Company.Id,
                },
                CreatedBy = data.CreatedBy,
                CreatedOn = data.CreatedOn,
                UpdatedOn = data.UpdatedOn,
                UpdatedBy = data.UpdatedBy
            };

        private readonly Expression<
            Func<RoadshowInvite, RoadshowInviteDetailsModel>
        > projectToRoadshowInviteDetailsModel = data =>
            new RoadshowInviteDetailsModel()
            {
                Id = data.Id,
                RoadshowId = data.RoadshowId,
                Status = data.Status,
                CompanyId = data.CompanyId,
                RoadshowEvents = data.RoadshowEvents
                    .Select(
                        rem =>
                            new RoadshowInviteEventModel
                            {
                                Id = rem.Id,
                                DateFrom = rem.DateFrom,
                                DateTo = rem.DateTo,
                                DefaultLocation = new DefaultLocationModel
                                {
                                    Id = rem.DefaultLocation.Id,
                                    Title = rem.DefaultLocation.Title,
                                    Address = rem.DefaultLocation.Address,
                                    Country = rem.DefaultLocation.Country,
                                    Vicinity = rem.DefaultLocation.Vicinity
                                },
                                RoadshowOffers = rem.RoadshowEventOffers
                                    .Select(
                                        reo =>
                                            new RoadshowOfferModel
                                            {
                                                Id = reo.RoadshowOffer.Id,
                                                CompanyId = reo.RoadshowOffer
                                                    .RoadshowProposal
                                                    .Company
                                                    .Id,
                                                Title = reo.RoadshowOffer.Title,
                                                RoadshowProposalId =
                                                    reo.RoadshowOffer.RoadshowProposalId,
                                                RoadshowProposalTitle = reo.RoadshowOffer
                                                    .RoadshowProposal
                                                    .Title,
                                                RoadshowProposalSubject = reo.RoadshowOffer
                                                    .RoadshowProposal
                                                    .Subject,
                                                Description = reo.RoadshowOffer.Description,
                                                RoadshowDetails = reo.RoadshowOffer.RoadshowDetails,
                                                EquipmentItem = reo.RoadshowOffer.EquipmentItem,
                                                PromotionCode = reo.RoadshowOffer.PromotionCode,
                                                //Status = reo.RoadshowOffer.Status,
                                                Category = reo.RoadshowOffer.RoadshowOfferCategories
                                                    .Select(c => c.Category.Title)
                                                    .FirstOrDefault(),
                                                CreatedBy = reo.RoadshowOffer.CreatedBy,
                                                CreatedOn = reo.RoadshowOffer.CreatedOn,
                                                UpdatedBy = reo.RoadshowOffer.UpdatedBy,
                                                UpdatedOn = reo.RoadshowOffer.UpdatedOn,
                                                MainImage = reo.RoadshowOffer.OfferDocuments
                                                    .Where(
                                                        od =>
                                                            od.Type == OfferDocumentType.Thumbnail
                                                            && od.Cover
                                                    )
                                                    .Select(od => od.DocumentId.ToString())
                                                    .FirstOrDefault(),
                                                Tag = reo.RoadshowOffer.RoadshowOfferTags
                                                    .Select(ot => ot.Tag.Title)
                                                    .FirstOrDefault(),
                                                RoadshowOfferCategories =
                                                    reo.RoadshowOffer.RoadshowOfferCategories
                                                        .Select(
                                                            oc =>
                                                                new RoadshowOfferCategoryModel
                                                                {
                                                                    Id = oc.CategoryId,
                                                                    Title = oc.Category.Title
                                                                }
                                                        )
                                                        .ToList(),
                                                RoadshowOfferCollections =
                                                    reo.RoadshowOffer.RoadshowOfferCollections
                                                        .Select(
                                                            oc =>
                                                                new RoadshowOfferCollectionModel
                                                                {
                                                                    Id = oc.CollectionId,
                                                                    Title = oc.Collection.Title
                                                                }
                                                        )
                                                        .ToList(),
                                                RoadshowOfferTags =
                                                    reo.RoadshowOffer.RoadshowOfferTags
                                                        .Select(
                                                            ot =>
                                                                new RoadshowOfferTagModel
                                                                {
                                                                    Id = ot.TagId,
                                                                    Title = ot.Tag.Title
                                                                }
                                                        )
                                                        .ToList(),
                                                OfferAttachments = reo.RoadshowOffer.OfferDocuments
                                                    .Where(
                                                        od =>
                                                            (od.Type == OfferDocumentType.Document)
                                                    )
                                                    .Select(
                                                        od =>
                                                            new AttachmentModel
                                                            {
                                                                Id = od.DocumentId.ToString(),
                                                                Name = od.Document.Name,
                                                                Type = od.Type.ToString()
                                                            }
                                                    )
                                                    .ToList(),
                                                Video = reo.RoadshowOffer.OfferDocuments
                                                    .Where(
                                                        od => (od.Type == OfferDocumentType.Video)
                                                    )
                                                    .Select(
                                                        od =>
                                                            new VideoModel
                                                            {
                                                                Id = od.DocumentId.ToString(),
                                                                Type = od.Document.MimeType
                                                            }
                                                    )
                                                    .FirstOrDefault(),
                                                RoadshowOfferRatings = reo.RoadshowOffer.OfferRating
                                                    .Select(
                                                        or =>
                                                            new RoadshowOfferRatingModel
                                                            {
                                                                RoadshowOfferId =
                                                                    or.RoadshowOfferId,
                                                                ApplicationUserId =
                                                                    or.ApplicationUserId,
                                                                Rating = or.Rating,
                                                                CommentText = or.CommentText,
                                                                BuyerFirstName =
                                                                    or.ApplicationUser.FirstName,
                                                                BuyerLastName =
                                                                    or.ApplicationUser.LastName,
                                                                OfferTitle = or.RoadshowOffer.Title,
                                                                CreatedBy = or.CreatedBy,
                                                                CreatedOn = or.CreatedOn,
                                                                Status = or.Status
                                                            }
                                                    )
                                                    .ToList(),
                                                RoadshowVouchers =
                                                    reo.RoadshowOffer.RoadshowVouchers
                                                        .Select(
                                                            ov =>
                                                                new RoadshowVoucherModel
                                                                {
                                                                    Id = ov.Id,
                                                                    Details = ov.Details,
                                                                    Quantity = ov.Quantity,
                                                                    Validity = ov.Validity
                                                                }
                                                        )
                                                        .ToList(),
                                                Images = reo.RoadshowOffer.OfferDocuments
                                                    .Where(
                                                        od =>
                                                            (
                                                                od.Type
                                                                    != OfferDocumentType.Document
                                                                && od.Type
                                                                    != OfferDocumentType.Video
                                                            )
                                                    )
                                                    .Select(
                                                        od =>
                                                            new ImageModel
                                                            {
                                                                Id = od.DocumentId.ToString(),
                                                                Type = od.Type,
                                                                OriginalImageId =
                                                                    od.OriginalImageId,
                                                                CropCoordinates =
                                                                    new CropCoordinates
                                                                    {
                                                                        X1 = od.X1,
                                                                        X2 = od.X2,
                                                                        Y1 = od.Y1,
                                                                        Y2 = od.Y2
                                                                    },
                                                                CropNGXCoordinates =
                                                                    new CropCoordinates
                                                                    {
                                                                        X1 = od.cropX1,
                                                                        X2 = od.cropX2,
                                                                        Y1 = od.cropY1,
                                                                        Y2 = od.cropY2
                                                                    },
                                                            }
                                                    )
                                                    .ToList(),
                                                ImageUrls = reo.RoadshowOffer.OfferDocuments
                                                    .Where(
                                                        od =>
                                                            (
                                                                od.Type
                                                                    != OfferDocumentType.Document
                                                                && od.Type
                                                                    != OfferDocumentType.Video
                                                            )
                                                    )
                                                    .Select(
                                                        od =>
                                                            new ImageUrlsModel
                                                            {
                                                                Original = od.DocumentId.ToString(),
                                                                Thumbnail =
                                                                    od.DocumentId.ToString(),
                                                                Large = od.DocumentId.ToString(),
                                                            }
                                                    )
                                                    .ToList()
                                            }
                                    )
                                    .ToList()
                            }
                    )
                    .ToList()
            };

        private readonly Expression<
            Func<RoadshowInvite, RoadshowInviteModel>
        > projectToRoadshowInviteModel = data =>
            new RoadshowInviteModel()
            {
                Id = data.Id,
                Status = data.Status,
                CompanyId = data.CompanyId,
                Company = new CompanyModel
                {
                    Id = data.Company.Id,
                    NameArabic = data.Company.NameArabic,
                    NameEnglish = data.Company.NameEnglish,
                }
            };

        public async Task<bool> DeleteRoadshowEventToRoadshowInvite(int idevent)
        {
            var context = ContextFactory();
            var roadshowEvent = await context.RoadshowEvent.FindAsync(idevent);

            if (roadshowEvent != null)
            {
                context.Remove(roadshowEvent);
                context.SaveChanges();

                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task DeactivateRoadshowInvites(int companyId)
        {
            var context = ContextFactory();

            var roadshowInvitesIds = context.RoadshowInvite
                .Where(oa => oa.Company.Id == companyId)
                .Select(x => x.Id)
                .ToList();
            var roadshowInvites = context.RoadshowInvite
                .Where(x => roadshowInvitesIds.Contains(x.Id))
                .ToList();
            roadshowInvites.ForEach(o => o.Status = RoadshowInviteStatus.Deactivated);

            context.RoadshowInvite.UpdateRange(roadshowInvites);
            await context.SaveChangesAsync();
        }

        public async Task HardOfCompanyDeleteInvites(int companyId)
        {
            var context = ContextFactory();

            var roadshowInvitesIds = context.RoadshowInvite
                .Where(oa => oa.Company.Id == companyId)
                .Select(x => x.Id)
                .ToList();
            var roadshowInvites = context.RoadshowInvite
                .Where(x => roadshowInvitesIds.Contains(x.Id))
                .ToList();

            context.RoadshowInvite.RemoveRange(roadshowInvites);
            await context.SaveChangesAsync();
        }

        public IQueryable<RoadshowInviteModel> Get()
        {
            var context = ContextFactory();

            return context.RoadshowInvite.Select(projectToRoadshowInviteModel);
        }

        protected override IQueryable<RoadshowInviteModel> GetEntities()
        {
            throw new NotImplementedException();
        }

        public Task<List<ApplicationUserModel>> GetSupplierAdminsForInvite(int inviteId)
        {
            var context = ContextFactory();

            return (
                from ri in context.RoadshowInvite
                join c in context.Company on ri.CompanyId equals c.Id
                join cs in context.CompanySuppliers on c.Id equals cs.CompanyId
                join u in context.Users on cs.SupplierId equals u.Id
                where ri.Id == inviteId
                select new ApplicationUserModel()
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Active = u.Active,
                    Title = u.Title,
                    CompanyName = c.NameEnglish,
                    CompanyDescription = c.CompanyDescription,
                    Email = u.Email
                }
            ).ToListAsync();
        }

        public async Task UpdateOfRoadshowInviteDetails(
            RoadshowInviteDetailsModel roadshowInviteDetailsModel,
            string userId
        )
        {
            var context = ContextFactory();
            var roadshowInvite = new RoadshowInvite();

            var roles = await _roleService.GetUserRoles(userId);
            if (roles.Contains(Roles.Supplier) || roles.Contains(Roles.SupplierAdmin))
            {
                var companyId = await (
                    from cs in context.CompanySuppliers
                    where cs.SupplierId == userId
                    select cs.CompanyId
                ).FirstOrDefaultAsync();
                roadshowInvite = await context.RoadshowInvite
                    .Include(ri => ri.RoadshowEvents)
                    .ThenInclude(re => re.RoadshowEventOffers)
                    .ThenInclude(reo => reo.RoadshowOffer)
                    .FirstOrDefaultAsync(
                        ri => ri.Id == roadshowInviteDetailsModel.Id && ri.CompanyId == companyId
                    );
            }
            else
            {
                roadshowInvite = await context.RoadshowInvite
                    .Include(ri => ri.RoadshowEvents)
                    .ThenInclude(re => re.RoadshowEventOffers)
                    .ThenInclude(reo => reo.RoadshowOffer)
                    .FirstOrDefaultAsync(ri => ri.Id == roadshowInviteDetailsModel.Id);
            }

            if (roadshowInvite != null)
            {
                var oldStatus = roadshowInvite.Status;
                if (
                    CheckIfValidStatusTransition(
                        roadshowInvite.Status,
                        roadshowInviteDetailsModel.Status,
                        roles
                    )
                )
                {
                    roadshowInvite.Status = roadshowInviteDetailsModel.Status;
                }
                else
                {
                    return;
                }

                foreach (var comment in roadshowInviteDetailsModel.RoadshowComments.ToList())
                {
                    if (comment.CreatedBy == null)
                    {
                        context.RoadshowComment.Add(
                            new RoadshowComment()
                            {
                                RoadshowId = roadshowInviteDetailsModel.RoadshowId,
                                Text = comment.Text,
                                CreatedBy = userId,
                                CreatedOn = DateTime.Now.SpecifyKind(DateTimeKind.Utc)
                            }
                        );

                        context.SaveChanges();
                    }
                }

                var roadshow = await context.Roadshow
                    .Include(r => r.Locations)
                    .Where(r => r.Id == roadshowInviteDetailsModel.RoadshowId)
                    .FirstOrDefaultAsync();
                var allRoadshowInviteEvents = context.RoadshowEvent
                    .Where(x => x.RoadshowInviteId == roadshowInviteDetailsModel.Id)
                    .ToList();

                List<RoadshowEvent> updatedRoadshowInviteEvents = new List<RoadshowEvent>();
                roadshowInviteDetailsModel.RoadshowEvents
                    .ToList()
                    .ForEach(
                        x =>
                            updatedRoadshowInviteEvents.Add(
                                context.RoadshowEvent.Where(y => y.Id == x.Id).FirstOrDefault()
                            )
                    );

                var eventsToDelete = allRoadshowInviteEvents
                    .Except(updatedRoadshowInviteEvents, new RoadshowEventEqualityHelper())
                    .ToList();

                foreach (var x in roadshowInviteDetailsModel.RoadshowEvents.ToList())
                {
                    if (
                        CheckIfDataIsValidToChangeEvents(roles, x, roadshow)
                        && (
                            oldStatus == RoadshowInviteStatus.Draft
                            || oldStatus == RoadshowInviteStatus.Renegotiation
                        )
                    )
                    {
                        if (x.Id != 0)
                        {
                            var roadshowEvent = roadshowInvite.RoadshowEvents.FirstOrDefault(
                                re => re.Id == x.Id
                            );
                            roadshowEvent.RoadshowInviteId = roadshowInviteDetailsModel.Id;
                            roadshowEvent.DefaultLocationId = x.DefaultLocation.Id;
                            roadshowEvent.DateFrom = x.DateFrom;
                            roadshowEvent.DateTo = x.DateTo;
                            context.RoadshowEvent.Update(roadshowEvent);
                            context.SaveChanges();
                        }
                        else
                        {
                            context.RoadshowEvent.Add(
                                new RoadshowEvent()
                                {
                                    DefaultLocationId = x.DefaultLocation.Id,
                                    RoadshowInviteId = roadshowInviteDetailsModel.Id,
                                    DateFrom = x.DateFrom,
                                    DateTo = x.DateTo
                                }
                            );
                            context.SaveChanges();
                        }
                    }

                    var allRoadshowEventOffers = (
                        from ro in context.RoadshowOffer
                        join reo in context.RoadshowEventOffer on ro.Id equals reo.RoadshowOfferId
                        where reo.RoadshowEventId == x.Id
                        select reo
                    ).ToList();

                    List<RoadshowEventOffer> updatedRoadshowEventOffers =
                        new List<RoadshowEventOffer>();

                    foreach (var o in x.RoadshowOffers.ToList())
                    {
                        var y = context.RoadshowEventOffer
                            .Where(y => y.RoadshowEventId == x.Id && y.RoadshowOfferId == o.Id)
                            .FirstOrDefault();
                        updatedRoadshowEventOffers.Add(y);
                    }

                    var roadshowEventOffersToDelete = allRoadshowEventOffers
                        .Except(updatedRoadshowEventOffers, new RoadshowEventOffersEqualityHelper())
                        .ToList();

                    context.RoadshowEventOffer.RemoveRange(roadshowEventOffersToDelete);
                    context.SaveChanges();

                    var roadshowEventOffers = context.RoadshowEventOffer
                        .Where(e => e.RoadshowEventId == x.Id)
                        .ToList();

                    foreach (var y in x.RoadshowOffers.ToList())
                    {
                        if (CheckIfDataIsValidToChangeRoadshowOffers(oldStatus, roles))
                        {
                            var roadshowEventOffer = new RoadshowEventOffer()
                            {
                                RoadshowEventId = x.Id,
                                RoadshowEvent = null,
                                RoadshowOfferId = y.Id,
                                RoadshowOffer = null
                            };
                            if (
                                !roadshowEventOffersToDelete
                                    .ToList()
                                    .Any(z => z.RoadshowOfferId == y.Id)
                                && !roadshowEventOffers.Any(
                                    reo =>
                                        reo.RoadshowEventId == x.Id && reo.RoadshowOfferId == y.Id
                                )
                            )
                            {
                                context.RoadshowEventOffer.Add(
                                    new RoadshowEventOffer()
                                    {
                                        RoadshowOfferId = y.Id,
                                        RoadshowEventId = x.Id
                                    }
                                );
                                context.SaveChanges();
                            }
                        }
                    }
                }

                context.RoadshowEvent.RemoveRange(eventsToDelete);
                await context.SaveChangesAsync();
            }
        }

        private bool CheckIfValidStatusTransition(
            RoadshowInviteStatus currentStatus,
            RoadshowInviteStatus nextStatus,
            List<Roles> userRoles
        )
        {
            switch (currentStatus)
            {
                case RoadshowInviteStatus.Draft:
                    return (nextStatus == RoadshowInviteStatus.Invited)
                        && (
                            userRoles.Contains(Roles.Admin)
                            || userRoles.Contains(Roles.AdnocCoordinator)
                        );
                case RoadshowInviteStatus.Invited:
                    return (
                        (
                            nextStatus == RoadshowInviteStatus.Accepted
                            || nextStatus == RoadshowInviteStatus.Rejected
                            || nextStatus == RoadshowInviteStatus.Renegotiation
                        )
                            && (
                                userRoles.Contains(Roles.Supplier)
                                || userRoles.Contains(Roles.SupplierAdmin)
                            )
                        || (
                            (
                                userRoles.Contains(Roles.Admin)
                                || userRoles.Contains(Roles.AdnocCoordinator)
                            )
                            && nextStatus == RoadshowInviteStatus.Blocked
                        )
                    );
                case RoadshowInviteStatus.Accepted:
                    return (
                            nextStatus == RoadshowInviteStatus.Accepted
                            || nextStatus == RoadshowInviteStatus.Review
                            || nextStatus == RoadshowInviteStatus.Rejected
                            || nextStatus == RoadshowInviteStatus.Invited
                        )
                        && (
                            userRoles.Contains(Roles.Supplier)
                            || userRoles.Contains(Roles.SupplierAdmin)
                        );
                case RoadshowInviteStatus.Review:
                    return (
                            nextStatus == RoadshowInviteStatus.Returned
                            || nextStatus == RoadshowInviteStatus.Accepted
                            || nextStatus == RoadshowInviteStatus.Approved
                            || nextStatus == RoadshowInviteStatus.Blocked
                        )
                        && (
                            userRoles.Contains(Roles.Admin)
                            || userRoles.Contains(Roles.AdnocCoordinator)
                        );
                case RoadshowInviteStatus.Renegotiation:
                    return (
                            nextStatus == RoadshowInviteStatus.Renegotiation
                            || nextStatus == RoadshowInviteStatus.Invited
                        )
                        && (
                            userRoles.Contains(Roles.Admin)
                            || userRoles.Contains(Roles.AdnocCoordinator)
                        );
                case RoadshowInviteStatus.Returned:
                    return (
                            nextStatus == RoadshowInviteStatus.Returned
                            || nextStatus == RoadshowInviteStatus.Review
                        )
                        && (
                            userRoles.Contains(Roles.SupplierAdmin)
                            || userRoles.Contains(Roles.Supplier)
                        );
                default:
                    return false;
            }
        }

        private bool CheckIfDataIsValidToChangeEvents(
            List<Roles> userRoles,
            RoadshowInviteEventModel roadshowEvent,
            Roadshow roadshow
        )
        {
            if (!userRoles.Contains(Roles.Admin) && !userRoles.Contains(Roles.AdnocCoordinator))
            {
                return false;
            }

            if (
                roadshowEvent.DateFrom < roadshow.DateFrom || roadshowEvent.DateTo > roadshow.DateTo
            )
            {
                return false;
            }

            if (
                !roadshow.Locations.Any(
                    rl => rl.DefaultLocationId == roadshowEvent.DefaultLocation.Id
                )
            )
            {
                return false;
            }

            return true;
        }

        private bool CheckIfDataIsValidToChangeRoadshowOffers(
            RoadshowInviteStatus roadshowInviteStatus,
            List<Roles> userRoles
        )
        {
            if (
                (
                    roadshowInviteStatus == RoadshowInviteStatus.Accepted
                    || roadshowInviteStatus == RoadshowInviteStatus.Returned
                ) && (userRoles.Contains(Roles.Supplier) || userRoles.Contains(Roles.SupplierAdmin))
            )
            {
                return true;
            }

            return false;
        }

        private readonly Expression<
            Func<RoadshowOffer, RoadshowEventOffer>
        > roadshowOfferToRoadshowEventOffer = data =>
            new RoadshowEventOffer()
            {
                RoadshowEventId = data.RoadshowEventOffers.FirstOrDefault().RoadshowEventId,
                RoadshowOfferId = data.Id
            };
    }
}
