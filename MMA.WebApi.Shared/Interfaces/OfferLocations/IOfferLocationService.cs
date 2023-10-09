using MMA.WebApi.Shared.Models.Offer;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.OfferLocations
{

    public interface IOfferLocationService
    {
        Task CreateOfferLocation(OfferLocationModel model);
    }
}
