using System;

namespace MMA.WebApi.Shared.Models.Offer
{
    public class OfferImageModel
    {
        public int Id { get; set; }
        public int Index { get; set; }
        public string ImageUrl { get; set; }
        public string CreatedById { get; set; }
        public DateTime CreatedOn { get; set; }
        public string LastUpdateById { get; set; }
        public DateTime LastUpdateOn { get; set; }

    }
}
