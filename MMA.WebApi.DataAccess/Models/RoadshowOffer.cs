using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MMA.WebApi.DataAccess.Models
{
    public class RoadshowOffer : IChangeable, IVisitable<IChangeable>
    {
        public int Id { get; set; }
        public virtual ICollection<RoadshowOfferCategory> RoadshowOfferCategories { get; set; } = new List<RoadshowOfferCategory>();
        public virtual ICollection<RoadshowOfferCollection> RoadshowOfferCollections { get; set; } = new List<RoadshowOfferCollection>();
        public virtual ICollection<RoadshowOfferTag> RoadshowOfferTags { get; set; } = new List<RoadshowOfferTag>();
        public virtual ICollection<RoadshowOfferDocument> OfferDocuments { get; set; } = new List<RoadshowOfferDocument>();
        public virtual ICollection<RoadshowOfferRating> OfferRating { get; set; } = new List<RoadshowOfferRating>();
        public virtual ICollection<RoadshowVoucher> RoadshowVouchers { get; set; } = new List<RoadshowVoucher>();
        public virtual ICollection<RoadshowEventOffer> RoadshowEventOffers { get; set; } = new List<RoadshowEventOffer>();
        public virtual ICollection<UserFavouritesRoadshowOffer> UserFavouritesRoadshowOffers { get; set; }
        public string Title { get; set; }
        [ForeignKey("RoadshowProposalId")]
        public int RoadshowProposalId { get; set; }
        public virtual RoadshowProposal RoadshowProposal { get; set; }
        public string Description { get; set; }
        public string RoadshowDetails { get; set; }
        public string EquipmentItem { get; set; }
        public string PromotionCode { get; set; }
        public Declares.RoadshowOfferStatus Status { get; set; }
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
