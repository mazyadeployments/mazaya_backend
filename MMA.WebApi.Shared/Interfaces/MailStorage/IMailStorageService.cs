using Microsoft.Extensions.Logging;
using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Models.Announcement;
using MMA.WebApi.Shared.Models.ApplicationUser;
using MMA.WebApi.Shared.Models.Comments;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.MailStorage;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.MailStorage
{
    public interface IMailStorageService
    {
        Task CheckMessageQueue();
        Task CreateMail(EmailDataModel emailDataModel);
        Task CreateMailForAdnocTeam(EmailDataModel emailDataModel);
        Task DeleteListAsync(IEnumerable<int> list);
        Task<PaginationListModel<MailStorageModel>> SearchMailStorages(QueryModel queryModel);
        Task<MailStorageModel> GetMailStorageDetails(int id);
        Task CreateShareMail(string userId, string userMail, string subject, string body);
        EmailDataModel CreateMailData(
            string userId,
            int? offerId,
            string companyName,
            Declares.MessageTemplateList messageTemplate,
            bool isApproved
        );
        Task SetCreatedByName(string deletedUser, CommentBaseModel comment);
        Task CreateMailForListUsersForSurvey(
            ICollection<ApplicationUserModel> users,
            int surveyId,
            ILogger log
        );
        Task CreateMailForSupplierAnnouncement(
            AnnouncementModel model,
            string userId,
            ICollection<ApplicationUserModel> users
        );
        Task<EmailDataModel> CreateMailDataForRoadshow(
            string userId,
            string userEmail,
            int roadshowId,
            Declares.MessageTemplateList messageTemplate,
            bool isApproved
        );
    }
}
