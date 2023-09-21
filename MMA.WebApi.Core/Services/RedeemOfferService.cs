using MMA.WebApi.Shared.Interfaces.Offers;
using MMA.WebApi.Shared.Interfaces.RedeemOffer;
using System.Threading.Tasks;

namespace MMA.WebApi.Core.Services
{
    class RedeemOfferService : IRedeemOfferService
    {
        private readonly IRedeemOfferRepository _redeemOfferRepository;
        private readonly IOfferRepository _offerRepository;
        public RedeemOfferService(IRedeemOfferRepository redeemOfferRepository, IOfferRepository offerRepository)
        {
            _redeemOfferRepository = redeemOfferRepository;
            _offerRepository = offerRepository;
        }
        public async Task<byte[]> GenerateQRCode(int offerId, string userId)
        {
            var img = await _redeemOfferRepository.GenerateQRCode(offerId, userId);

            return img;
        }

        public async Task<bool> RedeemQRCode(int offerId, string userId, string creatorId)
        {
            return await _redeemOfferRepository.RedeemQRCode(offerId, userId, creatorId);
        }


    }
}
