using System;

namespace MMA.WebApi.Shared.Models.Offer
{
    public class OfferExcelModel
    {
        public int OfferId { get; set; }
        public string CompanyNameEnglish { get; set; }
        public string FocalPoint { get; set; }
        public string CompanyEmail { get; set; }
        public string PhoneNumber { get; set; }
        public string PriceFiled { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidUntil { get; set; }
        public string Description { get; set; }
        public string status { get; set; }

    }
}
