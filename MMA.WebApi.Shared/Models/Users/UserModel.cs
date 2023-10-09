//using MMA.WebApi.Shared.Models.GeneraVisibility;
using System.Collections.Generic;

namespace MMA.WebApi.Shared.Models.Users
{
    public class UserModel
    {
        public int Id { get; set; }
        public string AccountDataId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Designation { get; set; }
        public bool MakeDecision { get; set; }
        public string MiddleName { get; set; }
        public string MobilePhone { get; set; }
        public string StatusName { get; set; }
        public bool StatusId { get; set; }
        public string Title { get; set; }
        public string WorkPhone { get; set; }
        public byte[] Photo { get; set; }
        public IEnumerable<string> UnitNames { get; set; }
        public string PhotoBase64 { get; set; }
        public string PhotoURL { get; set; }
        public string AlternativeEmail { get; set; }
        //public VisibilitySettings VisibilitySettings { get; set; }
        public IEnumerable<string> RoleNames { get; set; }
        public string CompanyName { get; set; }
        public int CompanyId { get; set; }
        public string Initials { get; set; }
    }
}
