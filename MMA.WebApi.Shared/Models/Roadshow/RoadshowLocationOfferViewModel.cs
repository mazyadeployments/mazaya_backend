using System;

namespace MMA.WebApi.Shared.Models.Roadshow
{
    public class RoadshowLocationOfferViewModel
    {
        public int RoadshowId { get; set; }
        public int LocationId { get; set; }
        public int OfferId { get; set; }
        public string City { get; set; }
        public DateTime RoadShowStartDate { get; set; }
        public DateTime RoadShowEndDate { get; set; }
        public string OfferTitle { get; set; }
        public string OfferDescription { get; set; }
        public string CategoryTitle { get; set; }
        public string CountryName { get; set; }
        public Guid DocumentId { get; set; }
        public int ImageType { get; set; }
        public string ImageUrl { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime UpdatedOn { get; set; }

        public string UpdatedBy { get; set; }
    }
}
