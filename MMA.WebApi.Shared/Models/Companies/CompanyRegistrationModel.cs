using MMA.WebApi.Shared.Models.Attachment;
using System;

namespace MMA.WebApi.Shared.Models.Companies
{
    public class CompanyRegistrationModel
    {
        public DateTime TradeLicenceExpiryDate { get; set; }
        public AttachmentModel TradeLicence { get; set; }
        public bool TermsAndConditions { get; set; }
    }
}
