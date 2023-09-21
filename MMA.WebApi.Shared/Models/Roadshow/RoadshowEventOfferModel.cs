namespace MMA.WebApi.Shared.Models.Roadshow
{
    public class RoadshowEventOfferModel
    {
        public int RoadshowEventId { get; set; }
        public RoadshowEventModel RoadshowEvent { get; set; }
        public int RoadshowOfferId { get; set; }
        public RoadshowOfferModel RoadshowOffer { get; set; }
    }
}