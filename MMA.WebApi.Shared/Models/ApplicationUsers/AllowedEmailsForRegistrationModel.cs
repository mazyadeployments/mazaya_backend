using System;
using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.WebApi.Shared.Models.ApplicationUsers
{
    public class AllowedEmailsForRegistrationModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public UserType UserType { get; set; }
        public bool InviteSent { get; set; }
        public DateTime InviteSentOn { get; set; }
        public string InviteSentBy { get; set; }
    }
}
