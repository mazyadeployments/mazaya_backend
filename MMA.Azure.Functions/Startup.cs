using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MMA.Azure.Functions;
using MMA.WebApi.DataAccess.Bootstrap;
using MMA.WebApi.DataAccess.MailStorage;
using MMA.WebApi.Shared.Interfaces.MailStorage;
using System;

[assembly: FunctionsStartup(typeof(Startup))]
namespace MMA.Azure.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            //var connectionString = builder.GetContext().Configuration.GetConnectionString("eMarketOffers");
            //ServiceCollectionExtensions.AddDataAccess(builder.Services, builder.GetContext().Configuration).WithMMADbContext(connectionString);

            MMA.Azure.Functions.Bootstrap.ServiceCollectionExtensions.AddServices(builder.Services, builder.GetContext().Configuration);
        }
    }
}
