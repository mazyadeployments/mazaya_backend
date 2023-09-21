using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Helpers;
using MMA.WebApi.Shared.Interfaces.Categories;
using MMA.WebApi.Shared.Interfaces.Collection;
using MMA.WebApi.Shared.Interfaces.Membership;
using MMA.WebApi.Shared.Interfaces.Offers;
using MMA.WebApi.Shared.Interfaces.Tag;
using MMA.WebApi.Shared.Models.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.WebApi.Controllers
{
    [Route("[controller]")]
    [Authorize("Bearer")]
    [ApiController]
    public class FiltersController : BaseController
    {
        private readonly ICategoryService _categoryService;
        private readonly IOfferService _offerService;
        private readonly ITagService _tagService;
        private readonly IRoadshowService _roadshowService;
        private readonly ICollectionService _collectionService;
        private readonly IConfiguration _configuration;
        private readonly IMembershipECardService _membershipECardService;
        private readonly UserManager<ApplicationUser> userManager;

        public FiltersController(
            ICategoryService categoryService,
            IOfferService offerService,
            ITagService tagService,
            IRoadshowService roadshowService,
            ICollectionService collectionService,
            IMembershipECardService membershipECardService,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration
        )
        {
            _membershipECardService = membershipECardService;
            _categoryService = categoryService;
            _offerService = offerService;
            _tagService = tagService;
            _roadshowService = roadshowService;
            _collectionService = collectionService;
            this.userManager = userManager;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetfilterData()
        {
            var roles = _offerService.GetUserRoles(UserId);

            FilterData filterData = new FilterData();
            filterData.Category = await _categoryService.GetCategories();
            filterData.Areas = _offerService.GetAllDefaultAreas();
            filterData.Tags = await _tagService.GetTags();
            filterData.Locations = _roadshowService.GetAllDefaultLocations();
            filterData.Collections = await _collectionService.GetCollections();
            filterData.Memberships = _membershipECardService.GetMembershipsForUser(
                UserId,
                roles.Contains(Roles.Buyer)
            );
            return Ok(filterData);
        }

        [HttpGet("Onehub")]
        [AllowAnonymous]
        public async Task<IActionResult> GetfilterDataForOneHub()
        {
            var handler = new JwtSecurityTokenHandler();
            var id_token = Request.Headers["Authorization"]
                .FirstOrDefault()
                .Substring("Bearer ".Length);
            var jwtToken = handler.ReadJwtToken(id_token);
            JWTHelper.ValidateOneHubToken(id_token, jwtToken, _configuration);

            var userId = JWTHelper.GetUserIdFromOneHubToken(
                Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", ""),
                userManager
            );
            if (userId.IsNullOrEmpty())
                return Unauthorized();

            var roles = _offerService.GetUserRoles(userId);

            FilterData filterData = new FilterData();
            filterData.Category = await _categoryService.GetCategories();
            filterData.Areas = _offerService.GetAllDefaultAreas();
            filterData.Tags = await _tagService.GetTags();
            filterData.Locations = _roadshowService.GetAllDefaultLocations();
            filterData.Collections = await _collectionService.GetCollections();
            filterData.Memberships = _membershipECardService.GetMembershipsForUser(
                UserId,
                roles.Contains(Roles.Buyer)
            );
            return Ok(filterData);
        }
    }
}
