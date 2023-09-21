using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using MMA.WebApi.Shared.Interfaces.Offers;
using System;
using System.Threading.Tasks;

namespace MMA.Azure.Functions
{
    public class OffersPushNotification
    {
        private readonly IOfferService _offerService;

        public OffersPushNotification(IOfferService offerService)
        {
            _offerService = offerService;
        }

        /*       [FunctionName("OffersPushNotification")]
               public async Task RunAsync(
                   [TimerTrigger("0 * * * * sun,tue,thu", RunOnStartup = true)] TimerInfo myTimer,
                   ILogger log
               )
               {
                   try
                   {
                       log.LogInformation($"OffersPushNotification started at: {DateTime.Now}");

                       var result = await _offerService.SendPushNotification();

                       log.LogInformation($"OffersPushNotification result: {result}");
                       log.LogInformation($"OffersPushNotification done at: {DateTime.Now}");
                   }
                   catch (Exception e)
                   {
                       log.LogInformation($"OffersPushNotification caused exception: {e.ToString()}");
                   }
               }
           */
    }
}
