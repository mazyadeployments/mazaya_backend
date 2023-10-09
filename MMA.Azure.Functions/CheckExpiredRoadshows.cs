using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using MMA.WebApi.Shared.Interfaces.Offers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MMA.Azure.Functions
{
    public class CheckExpiredRoadshows
    {
        private readonly IRoadshowService _roadshowService;

        public CheckExpiredRoadshows(IRoadshowService roadshowService)
        {
            _roadshowService = roadshowService;
        }

        [FunctionName("CheckExpiredRoadshows")]
        public async Task RunAsync([TimerTrigger("0 0 20 * * *", RunOnStartup = true)] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"CheckExpiredRoadshows started at: {DateTime.Now}");
            try
            {
                await _roadshowService.CheckExpiredRoadshows(log);
            }
            catch (Exception e)
            {
                log.LogError("Azure function error -> " + e.ToString());
            }

            log.LogInformation($"CheckExpiredRoadshows done at: {DateTime.Now}");
        }
    }
}
