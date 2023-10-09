using MMA.WebApi.Shared.Models.Configuration;
using System.Collections.Generic;

namespace MMA.WebApi.Shared.Interfaces.Configuration
{
    public interface IIdentityProviderService
    {
        IEnumerable<IdentityProviderInfo> GetIdentityProviders();
    }
}
