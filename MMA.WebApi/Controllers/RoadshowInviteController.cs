using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MMA.WebApi.Shared.Interfaces.RoadshowInvite;
using MMA.WebApi.Shared.Models.Roadshow;
using System.Threading.Tasks;

namespace MMA.WebApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize("Bearer")]
    [ApiController]
    public class RoadshowInviteController : BaseController
    {
        private readonly IRoadshowInviteService _roadshowInviteService;
        public RoadshowInviteController(IRoadshowInviteService roadshowInviteService)
        {
            _roadshowInviteService = roadshowInviteService;
        }

        [HttpPost]
        public async Task<IActionResult> Update(RoadshowInviteModel model)
        {
            //API SECURITY: CHECK THIS ONE!!!
            var result = await _roadshowInviteService.Update(model, UserId);

            if (result is null) return NotFound("Roadshow invite not found");

            return Ok(result);
        }
    }
}
