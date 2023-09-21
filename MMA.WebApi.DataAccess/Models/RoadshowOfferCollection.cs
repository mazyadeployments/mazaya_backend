namespace MMA.WebApi.DataAccess.Models
{
    public class RoadshowOfferCollection
    {
        public int RoadshowOfferId { get; set; }
        public RoadshowOffer RoadshowOffer { get; set; }
        public int CollectionId { get; set; }
        public Collection Collection { get; set; }
    }
}
