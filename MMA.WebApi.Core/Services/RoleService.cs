using Microsoft.AspNetCore.Identity;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Interfaces.Roles;
using MMA.WebApi.Shared.Models.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.WebApi.Core.Services
{
    public class RoleService : IRoleService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public RoleService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<List<Roles>> GetUserRoles(string userId)
        {
            ApplicationUser applicationUser = await _userManager.FindByIdAsync(userId);

            List<Roles> formattedRoles = new List<Roles>();
            var roles = await _userManager.GetRolesAsync(applicationUser);
            foreach (string userRole in roles)
            {
                Enum.TryParse(userRole, out Roles role);
                formattedRoles.Add(role);
            }

            return formattedRoles;
        }


        public async Task<Dictionary<string, List<Roles>>> GetUsersRoles(List<string> userIds)
        {
            Dictionary<string, List<Roles>> userRoles = new Dictionary<string, List<Roles>>();

            foreach (var userId in userIds)
            {
                userRoles[userId] = await GetUserRoles(userId);
            }

            return userRoles;
        }
        public bool CheckIfUserIsJustAdmin(string userId)
        {
            ApplicationUser applicationUser = _userManager.FindByIdAsync(userId).Result;

            List<Roles> roles = new List<Roles>();
            foreach (string userRole in _userManager.GetRolesAsync(applicationUser).Result.ToList())
            {
                Enum.TryParse(userRole, out Roles role);
                roles.Add(role);
            }

            return roles.Contains(Roles.Admin);
        }

        public bool CheckIfUserIsAdmin(string userId)
        {
            ApplicationUser applicationUser = _userManager.FindByIdAsync(userId).Result;

            List<Roles> roles = new List<Roles>();
            foreach (string userRole in _userManager.GetRolesAsync(applicationUser).Result.ToList())
            {
                Enum.TryParse(userRole, out Roles role);
                roles.Add(role);
            }

            return roles.Contains(Roles.Admin) || roles.Contains(Roles.AdnocCoordinator);
        }

        public bool CheckIfUserIsAdminOrSupplierAdmin(string userId)
        {
            ApplicationUser applicationUser = _userManager.FindByIdAsync(userId).Result;

            List<Roles> roles = new List<Roles>();
            foreach (string userRole in _userManager.GetRolesAsync(applicationUser).Result.ToList())
            {
                Enum.TryParse(userRole, out Roles role);
                roles.Add(role);
            }

            return roles.Contains(Roles.Admin) || roles.Contains(Roles.AdnocCoordinator) || roles.Contains(Roles.SupplierAdmin);
        }

        public bool CheckIfUserIsAdminOrSupplier(string userId)
        {
            ApplicationUser applicationUser = _userManager.FindByIdAsync(userId).Result;

            List<Roles> roles = new List<Roles>();
            foreach (string userRole in _userManager.GetRolesAsync(applicationUser).Result.ToList())
            {
                Enum.TryParse(userRole, out Roles role);
                roles.Add(role);
            }

            return roles.Contains(Roles.Admin) || roles.Contains(Roles.AdnocCoordinator) || roles.Contains(Roles.SupplierAdmin) || roles.Contains(Roles.Supplier);
        }
        public async Task<bool> CheckRolesForCreatingQrCodeForRedeem(string userId)
        {
            ApplicationUser applicationUser = await _userManager.FindByIdAsync(userId);

            List<Roles> roles = new List<Roles>();
            var userRoles = await _userManager.GetRolesAsync(applicationUser);

            foreach (string userRole in userRoles)
            {
                Enum.TryParse(userRole, out Roles role);
                roles.Add(role);
            }
            return roles.ContainsOneOrMore(Roles.SupplierAdmin, Roles.Supplier, Roles.AdnocCoordinator,
                                                Roles.Buyer);


        }
        public async Task<RoleContainModel> CheckIfUserIsNotBuyer(string userId)
        {
            ApplicationUser applicationUser = await _userManager.FindByIdAsync(userId);

            List<Roles> roles = new List<Roles>();
            var userRoles = await _userManager.GetRolesAsync(applicationUser);

            foreach (string userRole in userRoles)
            {
                Enum.TryParse(userRole, out Roles role);
                roles.Add(role);
            }

            var roleObject = new RoleContainModel()
            {
                IsBuyer = !roles.ContainsOneOrMore(Roles.Admin, Roles.AdnocCoordinator,
                                                Roles.SupplierAdmin, Roles.Supplier,
                                                Roles.Reviewer),
                Roles = roles
            };
            return roleObject;
        }

        public string GetRoleName(string role)
        {
            return role switch
            {
                nameof(Roles.Admin) => Roles.Admin.ToString(),
                nameof(Roles.AdnocCoordinator) => "ADNOC Coordinator",
                nameof(Roles.Buyer) => Roles.Buyer.ToString(),
                nameof(Roles.Reviewer) => Roles.Reviewer.ToString(),
                nameof(Roles.SuperAdmin) => "Super Admin",
                nameof(Roles.SupplierAdmin) => "Supplier Admin",
                nameof(Roles.Supplier) => Roles.Supplier.ToString(),
                nameof(Roles.RoadshowFocalPoint) => "Roadshow Focal Point",
                _ => Roles.Buyer.ToString(),
            };
        }

        public bool CheckIfUserIsSupplierOrSupplierAdmin(string userId)
        {
            ApplicationUser applicationUser = _userManager.FindByIdAsync(userId).Result;

            List<Roles> roles = new List<Roles>();
            foreach (string userRole in _userManager.GetRolesAsync(applicationUser).Result.ToList())
            {
                Enum.TryParse(userRole, out Roles role);
                roles.Add(role);
            }

            return roles.Contains(Roles.SupplierAdmin) || roles.Contains(Roles.Supplier);

        }

        public bool CheckIfUserIsRoadshowFocalPoint(string userId)
        {
            ApplicationUser applicationUser = _userManager.FindByIdAsync(userId).Result;
            List<Roles> roles = new List<Roles>();
            foreach (string userRole in _userManager.GetRolesAsync(applicationUser).Result.ToList())
            {
                Enum.TryParse(userRole, out Roles role);
                roles.Add(role);
            }
            return roles.Contains(Roles.RoadshowFocalPoint);
        }
    }
}
