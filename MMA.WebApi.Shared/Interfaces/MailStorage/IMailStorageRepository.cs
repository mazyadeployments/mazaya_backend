using Microsoft.Extensions.Logging;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Models.Announcement;
using MMA.WebApi.Shared.Models.ApplicationUser;
using MMA.WebApi.Shared.Models.MailStorage;
using MMA.WebApi.Shared.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.WebApi.Shared.Interfaces.MailStorage
{
    public interface IMailStorageRepository
    {
        Task<MailStorageModel> GetSingleAsync(Expression<Func<MailStorageModel, bool>> query);
        Task CreateMail(EmailDataModel emailDataModel);
        Task CreateShareMail(string userId, string userMail, string subject, string body);
        Task CheckMessageQueue();
        Task DeleteListAsync(IEnumerable<int> list);
        IQueryable<MailStorageModel> Get(QueryModel queryModel);
        EmailDataModel CreateMailData(
            InviteUsersModel invitedUser,
            MessageTemplateList messageTemplateList
        );
        Task CreateMailForAllUsers(
            ICollection<ApplicationUserModel> allUsers,
            int surveyId,
            ILogger log
        );
        Task CreateEmialInvitationForUsersFromSpecificExcelFile(
            IEnumerable<EmailDataModel> mailModels,
            string userId
        );
        Task CreateMailsForAnnouncement(
            AnnouncementModel model,
            string userId,
            ICollection<ApplicationUserModel> users
        );
        Task<Tuple<int, int>> GetCountSentAnnouncement(int announcementId);
    }
}
