using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;

namespace MMA.WebApi.DataAccess.Models
{

    public class Offer : IChangeable, IVisitable<IChangeable>
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Brand { get; set; }
        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }
        public string Description { get; set; }
        public virtual ICollection<OfferCategory> OfferCategories { get; set; }
        public virtual ICollection<OfferCollection> OfferCollections { get; set; }
        public virtual ICollection<OfferTag> OfferTags { get; set; }
        public string PromotionCode { get; set; }
        public decimal? PriceFrom { get; set; }
        public decimal? PriceTo { get; set; }
        public decimal? OriginalPrice { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public decimal? DiscountFrom { get; set; }
        public decimal? DiscountTo { get; set; }
        public string PriceCustom { get; set; }
        // enum OfferStatus
        public string Status { get; set; }
        public decimal? Discount { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidUntil { get; set; }
        public string WhatYouGet { get; set; }
        public string PriceList { get; set; }
        public string TermsAndCondition { get; set; }
        public string AboutCompany { get; set; }
        public virtual ICollection<OfferLocation> OfferLocations { get; set; }
        public virtual ICollection<OfferDocument> OfferDocuments { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public bool FlagIsLatest { get; set; }
        public bool FlagIsWeekendOffer { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public DateTime ReviewedOn { get; set; }
        public string ReviewedBy { get; set; }
        public DateTime DecisionOn { get; set; }
        public string DecisionBy { get; set; }
        public bool? BannerActive { get; set; }
        public string BannerUrl { get; set; }
        public bool? AnnouncementActive { get; set; }
        public int MembershipType { get; set; }
        //phone
        public string CountryCode { get; set; }
        public string E164Number { get; set; }
        public string InternationalNumber { get; set; }
        public string Number { get; set; }

        public virtual ICollection<UserFavouritesOffer> UserFavouritesOffers { get; set; }
        public virtual ICollection<OfferRating> OfferRating { get; set; }
        public int ReportCount { get; set; }
        public Guid? SpecialAnnouncement { get; set; }
        public bool IsPrivate { get; set; }
        public virtual IEnumerable<OffersMemberships> Memberships { get; set; }
        public void Accept(IVisitor<IChangeable> visitor)
        {
            if (visitor != null)
            {
                visitor.Visit(this);
            }
        }
    }
}
