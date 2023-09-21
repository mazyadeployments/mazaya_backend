using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MMA.WebApi.Helpers;
using MMA.WebApi.Shared.Constants;
using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.Collection;
using MMA.WebApi.Shared.Interfaces.Image;
using MMA.WebApi.Shared.Interfaces.Roles;
using MMA.WebApi.Shared.Models.Collection;
using MMA.WebApi.Shared.Models.Image;
using MMA.WebApi.Shared.Monads;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Controllers
{
    [Route("[controller]")]
    [Authorize("Bearer")]
    [ApiController]
    public class CollectionController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly ICollectionService _collectionService;
        private readonly IRoleService _roleService;
        private readonly IImageUtilsService _imageUtilsService;

        public CollectionController(IConfiguration configuration, ICollectionService collectionService, IRoleService roleService, IImageUtilsService imageUtilsService)
        {
            _configuration = configuration;
            _collectionService = collectionService;
            _roleService = roleService;
            _imageUtilsService = imageUtilsService;
        }

        [HttpGet()]
        public async Task<IActionResult> Get()
        {
            return Ok(await _collectionService.GetCollections());
        }
        [HttpPost("page/{pageNumber}")]
        public async Task<IActionResult> GetCollectionsPage(QueryModel queryModel, int pageNumber)
        {
            queryModel.PaginationParameters = new PaginationParameters { PageNumber = pageNumber };
            queryModel.PaginationParameters.PageSize = 4;
            return Ok(await _collectionService.GetCollectionsPage(queryModel));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _collectionService.GetCollection(id);

            if (result is Maybe<CollectionModel>.None)
                return NotFound();

            return Ok(result.Value);
        }

        [HttpPost()]
        public async Task<IActionResult> CreateOrUpdate(CollectionModel model)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                var images = new List<ImageModel>() { model.Image };
                var croppedImages = await _imageUtilsService.PrepareImagesForUpload(images);
                model.ImageSets = croppedImages;

                var result = await _collectionService.CreateCollection(model, UserId);

                // Handle image upload in background
                //BackgroundJob.Enqueue(() => _imageUtilsService.CreateImages(croppedImages, Declares.ImageForType.Collection));
                new ImageUploadHelper(_configuration).UploadImagesInBackground(croppedImages, Declares.ImageForType.Collection);

                if (result is Maybe<CollectionModel>.None)
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
                await _collectionService.DeleteCollection(id);
                return Ok(JsonConvert.SerializeObject(MessageConstants.DeleteMessages.ItemSuccessfullyDeleted("Collection")));
            }

            return BadRequest("You do not have permission to do this action. Please contact system administartor.");
        }
    }
}