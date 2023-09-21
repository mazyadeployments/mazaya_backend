using System;
using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.WebApi.DataAccess.Models
{
    public class AdnocTermsAndConditions
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string ContentArabic { get; set; }
        public AdnocTermsAndConditionType Type { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
