using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using MMA.WebApi.Shared.Interfaces.ApplicationUsers;
using MMA.WebApi.Shared.Interfaces.Membership;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MMA.Azure.Functions
{
    public class CheckMembershipECard
    {
        private readonly IMembershipECardService _membershipECardService;
        public CheckMembershipECard(IMembershipECardService membershipECardService)
        {
            _membershipECardService = membershipECardService;
        }
        [FunctionName("CheckMembershipECard")]
        public async Task RunAsync([TimerTrigger("0 0 0 * * *", RunOnStartup = true)] TimerInfo myTimer, ILogger log)
        {
            try
            {
                log.LogInformation($"CheckMembershipECard started at: {DateTime.Now}");

                await _membershipECardService.CheckMembershipECardsValidTo();

                log.LogInformation($"CheckMembershipECard done at: {DateTime.Now}");
            }
            catch (Exception e)
            {
                log.LogError("Azure Function:" + e.ToString());
            }
        }
    }
}

