using MMA.WebApi.Shared.Models.MailStorage;
using MMA.WebApi.Shared.Models.UserNotifications;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.UserNotifications
{
    public interface IUserNotificationRepository
    {
        Task<IQueryable<UserNotificationModel>> GetNotificationForUser(string userId);
        Task AcknowledgeNotification(int id);
        Task CreateUserNotification(EmailDataModel emailDataModel, string title, string message);
        Task AcknowledgeAllNotifications(string userId);
        Task<int> GetNotificationCountForUser(string userId);
        Task CreateNotificationForPublishJob(string userId, int userCount, int surveyId);
        Task CreateNotificationsForSurveys(ICollection<EmailDataModel> emailDataModel);//, string title, string message);
        Task CreateNotificationForOfferReport(int offerId, ICollection<string> usersId);
        Task CreateNotificationForUserInvitation(string userId);
    }
}
