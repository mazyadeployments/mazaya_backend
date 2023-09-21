using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MMA.WebApi.Shared.Interfaces.UserNotifications;
using System.Threading.Tasks;

namespace MMA.WebApi.Controllers
{
    [Route("[controller]")]
    [Authorize("Bearer")]
    [ApiController]
    public class UserNotificationsController : BaseController
    {
        private readonly IUserNotificationService _userNotificationService;
        public UserNotificationsController(IUserNotificationService userNotificationService)
        {
            _userNotificationService = userNotificationService;
        }


        [HttpGet]
        [Route("count")]
        public async Task<int> GetNotificationsCount()
        {
            string userId = this.UserId;
            return await _userNotificationService.GetNotificationCountForUser(userId);
        }

        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetNotifications()
        {
            string userId = this.UserId;
            var result = await _userNotificationService.GetNotificationForUser(userId);
            if (result == null)
                return NotFound();
            return Ok(result);
        }


        [HttpPost]
        [Route("{id}/acknowledge")]
        public async Task<IActionResult> AcknowledgeNotification(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            await this._userNotificationService.AcknowledgeNotification(id);
            return Ok();
        }

        [HttpPost]
        [Route("acknowledge")]
        public async Task<IActionResult> AcknowledgeAllNotifications()
        {
            string userId = this.UserId;
            await this._userNotificationService.AcknowledgeAllNotifications(userId);
            return Ok();
        }
    }
}
