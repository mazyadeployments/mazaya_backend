using MMA.WebApi.Shared.Models.Attachment;
using MMA.WebApi.Shared.Models.Image;
using System;
using System.Collections.Generic;

namespace MMA.WebApi.Shared.Models.Offer
{
    public class ADNOCOneOfferModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Brand { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public decimal? Price { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidUntil { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string WhatYouGet { get; set; }
        public string PriceList { get; set; }
        public string FinePrint { get; set; }
        public int CategoryId { get; set; }
        public int? CollectionId { get; set; }
        public int SellerId { get; set; }
        public string SellerName { get; set; }
        public decimal Longtitude { get; set; }
        public decimal Latitude { get; set; }
        public MMA.WebApi.Shared.Enums.Declares.Tag? Tag { get; set; }
        public bool IsFavorite { get; set; }
        public decimal AverageRemark { get; set; }
        public int ReviewsCount { get; set; }

        public DateTime? LastUpdateOn;

        public List<OfferImageModel> Images { get; set; }
        public List<AttachmentModel> Attachments { get; set; }
        public List<ImageUrlsModel> ImageUrls { get; set; }
    }
}
