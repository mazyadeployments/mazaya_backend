using System.Collections.Generic;

namespace MMA.WebApi.Shared.Models.Offer
{
    public class OneHubOfferModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Brand { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public decimal? Price { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int SellerId { get; set; }
        public string SellerName { get; set; }
        public string Longtitude { get; set; }
        public string Latitude { get; set; }
        public string OfferUrl { get; set; }
        public List<OneHubOfferImageModel> Images { get; set; }
    }
}
