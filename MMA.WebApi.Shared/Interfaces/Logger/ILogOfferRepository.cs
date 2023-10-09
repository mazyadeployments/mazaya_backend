using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.Logger
{
    public interface ILogOfferRepository
    {
        Task LogOfferClick(int offerId, string userId);
        Task LogBannerClick(int offerId, string userId);
        Task LogSearchKeyword(string keyword, string userId);
        Task LogSearchOffers(List<int> offersIds, string userId);
    }
}
