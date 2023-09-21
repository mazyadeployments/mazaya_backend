using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Helpers;
using MMA.WebApi.Shared.Constants;
using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.ApplicationUsers;
using MMA.WebApi.Shared.Interfaces.Image;
using MMA.WebApi.Shared.Interfaces.LogAnalytics;
using MMA.WebApi.Shared.Interfaces.Offers;
using MMA.WebApi.Shared.Interfaces.Roles;
using MMA.WebApi.Shared.Models.Offer;
using MMA.WebApi.Shared.Monads;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace MMA.WebApi.Controllers
{
    [Route("[controller]")]
    [Authorize("Bearer")]
    [ApiController]
    public class OfferController : BaseController
    {
        private readonly IOfferService _offerService;
        private readonly IRoleService _roleService;
        private readonly IImageUtilsService _imageUtilsService;
        private readonly IConfiguration _configuration;
        private readonly IApplicationUserService _applicationUserService;
        private readonly ILogAnalyticsService _logAnalyticsService;
        private readonly UserManager<ApplicationUser> _userManager;

        public OfferController(
            IConfiguration configuration,
            IOfferService OfferService,
            IRoleService roleService,
            IImageUtilsService imageUtilsService,
            ILogAnalyticsService logAnalyticsService,
            IApplicationUserService applicationUserService,
            UserManager<ApplicationUser> userManager
        )
        {
            _logAnalyticsService = logAnalyticsService;
            _configuration = configuration;
            _offerService = OfferService;
            _roleService = roleService;
            _imageUtilsService = imageUtilsService;
            _applicationUserService = applicationUserService;
            _userManager = userManager;
        }

        [HttpGet()]
        public async Task<IActionResult> Get()
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                return Ok(await _offerService.GetOffers());
            }

            return Unauthorized();
        }

        [HttpGet("specific/{id}")]
        public async Task<IActionResult> GetSpecificOfferById(int id)
        {
            var roleObject = await _roleService.CheckIfUserIsNotBuyer(UserId);
            if (roleObject.IsBuyer)
                return Unauthorized();

            var result = await _offerService.GetSpecificOfferById(id, UserId, roleObject.Roles);

            if (result is Maybe<OfferModel>.None || result == null)
                return NotFound("Offer not found");

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
                var result = await _offerService.GetSpecificOfferByIdForMobile(id, UserId);

                if (result == null)
                    return NotFound("Offer not found");

                return Ok(result);
            }

            return Unauthorized();
        }

        [HttpPost("page/{pageNumber}")]
        public async Task<IActionResult> GetAllOffersPage(QueryModel queryModel, int pageNumber)
        {
            queryModel.PaginationParameters = new PaginationParameters { PageNumber = pageNumber };

            return Ok(await _offerService.GetAllOffers(queryModel, UserId));
        }

        [AllowAnonymous]
        [HttpPost("OneHubPage/{pageNumber}")]
        public async Task<IActionResult> GetAllOffersPageOnOneHub(
            OneHubQueryModel queryModel,
            int pageNumber
        )
        {
            var handler = new JwtSecurityTokenHandler();
            var id_token = Request.Headers["Authorization"]
                .FirstOrDefault()
                .Substring("Bearer ".Length);
            var jwtToken = handler.ReadJwtToken(id_token);
            JWTHelper.ValidateOneHubToken(id_token, jwtToken, _configuration);

            var userId = JWTHelper.GetUserIdFromOneHubToken(
                Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", ""),
                _userManager
            );
            if (userId.IsNullOrEmpty())
                return Unauthorized();

            queryModel.PaginationParameters = new PaginationParameters
            {
                PageNumber = pageNumber,
                PageSize = queryModel.PageSize
            };
            if (queryModel.Filter.IsFavorite)
                return Ok(await _offerService.GetFavoritesOffersOnehubPage(queryModel, userId));
            else
                return Ok(await _offerService.GetAllOffersForOneHub(queryModel, userId));
        }

        [HttpPost("my-offers/page/{pageNumber}")]
        public async Task<IActionResult> GetMyOffersPage(QueryModel queryModel, int pageNumber)
        {
            var roles = await _roleService.GetUserRoles(UserId);
            if (roles.Contains(Declares.Roles.Buyer))
            {
                return Unauthorized(
                    "You do not have permission to do this action. Please contact system administartor."
                );
            }
            else
            {
                queryModel.PaginationParameters = new PaginationParameters
                {
                    PageNumber = pageNumber
                };
                return Ok(await _offerService.GetMyOffers(queryModel, UserId));
            }
        }

        [HttpPost("membership/{membershipType}")]
        public async Task<IActionResult> GetOfferForMemebershipType(
            QueryModel queryModel,
            int membershipType
        )
        {
            return Ok(await _offerService.GetOffersForMembership(queryModel, membershipType));
        }

        [HttpPost("specific/page/{pageNumber}")]
        public async Task<IActionResult> GetSpecificOffersPage(
            QueryModel queryModel,
            int pageNumber
        )
        {
            var roles = await _roleService.GetUserRoles(UserId);
            if (roles.Contains(Declares.Roles.Buyer))
            {
                return Unauthorized(
                    "You do not have permission to do this action. Please contact system administartor."
                );
            }
            else
            {
                queryModel.PaginationParameters = new PaginationParameters
                {
                    PageNumber = pageNumber
                };
                return Ok(await _offerService.GetSpecificOffersPage(queryModel, UserId));
            }
        }

        [HttpGet("error")]
        public IActionResult GetError()
        {
            throw new Exception("Error");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var roles = await _roleService.GetUserRoles(UserId);
            if (
                roles.Contains(Declares.Roles.Admin)
                || roles.Contains(Declares.Roles.AdnocCoordinator)
                || roles.Contains(Declares.Roles.Buyer)
            )
            {
                var result = await _offerService.GetOfferById(id, UserId, roles);

                if (result == null)
                    return NotFound("Offer not found");
                if (roles.Contains(Declares.Roles.Buyer))
                    await _logAnalyticsService.LogOfferClick(id, UserId);
                return Ok(result.Value);
            }
            return Unauthorized();
        }

        [AllowAnonymous]
        [HttpGet("onehuboffer/{id}")]
        public async Task<IActionResult> GetForOnehubById(int id)
        {
            var handler = new JwtSecurityTokenHandler();
            var id_token = Request.Headers["Authorization"]
                .FirstOrDefault()
                .Substring("Bearer ".Length);
            var jwtToken = handler.ReadJwtToken(id_token);
            JWTHelper.ValidateOneHubToken(id_token, jwtToken, _configuration);

            var userId = JWTHelper.GetUserIdFromOneHubToken(
                Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", ""),
                _userManager
            );
            if (userId.IsNullOrEmpty())
                return Unauthorized();

            var roles = await _roleService.GetUserRoles(userId);
            if (
                roles.Contains(Declares.Roles.Admin)
                || roles.Contains(Declares.Roles.AdnocCoordinator)
                || roles.Contains(Declares.Roles.Buyer)
            )
            {
                var result = await _offerService.GetOnehubOfferById(id, UserId, roles);

                if (result == null)
                    return NotFound("Offer not found");
                if (roles.Contains(Declares.Roles.Buyer))
                    await _logAnalyticsService.LogOfferClick(id, UserId);
                return Ok(result.Value);
            }
            return Unauthorized();
        }

        [HttpPost("{offerId}/favourite")]
        public async Task<IActionResult> SetOfferAsFavourite(OfferFavoriteModel offerFavorite)
        {
            //API SECURITY: CHECK THIS ONE!!!
            await _offerService.SetOfferAsFavourite(offerFavorite, UserId);
            return Ok(JsonConvert.SerializeObject("Favorite offer saved."));
        }

        [HttpPost("share")]
        public async Task<IActionResult> ShareOffer(OfferShareModel offerShareModel)
        {
            //API SECURITY: CHECK THIS ONE!!!
            await _offerService.ShareOffer(offerShareModel, UserId);
            return Ok(JsonConvert.SerializeObject("Offer shared."));
        }

        [HttpPost("step")]
        public async Task<IActionResult> Step(OfferModel model)
        {
            var roles = await _roleService.GetUserRoles(UserId);
            //API SECURITY: CHECK THIS ONE!!!
            if (CanManageOffers(model, roles))
            {
                await _offerService.Step(model, UserId);
                return Ok(
                    JsonConvert.SerializeObject(
                        MessageConstants.SaveMessages.ItemSuccessfullySaved("Step")
                    )
                );
            }
            return Unauthorized(
                "You do not have permission to do this action. Please contact system administartor."
            );
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (_roleService.CheckIfUserIsAdminOrSupplier(UserId))
            {
                await _offerService.DeleteOffer(id, UserId);
                return Ok(
                    JsonConvert.SerializeObject(
                        MessageConstants.DeleteMessages.ItemSuccessfullyDeleted("Offer")
                    )
                );
            }

            return Unauthorized(
                "You do not have permission to do this action. Please contact system administartor."
            );
        }

        [HttpPost()]
        public async Task<IActionResult> CreateOrUpdate(OfferModel model)
        {
            var roles = await _roleService.GetUserRoles(UserId);
            if (CanManageOffers(model, roles))
            {
                var croppedImages = await _imageUtilsService.PrepareImagesForUpload(model.Images);
                model.Images = croppedImages;
                var result = await _offerService.CreateOrUpdate(model, UserId, roles);

                // Handle image upload in background
                //BackgroundJob.Enqueue(() => _imageUtilsService.CreateImages(croppedImages, Declares.ImageForType.Offer));
                new ImageUploadHelper(_configuration).UploadImagesInBackground(
                    croppedImages,
                    Declares.ImageForType.Offer
                );

                if (result == null)
                    return NotFound("Offer not found");

                return Ok(result.Value);
            }

            return Unauthorized(
                "You do not have permission to do this action. Please contact system administartor."
            );
        }

        [HttpPost("delta-migration")]
        public async Task<IActionResult> DeltaMigration(OfferModel model)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                var croppedImages = await _imageUtilsService.PrepareImagesForUpload(model.Images);
                model.Images = croppedImages;
                var result = await _offerService.DeltaMigration(
                    model,
                    UserId,
                    new List<Declares.Roles>() { Declares.Roles.Admin }
                );

                // Handle image upload in background
                //BackgroundJob.Enqueue(() => _imageUtilsService.CreateImages(croppedImages, Declares.ImageForType.Offer));
                new ImageUploadHelper(_configuration).UploadImagesInBackground(
                    croppedImages,
                    Declares.ImageForType.Offer
                );

                if (result == null)
                    return NotFound("Offer not found");

                return Ok(result);
            }

            return Unauthorized(
                "You do not have permission to do this action. Please contact system administartor."
            );
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update(OfferModel model)
        {
            var roles = await _roleService.GetUserRoles(UserId);
            if (CanManageOffers(model, roles))
            {
                var result = await _offerService.UpdateOffer(model, UserId, roles);

                if (result is Maybe<OfferModel>.None)
                    return NotFound("Offer not found");

                return Ok(result.Value);
            }

            return Unauthorized(
                "You do not have permission to do this action. Please contact system administartor."
            );
        }

        [HttpGet("weekend")]
        public async Task<IActionResult> GetWeekendOffers()
        {
            return Ok(await _offerService.GetWeekendOffers(UserId));
        }

        [HttpPost("weekend/page/{pageNumber}")]
        public async Task<IActionResult> GetWeekendOffersPage(QueryModel queryModel, int pageNumber)
        {
            queryModel.UserId = UserId;
            queryModel.PaginationParameters = new PaginationParameters { PageNumber = pageNumber };
            return Ok(await _offerService.GetWeekendOffers(queryModel));
        }

        [HttpPost("favorites/page/{pageNumber}")]
        public async Task<IActionResult> GetFavoritesOffersPage(
            QueryModel queryModel,
            int pageNumber
        )
        {
            //API SECURITY: CHECK THIS ONE!!!
            queryModel.PaginationParameters = new PaginationParameters { PageNumber = pageNumber };
            return Ok(await _offerService.GetFavoritesOffersPage(queryModel, UserId));
        }

        [HttpPost()]
        [Route("page/{pageNumber}/search")]
        public async Task<IActionResult> GetOffersSearchPage(QueryModel queryModel, int pageNumber)
        {
            queryModel.PaginationParameters = new PaginationParameters { PageNumber = pageNumber };
            return Ok(await _offerService.GetOffersSearchPageAsync(queryModel, UserId));
        }

        [HttpPost()]
        [Route("page/{pageNumber}/category")]
        public async Task<IActionResult> GetOffersByCategoryPage(
            int id,
            QueryModel queryModel,
            int pageNumber
        )
        {
            queryModel.UserId = UserId;
            queryModel.PaginationParameters = new PaginationParameters { PageNumber = pageNumber };
            return Ok(await _offerService.GetOffersByCategoryPage(id, queryModel));
        }

        [HttpPost()]
        [Route("page/{pageNumber}/collection")]
        public async Task<IActionResult> GetOffersByCollectionPage(
            int id,
            QueryModel queryModel,
            int pageNumber
        )
        {
            var roleObject = await _roleService.CheckIfUserIsNotBuyer(UserId);
            queryModel.UserId = UserId;
            queryModel.PaginationParameters = new PaginationParameters { PageNumber = pageNumber };
            return Ok(
                await _offerService.GetOffersByCollectionPage(
                    id,
                    queryModel,
                    UserId,
                    roleObject.IsBuyer
                )
            );
        }

        [HttpPost()]
        [Route("page/{pageNumber}/tag")]
        public async Task<IActionResult> GetOffersByTagPage(
            int id,
            QueryModel queryModel,
            int pageNumber
        )
        {
            queryModel.UserId = UserId;
            queryModel.PaginationParameters = new PaginationParameters { PageNumber = pageNumber };
            return Ok(await _offerService.GetOffersByTagPage(id, queryModel));
        }

        /// <summary>
        /// Method used for transfering offers from one supplier to another
        /// </summary>
        /// <param name="id"></param>
        /// <param name="queryModel"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [HttpPost()]
        [Route("transfer-offers")]
        public async Task<IActionResult> TransferOffers(TransferOffersModel transferOffersModel)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                await _offerService.TransferOffers(transferOffersModel);
                return Ok();
            }

            return Unauthorized();
        }

        /// <summary>
        /// For each offer checks if QR Code exists and returns it in that case or creates new QR Code if it didn't exist
        /// </summary>
        /// <param name="offerId"></param>
        /// <returns></returns>
        [HttpGet()]
        [Route("{offerId}/qrCode")]
        public async Task<IActionResult> GetQRCodeForOffer(int offerId)
        {
            //API SECURITY: CHECK THIS ONE!!!
            var qrCode = await _offerService.GetQRCodeForOffer(offerId, UserId);

            return Ok(qrCode);
        }

        [HttpGet()]
        [Route("{offerId}/GetPdfQrCode")]
        public async Task<IActionResult> GetPdfQRCodeForOffer(int offerId)
        {
            //API SECURITY: CHECK THIS ONE!!!
            var qrCode = await _offerService.GetPdfQRCodeForOffer(offerId, UserId);
            if (qrCode == null)
                return BadRequest();
            return File(qrCode, "application/pdf");
        }

        [HttpGet()]
        [Route("{offerId}/GetPdfQrCodeAndroid")]
        public async Task<IActionResult> GetPdfQRCodeForOfferAndroid(int offerId)
        {
            //API SECURITY: CHECK THIS ONE!!!
            var qrCode = await _offerService.GetPdfQRCodeForOffer(offerId, UserId);
            if (qrCode == null)
                return BadRequest();
            return Ok(qrCode);
        }

        [HttpGet()]
        [Route("about-company")]
        public async Task<IActionResult> GetOffersThatHaveEmptyAboutCompany()
        {
            //API SECURITY: CHECK THIS ONE!!!
            if (_roleService.CheckIfUserIsAdmin(UserId))
                return Ok(_offerService.GetOffersThatHaveEmptyAboutCompany());

            return BadRequest();
        }

        [HttpPost("return-to-pending/{offerId}")]
        public async Task<IActionResult> ReturnToPending(int offerId)
        {
            //API SECURITY: CHECK THIS ONE!!!
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                await _offerService.ReturnToPending(offerId);
                return Ok();
            }

            return BadRequest();
        }

        [HttpGet("renew/{offerId}")]
        public async Task<IActionResult> RenewOffer(int offerId)
        {
            var roleObject = await _roleService.CheckIfUserIsNotBuyer(UserId);

            if (roleObject.IsBuyer)
                return Unauthorized();

            await _offerService.RenewOffer(offerId);

            return Ok();
        }

        private bool CanManageOffers(OfferModel offer, List<Declares.Roles> roles)
        {
            return _offerService.CanManageOffers(roles, offer, UserId);
        }
    }
}
