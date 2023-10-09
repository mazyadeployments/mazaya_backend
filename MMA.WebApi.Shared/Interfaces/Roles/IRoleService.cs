using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Models.Roles;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.Roles
{
    public interface IRoleService
    {
        Task<List<Declares.Roles>> GetUserRoles(string userId);
        bool CheckIfUserIsAdmin(string userId);
        bool CheckIfUserIsJustAdmin(string userId);
        bool CheckIfUserIsAdminOrSupplierAdmin(string userId);
        bool CheckIfUserIsSupplierOrSupplierAdmin(string userId);
        bool CheckIfUserIsAdminOrSupplier(string userId);
        bool CheckIfUserIsRoadshowFocalPoint(string userId);
        Task<RoleContainModel> CheckIfUserIsNotBuyer(string userId);
        Task<bool> CheckRolesForCreatingQrCodeForRedeem(string userId);
        string GetRoleName(string role);

        Task<Dictionary<string, List<Declares.Roles>>> GetUsersRoles(List<string> userIds);
    }
}
