using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using MMA.WebApi.Shared.Interfaces.Announcement;
using MMA.WebApi.Shared.Interfaces.ApplicationUsers;
using MMA.WebApi.Shared.Interfaces.MailStorage;
using System;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Constants.ErrorConstants;
using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.Azure.Functions
{
    public class CheckNewAnnouncements
    {
        private readonly IAnnouncementService _announcementService;
        private readonly IApplicationUserService _applicationUserService;
        private readonly IMailStorageService _mailStorageService;

        public CheckNewAnnouncements(
            IAnnouncementService announcementService,
            IApplicationUserService applicationUserService,
            IMailStorageService mailStorageService
        )
        {
            _announcementService = announcementService;
            _applicationUserService = applicationUserService;
            _mailStorageService = mailStorageService;
        }

        [FunctionName("CheckNewAnnouncements")]
        public async Task RunAsync(
            [TimerTrigger("0 */1 * * * *", RunOnStartup = true)] TimerInfo myTimer,
            ILogger log
        )
        {
            log.LogInformation($"CheckNewAnnouncements started at: {DateTime.Now}");

            var announcement = await _announcementService.GetAnnouncementByStatus(
                AnnouncementStatus.Process
            );
            if (announcement != null)
            {
                var users = await _applicationUserService.FilterUsersForAnnouncement(announcement);
                await _mailStorageService.CreateMailForSupplierAnnouncement(
                    announcement,
                    announcement.CreatedBy,
                    users
                );
                await _announcementService.ChangeAnnouncementStatus(
                    AnnouncementStatus.Pending,
                    announcement.Id,
                    users.Count,
                    null,
                    null
                );
            }

            log.LogInformation($"CheckNewAnnouncements done at: {DateTime.Now}");
        }
    }
}
