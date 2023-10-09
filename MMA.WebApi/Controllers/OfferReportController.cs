using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.OfferReports;
using MMA.WebApi.Shared.Models.Offer;
using System.Threading.Tasks;

namespace MMA.WebApi.Controllers
{
    [Route("[controller]")]
    [Authorize("Bearer")]

    [ApiController]
    public class OfferReportController : BaseController
    {
        private readonly IOfferReportService _offerReportService;
        public OfferReportController(IOfferReportService offerReportService)
        {
            _offerReportService = offerReportService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOfferReports(int id)
        {
            var reports = await _offerReportService.GetReportsByOfferId(id);
            return Ok(reports);

        }

        [HttpGet("resolveRepor/{reportId}")]
        public async Task<IActionResult> ResolveReport(int reportId)
        {
            var reports = await _offerReportService.ResolveOfferReport(reportId);
            return Ok(reports);
        }
        [HttpGet("isreported/{id}")]
        public async Task<IActionResult> IsReported(int id)
        {
            var reports = await _offerReportService.IsReported(UserId, id);
            return Ok(reports);

        }

        [HttpPost()]
        public async Task<IActionResult> PostOfferReport(OfferReportModel offerReportData)
        {
            await _offerReportService.SaveAsync(offerReportData, UserId);
            return Ok();
        }
        [HttpPost("offers")]
        public async Task<IActionResult> GetAllReportedOffer(QueryModel queryModel)
        {
            var temp = await _offerReportService.GetAllReportedOffer(queryModel);
            return Ok(temp);
        }
    }
}
