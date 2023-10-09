using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using MMA.WebApi.Shared.Interfaces.MailStorage;
using System;
using System.Threading.Tasks;

namespace MMA.Azure.Functions
{
    public class CheckMessageQueue
    {
        private readonly IMailStorageRepository mailStorageRepository;

        public CheckMessageQueue(IMailStorageRepository mailStorageRepository)
        {
            this.mailStorageRepository = mailStorageRepository;
        }

        [FunctionName("CheckMessageQueue")]
        public async Task RunAsync(
            [TimerTrigger("0 */1 * * * *", RunOnStartup = true)] TimerInfo myTimer,
            ILogger log
        )
        {
            log.LogInformation($"CheckMessageQueue started at: {DateTime.Now}");

            await mailStorageRepository.CheckMessageQueue();

            log.LogInformation($"CheckMessageQueue done at: {DateTime.Now}");
        }
    }
}
