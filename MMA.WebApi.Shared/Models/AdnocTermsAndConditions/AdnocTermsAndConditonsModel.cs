using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.WebApi.Shared.Models.AdnocTermsAndConditions
{
    public class AdnocTermsAndConditionsModel
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string ContentArabic { get; set; }
        public AdnocTermsAndConditionType Type { get; set; }
    }
}
