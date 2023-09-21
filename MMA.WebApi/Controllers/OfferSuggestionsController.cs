using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.OfferSuggestions;
using MMA.WebApi.Shared.Interfaces.Roles;
using MMA.WebApi.Shared.Models.OfferSuggestions;
using System.Threading.Tasks;

namespace MMA.WebApi.Controllers
{
    [Route("[controller]")]
    [Authorize("Bearer")]
    [ApiController]
    public class OfferSuggestionsController : BaseController
    {
        private readonly IRoleService _roleService;
        private readonly IOfferSuggestionsService _offerSuggestionsService;

        public OfferSuggestionsController(
            IRoleService roleService,
            IOfferSuggestionsService offerSuggestionsService
        )
        {
            _roleService = roleService;
            _offerSuggestionsService = offerSuggestionsService;
        }

        [HttpPost("suggestions")]
        public async Task<IActionResult> GetAllOfferSuggestions(QueryModel query)
        {
            query.PaginationParameters.PageNumber = query.Page;
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                return Ok(await _offerSuggestionsService.GetAllOfferSuggestions(query));
            }
            else
                return Unauthorized();
        }

        [HttpPost]
        public async Task<IActionResult> CreateOfferSuggestion(
            [FromBody] OfferSuggestionModel model
        )
        {
            model.UserId = UserId;
            await _offerSuggestionsService.CreateOfferSuggestion(model);
            return Ok();
        }

        [HttpGet("complete/{id}")]
        public async Task<IActionResult> CompleteOfferSuggestion(int id)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                await _offerSuggestionsService.CompleteOfferSuggestionAsync(id, UserId);
                return Ok();
            }
            return Unauthorized();
        }

        [HttpGet]
        public async Task<IActionResult> GetMySuggestion()
        {
            if (_roleService.CheckIfUserIsNotBuyer(UserId).Result.IsBuyer)
            {
                return Ok(await _offerSuggestionsService.GetAllOfferSuggestionsByUserId(UserId));
            }
            return Ok();
        }
    }
}
