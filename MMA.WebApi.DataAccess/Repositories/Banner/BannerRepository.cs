using Microsoft.EntityFrameworkCore;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.DataAccess.Repository.Offers;
using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Interfaces.Banner;
using MMA.WebApi.Shared.Models.Banner;
using MMA.WebApi.Shared.Models.Image;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MMA.WebApi.DataAccess.Repositories.Banners
{
    public class BannerRepository : BaseRepository<BannerModel>, IBannerRepository
    {
        private const string MIGRATION_DATE = "2021-02-05";
        public BannerRepository(Func<MMADbContext> contexFactory) : base(contexFactory) { }

        public IQueryable<BannerViewModel> GetBannerViewModel()
        {
            var context = ContextFactory();


            var banner = context.Offer
                .Where(x => x.BannerActive.Value && x.ValidUntil > DateTime.UtcNow && x.ValidFrom <= DateTime.UtcNow && x.Status == Declares.OfferStatus.Approved.ToString())
                .OrderBy(x => x.UpdatedOn)
                .Select(offer => new BannerViewModel()
                {
                    OfferId = offer.Id,
                    OfferTitle = FormatTitle(offer.Title, offer.CreatedOn, offer.Company.NameEnglish),
                    TagTitle = offer.OfferTags.FirstOrDefault().Tag.Title,
                    CompanyTitle = offer.Company.NameEnglish,
                    BrandTitle = offer.Brand,
                    Images = offer.OfferDocuments.Where(od => (int)od.Type != 1)
                                           .Select(od => new ImageModel
                                           {
                                               Id = !string.IsNullOrEmpty(od.DocumentId.ToString()) ? od.DocumentId.ToString() : string.Empty,
                                               Type = od.Type,
                                               OriginalImageId = od.OriginalImageId,
                                               CropCoordinates = new CropCoordinates
                                               {
                                                   X1 = od.X1,
                                                   X2 = od.X2,
                                                   Y1 = od.Y1,
                                                   Y2 = od.Y2
                                               },
                                               CropNGXCoordinates = new CropCoordinates
                                               {
                                                   X1 = od.cropX1,
                                                   X2 = od.cropX2,
                                                   Y1 = od.cropY1,
                                                   Y2 = od.cropY2
                                               },
                                           }).ToList(),
                    DiscountFrom = offer.DiscountFrom,
                    DiscountTo = offer.DiscountTo,
                    PriceFrom = offer.PriceFrom,
                    PriceTo = offer.PriceTo,

                    OriginalPrice = (offer.OriginalPrice == offer.DiscountedPrice) || ((offer.DiscountedPrice.HasValue && !offer.OriginalPrice.HasValue) || (offer.OriginalPrice.HasValue && !offer.DiscountedPrice.HasValue)) ? null : offer.OriginalPrice,
                    Discount = (!offer.Discount.HasValue && offer.Discount.Value <= 0) && (offer.OriginalPrice == offer.DiscountedPrice) || ((offer.DiscountedPrice.HasValue && !offer.OriginalPrice.HasValue) || (offer.OriginalPrice.HasValue && !offer.DiscountedPrice.HasValue)) ? null : offer.Discount,
                    DiscountedPrice = (offer.OriginalPrice == offer.DiscountedPrice) || ((offer.DiscountedPrice.HasValue && !offer.OriginalPrice.HasValue) || (offer.OriginalPrice.HasValue && !offer.DiscountedPrice.HasValue)) ? null : offer.DiscountedPrice,
                    PriceCustom = CheckIfCustomPrice(offer.Discount, offer.DiscountedPrice, offer.OriginalPrice, offer.PriceCustom),
                    BannerUrl = offer.BannerUrl,
                    PriceType = OfferRepository.GetPriceType(offer),
                });
            return banner;
        }




        private static string CheckIfCustomPrice(decimal? discount, decimal? discountedPrice, decimal? originalPrice, string priceCustom)
        {
            var price = string.Empty;
            if (!string.IsNullOrWhiteSpace(priceCustom)) price = priceCustom;

            if ((!discount.HasValue || discount <= 0) && (discountedPrice.HasValue && originalPrice.HasValue && discountedPrice.Value > 0 && originalPrice.Value > 0 && (discountedPrice.Value == originalPrice.Value)) ||
                (discountedPrice.HasValue && !originalPrice.HasValue) || (originalPrice.HasValue && !discountedPrice.HasValue))
            {
                var value = originalPrice ?? discountedPrice;
                price = "AED " + value.Value.ToString("#.##");
            }

            return price;
        }


        /// <summary>
        /// We include company names for offers only if it is offer from old system
        /// Migration of DB was done on 04.02.2021. (Set in const MIGRATION_DATE)
        /// </summary>
        /// <param name="offer"></param>
        /// <returns></returns>
        private static string FormatTitle(string title, DateTime offerCreationDate, string companyName)
        {
            return offerCreationDate < DateTime.Parse(MIGRATION_DATE) ? companyName : title;
        }

        protected override IQueryable<BannerModel> GetEntities()
        {
            throw new NotImplementedException();
        }

        public IQueryable<BannerModel> Get()
        {
            throw new NotImplementedException();
        }

        public async Task<int> GetBannerCount()
        {
            var context = ContextFactory();

            return await context.Offer.Where(x => x.BannerActive.Value).CountAsync();
        }
    }
}
