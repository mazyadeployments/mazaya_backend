using MMA.WebApi.Shared.Models.Offer;

namespace MMA.WebApi.Shared.Models
{
    public class ReportedOfferModel
    {
        public OfferModel offerModel { get; set; }
        public int ReportCount { get; set; }
    }
}
