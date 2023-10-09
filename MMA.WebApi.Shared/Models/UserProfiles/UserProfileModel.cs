using System;
using System.Collections.Generic;

namespace MMA.WebApi.Shared.Models.Users
{
    public class UserProfileModel
    {
        public int Id { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string AccountDataId { get; set; }
        public int UserId { get; set; }
        public bool StatusId { get; set; }
        public IEnumerable<string> UnitNames { get; set; }
        //public int UnitId { get; set; }
        public DateTime ChangeTime { get; set; }
        public string ChangedByUser { get; set; }
        public IEnumerable<int> SelectedUnitIds { get; set; }
        public int? CompanyId { get; set; }
        public string CompanyName { get; set; }


    }
}
