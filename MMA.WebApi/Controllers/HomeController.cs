using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MMA.WebApi.Shared.Interfaces.Categories;
using MMA.WebApi.Shared.Interfaces.Collection;
using MMA.WebApi.Shared.Interfaces.Offers;
using System.Threading.Tasks;

namespace MMA.WebApi.Controllers
{
    [Route("[controller]")]
    [Authorize("Bearer")]
    [ApiController]
    public class HomeController : BaseController
    {
        private readonly ICategoryService _categoryService;
        private readonly ICollectionService _collectionService;
        private readonly IOfferService _offerService;

        public HomeController(ICategoryService categoryService, IOfferService offerService, ICollectionService collectionService)
        {
            _categoryService = categoryService;
            _offerService = offerService;
            _collectionService = collectionService;
        }

        [HttpGet("offers")]
        public async Task<IActionResult> GetOffers([FromQuery] int limit = 4)
        {
            //API SECURITY: CHECK THIS ONE!!!
            return Ok(await _offerService.GetLatestOffers(limit));
        }
    }
}