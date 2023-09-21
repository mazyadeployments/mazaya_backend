using MMA.WebApi.Shared.Models.Country;
using MMA.WebApi.Shared.Models.Offer;
using System;
using System.Collections.Generic;

namespace MMA.WebApi.Shared.Models.Location
{
    public class LocationModel
    {
        public int Id { get; set; }

        public CountryModel Country { get; set; }

        public string City { get; set; }
        public DateTime RoadShowStartDate { get; set; }
        public DateTime RoadShowEndDate { get; set; }
        public IEnumerable<OfferModel> Offers { get; set; }
        public int RoadshowId { get; set; }
    }
}
