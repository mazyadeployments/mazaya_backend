using System;

namespace MMA.WebApi.Shared.Models.ActionVisibility
{
    public class ActionVisibilityModel
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string UserAccountDataId { get; set; }
        public int? ActionId { get; set; }
        public bool? CanSeeAll { get; set; }
        public int? VisibilityType { get; set; }
        public DateTime? Expiring { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
    }
}
