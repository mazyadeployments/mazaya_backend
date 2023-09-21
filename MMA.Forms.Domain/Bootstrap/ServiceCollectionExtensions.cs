using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MMA.Forms.Domain.Services;
using MMA.WebApi.Shared.Interfaces.AgendaItemSections;

namespace MMA.Forms.Domain.Bootstrap
{
    public static class ServiceCollectionExtensions
    {
        public static void AddFormsServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddTransient<IAgendaItemFormsService, AgendaItemFormsService>();
        }
    }
}
