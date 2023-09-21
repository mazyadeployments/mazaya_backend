using MMA.WebApi.Shared.Models.Attachment;
using MMA.WebApi.Shared.Models.Banner;
using MMA.WebApi.Shared.Models.Category;
using MMA.WebApi.Shared.Models.Collection;
using MMA.WebApi.Shared.Models.Comments;
using MMA.WebApi.Shared.Models.Companies;
using MMA.WebApi.Shared.Models.Image;
using MMA.WebApi.Shared.Models.Membership;
using MMA.WebApi.Shared.Models.Offer;
using MMA.WebApi.Shared.Models.OfferDocument;
using MMA.WebApi.Shared.Models.Ratings;
using MMA.WebApi.Shared.Models.Tag;
using System;
using System.Collections.Generic;

namespace MMA.WebApi.Shared.Models.OneHubModels
{
    public class OfferOneHubModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public virtual IEnumerable<CategoryModel> Categories { get; set; }
        public virtual IEnumerable<CollectionModel> Collections { get; set; }
        public virtual IEnumerable<TagModel> Tags { get; set; }
        public virtual IEnumerable<OfferRatingModel> OfferRating { get; set; }
        public int? BannerId { get; set; }
        public virtual BannerModel Banner { get; set; }
        public int OfferAgreementId { get; set; }
        public int CompanyId { get; set; }
        public string PromotionCode { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public decimal? PriceFrom { get; set; }
        public decimal? PriceTo { get; set; }
        public decimal? DiscountFrom { get; set; }
        public decimal? DiscountTo { get; set; }
        public string PriceCustom { get; set; }
        public string Status { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidUntil { get; set; }
        public decimal AverageRatingOfAllCompanyOffers { get; set; }
        public decimal NumberOfVotesOnCompanyOffers { get; set; }
        public decimal RatingPercent { get; set; }
        public decimal Rating { get; set; }
        public int? Votes { get; set; }
        public string WhatYouGet { get; set; }
        public string PriceList { get; set; }
        public string TermsAndCondition { get; set; }
        public string AboutCompany { get; set; }
        public ICollection<OfferDocumentModel> OfferDocuments { get; set; }
        public virtual IEnumerable<OfferLocationModel> Locations { get; set; }
        public virtual int LocationsCount { get; set; }
        public bool FlagIsLatest { get; set; }
        public bool FlagIsWeekendOffer { get; set; }
        public string MainImage { get; set; }
        public bool? BannerActive { get; set; }
        public string BannerUrl { get; set; }
        public bool? AnnouncementActive { get; set; }
        public string Image { get; set; }
        public string QRcodeImage { get; set; }
        public ImageModel AnnouncementImage { get; set; }
        public List<AttachmentModel> Attachments { get; set; }
        public List<ImageUrlsModel> ImageUrls { get; set; }
        public virtual IList<CommentModel> Comments { get; set; }
        public string Action { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime ReviewedOn { get; set; }
        public string ReviewedBy { get; set; }
        public DateTime DecisionOn { get; set; }
        public string DecisionBy { get; set; }
        public decimal? OriginalPrice { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public string Type { get; set; }
        public decimal? Discount { get; set; }
        public bool IsFavourite { get; set; }
        public bool IsAlreadyRated { get; set; }
        public string CompanyNameEnglish { get; set; }
        public string CompanyOfficialEmail { get; set; }
        public string CompanyNameArabic { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyPOBox { get; set; }
        public string CompanyWebsite { get; set; }
        public string CompanyPhoneNumber { get; set; }
        public string CompanyLogo { get; set; }
        public PhoneNumberModel PhoneNumber { get; set; }
        public int ReportCount { get; set; }
        public MembershipModel[] MembershipType { get; set; }
        public bool isPrivate { get; set; }
        public string PriceFiled { get; set; }
        public string OfferUrl { get; set; }
        public OneHubCompanyModel CompanyModel { get; set; }
    }

    public class OfferOneHubCardModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public virtual IEnumerable<CategoryModel> Categories { get; set; }
        public virtual IEnumerable<CollectionModel> Collections { get; set; }
        public virtual IEnumerable<TagModel> Tags { get; set; }
        public string City { get; set; }
        public decimal? PriceFrom { get; set; }
        public decimal? PriceTo { get; set; }
        public decimal? DiscountFrom { get; set; }
        public decimal? DiscountTo { get; set; }
        public string PriceCustom { get; set; }
        public string Status { get; set; }
        public virtual IEnumerable<OfferLocationModel> Locations { get; set; }
        public string MainImage { get; set; }
        public string BannerUrl { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime ReviewedOn { get; set; }
        public string ReviewedBy { get; set; }
        public DateTime DecisionOn { get; set; }
        public string DecisionBy { get; set; }
        public decimal? OriginalPrice { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public string Type { get; set; }
        public decimal? Discount { get; set; }
        public string CompanyNameEnglish { get; set; }
        public int ReportCount { get; set; }
        public bool isPrivate { get; set; }
        public string PriceFiled { get; set; }
    }

    public class OneHubCompanyModel
    {
        public string CompanyName { get; set; }
        public string CompanyAdders { get; set; }
        public decimal CompanyRating { get; set; }
        public string CompanyPhoneNumber { get; set; }
        public string CompanyWebsite { get; set; }
        public string CompanyImageUrl { get; set; }
    }
}
