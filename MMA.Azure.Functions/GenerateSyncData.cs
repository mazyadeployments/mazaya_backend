using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using MMA.WebApi.Shared.Interfaces.Offers;
using MMA.WebApi.Shared.Models.Mobile;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MMA.Azure.Functions
{
    public class GenerateSyncData
    {
        private readonly IOfferService _offerService;
        private readonly IMobileCacheDataService _mobileCacheDataService;

        public GenerateSyncData(IOfferService offerService,
                                IMobileCacheDataService mobileCacheDataService)
        {
            _offerService = offerService;
            _mobileCacheDataService = mobileCacheDataService;
        }


        [FunctionName("GenerateSyncData")]
        public async Task RunAsync([TimerTrigger("0 */1 * * * *", RunOnStartup = true)] TimerInfo myTimer, ILogger log)
        {
            try
            {
                log.LogInformation($"GenerateSyncData started at: {DateTime.Now}");

                var syncData = await MakeSyncData();

                log.LogInformation($"GenerateSyncData result: {syncData}");
                log.LogInformation($"GenerateSyncData done at: {DateTime.Now}");

                await _mobileCacheDataService.SetMobileCacheData(syncData);
                log.LogInformation($"MemoryCache set with SyncData result at: {DateTime.Now}");
            }
            catch (Exception e)
            {
                log.LogInformation($"GenerateSyncData caused exception: {e.ToString()}");
            }
        }

        private async Task<SynchronizationDataModel> MakeSyncData()
        {
            return await _offerService.GenerateSynchronizationData();
        }
    }
}
