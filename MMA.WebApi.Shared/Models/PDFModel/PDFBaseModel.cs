using MMA.WebApi.Shared.Models.Companies;
using System;
using System.ComponentModel.DataAnnotations;

namespace MMA.WebApi.Shared.Models.PDFModel
{
    public abstract class PDFBaseModel
    {
        [Required]
        public string Subject { get; set; }
        [Required]
        public CompanyModel Company { get; set; }
        [Required]
        public string TermsAndCondition { get; set; }
        [Required]
        public bool TermsAndConditionChecked { get; set; }
        [Required]
        public DateTime OfferEffectiveDate { get; set; }
        [Required]
        public DateTime ExpiryDate { get; set; }
        [Required]
        public string Name { get; set; }
        public string Title { get; set; }
        [Required]
        public string Signature { get; set; }
        public string Manager { get; set; }
        public int PartnersCount { get; set; }
        [Required]
        public int ActivitiesCount { get; set; }
        [Required]
        public int LocationsCount { get; set; }
    }
}
