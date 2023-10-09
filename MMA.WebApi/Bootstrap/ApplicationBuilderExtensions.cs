using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MMA.WebApi.DataAccess.Models;

namespace MMA.WebApi.Bootstrap
{
    public static class ApplicationBuilderExtensions
    {
        public static void SeedDatabase(this IApplicationBuilder app)
        {
            // migrate & seed all DB contexts in this method

            using (var smaDbContext = app.ApplicationServices.GetService<MMADbContext>())
            {
                smaDbContext.Database.Migrate();
            }
        }
    }
}
