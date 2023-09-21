using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MMA.WebApi.Attributes;
using MMA.WebApi.Shared.Constants;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.ApplicationUsers;
using MMA.WebApi.Shared.Interfaces.Image;
using MMA.WebApi.Shared.Interfaces.Roles;
using MMA.WebApi.Shared.Interfaces.UserNotifications;
using MMA.WebApi.Shared.Models.ApplicationUser;
using MMA.WebApi.Shared.Models.ApplicationUsers;
using MMA.WebApi.Shared.Models.Users;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace MMA.WebApi.Controllers
{
    [Route("[controller]")]
    [Authorize("Bearer")]
    [ApiController]
    public class UsersController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly IApplicationUserService _applicationUserService;
        private readonly IRoleService _roleService;
        private readonly IImageUtilsService _imageUtilsService;
        private readonly IUserNotificationService _userNotificationService;


        public UsersController(IConfiguration configuration, IApplicationUserService applicationUserService, IRoleService roleService, IImageUtilsService imageUtilsService, IUserNotificationService userNotificationService)
        {
            _configuration = configuration;
            _applicationUserService = applicationUserService;
            _roleService = roleService;
            _imageUtilsService = imageUtilsService;
            _userNotificationService = userNotificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            //API SECURITY: CHECK THIS ONE!!!
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                var users = await _applicationUserService.GetAll();
                if (users == null)
                    return NotFound();
                return Ok(users);
            }

            return Unauthorized();
        }

        [HttpGet("userexists/{email}")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckIfUserExists(string email)
        {
            var exists = await _applicationUserService.CheckIfUserExists(email);
            return Ok(JsonConvert.SerializeObject(exists));
        }

        [HttpGet("getuser")]
        public async Task<IActionResult> GetUser()
        {
            //API SECURITY: CHECK THIS ONE!!!
            string userId = UserId;
            var user = await _applicationUserService.GetById(userId);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        [HttpPost("buyers/page/{pageNumber}")]
        public async Task<IActionResult> GetAllBuyersPage(QueryModel queryModel, int pageNumber)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                queryModel.PaginationParameters = new PaginationParameters { PageNumber = pageNumber };
                queryModel.PaginationParameters.PageSize = 15;
                return Ok(await _applicationUserService.GetAllBuyersPage(queryModel));
            }

            return BadRequest("You do not have permission to do this action. Please contact system administartor.");
        }

        [HttpPost("adnocusers/page/{pageNumber}")]
        public async Task<IActionResult> GetAllAdnocUsersPage(QueryModel queryModel, int pageNumber)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                queryModel.PaginationParameters = new PaginationParameters { PageNumber = pageNumber };
                queryModel.PaginationParameters.PageSize = 15;
                return Ok(await _applicationUserService.GetAllAdnocUsersPage(queryModel));
            }

            return BadRequest("You do not have permission to do this action. Please contact system administartor.");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            //API SECURITY: CHECK THIS ONE!!!
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                var user = await _applicationUserService.GetById(id);
                if (user == null)
                    return NotFound();
                return Ok(user);
            }

            return Unauthorized();
        }

        [HttpGet("picture")]
        public async Task<IActionResult> GetUsersProfilePictureById()
        {
            //API SECURITY: CHECK THIS ONE!!!
            return Ok(await _applicationUserService.GetProfilePictureById(UserId));
        }

        [HttpGet("ecard")]
        public async Task<IActionResult> GetUsersECard()
        {
            var eCard = await _applicationUserService.GenerateECardForUser(UserId);
            return Ok(eCard);
        }

        [HttpPost()]
        public async Task<IActionResult> CreateOrUpdate(ApplicationUserModel model)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId) && model.Id != UserId)
            {
                //model.ImageSets = new List<ImageModel>();
                //if (model.Image != null)
                //    model.ImageSets.Add(model.Image);
                //var croppedImages = await _imageUtilsService.PrepareImagesForUpload(model.ImageSets);
                //model.ImageSets = croppedImages;
                return Ok(await _applicationUserService.CreateOrUpdateUser(model, UserId));
            }

            return BadRequest(new Exception("It's not possible to change Your profile details."));
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                if (id == UserId)
                    return BadRequest("Cannot delete logged in user.");

                await _applicationUserService.DeleteUser(id);
                return Ok(JsonConvert.SerializeObject(MessageConstants.DeleteMessages.ItemSuccessfullyDeleted("User")));
            }

            return BadRequest("You do not have permission to do this action. Please contact system administartor.");
        }
        [HttpPost]
        [Route("AddUsersWithMobileFromSpecificExcelFile")]
        public async Task<IActionResult> AddUsersWithMobileFromSpecificExcelFile()
        {

            var files = HttpContext.Request.Form.Files;

            await _applicationUserService.AddUsersInvitationForMobileFromSpecificExcelFile(files, UserId);
            await _userNotificationService.CreateNotificationForUserInvitation(UserId);

            return Ok();
        }
        [HttpPost]
        [Route("AddUsersWithEmailFromSpecificExcelFile/{userType}")]
        public async Task<IActionResult> AddUsersWithEmailFromSpecificExcelFile(int userType)
        {

            var files = HttpContext.Request.Form.Files;

            await _applicationUserService.AddUsersInvitationForEmailFromSpecificExcelFile(files, UserId, userType);
            await _userNotificationService.CreateNotificationForUserInvitation(UserId);

            return Ok();
        }
        [HttpDelete]
        [Route("username/{username}")]
        public async Task<IActionResult> DeleteByUsername(string username)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                await _applicationUserService.DeleteByUsername(username);
                return Ok(JsonConvert.SerializeObject(MessageConstants.DeleteMessages.ItemSuccessfullyDeleted(username)));
            }

            return BadRequest("You do not have permission to do this action. Please contact system administartor.");
        }

        [HttpPost]
        [Route("invitenewuser")]
        public async Task<IActionResult> InviteUser(InviteUserModel invitedUserModel)
        {

            var isRegistered = await _applicationUserService.CheckIfUserExists(invitedUserModel.Email);
            if (isRegistered)
            {
                return BadRequest("The user is already registered.");
            }

            var invited = await _applicationUserService.DoesUserInvited(invitedUserModel.Email);
            if (invited)
            {
                return BadRequest("User is already invited.");
            }

            if (!await _applicationUserService.CanUserSendInvitation(UserId))
            {

                await _applicationUserService.SetUserInvitation(UserId, invitedUserModel.Email);
                return Ok();
            }

            return BadRequest("You do not have permission to do this action. Please contact system administartor.");
        }

        [HttpPost("update-ecard")]
        public async Task<IActionResult> UpdateECardSequence(UpdateECardModel updateECardModel)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                return Ok(await _applicationUserService.UpdateECardSequence(updateECardModel.Username, updateECardModel.ECardSequence));
            }

            return BadRequest();
        }

        [HttpDelete]
        [Route("invited/{username}")]
        public async Task<IActionResult> DeleteInvitedUser(string username)
        {
            await _applicationUserService.DeleteOfInvitedUser(UserId, username);
            return Ok();
        }

        [HttpGet]
        [Route("invited")]
        public async Task<IActionResult> GetInvitedUsers()
        {
            return Ok(await _applicationUserService.GetInvitedUsers(UserId));
        }

        [HttpGet("all-invited-users")]
        public async Task<IActionResult> GetAllUserInvitations()
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
                return Ok(await _applicationUserService.GetAllUserInvitations());

            return Unauthorized();
        }


        [HttpPost]
        [Route("fcmmessagestoken")]
        [ValidateModelStateFilter]
        public async Task<IActionResult> SetUserFcmMessagesToken(FcmMessageTokenModel fcmMessageTokenModel)
        {
            await _applicationUserService.SetFcmMessagesToken(UserId, fcmMessageTokenModel.Token);

            return Ok();
        }

        [HttpDelete]
        [Route("remove-fcmmessagestoken")]
        [ValidateModelStateFilter]
        public async Task<IActionResult> RemoveUserFcmMessagesToken(FcmMessageTokenModel fcmMessageTokenModel)
        {
            await _applicationUserService.RemoveUserFcmMessagesToken(UserId, fcmMessageTokenModel.Token);

            return Ok();
        }

        [HttpGet("receive-announcement/{status}")]
        public async Task<IActionResult> ChangeReceivedAnnouncementStatus(bool status)
        {
            return Ok(await _applicationUserService.ChangeReceivedAnnouncementStatusForUser(status, UserId));
        }
    }
}
