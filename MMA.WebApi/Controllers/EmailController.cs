using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MMA.WebApi.Shared.Interfaces.MailStorage;

namespace MMA.WebApi.Controllers
{
    [Route("[controller]")]
    [Authorize("Bearer")]
    [ApiController]
    public class EmailController : BaseController
    {
        private readonly IMailStorageService _mailStorageService;
        public EmailController(IMailStorageService mailStorageService)
        {
            _mailStorageService = mailStorageService;
        }

        [HttpGet("CheckMessageQueue")]
        public IActionResult CheckMessageQueue()
        {
            //API SECURITY: CHECK THIS ONE!!!
            return Ok(_mailStorageService.CheckMessageQueue());
        }
    }
}
