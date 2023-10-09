namespace MMA.WebApi.DataAccess.Models
{
    public class RoadshowOfferCategory
    {
        public int RoadshowOfferId { get; set; }
        public RoadshowOffer RoadshowOffer { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
