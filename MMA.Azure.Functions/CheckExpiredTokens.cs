using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using MMA.WebApi.Shared.Interfaces.ExpiredToken;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MMA.Azure.Functions
{
    public class CheckExpiredTokens
    {
        private readonly IExpiredTokenService _expiredTokenService;

        public CheckExpiredTokens(IExpiredTokenService expiredTokenService)
        {
            _expiredTokenService = expiredTokenService;
        }

        [FunctionName("CheckExpiredTokens")]
        public async Task RunAsync(
            [TimerTrigger("0 * * * * *", RunOnStartup = true)] TimerInfo myTimer,
            ILogger log
        )
        {
            log.LogInformation($"CheckExpiredTokens started at: {DateTime.Now}");
            try
            {
                await _expiredTokenService.DeleteExpiredTokens(log);
            }
            catch (Exception e)
            {
                log.LogError("Azure function error -> " + e.ToString());
            }

            log.LogInformation($"CheckExpiredTokens done at: {DateTime.Now}");
        }
    }
}
