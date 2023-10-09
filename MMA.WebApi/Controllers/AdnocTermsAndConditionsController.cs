using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MMA.WebApi.Shared.Interfaces.AdnocTermsAndConditions;
using MMA.WebApi.Shared.Interfaces.Roles;
using MMA.WebApi.Shared.Models.AdnocTermsAndConditions;
using System.Threading.Tasks;

namespace MMA.WebApi.Controllers
{
    [Route("[controller]")]
    [Authorize("Bearer")]
    [ApiController]
    public class AdnocTermsAndConditionsController : BaseController
    {
        private readonly IAdnocTermsAndConditionsService _adnocTermsAndConditionsService;
        private readonly IRoleService _roleService;

        public AdnocTermsAndConditionsController(IAdnocTermsAndConditionsService adnocTermsAndConditionsService, IRoleService roleService)
        {
            _adnocTermsAndConditionsService = adnocTermsAndConditionsService;
            _roleService = roleService;
        }

        [HttpGet("offer")]
        public async Task<IActionResult> GetOfferTermsAndConditions()
        {
            //API SECURITY: CHECK THIS ONE!!!
            return Ok(await _adnocTermsAndConditionsService.GetAdnocTermsAndConditionsForOffer());
        }

        [HttpGet("proposal")]
        public async Task<IActionResult> GetProposalsTermsAndConditions()
        {
            //API SECURITY: CHECK THIS ONE!!!
            return Ok(await _adnocTermsAndConditionsService.GetAdnocTermsAndConditionsForProposal());
        }

        [HttpGet("global")]
        public async Task<IActionResult> GetGlobalTermsAndConditions()
        {
            //API SECURITY: CHECK THIS ONE!!!
            return Ok(await _adnocTermsAndConditionsService.GetGlobalAdnocTermsAndConditions());
        }


        [HttpPost()]
        public async Task<IActionResult> UpdateAdnocTermsAndConditions(AdnocTermsAndConditionsModel adnocTermsAndConditionsModel)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                await _adnocTermsAndConditionsService.UpdateAdnocTermsAndConditions(adnocTermsAndConditionsModel, UserId);
                return Ok();
            }

            return BadRequest("You don't have permission to do this action.");
        }
    }
}