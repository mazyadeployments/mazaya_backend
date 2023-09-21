using MMA.WebApi.Shared.Models.Categories;
using System;
using System.Collections.Generic;

namespace MMA.WebApi.Shared.Models.Companies
{
    public class CompanyCardModel
    {
        public int Id { get; set; }
        public IEnumerable<CategoryLiteModel> Categories { get; set; } = new List<CategoryLiteModel>();
        public string NameEnglish { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string OfficialEmail { get; set; }
        public string Mobile { get; set; }
        public string Logo { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
    }
}
