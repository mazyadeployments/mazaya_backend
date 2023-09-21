using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Models.Attachment;
using MMA.WebApi.Shared.Models.Companies;
using System;
using System.Collections.Generic;

namespace MMA.WebApi.Shared.Models.Roadshow
{
    public class RoadshowProposalModel
    {
        public int Id { get; set; }
        public ICollection<RoadshowOfferProposalDocumentModel> Documents { get; set; } = new List<RoadshowOfferProposalDocumentModel>();
        public ICollection<RoadshowVoucherModel> RoadshowVouchers { get; set; } = new List<RoadshowVoucherModel>();
        public ICollection<RoadshowOfferModel> RoadshowOffers { get; set; } = new List<RoadshowOfferModel>();
        public ICollection<AttachmentModel> Attachments { get; set; } = new List<AttachmentModel>();
        public Declares.RoadshowProposalStatus Status { get; set; }
        public string RoadshowDetails { get; set; }
        public string EquipmentItem { get; set; }
        public string Subject { get; set; }
        public CompanyModel Company { get; set; }
        public string TermsAndCondition { get; set; }
        public bool TermsAndConditionChecked { get; set; }
        public DateTime OfferEffectiveDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Signature { get; set; }
        public string Manager { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public int OffersCount { get; set; }
    }
}