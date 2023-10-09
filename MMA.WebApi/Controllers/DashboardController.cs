using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MMA.WebApi.Shared.Interfaces.Dashboard;
using MMA.WebApi.Shared.Interfaces.Roles;
using System.Threading.Tasks;

namespace MMA.WebApi.Controllers
{
    [Route("[controller]")]
    [Authorize("Bearer")]
    [ApiController]
    public class DashboardController : BaseController
    {
        private readonly IDashboardService _dashboardService;
        private readonly IRoleService _roleService;
        public DashboardController(IDashboardService dashboardService, IRoleService roleService)
        {
            _dashboardService = dashboardService;
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboardItems()
        {
            //API SECURITY: CHECK THIS ONE!!!
            var roleContain = await _roleService.CheckIfUserIsNotBuyer(UserId);

            if (roleContain.IsBuyer)
                return BadRequest("You do not have permission to do this action. Please contact system administartor.");

            return Ok(await _dashboardService.GetDashboardItems(UserId));

        }
    }
}
