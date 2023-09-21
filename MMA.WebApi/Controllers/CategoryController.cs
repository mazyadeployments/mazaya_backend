using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MMA.WebApi.Shared.Constants;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.Categories;
using MMA.WebApi.Shared.Interfaces.Roles;
using MMA.WebApi.Shared.Models.Category;
using MMA.WebApi.Shared.Monads;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace MMA.WebApi.Controllers
{

    [Route("[controller]")]
    [Authorize("Bearer")]
    [ApiController]
    public class CategoryController : BaseController
    {
        private readonly ICategoryService _categoryService;
        private readonly IRoleService _roleService;


        public CategoryController(ICategoryService CategoryService, IRoleService roleService)
        {

            _roleService = roleService;
            _categoryService = CategoryService;
        }

        [HttpGet()]
        public async Task<IActionResult> Get()
        {
            return Ok(await _categoryService.GetCategories());
        }


        [HttpPost("page/{pageNumber}")]
        public async Task<IActionResult> GetCategoriesPage(QueryModel queryModel, int pageNumber)
        {
            queryModel.PaginationParameters = new PaginationParameters { PageNumber = pageNumber };
            queryModel.PaginationParameters.PageSize = 18;
            return Ok(await _categoryService.GetCategoriesPage(queryModel));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _categoryService.GetCategory(id);

            if (result is Maybe<CategoryModel>.None)
                return NotFound();

            return Ok(result.Value);
        }

        [HttpPost()]
        public async Task<IActionResult> CreateOrUpdate(CategoryModel model)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                var result = await _categoryService.CreateOrUpdateAsync(model, UserId);

                if (result is Maybe<CategoryModel>.None)
                    return NotFound();

                return Ok(result.Value);
            }

            return BadRequest("You do not have permission to do this action. Please contact system administartor.");

        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                await _categoryService.DeleteCategory(id);
                return Ok(JsonConvert.SerializeObject(MessageConstants.DeleteMessages.ItemSuccessfullyDeleted("Category")));
            }

            return BadRequest("You do not have permission to do this action. Please contact system administartor.");
        }
    }
}