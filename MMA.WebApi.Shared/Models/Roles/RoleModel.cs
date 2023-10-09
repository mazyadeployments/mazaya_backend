using MMA.WebApi.Shared.Enums;
using System.Collections.Generic;

namespace MMA.WebApi.Shared.Models.Roles
{
    public class RoleModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> PermissionsNames { get; set; }
    }


    public class RoleContainModel
    {
        public List<Declares.Roles> Roles { get; set; }
        public bool IsBuyer { get; set; }
    }
}
