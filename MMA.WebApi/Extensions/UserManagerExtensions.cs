using Microsoft.AspNetCore.Identity;
using MMA.WebApi.DataAccess.Models;
using System;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.WebApi.Extensions
{
    public static class UserManagerExtensions
    {
        public static async Task SetPlatformInfo(this UserManager<ApplicationUser> userManager, ApplicationUser user)
        {
            user.LastDataSynchronizationOn = DateTime.UtcNow;
            user.PlatformType = PlatformType.Web.ToString();
            await userManager.UpdateAsync(user);
        }
    }
}
