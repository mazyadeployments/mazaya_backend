using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MMA.WebApi.Shared.Interfaces.LogAnalytics;
using MMA.WebApi.Shared.Interfaces.Roles;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Controllers
{
    [Route("[controller]")]
    [Authorize("Bearer")]
    [ApiController]
    public class LogController : BaseController
    {
        private readonly ILogAnalyticsService _logAnalyticsService;
        private readonly IRoleService _roleService;

        public LogController(ILogAnalyticsService logService, IRoleService roleService)
        {
            _roleService = roleService;
            _logAnalyticsService = logService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> LogOfferClick(int id)
        {
            await _logAnalyticsService.LogOfferClick(id, UserId);
            return Ok();
        }
        [HttpGet("banner_click/{id}")]
        public async Task<IActionResult> LogBaneerClick(int id)
        {
            var roleContain = await _roleService.CheckIfUserIsNotBuyer(UserId);

            if (roleContain.IsBuyer)
            {
                await _logAnalyticsService.LogBannerClick(id, UserId);
            }

            return Ok();
        }

        [HttpPost()]
        public async Task<IActionResult> LogSearchKeyword(SearchModel searchModel)
        {
            var roleContain = await _roleService.CheckIfUserIsNotBuyer(UserId);

            if (roleContain.IsBuyer)
            {
                BackgroundJob.Enqueue(() => _logAnalyticsService.LogSearchKeywordAdnOffer(searchModel.Keywoard, searchModel.Ids, UserId));
            }
            return Ok();
        }

    }

    public class SearchModel
    {
        public string Keywoard { get; set; }
        public IEnumerable<int> Ids { get; set; }
    }
}
