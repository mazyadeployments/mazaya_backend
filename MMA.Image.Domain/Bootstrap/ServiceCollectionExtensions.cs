using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MMA.Image.Domain.Services;
using MMA.WebApi.Shared.Interfaces.Image;

namespace MMA.Image.Domain.Bootstrap
{
    public static class ServiceCollectionExtensions
    {
        public static void AddImageServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddTransient<IImageService, ImageService>();
        }
       
    }
}
