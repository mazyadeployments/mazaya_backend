using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
//using MMA.Delegation.Domain.Bootstrap;
//using MMA.Forms.Domain.Bootstrap;
//using MMA.Image.Domain.Bootstrap;
using MMA.WebApi.Core.Bootstrap;
using MMA.WebApi.Helpers;
using MMA.WebApi.Shared.Interfaces.Helpers;
using MMA.WebApi.Shared.Interfaces.Logger;
using MMA.WebApi.Shared.Models.Logger;
//using MMA.Workflow.Domain.Bootstrap;


namespace MMA.WebApi.Bootstrap
{

    public static class ServiceCollectionExtensions
    {
        public static void AddWebApi(this IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton(config);
            //add helpers 
            services.AddSingleton<IDocumentHelper, DocumentHelper>();
            // Add services
            services.AddServices(config);
            services.AddServicesDocuments(config);
            //services.AddServicesWorkflow(config);
            //services.AddServicesDelegation(config);
            //services.AddImageServices(config);
            //services.AddFormsServices(config);

            services.AddTransient<IAppLogger, Logger>();
        }
    }
}
