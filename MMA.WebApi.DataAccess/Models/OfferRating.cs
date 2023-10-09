using MMA.WebApi.Shared.Interfaces.GenericData;
using System;

namespace MMA.WebApi.DataAccess.Models
{
    public class OfferRating : IChangeable
    {
        public int OfferId { get; set; }
        public Offer Offer { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public decimal Rating { get; set; }

        public string CommentText { get; set; }

        /// <summary>
        /// enum OfferCommentStatus
        /// </summary>
        public string Status { get; set; }

        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
    }
}
