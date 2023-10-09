using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Models.Banner;
using System.Linq;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.Banner
{
    public interface IBannerRepository : IQueryableRepository<BannerModel>
    {
        IQueryable<BannerViewModel> GetBannerViewModel();
        Task<int> GetBannerCount();
    }
}
