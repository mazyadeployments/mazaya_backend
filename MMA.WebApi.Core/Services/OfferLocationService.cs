using MMA.WebApi.Shared.Interfaces.OfferLocations;
using MMA.WebApi.Shared.Models.Offer;
using System.Threading.Tasks;

namespace MMA.WebApi.Core.Services
{
    public class OfferLocationService : IOfferLocationService
    {
        private readonly IOfferLocationRepository _offerLocationRepository;
        public OfferLocationService(IOfferLocationRepository offerLocationRepository)
        {
            _offerLocationRepository = offerLocationRepository;
        }
        public async Task CreateOfferLocation(OfferLocationModel model)
        {
            await _offerLocationRepository.CreateAsync(model);
        }
    }
}
