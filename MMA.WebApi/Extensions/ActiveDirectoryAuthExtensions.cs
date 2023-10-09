using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MMA.WebApi.Core.Helpers;
using MMA.WebApi.Shared.Models.B2C;

namespace MMA.WebApi.Extenstions
{
    public static class ActiveDirectoryAuthExtensions
    {
        public static IServiceCollection AddAzureActiveDirectoryAuthorization(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            var sections = new string[] { "AzureAD", "AzureB2C" };
            var identityProviders = ConfigHelper.ReadAllIdentityProviders(sections, configuration);

            foreach (var idp in identityProviders)
            {
                services.AddActiveDirectoryAuthorization(idp);
            }

            return services;
        }

        private static IServiceCollection AddActiveDirectoryAuthorization(
                this IServiceCollection services,
                IdentityProviderConfig identityProviderConfig
        )
        {
            var policyName = identityProviderConfig.PolicyName;
            services.AddAuthorization(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(policyName)
                    .RequireAuthenticatedUser()
                    .Build();

                options.AddPolicy(policyName, policy);
            })
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = policyName;
                options.DefaultChallengeScheme = policyName;
            })
            .AddJwtBearer(policyName, options =>
            {
                options.Authority = identityProviderConfig.Authority;
                options.Audience = identityProviderConfig.Audience;
            });

            return services;
        }
    }
}
