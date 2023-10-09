using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.LogAnalytics
{
    public interface ILogAnalyticsService
    {
        Task LogOfferClick(int offerId, string userId);
        Task LogBannerClick(int offerId, string userId);
        Task LogSearchKeywordAdnOffer(string keyword, IEnumerable<int> offerIds, string userId);
    }
}
