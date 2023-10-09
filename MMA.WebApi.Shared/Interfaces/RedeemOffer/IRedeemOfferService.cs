using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.RedeemOffer
{
    public interface IRedeemOfferService
    {
        Task<byte[]> GenerateQRCode(int offerId, string userId);
        Task<bool> RedeemQRCode(int offerId, string userId, string creatorId);
    }
}
