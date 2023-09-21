using MMA.WebApi.Shared.Interfaces;
using MMA.WebApi.Shared.Interfaces.LogAnalytics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Core.Services
{
    public class LogAnalyticsService : ILogAnalyticsService
    {
        private readonly ILogAnalyticsRepository _logAnalyticsRepository;
        public LogAnalyticsService(ILogAnalyticsRepository logAnalyticsRepository)
        {
            _logAnalyticsRepository = logAnalyticsRepository;
        }

        public Task LogBannerClick(int offerId, string userId)
        {
            return _logAnalyticsRepository.LogBannerClick(offerId, userId);

        }

        public async Task LogOfferClick(int offerId, string userId)
        {
            await _logAnalyticsRepository.LogOfferClick(offerId, userId);
        }

        public async Task LogSearchKeywordAdnOffer(string keyword, IEnumerable<int> offerIds, string userId)
        {
            if (string.IsNullOrEmpty(keyword))
                return;
            await _logAnalyticsRepository.LogSearchKeyword(keyword, userId);
            await _logAnalyticsRepository.LogSearchOffers(offerIds, userId);


        }
    }
}
