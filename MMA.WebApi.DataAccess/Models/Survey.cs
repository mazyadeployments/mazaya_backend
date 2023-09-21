using MMA.WebApi.Shared.Interfaces.GenericData;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MMA.WebApi.DataAccess.Models
{
    public class Survey : IChangeable
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public string AdminId { get; set; }
        [ForeignKey("AdminId")]
        public virtual ApplicationUser AspNetUser { get; set; }

        public string Description { get; set; }
        public string UsersId { get; set; }
        public string UserTypes { get; set; }
        public string UserRoles { get; set; }
        public string Status { get; set; }
        public string Questions { get; set; }
        public bool IsDeleted { get; set; }
        public bool ForAllUsers { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public int Opportunity { get; set; }
        public bool IsCreateMail { get; set; }
        public bool IsQuickSurvey { get; set; }

    }
}
