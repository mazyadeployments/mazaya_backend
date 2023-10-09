using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.OfferRating;
using MMA.WebApi.Shared.Interfaces.Roles;
using MMA.WebApi.Shared.Models.Ratings;
using Newtonsoft.Json;
using System.Threading.Tasks;
namespace MMA.WebApi.Controllers
{
    [Route("[controller]")]
    [Authorize("Bearer")]
    [ApiController]
    public class OfferRatingController : BaseController
    {
        private readonly IOfferRatingService _offerRatingService;
        private readonly IRoleService _roleService;
        public OfferRatingController(IOfferRatingService OfferRatingService, IRoleService roleService)
        {
            _offerRatingService = OfferRatingService;
            _roleService = roleService;
        }

        /// <summary>
        /// set offer rating and comment
        /// </summary>
        /// <param name="offerRating"></param>
        /// <param name="offerId"></param>
        /// <returns></returns>
        [HttpPost()]
        [Route("rating")]
        public async Task<IActionResult> RateOffer(OfferRatingModel offerRating)
        {
            var roles = await _roleService.GetUserRoles(UserId);

            if (roles.Contains(Declares.Roles.Buyer) || roles.Contains(Declares.Roles.AdnocCoordinator))
            {
                string userId = this.UserId;
                var response = await _offerRatingService.RateOffer(offerRating, userId);
                return Ok(response);
                //return Ok(JsonConvert.SerializeObject("Offer's rating saved."));
            }

            return BadRequest("You do not have permission to do this action. Please contact system administartor.");
        }

        /// <summary>
        /// for ADNOC admin
        /// return all comments for review
        /// </summary>
        /// <returns></returns>
        [HttpPost()]
        [Route("comments/{pageNumber}")]
        public async Task<IActionResult> GetAllComments(QueryModel queryModel, int pageNumber)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                queryModel.PaginationParameters = new PaginationParameters { PageNumber = pageNumber };
                queryModel.PaginationParameters.PageSize = 18;

                return Ok(await _offerRatingService.GetAllComments(queryModel));
            }

            return BadRequest("You do not have permission to do this action. Please contact system administartor.");
        }

        /// <summary>
        /// for ADNOC admin
        /// to set comment status: OfferCommentStatus
        /// </summary>
        /// <returns></returns>
        [HttpPost()]
        [Route("comments/setstatus")]
        public async Task<IActionResult> SetCommentStatus(int offerId, int commentStatus)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                string userId = this.UserId;
                await _offerRatingService.SetCommentStatus(offerId, commentStatus, userId);
                return Ok(JsonConvert.SerializeObject("Offer's comment status updated."));
            }

            return BadRequest("You do not have permission to do this action. Please contact system administartor.");
        }

        [HttpGet()]
        [Route("{offerId}/averagerating")]
        public async Task<IActionResult> GetAverageRatingForOffer(int offerId)
        {
            return Ok(await _offerRatingService.GetAverageRatingForOffer(offerId));
        }

        [HttpGet()]
        [Route("roadshow/{offerId}/averagerating")]
        public async Task<IActionResult> GetAverageRatingForRoadshowOffer(int offerId)
        {
            return Ok(await _offerRatingService.GetAverageRatingForRoadshowOffer(offerId));
        }


        [HttpPost()]
        [Route("{offerId}/publishrating")]
        public async Task<IActionResult> PublishRating(OfferRatingModel offerRating)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
                return Ok(await _offerRatingService.PublishRating(offerRating));

            return BadRequest("You do not have permission to do this action. Please contact system administartor.");
        }

        /// <summary>
        /// for ADNOC admin
        /// return all comments for review
        /// </summary>
        /// <returns></returns>
        [HttpPost()]
        [Route("ratings/{pageNumber}")]
        public async Task<IActionResult> GetAllRatingsForUser(QueryModel queryModel, int pageNumber)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                queryModel.PaginationParameters = new PaginationParameters { PageNumber = pageNumber };
                queryModel.PaginationParameters.PageSize = 18;

                return Ok(await _offerRatingService.GetAllComments(queryModel));
            }

            return BadRequest("You do not have permission to do this action. Please contact system administartor.");
        }
    }
}
