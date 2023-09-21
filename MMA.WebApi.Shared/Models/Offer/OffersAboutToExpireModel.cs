using System.Collections.Generic;

namespace MMA.WebApi.Shared.Models.Offer
{
    public class OffersAboutToExpireModel
    {
        public IEnumerable<OfferModel> ThreeWeeksBeforeExpiration { get; set; }
        public IEnumerable<OfferModel> WeekBeforeExpiration { get; set; }
        public IEnumerable<OfferModel> DayBeforeExpiration { get; set; }
    }
}
