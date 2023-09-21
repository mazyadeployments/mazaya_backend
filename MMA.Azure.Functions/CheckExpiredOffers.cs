using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using MMA.WebApi.Shared.Interfaces.Offers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MMA.Azure.Functions
{
    public class CheckExpiredOffers
    {
        private readonly IOfferService _offerService;
        public CheckExpiredOffers(IOfferService offerService)
        {
            _offerService = offerService;
        }

        [FunctionName("CheckExpiredOffers")]
        public async Task RunAsync([TimerTrigger("0 0 20 * * *", RunOnStartup = true)] TimerInfo myTimer, ILogger log)
        {
            try
            {
                log.LogInformation($"CheckExpiredOffers started at: {DateTime.Now}");

                await _offerService.CheckExpiredOffers(log);

                log.LogInformation($"CheckExpiredOffers done at: {DateTime.Now}");
            }
            catch (Exception e)
            {
                log.LogInformation($"CheckExpiredOffers caused exception: {e.ToString()}");
            }
        }
    }
}
