using MMA.WebApi.Shared.Models.Banner;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.Banner
{
    public interface IBannerService
    {
        IEnumerable<BannerViewModel> GetBanners(int limit);
        Task CreateOrUpdateBanner(List<int> offerIds, string userId);
    }
}
