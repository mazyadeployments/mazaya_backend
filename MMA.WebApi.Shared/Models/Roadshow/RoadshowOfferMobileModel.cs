using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Models.Attachment;
using MMA.WebApi.Shared.Models.DefaultLocations;
using MMA.WebApi.Shared.Models.Document;
using MMA.WebApi.Shared.Models.Image;
using System;
using System.Collections.Generic;

namespace MMA.WebApi.Shared.Models.Roadshow
{
    public class RoadshowOfferMobileModel
    {
        public int Id { get; set; }
        public ICollection<RoadshowOfferCategoryModel> RoadshowOfferCategories { get; set; } = new List<RoadshowOfferCategoryModel>();
        public ICollection<RoadshowOfferCollectionModel> RoadshowOfferCollections { get; set; } = new List<RoadshowOfferCollectionModel>();
        public ICollection<RoadshowOfferTagModel> RoadshowOfferTags { get; set; } = new List<RoadshowOfferTagModel>();
        public ICollection<RoadshowOfferDocumentModel> OfferDocuments { get; set; } = new List<RoadshowOfferDocumentModel>();
        public ICollection<AttachmentModel> OfferAttachments { get; set; } = new List<AttachmentModel>();
        public ICollection<RoadshowOfferRatingModel> RoadshowOfferRatings { get; set; } = new List<RoadshowOfferRatingModel>();
        public ICollection<RoadshowVoucherModel> RoadshowVouchers { get; set; } = new List<RoadshowVoucherModel>();
        public ICollection<DefaultLocationMobileModel> RoadshowLocations { get; set; } = new List<DefaultLocationMobileModel>();
        public List<ImageModel> Images { get; set; } = new List<ImageModel>();
        public ICollection<ImageUrlsModel> ImageUrls { get; set; } = new List<ImageUrlsModel>();
        public string Title { get; set; }
        public int RoadshowProposalId { get; set; }
        public string RoadshowProposalTitle { get; set; }
        public virtual RoadshowProposalModel RoadshowProposal { get; set; }
        public string Description { get; set; }
        public int RoadshowId { get; set; }
        public string RoadshowTitle { get; set; }
        public string RoadshowDetails { get; set; }
        public string EquipmentItem { get; set; }
        public string PromotionCode { get; set; }
        public Declares.RoadshowStatus Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public string MainImage { get; set; }
        public string Tag { get; set; }
        public decimal RatingPercent { get; set; }
        public decimal Rating { get; set; }
        public string Category { get; set; }
        public int CompanyId { get; set; }
        public VideoModel Video { get; set; }
        public virtual IEnumerable<DefaultLocationMobileModel> Locations { get; set; }
        public bool IsFavourite { get; set; }
        public bool IsAlreadyRated { get; set; }
        public string CompanyNameEnglish { get; set; }
        public string CompanyNameArabic { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyPOBox { get; set; }
        public string CompanyWebsite { get; set; }
        public string CompanyPhoneNumber { get; set; }
        public string CompanyLogo { get; set; }
        public DateTime RoadshowEventDateFrom { get; set; }
        public DateTime RoadshowEventDateTo { get; set; }
    }
}

