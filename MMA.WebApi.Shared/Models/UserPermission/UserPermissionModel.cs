namespace MMA.WebApi.Shared.Models.UserPermission
{
    public class UserPermissionModel
    {
        public string PermissionId { get; set; }
        public int ProfileId { get; set; }

        public string RoleId { get; set; }

        public int CompanyId { get; set; }

        public int UnitId { get; set; }

        public string Condition { get; set; }

    }
}
