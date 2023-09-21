using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MMA.Documents.Domain.Services;
using MMA.WebApi.Shared.Interfaces.Domain.Document;

namespace MMA.WebApi.Core.Bootstrap
{
    public static class ServiceCollectionExtensions
    {

        public static void AddServicesDocuments(this IServiceCollection services, IConfiguration config)
        {
            services.AddTransient<IDocumentService, DocumentService>();
        }
    }
}
