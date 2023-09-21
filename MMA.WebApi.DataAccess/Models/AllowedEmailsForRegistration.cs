using System;
using System.ComponentModel.DataAnnotations;
using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.WebApi.DataAccess.Models
{
    public class AllowedEmailsForRegistration
    {
        public int Id { get; set; }

        [MaxLength(256)]
        public string Email { get; set; }
        public DateTime CreatedOn { get; set; }

        [MaxLength(1000)]
        public string CreatedBy { get; set; }

        public DateTime UpdatedOn { get; set; }

        [MaxLength(1000)]
        public string UpdatedBy { get; set; }
        public UserType UserType { get; set; }
        public bool InviteSent { get; set; }
        public DateTime InviteSentOn { get; set; }
        public string InviteSentBy { get; set; }
    }
}
