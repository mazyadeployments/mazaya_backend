using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using MMA.WebApi.Shared.Interfaces.ExpiredToken;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MMA.WebApi.Security
{
    public class IsTokenExpiredHandler : IAuthorizationHandler
    {
        private readonly IExpiredTokenService _expiredTokenService;
        private readonly IHttpContextAccessor _contextAccessor;

        public IsTokenExpiredHandler(
            IExpiredTokenService expiredTokenService,
            IHttpContextAccessor contextAccessor
        )
        {
            _expiredTokenService = expiredTokenService;
            _contextAccessor = contextAccessor;
        }

        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            /*NEW*/
            string token = ExtractToken();
            if (string.IsNullOrEmpty(token))
                return Task.CompletedTask;

            var pendingRequirements = context.PendingRequirements.ToList();
            foreach (var requirement in pendingRequirements)
            {
                if (!(requirement is IsTokenExpiredRequirement))
                    continue;

                var claimName = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name);
                if (claimName == null)
                    return Task.CompletedTask;

                var userId = claimName.Value;

                if (userId == null)
                    return Task.CompletedTask;

                bool expiredToken = _expiredTokenService.IsTokenExpired(token, userId).Result;
                if (expiredToken)
                    context.Fail();
                else
                    context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }

        private string ExtractToken()
        {
            string token = "";
            if (
                _contextAccessor.HttpContext.Request.Headers.ContainsKey("Authorization")
                && _contextAccessor.HttpContext.Request.Headers["Authorization"][0].StartsWith(
                    "Bearer "
                )
            )
            {
                token = _contextAccessor.HttpContext.Request.Headers["Authorization"][0].Substring(
                    "Bearer ".Length
                );
            }
            return token;
        }
    }
}
