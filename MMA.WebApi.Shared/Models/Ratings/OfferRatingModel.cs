using System;

namespace MMA.WebApi.Shared.Models.Ratings
{
    public class OfferRatingModel
    {

        public int OfferId { get; set; }
        public string ApplicationUserId { get; set; }
        public decimal Rating { get; set; }
        public string CommentText { get; set; }
        public string BuyerFirstName { get; set; }
        public string BuyerLastName { get; set; }
        public string BuyerUsername { get; set; }
        public string CompanyEnglishName { get; set; }
        public string CompanyArabicName { get; set; }
        public string OfferTitle { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public string Status { get; set; }
        public bool IsRoadshowOffer { get; set; }
        public string BuyerEmail { get; set; }
        public string BuyerPhone { get; set; }
    }
}
