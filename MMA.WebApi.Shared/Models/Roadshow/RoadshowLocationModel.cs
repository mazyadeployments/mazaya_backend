using System.Collections.Generic;

namespace MMA.WebApi.Shared.Models.Roadshow
{
    public class RoadshowLocationModel
    {
        public int Id { get; set; }
        public int DefaultLocationId { get; set; }
        public int OffersCount { get; set; }
        public string Address { get; set; }
        public string Vicinity { get; set; }
        public string Country { get; set; }
        public string Title { get; set; }
        public List<RoadshowOfferCardModel> RoadshowOffers { get; set; }
        public List<RoadshowOfferMobileModel> RoadshowOffersMobile { get; set; } = new List<RoadshowOfferMobileModel>();
    }
}