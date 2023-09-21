using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using MMA.WebApi.Shared;
using MMA.WebApi.Shared.Interfaces.Announcement;
using System;
using System.Threading.Tasks;

namespace MMA.Azure.Functions
{
    public class CheckAnnouncementMailsCount
    {
        private readonly IAnnouncementService _announcementService;

        public CheckAnnouncementMailsCount(IAnnouncementService announcementService)
        {
            _announcementService = announcementService;
        }

        [FunctionName("CheckAnnouncementMailsCount")]
        public async Task Run(
            [TimerTrigger("0 */1 * * * *", RunOnStartup = true)] TimerInfo myTimer,
            ILogger log
        )
        {
            log.LogInformation($"CheckAnnouncementMailsCount started at: {DateTime.Now}");
            var announcement = await _announcementService.GetAnnouncementByStatus(
                WebApi.Shared.Enums.Declares.AnnouncementStatus.Pending
            );
            if (announcement != null)
                await _announcementService.SetSentMailsCounts(announcement.Id);

            log.LogInformation($"CheckAnnouncementMailsCount done at: {DateTime.Now}");
        }
    }
}
