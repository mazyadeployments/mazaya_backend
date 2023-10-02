using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MMA.WebApi.Shared.Interfaces.MazayaCategory;
using MMA.WebApi.Shared.Interfaces.Roles;
using MMA.WebApi.Shared.Interfaces.MazayaPaymentgateway;
using MMA.WebApi.Core.Services;
using System.Threading.Tasks;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Models.MazayaCategory;
using MMA.WebApi.Shared.Monads;
using MMA.WebApi.Shared.Models.MazayaPaymentgateway;
using MMA.WebApi.Shared.Constants;
using Newtonsoft.Json;
using System;

namespace MMA.WebApi.Controllers
{
    [Route("[controller]")]
    [Authorize("Bearer")]
    [ApiController]
    public class MazayaPaymentgatewayController : BaseController
    {
        private readonly IMazayaPaymentgatewayService _mazayapaymentgatewayService;
        private readonly IRoleService _roleService;
        public MazayaPaymentgatewayController(IMazayaPaymentgatewayService mazayapaymentgatewayService, IRoleService roleService)
        {

            _roleService = roleService;
            _mazayapaymentgatewayService = mazayapaymentgatewayService;
        }

        [HttpGet()]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(await _mazayapaymentgatewayService.GetMazayaPaymentgateways());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error {ex}");
            }
            
        }


        [HttpPost("page/{pageNumber}")]
        public async Task<IActionResult> GetMazayapaymentgatewayPage(QueryModel queryModel, int pageNumber)
        {
            try
            {
                queryModel.PaginationParameters = new PaginationParameters { PageNumber = pageNumber };
                queryModel.PaginationParameters.PageSize = 18;
                return Ok(await _mazayapaymentgatewayService.GetMazayaPaymentgatewaypage(queryModel));
            }catch(Exception ex)
            {
                return StatusCode(500, $"Internal server error {ex}");
            }
            
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _mazayapaymentgatewayService.GetMazayaPaymentgateway(id);

                if (result is Maybe<MazayaPaymentgatewayModel>.None)
                    return NotFound();

                return Ok(result.Value);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error {ex}");
            }
            
        }

        [HttpPost()]
        public async Task<IActionResult> CreateOrUpdate(MazayaPaymentgatewayModel model)
        {
            try
            {
                var result = await _mazayapaymentgatewayService.CreateOrUpdateAsync(model, UserId);

                if (result is Maybe<MazayaPaymentgatewayModel>.None)
                    return NotFound();

                return Ok(result.Value);
            }catch(Exception ex)
            {
                return StatusCode(500, $"Internal server error {ex}");
            }
            

        }
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _mazayapaymentgatewayService.DeleteMazayaPaymentgateway(id);
                return Ok(JsonConvert.SerializeObject(MessageConstants.DeleteMessages.ItemSuccessfullyDeleted("Mazayapaymentgateway")));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error {ex}");
            }
        }
    }
}
