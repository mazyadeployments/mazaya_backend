using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MMA.WebApi.DataAccess.Extensions;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.ApplicationUsers;
using MMA.WebApi.Shared.Interfaces.MailStorage;
using MMA.WebApi.Shared.Models.Announcement;
using MMA.WebApi.Shared.Models.ApplicationUser;
using MMA.WebApi.Shared.Models.Comments;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.MailStorage;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMA.WebApi.Core.Services
{
    public class MailStorageService : IMailStorageService
    {
        private readonly IMailStorageRepository _mailRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IApplicationUsersRepository _applicationUsersRepository;

        public MailStorageService(
            IMailStorageRepository mailRepo,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            IApplicationUsersRepository applicationUsersRepository
        )
        {
            _mailRepo = mailRepo;
            _userManager = userManager;
            _configuration = configuration;
            _applicationUsersRepository = applicationUsersRepository;
        }

        public async Task CheckMessageQueue()
        {
            await _mailRepo.CheckMessageQueue();
        }

        public async Task CreateMail(EmailDataModel emailDataModel)
        {
            await _mailRepo.CreateMail(emailDataModel);
        }

        public async Task CreateMailForAdnocTeam(EmailDataModel emailDataModel)
        {
            var coordinators = await _userManager.GetUsersInRoleAsync("ADNOC Coordinator");
            if (!coordinators.Any())
                return;

            var emailData = CreateMailData(
                coordinators.FirstOrDefault().Id,
                null,
                "",
                emailDataModel.MailTemplateId,
                false
            );
            await CreateMail(emailData);
        }

        public async Task DeleteListAsync(IEnumerable<int> list)
        {
            await _mailRepo.DeleteListAsync(list);
        }

        public async Task<PaginationListModel<MailStorageModel>> SearchMailStorages(
            QueryModel queryModel
        )
        {
            var mailStorages = _mailRepo.Get(queryModel).OrderByDescending(x => x.StatusOn);

            return await Task.FromResult(
                mailStorages.ToPagedList(
                    queryModel.PaginationParameters.PageNumber,
                    queryModel.PaginationParameters.PageSize
                )
            );
        }

        public async Task<MailStorageModel> GetMailStorageDetails(int id)
        {
            return await _mailRepo.GetSingleAsync(x => x.Id.Equals(id));
        }

        public async Task CreateShareMail(
            string userId,
            string userMail,
            string subject,
            string body
        )
        {
            await _mailRepo.CreateShareMail(userId, userMail, subject, body);
        }

        public EmailDataModel CreateMailData(
            string userId,
            int? offerId,
            string companyName,
            Declares.MessageTemplateList messageTemplate,
            bool isApproved
        )
        {
            ApplicationUserModel applicationUserModel = new ApplicationUserModel
            {
                Email = _configuration["Emails:SysAdminMail"],
                Id = userId
            };

            var emailData = new EmailDataModel()
            {
                User = applicationUserModel,
                MailTemplateId = messageTemplate,
                OfferId = offerId,
                CompanyName = companyName,
                IsApproved = isApproved
            };

            return emailData;
        }

        public async Task<EmailDataModel> CreateMailDataForRoadshow(
            string userId,
            string userEmail,
            int roadshowId,
            Declares.MessageTemplateList messageTemplate,
            bool isApproved
        )
        {
            ApplicationUserModel applicationUserModel = new ApplicationUserModel
            {
                Email = userEmail,
                Id = userId
            };

            var emailData = new EmailDataModel()
            {
                User = applicationUserModel,
                MailTemplateId = messageTemplate,
                IsApproved = isApproved,
                RoadshowId = roadshowId
            };

            return emailData;
        }

        public async Task SetCreatedByName(string deletedUser, CommentBaseModel comment)
        {
            ApplicationUser applicationUser = await _userManager.FindByIdAsync(comment.CreatedBy);
            comment.CreatedByName =
                applicationUser == null ? deletedUser : applicationUser.NormalizedUserName;
            var createUserRole = await _userManager.GetRolesAsync(applicationUser);
            if (createUserRole.Contains(Declares.Roles.AdnocCoordinator.ToString()))
            {
                comment.CreatedByName =
                    applicationUser.Email != null ? applicationUser.Email : "REVIEWER";
            }
        }

        public async Task CreateMailForListUsersForSurvey(
            ICollection<ApplicationUserModel> users,
            int surveyId,
            ILogger log
        )
        {
            await _mailRepo.CreateMailForAllUsers(users, surveyId, log);
        }

        public async Task CreateMailForSupplierAnnouncement(
            AnnouncementModel model,
            string userId,
            ICollection<ApplicationUserModel> users
        )
        {
            await _mailRepo.CreateMailsForAnnouncement(model, userId, users);
        }
    }
}
