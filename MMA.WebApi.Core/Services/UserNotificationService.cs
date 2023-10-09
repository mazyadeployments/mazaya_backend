using MMA.WebApi.Shared.Interfaces.UserNotifications;
using MMA.WebApi.Shared.Models.UserNotifications;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Core.Services
{
    public class UserNotificationService : IUserNotificationService
    {

        private readonly IUserNotificationRepository _userNotificationRepository;
        public UserNotificationService(IUserNotificationRepository userNotificationRepository)
        {
            _userNotificationRepository = userNotificationRepository;
        }

        public async Task<IEnumerable<UserNotificationModel>> GetNotificationForUser(string userId)
        {
            var notifications = await _userNotificationRepository.GetNotificationForUser(userId);
            return notifications;
        }

        public async Task<int> GetNotificationCountForUser(string userId)
        {
            return await _userNotificationRepository.GetNotificationCountForUser(userId);
        }

        public async Task AcknowledgeNotification(int id)
        {
            await _userNotificationRepository.AcknowledgeNotification(id);
        }

        public async Task AcknowledgeAllNotifications(string userId)
        {
            await _userNotificationRepository.AcknowledgeAllNotifications(userId);
        }

        public async Task CreateNotificationForPublishJob(string userId, int userCount, int surveyId)
        {
            await _userNotificationRepository.CreateNotificationForPublishJob(userId, userCount, surveyId);
        }

        public async Task CreateNotificationForOfferReport(int offerId, ICollection<string> usersId)
        {
            await _userNotificationRepository.CreateNotificationForOfferReport(offerId, usersId);
        }

        public async Task CreateNotificationForUserInvitation(string userId)
        {
            await _userNotificationRepository.CreateNotificationForUserInvitation(userId);
        }
    }
}
