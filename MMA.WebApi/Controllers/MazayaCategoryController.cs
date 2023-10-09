using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MMA.WebApi.Shared.Interfaces.Categories;
using MMA.WebApi.Shared.Interfaces.Roles;
using MMA.WebApi.Shared.Interfaces.MazayaCategory;
using MMA.WebApi.Core.Services;
using System.Threading.Tasks;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Models.Category;
using MMA.WebApi.Shared.Monads;
using MMA.WebApi.Shared.Models.MazayaCategory;
using MMA.WebApi.Shared.Constants;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Azure;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Collections.Generic;
using System;
using MMA.WebApi.Models;
using MMA.WebApi.Shared.Interfaces.MazayaSubCategory;
using Passbook.Generator.Tags;
using MMA.WebApi.Shared.Models.MazayaSubCategory;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Models.Image;
using MMA.WebApi.Shared.Models.MazayacategoryDocument;
using System.Web.Http.Results;
using ImageMagick;
using System.Linq;
//using Spire.Pdf.Exporting.XPS.Schema;

namespace MMA.WebApi.Controllers
{
    [Route("[controller]")]
    [Authorize("Bearer")]
    [ApiController]
    public class MazayaCategoryController : BaseController
    {
        private readonly IMazayaCategoryService _mazayacategoryService;
        private readonly IRoleService _roleService;
        public IWebHostEnvironment webHostEnvironment;
        private readonly IMazayaSubCategoryService _mazayasubcategoryService;
        public MazayaCategoryController(IMazayaSubCategoryService mazayasubCategoryService,IMazayaCategoryService mazayaCategoryService, IRoleService roleService, IWebHostEnvironment webHostEnvironment)
        {
            this.webHostEnvironment = webHostEnvironment;
            _roleService = roleService;
            _mazayacategoryService = mazayaCategoryService;
            _mazayasubcategoryService = mazayasubCategoryService;
        }

        [HttpGet("getallmembership")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var lst = await _mazayacategoryService.GetMazayaCategories();
                

                foreach (var category in lst)
                {
                    if(category.facilities != null)
                    {
                        string[] resultArray = category.facilities.Split(',');
                        category.facilitiesarray = resultArray;
                    }                                     
                    var obj = await _mazayasubcategoryService.GetMazayasubCategories();
                    List<MazayaSubCategoryModel> sublist = obj.Where(x => x.MazayaCategoryId == category.Id).ToList();
                    category.subcategorylist = sublist;
                }

                return Ok(ApiResponse<IEnumerable<MazayaCategoryModel>>.Response($"Mazayacategory list", lst));
               
            }
            catch (Exception ex)
            {                
                return StatusCode(500, $"Internal server error {ex}");
            }
        }

        [HttpPost("page/{pageNumber}")]
        public async Task<IActionResult> GetMazayaCategoriesPage(QueryModel queryModel, int pageNumber)
        {
            try
            {
                queryModel.PaginationParameters = new PaginationParameters { PageNumber = pageNumber };
                queryModel.PaginationParameters.PageSize = 18;
                return Ok(await _mazayacategoryService.GetMazayaCategoriesPage(queryModel));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error {ex}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                    var result = await _mazayacategoryService.GetMazayaCategory(id);
                    string[] resultArray = result.Value.facilities.Split(',');
                    result.Value.facilitiesarray = resultArray;
                
                if (result is Maybe<MazayaCategoryModel>.None)
                    return NotFound();

                return Ok(result.Value);
            }catch(Exception ex) {
                return StatusCode(500, $"Internal server error {ex}");
            }
        }

        [HttpPost()]
        public async Task<IActionResult> CreateOrUpdate([FromForm] MazayaCategoryModel model )
        {
            try
            {


                //if (_roleService.CheckIfUserIsAdmin(UserId))
                //{
                var result = await _mazayacategoryService.CreateOrUpdateAsync(model, UserId);

                if (result is Maybe<MazayaCategoryModel>.None)
                    return NotFound();

                var filePaths = new List<string>();
                if (model.cat_image != null)
                {
                    var fileName = model.cat_image.FileName;
                    FileInfo currentFile = new FileInfo(fileName);

                    fileName = result.Value.Id + "_mazayacategory" + currentFile.Extension;
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"Images\", fileName);
                    filePaths.Add(filePath);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.cat_image.CopyToAsync(stream);
                    }
                    result.Value.image = fileName;
                    result = await _mazayacategoryService.CreateOrUpdateAsync(result.Value, UserId);
                    result.Value.image = fileName;
                }
                string[] resultArray = result.Value.facilities.Split(',');
                result.Value.facilitiesarray = resultArray;

                return Ok(result.Value);
                //}

                //return BadRequest("You do not have permission to do this action. Please contact system administartor.");
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


                //if (_roleService.CheckIfUserIsAdmin(UserId))
                //{
                await _mazayacategoryService.DeleteMazayaCategory(id);
                return Ok(JsonConvert.SerializeObject(MessageConstants.DeleteMessages.ItemSuccessfullyDeleted("MazayaCategory")));
                //}

                // return BadRequest("You do not have permission to do this action. Please contact system administartor.");
            }catch(Exception ex)
            {
                return StatusCode(500, $"Internal server error {ex}");
            }
        }

    }
}
