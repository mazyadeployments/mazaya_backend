using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MMA.WebApi.DataAccess.Models;

namespace MMA.WebApi.DataAccess.Bootstrap
{
    public static class ServiceCollectionExtensions
    {
        public static IDataAccessBuilder AddDataAccess(this IServiceCollection services, IConfiguration config)
        {
            // return builder for further (optional) configurations by composition root

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                   .AddEntityFrameworkStores<MMADbContext>();

            services.AddLocalization(o =>
            {

                o.ResourcesPath = "Resources";
            });

            return new DataAccessBuilder(services);
        }
    }
}
