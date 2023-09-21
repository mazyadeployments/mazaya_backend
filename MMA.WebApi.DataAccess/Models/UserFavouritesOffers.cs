using System;

namespace MMA.WebApi.DataAccess.Models
{
    public class UserFavouritesOffer
    {
        public int OfferId { get; set; }
        public Offer Offer { get; set; }
        public string ApplicationUserId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsFavourite { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
