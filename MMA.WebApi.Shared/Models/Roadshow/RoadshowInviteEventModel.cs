using MMA.WebApi.Shared.Models.DefaultLocations;
using System;
using System.Collections.Generic;

namespace MMA.WebApi.Shared.Models.Roadshow
{
    public class RoadshowInviteEventModel
    {
        public int Id { get; set; }
        public ICollection<RoadshowOfferModel> RoadshowOffers { get; set; } = new List<RoadshowOfferModel>();
        public int RoadshowOffersCount { get; set; }
        public DefaultLocationModel DefaultLocation { get; set; }
        public int RoadshowInviteId { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}
