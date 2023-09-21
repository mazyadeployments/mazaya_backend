using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace MMA.Forms.Domain.Controllers
{
    public class BaseController : ControllerBase
    {
        private string userId = "SystemGenerated";

        protected string UserId
        {
            get
            {
                userId = (User?.Identity?.IsAuthenticated == true && User?.FindFirst(JwtRegisteredClaimNames.Jti).Value != null) ? User.FindFirst(JwtRegisteredClaimNames.Jti).Value : "SystemGenerated";
                return userId;
            }
        }
    }
}
