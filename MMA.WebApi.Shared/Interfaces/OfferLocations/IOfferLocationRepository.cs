using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Models.Offer;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.OfferLocations
{
    public interface IOfferLocationRepository : IQueryableRepository<OfferLocationModel>
    {
        Task CreateAsync(OfferLocationModel model);
    }
}
