using System;
using System.Collections.Generic;

namespace MMA.WebApi.Shared.Models.OneAdnoc
{
    public class OfferViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Brand { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public decimal Price { get; set; }
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
        public string Longtitude { get; set; }
        public string Latitude { get; set; }
        public Enums.Declares.Tag? Tag { get; set; }
        public bool IsFavorite { get; set; }
        public decimal AverageRemark { get; set; }
        public int ReviewsCount { get; set; }

        public IEnumerable<OfferImageModel> Images { get; set; }
        public IEnumerable<OfferAttachmentModel> Attachments { get; set; }
    }
}
