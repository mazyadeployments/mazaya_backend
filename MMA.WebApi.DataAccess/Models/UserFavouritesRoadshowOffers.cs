using System;

namespace MMA.WebApi.DataAccess.Models
{
    public class UserFavouritesRoadshowOffer
    {
        public int RoadshowOfferId { get; set; }
        public RoadshowOffer RoadshowOffer { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsFavourite { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
