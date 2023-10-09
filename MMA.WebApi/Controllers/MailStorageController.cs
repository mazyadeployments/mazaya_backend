using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.MailStorage;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Controllers
{
    [Route("[controller]")]
    [Authorize("Bearer")]
    [ApiController]
    public class MailStorageController : BaseController
    {
        private readonly IMailStorageService _mailStorageService;

        public MailStorageController(IMailStorageService mailStorageService)
        {
            _mailStorageService = mailStorageService;
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteMailStorageList([FromBody] IEnumerable<int> ids)
        {
            if (ids == null)
                return BadRequest();
            await _mailStorageService.DeleteListAsync(ids);
            return Ok();
        }

        [HttpPost("search/{pageNumber}")]
        public async Task<IActionResult> SearchMailStorages(QueryModel queryModel, int pageNumber)
        {
            queryModel.PaginationParameters = new PaginationParameters { PageNumber = pageNumber };
            var r = await _mailStorageService.SearchMailStorages(queryModel);
            return Ok(r);
            //var mailStorages = await this._mailStorageService.SearchMailStorages(search);
            //if (mailStorages == null)
            //    return NotFound();
            //return Ok(mailStorages);

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMailStorageDetails(int id)
        {
            var result = await _mailStorageService.GetMailStorageDetails(id);
            return Ok(result);
        }
    }
}
