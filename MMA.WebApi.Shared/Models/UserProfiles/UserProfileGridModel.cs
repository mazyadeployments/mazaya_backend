

namespace MMA.WebApi.Shared.Models.Users
{
    public class UserProfileGridModel
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public string UnitName { get; set; }
        public string StatusName { get; set; }
        public bool StatusId { get; set; }
        public string AccountDataId { get; set; }
        public string CompanyName { get; set; }
    }
}
