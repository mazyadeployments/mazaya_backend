using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces;
using MMA.WebApi.Shared.Interfaces.ApplicationUsers;
using MMA.WebApi.Shared.Interfaces.Membership;
using MMA.WebApi.Shared.Interfaces.Roles;
using MMA.WebApi.Shared.Models.Membership;
using System;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.WebApi.Controllers
{
    [Route("[controller]")]
    [Authorize("Bearer")]
    [ApiController]
    public class MembershipController : BaseController
    {
        private readonly IMembershipECardService _membershipECardService;
        private readonly IServiceNow _serviceNow;
        private readonly IRoleService _roleService;
        private readonly IApplicationUserService _applicationUserService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMembershipECardMakerRepository _membershipECardRepository;
        private readonly IConfiguration _configuration;

        public MembershipController(
            IMembershipECardService membershipECardService,
            IServiceNow serviceNow,
            IRoleService roleService,
            IApplicationUserService applicationUserService,
            UserManager<ApplicationUser> userManager,
            IMembershipECardMakerRepository membershipECardRepository,
            IConfiguration configuration
        )
        {
            _roleService = roleService;
            _serviceNow = serviceNow;
            _applicationUserService = applicationUserService;
            _membershipECardService = membershipECardService;
            _userManager = userManager;
            _membershipECardRepository = membershipECardRepository;
            _configuration = configuration;
        }

        [HttpPost()]
        public async Task<IActionResult> CreateNewMembershipCard(
            MembershipEcardModel membershipECardModel
        )
        {
            await _membershipECardService.AddMembershipCard(membershipECardModel, UserId);

            return Ok();
        }

        [HttpGet("apple-wallet/generic")]
        public async Task<IActionResult> GetAppleWalletCard()
        {
            string userId = UserId;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            var user = await _applicationUserService.GetById(userId);
            if (user == null)
            {
                return Unauthorized();
            }
            try
            {
                return _membershipECardService.GenerateAppleWalletCard(user, WalletCardType.Basic);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("google-wallet")]
        public async Task<IActionResult> GetGoogleWalletCard()
        {
            string userId = UserId;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            var user = await _applicationUserService.GetById(userId);
            if (user == null)
            {
                return Unauthorized();
            }
            try
            {
                var issuerId = _configuration["GoogleWallet:IssuerId"];
                var keyFilePath = _configuration["GoogleWallet:GOOGLE_APPLICATION_CREDENTIALS"];
                GoogleWalletHelper googleWalletHelper = new GoogleWalletHelper(
                    keyFilePath,
                    _configuration
                );
                var token = googleWalletHelper.CreateJWTNewObjects(
                    user.FirstName,
                    user.LastName,
                    user.ECardSequence,
                    issuerId,
                    user.Id + "class",
                    user.Id + "object"
                );
                return Ok(token);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet("google-wallet/membership/{id}")]
        public async Task<IActionResult> GetGoogleWalletCard(int id)
        {
            string userId = UserId;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            var user = await _applicationUserService.GetById(userId);
            if (user == null)
            {
                return Unauthorized();
            }
            if (id == 0)
                return BadRequest();
            try
            {
                var issuerId = _configuration["GoogleWallet:IssuerId"];
                var keyFilePath = _configuration["GoogleWallet:GOOGLE_APPLICATION_CREDENTIALS"];
                var azureStorage = _configuration["ConnectionStrings:AzureBlobStorage"];
                var azureStorageSplitted = azureStorage.Split("AccountName=");
                var azureStorageSplittedByAccountName = azureStorageSplitted[1].Split(";");
                GoogleWalletHelper googleWalletHelper = new GoogleWalletHelper(
                    keyFilePath,
                    _configuration
                );
                var ecard = await _membershipECardService.GetMembershipECardById(id);
                var membershipType = await _membershipECardService.GetWalletCardTypeByMembershipId(
                    ecard.MembershipId.ToString()
                );
                if (ecard == null)
                    throw new Exception();

                var token = googleWalletHelper.CreateJWTMembershipObjects(
                    ecard.Name,
                    ecard.Surname,
                    azureStorageSplittedByAccountName[0],
                    ecard.ValidTo == null ? "--" : ecard.ValidTo.Date.ToShortDateString(),
                    ecard.ECardSequence,
                    ecard.PhotoUrl,
                    membershipType,
                    issuerId,
                    ecard.ECardSequence + ".class",
                    ecard.Name + ecard.Surname + ".object"
                );
                return Ok(token);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet("GetMembershipsType")]
        public async Task<IActionResult> GetMembershipsType()
        {
            var roles = await _roleService.GetUserRoles(UserId);

            return Ok(
                _membershipECardService.GetMembershipsForUser(UserId, roles.Contains(Roles.Buyer))
            );
        }

        [HttpGet()]
        public async Task<IActionResult> GetOwnerCards()
        {
            var cards = await _membershipECardService.GetOwnerCards(UserId);

            return Ok(cards);
        }

        [HttpPost("UploadECardImage")]
        public async Task<IActionResult> UploadECardImage([FromForm] int type)
        {
            var files = HttpContext.Request.Form.Files;
            await _membershipECardService.UploadPicture(files, type);

            return Ok();
        }

        [HttpPost("UploadImageForTestAzureFunc")]
        public async Task<IActionResult> UploadImageForAzure()
        {
            var files = HttpContext.Request.Form.Files;
            return Ok(await _membershipECardService.UploadForTestuat(files));
        }

        [HttpGet("CreatePdfForMembershipECard/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> CreatePdfForMembershipECard(int id)
        {
            var data = await _membershipECardService.CreatePdfForMembershipECard(id, UserId);
            if (data != null)
                return File(data, "application/pdf");
            return BadRequest();
        }

        [HttpGet("apple-wallet/membership/{id}")]
        public async Task<IActionResult> GetMembershipAppleWalletCard(int id)
        {
            string userId = UserId;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            var user = await _applicationUserService.GetById(userId);
            if (user == null)
            {
                return Unauthorized();
            }
            try
            {
                return await _membershipECardService.GenerateMembershipAppleWalletCard(id, user.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("member")]
        public async Task<IActionResult> GetMemberCard()
        {
            var cards = await _membershipECardService.GetMemberCard(UserId);

            return Ok(cards);
        }

        [HttpGet("createmembershipbyemail/{email}")]
        public async Task<IActionResult> CreateMembershipsECardByEmail(string email)
        {
            var userData = await _roleService.CheckIfUserIsNotBuyer(UserId);

            if (userData.IsBuyer)
                return Unauthorized();

            var data = await _serviceNow.GetDataByMail(email);

            var user = await _userManager.FindByEmailAsync(email);

            if (data == null || user == null)
                return BadRequest();

            await _membershipECardService.CreateECardsForUser(data, user.Id);

            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("createECard-test/{email}")]
        public async Task<IActionResult> createECard_test(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return BadRequest();
            var UserData = await _serviceNow.GetDataByMail(email);
            if (UserData == null)
                return BadRequest();
            await _membershipECardRepository.CreateECardForUser(UserData, user.Id);

            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("getECard-test/{email}")]
        public async Task<IActionResult> GetECard_test(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            var cards = await _membershipECardService.GetOwnerCards(user.Id);
            return Ok(cards);
        }
    }
}
