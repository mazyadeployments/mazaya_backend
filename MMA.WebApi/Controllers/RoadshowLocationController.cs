using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.Offers;
using MMA.WebApi.Shared.Interfaces.Roles;
using MMA.WebApi.Shared.Models.DefaultLocations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Controllers
{
    [Route("[controller]")]
    [Authorize("Bearer")]
    [ApiController]
    public class RoadshowLocationController : BaseController
    {
        private readonly IRoadshowLocationService _roadshowLocationService;
        private readonly IRoleService _roleService;
        public RoadshowLocationController(IRoadshowLocationService roadshowLocationService, IRoleService roleService)
        {
            _roadshowLocationService = roadshowLocationService;
            _roleService = roleService;
        }

        [HttpPost("specific/{locationId}/page/{pageNumber}")]
        public async Task<IActionResult> GetById(QueryModel queryModel, int pageNumber, int locationId)
        {
            queryModel.PaginationParameters = new PaginationParameters { PageNumber = pageNumber };
            return Ok(_roadshowLocationService.GetRoadshowOffersForSpecificLocation(queryModel, locationId, UserId));
        }

        [HttpPost("specificdate/page/{pageNumber}")]
        public async Task<IActionResult> GetForSpecificDates(QueryModel queryModel, int pageNumber)
        {
            queryModel.PaginationParameters = new PaginationParameters { PageNumber = pageNumber };
            return Ok(_roadshowLocationService.GetRoadshowOffersForSpecificDates(queryModel, UserId));
        }

        [HttpPost("page/{pageNumber}")]
        public async Task<IActionResult> Get(QueryModel queryModel, int pageNumber)
        {
            queryModel.PaginationParameters = new PaginationParameters { PageNumber = pageNumber };
            return Ok(_roadshowLocationService.GetAllRoadshowOffersForAllLocations(queryModel, UserId));
        }

        [HttpGet("defaultlocations/{locationId}")]
        public async Task<IActionResult> GetDefaultLocationById(int locationId)
        {
            return Ok(await _roadshowLocationService.GetDefaultLocationById(locationId));
        }

        [HttpGet("defaultlocations")]
        public async Task<IActionResult> GetDefaultLocations()
        {
            return Ok(await _roadshowLocationService.GetDefaultLocations());
        }

        [HttpPost("defaultlocations")]
        public async Task<IActionResult> UpdateDefaultLocations(List<DefaultLocationModel> defaultLocations)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                await _roadshowLocationService.UpdateDefaultLocations(defaultLocations, UserId);
                return Ok();
            }

            return BadRequest("You do not have permission to do this action. Please contact system administartor.");
        }
    }
}