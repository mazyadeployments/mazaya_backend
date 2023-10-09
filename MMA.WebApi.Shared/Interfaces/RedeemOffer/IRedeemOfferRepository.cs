using MMA.WebApi.Shared.Models.RedeemOffer;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace MMA.WebApi.Shared.Interfaces.RedeemOffer
{
    public interface IRedeemOfferRepository
    {
        Task<byte[]> GenerateQRCode(int offerId, string userId);
        Task<bool> RedeemQRCode(int offerId, string buyerId, string supplierId);
        Task<ICollection<RedeemedOfferCountModel>> GetRedeemedOfferCounts();

    }
}
