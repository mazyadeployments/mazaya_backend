using Microsoft.Extensions.Configuration;
using MMA.WebApi.Core.Helpers;
using MMA.WebApi.Shared.Interfaces.Configuration;
using MMA.WebApi.Shared.Models.B2C;
using MMA.WebApi.Shared.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MMA.WebApi.Services
{
    public class IdentityProvidersService : IIdentityProviderService
    {
        private const string SEPARATOR = ";";
        private const string VERSION_SUFFIX = "/v2.0/";
        private readonly IConfiguration _configuration;
        private readonly IEnumerable<string> _sections;

        public IdentityProvidersService(IConfiguration configuration)
        {
            _configuration = configuration;
            _sections = _configuration["OAuthSections"].Split(SEPARATOR);
        }

        public IEnumerable<IdentityProviderInfo> GetIdentityProviders()
        {
            return ConfigHelper.ReadAllIdentityProviders(_sections, _configuration).Select(idp => GetIdentityProvider(idp));
        }

        private IdentityProviderInfo GetIdentityProvider(IdentityProviderConfig identityProviderConfig)
        {
            // NOTE: client application doesn't need version suffix for B2C
            var authority = identityProviderConfig.Authority.Replace(VERSION_SUFFIX, String.Empty);
            var sectionName = identityProviderConfig.SectionName;

            var frontendApplicationId = _configuration[$"{sectionName}:FrontendApplicationId"];
            var frontendRequiredScope = _configuration[$"{sectionName}:FrontendRequiredScope"];

            var identityProvider = new IdentityProviderInfo
            {
                TypeId = identityProviderConfig.TypeId,
                Authority = authority,
                ClientId = frontendApplicationId,
                Name = identityProviderConfig.DisplayName,
                Scopes = new[] { frontendRequiredScope },
                LoginApi = identityProviderConfig.LoginApi,
                KnownAuthorities = GetKnownAuthorities()
            };

            return identityProvider;
        }

        private IEnumerable<string> GetKnownAuthorities()
        {
            var configuredKnownAuthorities = _sections.Select(sectionName => _configuration[$"{sectionName}:KnownAuthorities"] ?? String.Empty);
            return configuredKnownAuthorities.Where(s => s != String.Empty);
        }
    }
}
