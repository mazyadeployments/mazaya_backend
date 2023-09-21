using System.Collections.Generic;

namespace MMA.WebApi.Shared.Models.Offer
{
    public class TransferOffersModel
    {
        public int CompanyId { get; set; }
        public IEnumerable<OfferModel> Offers { get; set; }
    }
}
