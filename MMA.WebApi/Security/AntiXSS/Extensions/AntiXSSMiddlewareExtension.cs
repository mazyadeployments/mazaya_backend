using Microsoft.AspNetCore.Builder;
using MMA.WebApi.Security.NewFolder;

namespace MMA.WebApi.Security.AntiXSS.Extensions
{
    public static class AntiXSSMiddlewareExtension
    {
        public static IApplicationBuilder UseAntiXSSMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AntiXSSMiddleware>();
        }
    }
}
