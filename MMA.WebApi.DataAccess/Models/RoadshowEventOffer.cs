namespace MMA.WebApi.DataAccess.Models
{
    public class RoadshowEventOffer
    {
        public int RoadshowEventId { get; set; }
        public RoadshowEvent RoadshowEvent { get; set; }
        public int RoadshowOfferId { get; set; }
        public RoadshowOffer RoadshowOffer { get; set; }
    }
}
