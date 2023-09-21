using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using MMA.WebApi.Shared.Interfaces.Offers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MMA.Azure.Functions
{
    public class OfferAboutToExpire
    {
        private readonly IOfferService _offerService;
        public OfferAboutToExpire(IOfferService offerService)
        {
            _offerService = offerService;
        }

        [FunctionName("OfferAboutToExpire")]
        public async Task RunAsync([TimerTrigger("0 0 20 * * *", RunOnStartup = true)] TimerInfo myTimer, ILogger log)
        {
            try
            {
                log.LogInformation($"CheckExpiredOffers started at: {DateTime.Now}");

                await _offerService.OffersAboutToExpire();

                log.LogInformation($"CheckExpiredOffers done at: {DateTime.Now}");
            }
            catch (Exception e)
            {
                log.LogInformation($"CheckExpiredOffers caused exception: {e.ToString()}");
            }
        }
    }
}
