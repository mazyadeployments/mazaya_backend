using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces
{
    public interface ILogAnalyticsRepository
    {
        Task LogOfferClick(int offerId, string userId);
        Task LogBannerClick(int offerId, string userId);
        Task LogSearchKeyword(string keyword, string userId);
        Task LogSearchOffers(IEnumerable<int> offersIds, string userId);
    }
}
