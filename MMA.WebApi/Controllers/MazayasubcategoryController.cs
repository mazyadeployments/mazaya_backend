using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MMA.WebApi.Core.Services;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Models;
using MMA.WebApi.Shared.Constants;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.MazayaCategory;
using MMA.WebApi.Shared.Interfaces.MazayaSubCategory;
using MMA.WebApi.Shared.Interfaces.Roles;
using MMA.WebApi.Shared.Models.MazayaCategory;
using MMA.WebApi.Shared.Models.MazayaSubCategory;
using MMA.WebApi.Shared.Monads;
using Newtonsoft.Json;
using Spire.Pdf.Exporting.XPS.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMA.WebApi.Controllers
{
    [Route("[controller]")]
    [Authorize("Bearer")]
    [ApiController]
    public class MazayasubcategoryController : BaseController
    {
        
        private readonly IMazayaSubCategoryService _mazayasubcategoryService;
        private readonly IRoleService _roleService;
        private readonly IMazayaCategoryService _mazayacategoryService;
        public MazayasubcategoryController(IMazayaSubCategoryService mazayasubCategoryService, IRoleService roleService, IMazayaCategoryService mazayacategoryService)
        {

            _roleService = roleService;
            _mazayasubcategoryService = mazayasubCategoryService;
            _mazayacategoryService = mazayacategoryService;
        }



        [HttpGet("getallmembership")]
        public async Task<IActionResult> GetAllMemembership()
        {
            try
            {
                var catogorylst = await _mazayacategoryService.GetMazayaCategories();
                foreach (var category in catogorylst)
                {
                    string[] resultArray = category.facilities.Split(',');
                    category.facilitiesarray = resultArray;
                }

                var subcategorylst = await _mazayasubcategoryService.GetMazayasubCategories();
                MazayaMembershipModel membershipModel = new MazayaMembershipModel();
                membershipModel.mazayaSubCategories = subcategorylst.ToList();
                membershipModel.mazayaCategories = catogorylst.ToList();
                return Ok(membershipModel);
              // return Ok(await _mazayasubcategoryService.GetallMembeship());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error {ex}");
            }
        }

        [HttpGet()]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(await _mazayasubcategoryService.GetMazayasubCategories());

            }catch(Exception ex) {
                return StatusCode(500, $"Internal server error {ex}");
            }
            
        }

        [HttpPost("page/{pageNumber}")]
        public async Task<IActionResult> GetMazayasubCategoriesPage(QueryModel queryModel, int pageNumber)
        {
            try
            {
                queryModel.PaginationParameters = new PaginationParameters { PageNumber = pageNumber };
                queryModel.PaginationParameters.PageSize = 18;
                return Ok(await _mazayasubcategoryService.GetMazayasubCategoriesPage(queryModel));
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Internal server error {ex}");
            }
            
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _mazayasubcategoryService.GetMazayasubCategory(id);

                if (result is Maybe<MazayaSubCategoryModel>.None)
                    return NotFound();

                return Ok(result.Value);
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Internal server error {ex}");
            }
            
        }
        [HttpPost()]
        public async Task<IActionResult> CreateOrUpdate(MazayaSubCategoryModel model)
        {
            try
            {
                var result = await _mazayasubcategoryService.CreateOrUpdateAsync(model, UserId);

                if (result is Maybe<MazayaSubCategoryModel>.None)
                    return NotFound();

                return Ok(result.Value);
            }
            catch (Exception ex)
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
                await _mazayasubcategoryService.DeleteMazayasubCategory(id);
                return Ok(JsonConvert.SerializeObject(MessageConstants.DeleteMessages.ItemSuccessfullyDeleted("MazayasubCategory")));

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error {ex}");
            }
         }


    }
}
