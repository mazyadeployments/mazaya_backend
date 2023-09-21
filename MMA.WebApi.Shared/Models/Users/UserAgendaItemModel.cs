using System.Collections.Generic;

namespace MMA.WebApi.Shared.Models.Users
{
    public class UserAgendaItemModel
    {
        public int UserId { get; set; }
        public int UserProfileId { get; set; }
        public int? Sequence { get; set; }
        public string FullName { get; set; }
        public string Designation { get; set; }
        public string Company { get; set; }
        public IEnumerable<string> Units { get; set; }
        public string RoleName { get; set; }
        public string Responsibility { get; set; }
    }
}
