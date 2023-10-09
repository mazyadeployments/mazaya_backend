using MMA.WebApi.Shared.Models.Image;
using System.Collections.Generic;

namespace MMA.WebApi.Shared.Models.Banner
{
    public class BannerViewModel
    {
        public int OfferId { get; set; }
        public string OfferTitle { get; set; }
        public string TagTitle { get; set; }
        public string PriceType { get; set; }
        public string CompanyTitle { get; set; }
        public string BrandTitle { get; set; }
        public IEnumerable<ImageModel> Images { get; set; }
        public List<ImageUrlsModel> ImageUrls { get; set; }
        public decimal? PriceFrom { get; set; }
        public decimal? PriceTo { get; set; }
        public decimal? OriginalPrice { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public decimal? Discount { get; set; }
        public decimal? DiscountFrom { get; set; }
        public decimal? DiscountTo { get; set; }
        public string PriceCustom { get; set; }
        public string MainImage { get; set; }
        public string BannerUrl { get; set; }
    }
}
