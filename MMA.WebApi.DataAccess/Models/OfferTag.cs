namespace MMA.WebApi.DataAccess.Models
{
    public class OfferTag
    {
        public int OfferId { get; set; }
        public Offer Offer { get; set; }
        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
