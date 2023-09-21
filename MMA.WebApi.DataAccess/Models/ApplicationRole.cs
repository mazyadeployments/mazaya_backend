using Microsoft.AspNetCore.Identity;

namespace MMA.WebApi.DataAccess.Models
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole()
        {
            //RolePermissions = new HashSet<RolePermission>();
        }

        //public ICollection<RolePermission> RolePermissions { get; set; }
    }
}
