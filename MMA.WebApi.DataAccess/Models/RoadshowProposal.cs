using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;

namespace MMA.WebApi.DataAccess.Models
{
    public class RoadshowProposal : IChangeable, IVisitable<IChangeable>
    {
        public int Id { get; set; }
        public ICollection<RoadshowOfferProposalDocument> Documents { get; set; } = new List<RoadshowOfferProposalDocument>();
        public ICollection<RoadshowVoucher> RoadshowVouchers { get; set; } = new List<RoadshowVoucher>();
        public ICollection<RoadshowOffer> RoadshowOffers { get; set; } = new List<RoadshowOffer>();
        public Declares.RoadshowProposalStatus Status { get; set; }
        public string RoadshowDetails { get; set; }
        public string EquipmentItem { get; set; }
        public string Subject { get; set; }
        public int? CompanyId { get; set; }
        public Company Company { get; set; }
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

        public void Accept(IVisitor<IChangeable> visitor)
        {
            if (visitor != null)
            {
                visitor.Visit(this);
            }
        }
    }
}
