using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MMA.WebApi.Shared.Interfaces.RedeemOffer;
using MMA.WebApi.Shared.Interfaces.Roles;
using System.Threading.Tasks;


namespace MMA.WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize("Bearer")]

    public class RedeemOfferController : BaseController
    {
        public readonly IRedeemOfferService _redeemOfferService;
        private readonly IRoleService _roleService;

        public RedeemOfferController(IRedeemOfferService redeemOfferService, IRoleService roleService)
        {
            _redeemOfferService = redeemOfferService;
            _roleService = roleService;
        }
        [HttpGet("createqr/{offerId}")]
        public async Task<IActionResult> CreateQRCode(int offerId)
        {
            var roleContain = await _roleService.CheckIfUserIsNotBuyer(UserId);

            if (await _roleService.CheckRolesForCreatingQrCodeForRedeem(UserId))
            {
                return File(await _redeemOfferService.GenerateQRCode(offerId, UserId), "image/png");

            }
            return BadRequest("You don't have permission");

        }

        [HttpGet("{offerId}/{buyerId}")]
        public async Task<IActionResult> RedeemCode(int offerId, string buyerId)
        {
            var roleContain = await _roleService.CheckIfUserIsNotBuyer(UserId);

            if (!roleContain.IsBuyer)
            {
                var temp = await _redeemOfferService.RedeemQRCode(offerId, buyerId, UserId);
                if (temp)
                    return Ok("Success redeem");
                else return BadRequest("Offer is not yours.");
            }
            return BadRequest("You don't have permission");

        }


    }
}
