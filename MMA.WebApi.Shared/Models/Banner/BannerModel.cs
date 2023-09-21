using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Models.Offer;
using System;
using System.Collections.Generic;

namespace MMA.WebApi.Shared.Models.Banner
{
    public class BannerModel
    {
        public int Id { get; set; }
        public IList<OfferModel> Offers { get; set; }
        public IList<int> OffersIds { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public QueryModel QueryModel { get; set; }
    }
}
