using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MMA.WebApi.Shared.Interfaces.Companies;
using System.Threading.Tasks;

namespace MMA.WebApi.Controllers
{
    [Route("[controller]")]
    [Authorize("Bearer")]
    [ApiController]
    public class CompanyLocationController : BaseController
    {
        private readonly ICompanyLocationsService _companyLocationService;

        public CompanyLocationController(ICompanyLocationsService companyLocationService)
        {
            _companyLocationService = companyLocationService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            //API SECURITY: CHECK THIS ONE!!!
            var companiesLocations = await _companyLocationService.GetAll();
            if (companiesLocations == null)
                return NotFound();
            return Ok(companiesLocations);
        }
    }
}