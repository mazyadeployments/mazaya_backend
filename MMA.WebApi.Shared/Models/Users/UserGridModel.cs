using System.Collections.Generic;

namespace MMA.WebApi.Shared.Models.Users
{
    public class UserGridModel
    {
        public int Id { get; set; }
        public string AccountDataId { get; set; }
        public string FullName { get; set; }
        public string Designation { get; set; }
        public bool StatusId { get; set; }
        public string PhotoURL { get; set; }
        public string CompanyName { get; set; }
        public IEnumerable<string> RoleNames { get; set; }
    }
}
