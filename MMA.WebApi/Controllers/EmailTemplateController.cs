using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MMA.WebApi.Shared.Interfaces.EmailTemplate;
using MMA.WebApi.Shared.Models.Email;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Controllers
{
    [Route("[controller]")]
    [Authorize("Bearer")]
    [ApiController]
    public class EmailTemplateController : BaseController
    {
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IEmailTemplateRootService _emailTemplateRootService;

        public EmailTemplateController(IEmailTemplateRootService emailTemplateRootService, IEmailTemplateService emailTemplateService)
        {
            _emailTemplateRootService = emailTemplateRootService;
            _emailTemplateService = emailTemplateService;
        }

        [HttpGet("getEmailsTemplates")]
        public async Task<IActionResult> GetEmails()
        {
            var result = await _emailTemplateService.GetEmails();
            return Ok(result);
        }

        [HttpGet("getEmailsTemplates/{id}")]
        public async Task<IActionResult> GetEmailDetails(int id)
        {
            var result = await _emailTemplateService.GetEmailDetails(id);
            return Ok(result);
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Post(EmailTemplateModel data)
        {
            int id = 0;
            if (data.Id == 0)
                id = await _emailTemplateService.CreatEmail(data, UserId);
            else
            {
                await _emailTemplateService.UpdateEmail(data, UserId);
                id = data.Id.Value;
            }

            return Ok(id);
        }

        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> UpdateEmail(EmailTemplateModel model)
        {
            await _emailTemplateService.UpdateEmail(model, UserId);
            return Ok();
        }

        [HttpDelete("getEmailsTemplates/{id}")]
        public async Task<IActionResult> DeleteEmail(int id)
        {
            if (id == null)
                return BadRequest();

            int email = await _emailTemplateService.DeleteEmail(id);
            return Ok(email);
        }

        //[HttpPost("emailTemplateRoot/search")]
        //public async Task<IActionResult> EmailTemplateRootSearch(SearchModel search)
        //{
        //    var roles = await this._emailTemplateRootService.EmailTemplateRootSearch(search);
        //    if (roles == null)
        //        return NotFound();
        //    return Ok(roles);

        //}

        [HttpGet("getEmailsTemplateRoots")]
        public async Task<IActionResult> GetEmailRoots()
        {
            var result = await _emailTemplateRootService.Get();
            return Ok(result);
        }

        [HttpGet("emailTemplateRoot/{id}")]
        public async Task<IActionResult> GetEmailTemplateRoots(int id)
        {
            var result = await _emailTemplateRootService.GetEmailTemplateRootDetails(id);
            return Ok(result);
        }

        [HttpPost]
        [Route("emailTemplateRoot/create")]
        public async Task<IActionResult> PostEmailTemplateRoot(EmailTemplateRootModel data)
        {
            int id = await _emailTemplateRootService.CreatEmailTemplateRoot(data, UserId);
            return Ok(id);
        }

        [HttpPost]
        [Route("emailTemplateRoot/update")]
        public async Task<IActionResult> UpdateEmailTemplateRoot(EmailTemplateRootModel model)
        {
            await _emailTemplateRootService.UpdateEmailTemplateRoot(model, UserId);
            return Ok();
        }

        [HttpPost("emailTemplateRoot/delete")]
        public async Task<IActionResult> DeleteEmailTemplateRoots([FromBody] IEnumerable<int> ids)
        {
            if (ids == null)
                return BadRequest();

            IEnumerable<int> emails = await _emailTemplateRootService.DeleteEmailTemplateRoots(ids);
            return Ok(emails);
        }
    }
}
