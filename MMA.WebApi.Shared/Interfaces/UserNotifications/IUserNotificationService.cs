using MMA.WebApi.Shared.Models.UserNotifications;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.UserNotifications
{
    public interface IUserNotificationService
    {
        Task<IEnumerable<UserNotificationModel>> GetNotificationForUser(string userId);
        Task AcknowledgeNotification(int id);
        Task AcknowledgeAllNotifications(string userId);
        Task<int> GetNotificationCountForUser(string userId);
        Task CreateNotificationForOfferReport(int offerId, ICollection<string> usersId);
        Task CreateNotificationForPublishJob(string userId, int userCount, int surveyId);
        Task CreateNotificationForUserInvitation(string userId);
    }
}
