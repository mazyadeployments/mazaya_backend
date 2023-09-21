using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MMA.WebApi.Shared.Interfaces.Banner;
using MMA.WebApi.Shared.Interfaces.Roles;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Controllers
{
    [Route("[controller]")]
    [Authorize("Bearer")]
    [ApiController]
    public class BannerController : BaseController
    {
        private readonly IRoleService _roleService;
        private readonly IBannerService _bannerService;
        public BannerController(IBannerService bannerService, IRoleService roleService)
        {
            _bannerService = bannerService;
            _roleService = roleService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(List<int> offersIds)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                await _bannerService.CreateOrUpdateBanner(offersIds, UserId);
                return Ok(JsonConvert.SerializeObject("Banner created"));
            }

            return BadRequest("You do not have permission to do this action. Please contact system administartor.");
        }

        [HttpGet()]
        public IActionResult GetBanners([FromQuery] int limit = 10)
        {
            //API SECURITY: CHECK THIS ONE!!!
            return Ok(_bannerService.GetBanners(limit));
        }
    }
}
