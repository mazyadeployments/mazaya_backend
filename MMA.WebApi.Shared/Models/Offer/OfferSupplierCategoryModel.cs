using System.Collections.Generic;

namespace MMA.WebApi.Shared.Models.Offer
{
    public class OfferSupplierCategoryModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string AboutCompany { get; set; }
        public string CompanyNameEnglish { get; set; }
        public string CompanyNameArabic { get; set; }
        public IEnumerable<string> Categories { get; set; }
    }
}
