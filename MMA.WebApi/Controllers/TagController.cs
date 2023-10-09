using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MMA.WebApi.Shared.Constants;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.Roles;
using MMA.WebApi.Shared.Interfaces.Tag;
using MMA.WebApi.Shared.Models.Tag;
using MMA.WebApi.Shared.Monads;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace MMA.WebApi.Controllers
{

    [Route("[controller]")]
    [Authorize("Bearer")]
    [ApiController]
    public class TagController : BaseController
    {
        private readonly ITagService _tagService;
        private readonly IRoleService _roleService;

        public TagController(ITagService tagService, IRoleService roleService)
        {
            _tagService = tagService;
            _roleService = roleService;
        }

        [HttpGet()]
        public async Task<IActionResult> Get()
        {
            return Ok(await _tagService.GetTags());
        }

        [HttpPost("page/{pageNumber}")]
        public async Task<IActionResult> GetTagsPage(QueryModel queryModel, int pageNumber)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                queryModel.PaginationParameters = new PaginationParameters { PageNumber = pageNumber };
                queryModel.PaginationParameters.PageSize = 20;
                return Ok(await _tagService.GetTagsPage(queryModel));
            }

            return BadRequest("You do not have permission to do this action. Please contact system administartor.");
        }
        [HttpPost()]
        public async Task<IActionResult> CreateOrUpdate(TagModel model)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                var result = await _tagService.CreateTag(model, UserId);

                if (result is Maybe<TagModel>.None)
                    return NotFound();

                return Ok(result.Value);
            }
            else
            {
                return BadRequest("You do not have permission to do this action. Please contact system administartor.");
            }

        }
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                await _tagService.DeleteTag(id);
                return Ok(JsonConvert.SerializeObject(MessageConstants.DeleteMessages.ItemSuccessfullyDeleted("Tag")));
            }
            else
            {
                return BadRequest("You do not have permission to do this action. Please contact system administartor.");
            }

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                var result = await _tagService.GetTag(id);

                if (result is Maybe<TagModel>.None)
                    return NotFound();

                return Ok(result.Value);
            }
            else
            {
                return BadRequest("You do not have permission to do this action. Please contact system administartor.");
            }

        }

    }
}