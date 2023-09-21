using MMA.WebApi.Shared.Models.DefaultLocations;
using System;
using System.Collections.Generic;

namespace MMA.WebApi.Shared.Models.Roadshow
{
    public class RoadshowEventModel
    {
        public int Id { get; set; }
        public ICollection<RoadshowEventOfferModel> RoadshowEventOffers { get; set; } = new List<RoadshowEventOfferModel>();
        public int RoadshowOffersCount { get; set; }
        public ICollection<DefaultLocationModel> DefaultLocations { get; set; }
        public int RoadshowInviteId { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}