using System.Collections.Generic;

namespace MMA.WebApi.Shared.Models.Configuration
{
    public class IdentityProvidersConfiguration
    {
        public IEnumerable<IdentityProviderInfo> LoginIdentityProviders { get; set; }
        public IEnumerable<IdentityProviderInfo> RegistrationIdentityProviders { get; set; }
    }
}
