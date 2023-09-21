namespace MMA.WebApi.DataAccess.Models
{
    public class OfferCollection
    {
        public int OfferId { get; set; }
        public Offer Offer { get; set; }
        public int CollectionId { get; set; }
        public Collection Collection { get; set; }
    }
}
