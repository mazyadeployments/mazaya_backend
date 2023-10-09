using Microsoft.AspNetCore.Identity;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Models.B2C;
using System;
using System.Threading.Tasks;

namespace MMA.WebApi.Core.Services
{
    public class UserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public ApplicationUser MapToApplicationUser(ApiConnectorData apiConnectorData)
        {
            var applicationUser = new ApplicationUser
            {
                UserName = apiConnectorData.Email,
                NormalizedUserName = apiConnectorData.Email,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow,
                Email = apiConnectorData.Email,
                EmailConfirmed = false,
                PhoneNumber = apiConnectorData.MobilePhone,
                Active = true
            };

            return applicationUser;
        }

        public async Task<bool> UserExists(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            return user != null;
        }
    }
}
