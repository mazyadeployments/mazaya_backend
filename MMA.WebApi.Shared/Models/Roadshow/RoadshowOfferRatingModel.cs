using System;

namespace MMA.WebApi.Shared.Models.Roadshow
{
    public class RoadshowOfferRatingModel
    {
        public int RoadshowOfferId { get; set; }
        public string ApplicationUserId { get; set; }
        public decimal Rating { get; set; }
        public string CommentText { get; set; }
        public string BuyerFirstName { get; set; }
        public string BuyerLastName { get; set; }
        public string CompanyEnglishName { get; set; }
        public string CompanyArabicName { get; set; }
        public string OfferTitle { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string Status { get; set; }
    }
}