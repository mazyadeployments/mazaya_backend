namespace MMA.WebApi.DataAccess.Models
{
    public class OfferLocation
    {
        public int Id { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string Address { get; set; }
        public string Vicinity { get; set; }
        public string Country { get; set; }
        public int OfferId { get; set; }
        public int DefaultAreaId { get; set; }
        public virtual DefaultArea DefaultArea { get; set; }
    }
}
