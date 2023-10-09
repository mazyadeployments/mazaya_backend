using Microsoft.Extensions.Configuration;
using MMA.WebApi.Shared.Models.B2C;
using System.Collections.Generic;
using System.Linq;

namespace MMA.WebApi.Core.Helpers
{
    public static class ConfigHelper
    {
        public static IEnumerable<IdentityProviderConfig> GetIdentityProviders(string sectionName, IConfiguration configuration)
        {
            var identityProviderConfigs = configuration.GetSection(sectionName).GetSection("IdentityProviders").GetChildren().ToList();

            foreach (var item in identityProviderConfigs)
            {
                var config = new IdentityProviderConfig();
                item.Bind(config);
                config.SectionName = sectionName;

                yield return config;
            }
        }

        public static IEnumerable<IdentityProviderConfig> ReadAllIdentityProviders(IEnumerable<string> sections, IConfiguration configuration)
        {
            return sections
                .Select(s => GetIdentityProviders(s, configuration))
                .Aggregate(Enumerable.Empty<IdentityProviderConfig>(), (all, arrayItem) => all.Concat(arrayItem));
        }

        public static IEnumerable<string> AuthenticationClaims(this IConfiguration configuration)
        {
            return configuration["AzureB2C:AcceptedClaims"].Split(";");
        }
    }
}
