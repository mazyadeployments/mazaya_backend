using Microsoft.AspNetCore.Builder;
using MMA.WebApi.Shared.Exceptions.CustomExceptionMiddleware;

namespace MMA.WebApi.Shared.Exceptions.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
