using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MMA.WebApi.Shared.Interfaces.MazayaCategory;
using MMA.WebApi.Shared.Interfaces.Roles;
using MMA.WebApi.Shared.Interfaces.MazayaPackageSubscriptions;
using MMA.WebApi.Core.Services;
using System.Threading.Tasks;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Models.MazayaCategory;
using MMA.WebApi.Shared.Monads;
using MMA.WebApi.Shared.Models.MazayaPackagesubscriptionsModel;
using MMA.WebApi.Shared.Constants;
using Newtonsoft.Json;
using System;

namespace MMA.WebApi.Controllers
{
    [Route("[controller]")]
    [Authorize("Bearer")]
    [ApiController]
    public class MazayapackagesubscriptionsController : BaseController
    {
        private readonly IMazayaPackagesubscriptionsService _mazayapackagesubscriptionsService;
        private readonly IRoleService _roleService;
        public MazayapackagesubscriptionsController(IMazayaPackagesubscriptionsService mazayapackagesubscriptionsService, IRoleService roleService)
        {

            _roleService = roleService;
            _mazayapackagesubscriptionsService = mazayapackagesubscriptionsService;
        }

        [HttpGet()]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(await _mazayapackagesubscriptionsService.GetMazayaPackagesubscriptions());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error {ex}");
            }
            
        }

        [HttpPost("page/{pageNumber}")]
        public async Task<IActionResult> GetMazayapackagesubscriptionsPage(QueryModel queryModel, int pageNumber)
        {
            try
            {
                queryModel.PaginationParameters = new PaginationParameters { PageNumber = pageNumber };
                queryModel.PaginationParameters.PageSize = 18;
                return Ok(await _mazayapackagesubscriptionsService.GetMazayaPackagesubscriptionspage(queryModel));

            }
            catch(Exception ex) {
                return StatusCode(500, $"Internal server error {ex}");
            }
           
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _mazayapackagesubscriptionsService.GetMazayaPackagesubscription(id);

                if (result is Maybe<MazayaPackageSubscriptionsModel>.None)
                    return NotFound();

                return Ok(result.Value);
            }catch(Exception ex)
            {
                return StatusCode(500, $"Internal server error {ex}");
            }
            
        }

        [HttpPost()]
        public async Task<IActionResult> CreateOrUpdate(MazayaPackageSubscriptionsModel model)
        {
            //if (_roleService.CheckIfUserIsAdmin(UserId))
            //{

            try
            {
                var result = await _mazayapackagesubscriptionsService.CreateOrUpdateAsync(model, UserId);

                if (result is Maybe<MazayaPackageSubscriptionsModel>.None)
                    return NotFound();

                return Ok(result.Value);
            }catch(Exception ex) {
                return StatusCode(500, $"Internal server error {ex}");
            }
           
            //}

            //return BadRequest("You do not have permission to do this action. Please contact system administartor.");

        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try {
                await _mazayapackagesubscriptionsService.DeleteMazayaPackagesubscriptions(id);
                return Ok(JsonConvert.SerializeObject(MessageConstants.DeleteMessages.ItemSuccessfullyDeleted("Mazayapackagesubscriptions")));
            }catch(Exception ex) {
                return StatusCode(500, $"Internal server error {ex}");
            }
        }
    }
}
