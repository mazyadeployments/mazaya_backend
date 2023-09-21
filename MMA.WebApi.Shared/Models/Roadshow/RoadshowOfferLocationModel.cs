namespace MMA.WebApi.Shared.Models.Roadshow
{
    public class RoadshowOfferLocationModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string Address { get; set; }
        public string Vicinity { get; set; }
        public string Country { get; set; }
        public int RoadshowOfferId { get; set; }
        public virtual RoadshowOfferModel RoadshowOffer { get; set; }
    }
}
