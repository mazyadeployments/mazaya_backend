using Microsoft.AspNetCore.Authorization;

namespace MMA.WebApi.Security
{
    public class IsTokenExpiredRequirement : IAuthorizationRequirement { }
}
