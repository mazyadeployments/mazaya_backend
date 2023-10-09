using System;

namespace MMA.WebApi.Shared.Models.OneAdnoc
{
    public class CollectionViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Enums.Declares.Tag? Tag { get; set; }
        public DateTime? ValidUntil { get; set; }
        public string Location { get; set; }
        public string SponsorLogoImageUrl { get; set; }
        public string ImageUrl { get; set; }
        public int OffersCount { get; set; }
    }
}
