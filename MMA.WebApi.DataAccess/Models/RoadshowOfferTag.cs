namespace MMA.WebApi.DataAccess.Models
{
    public class RoadshowOfferTag
    {
        public int RoadshowOfferId { get; set; }
        public RoadshowOffer RoadshowOffer { get; set; }
        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
