using MMA.WebApi.Shared.Enums;
using System;

namespace MMA.WebApi.Shared.Models.Roadshow
{
    public class RoadshowOfferCardModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int RoadshowProposalId { get; set; }
        public virtual RoadshowProposalModel RoadshowProposal { get; set; }
        public int RoadshowId { get; set; }
        public string RoadshowTitle { get; set; }
        public string Description { get; set; }
        public string PromotionCode { get; set; }
        public Declares.RoadshowStatus Status { get; set; }
        public string MainImage { get; set; }
        public string Tag { get; set; }
        public string Category { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}