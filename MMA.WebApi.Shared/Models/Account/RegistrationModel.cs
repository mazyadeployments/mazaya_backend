using System;

namespace MMA.WebApi.Shared.Models.Account
{
    public class RegistrationModel
    {
        public string CompanyName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string CompanyDescription { get; set; }
        public string Loz { get; set; }
        public string ConfirmLoz { get; set; }
        public DateTime ApprovedOn { get; set; }
        public string ApprovedBy { get; set; }
        public string Role { get; set; }
    }
}
