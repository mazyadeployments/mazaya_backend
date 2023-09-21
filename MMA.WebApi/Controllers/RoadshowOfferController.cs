using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MMA.WebApi.Helpers;
using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.Image;
using MMA.WebApi.Shared.Interfaces.Offers;
using MMA.WebApi.Shared.Interfaces.Roles;
using MMA.WebApi.Shared.Models.Offer;
using MMA.WebApi.Shared.Models.Roadshow;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace MMA.WebApi.Controllers
{
    [Route("[controller]")]
    [Authorize("Bearer")]
    [ApiController]
    public class RoadshowOfferController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly IRoadshowOfferService _roadshowOfferService;
        private readonly IRoleService _roleService;
        private readonly IImageUtilsService _imageUtilsService;
        public RoadshowOfferController(IConfiguration configuration, IRoadshowOfferService roadshowOfferService, IRoleService roleService, IImageUtilsService imageUtilsService)
        {
            _configuration = configuration;
            _roadshowOfferService = roadshowOfferService;
            _roleService = roleService;
            _imageUtilsService = imageUtilsService;
        }

        [HttpPost()]
        public async Task<IActionResult> CreateOrUpdate(RoadshowOfferModel model)
        {
            if (_roleService.CheckIfUserIsAdminOrSupplier(UserId))
            {
                var croppedImages = await _imageUtilsService.PrepareImagesForUpload(model.Images);
                model.Images = croppedImages;

                var result = await _roadshowOfferService.CreateOrUpdate(model, UserId);

                // Handle image upload in background
                //BackgroundJob.Enqueue(() => _imageUtilsService.CreateImages(croppedImages, Declares.ImageForType.Offer));
                new ImageUploadHelper(_configuration).UploadImagesInBackground(croppedImages, Declares.ImageForType.Offer);

                if (result is null)
                    return NotFound("Roadshow offer not found");

                return Ok(result);
            }

            return BadRequest("You do not have permission to do this action. Please contact system administartor.");
        }

        [HttpGet()]
        public async Task<IActionResult> Get()
        {
            return Ok(await _roadshowOfferService.GetRoadshowOffers());
        }

        [HttpPost("page/{pageNumber}")]
        public async Task<IActionResult> GetAllRoadshowOffers(QueryModel queryModel, int pageNumber)
        {
            queryModel.PaginationParameters = new PaginationParameters { PageNumber = pageNumber };
            return Ok(await _roadshowOfferService.GetAllRoadshowOffers(queryModel, UserId));
        }

        [HttpPost("page")]
        public async Task<IActionResult> GetAllRoadshowOffersForMyCompany(QueryModel queryModel)
        {
            if (_roleService.CheckIfUserIsAdminOrSupplier(UserId))
            {
                queryModel.PaginationParameters = new PaginationParameters { PageNumber = queryModel.Page, PageSize = 8 };
                return Ok(await _roadshowOfferService.GetAllRoadshowOffersForMyCompany(queryModel, UserId));
            }

            return BadRequest("You do not have permission to do this action. Please contact system administartor.");
        }


        [HttpPost("myactive")]
        public async Task<IActionResult> GetAllMyRoadshowOffers(QueryModel queryModel, int pageNumber)
        {
            //API SECURITY: CHECK THIS ONE!!!
            queryModel.PaginationParameters = new PaginationParameters { PageNumber = queryModel.Page, PageSize = 10 };
            return Ok(await _roadshowOfferService.GetAllRoadshowOffers(queryModel, UserId));
        }

        [HttpGet("specific/{id}")]
        public async Task<IActionResult> GetSpecificRoadshowOfferById(int id)
        {
            var result = await _roadshowOfferService.GetSpecificRoadshowOfferById(id, UserId);

            if (result == null)
                return NotFound("Roadshow Offer not found");

            return Ok(result.Value);
        }

        /// <summary>
        /// Mobile API for getting specific offer by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("mobile/specific/{id}")]
        public async Task<IActionResult> GetSpecificOfferByIdForMobile(int id)
        {
            var roles = await _roleService.GetUserRoles(UserId);
            if (roles.Contains(Declares.Roles.Buyer))
            {
                var result = await _roadshowOfferService.GetSpecificOfferByIdForMobile(id, UserId);

                if (result == null)
                    return NotFound("Offer not found");

                return Ok(result);
            }

            return Unauthorized();
        }


        [HttpPost("share")]
        public async Task<IActionResult> ShareOffer(OfferShareModel offerShareModel)
        {
            //API SECURITY: CHECK THIS ONE!!!
            await _roadshowOfferService.ShareOffer(offerShareModel, UserId);
            return Ok(JsonConvert.SerializeObject("Offer shared."));
        }

        /// <summary>
        /// For each offer checks if QR Code exists and returns it in that case or creates new QR Code if it didn't exist
        /// </summary>
        /// <param name="roadshowOfferId"></param>
        /// <returns></returns>
        [HttpGet()]
        [Route("{roadshowOfferId}/qrCode")]
        public async Task<IActionResult> GetQRCodeForRoadshowOffer(int roadshowOfferId)
        {
            //API SECURITY: CHECK THIS ONE!!!
            var qrCode = await _roadshowOfferService.GetQRCodeForRoadshowOffer(roadshowOfferId, UserId);

            return Ok(qrCode);
        }
        [HttpGet()]
        [Route("{roadshowOfferId}/GetPdfQrCode")]
        public async Task<IActionResult> GetPdfQRCodeForRoadshowOffer(int roadshowOfferId)
        {
            var qrCode = await _roadshowOfferService.GetPdfQRCodeForOffer(roadshowOfferId, UserId);
            if (qrCode == null)
                return BadRequest();
            return File(qrCode, "application/pdf");
        }

        [HttpPost("{roadshowOfferId}/favourite")]
        public async Task<IActionResult> SetOfferAsFavourite(RoadshowOfferFavoriteModel roadshowOfferFavorite)
        {
            var roles = await _roleService.GetUserRoles(UserId);
            if (roles.Contains(Declares.Roles.Buyer))
            {
                await _roadshowOfferService.SetRoadshowOfferAsFavourite(roadshowOfferFavorite, UserId);
                return Ok(JsonConvert.SerializeObject("Favorite offer saved."));
            }

            return BadRequest("You do not have permission to do this action. Please contact system administartor.");
        }

        /// <summary>
        /// Delete of roadshow offer if it's not attached to any invite or roadshow
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoadshowOffer(int id)
        {
            var roleContain = await _roleService.CheckIfUserIsNotBuyer(UserId);

            if (roleContain.IsBuyer) return Unauthorized();

            var response = await _roadshowOfferService.DeleteRSOffer(id, UserId);
            if (response.IsSuccessStatusCode)
                return Ok(JsonConvert.SerializeObject(response.Description));

            return BadRequest(JsonConvert.SerializeObject(response.Description));
        }
    }
}