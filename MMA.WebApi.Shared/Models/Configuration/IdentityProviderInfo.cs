using System.Collections.Generic;

namespace MMA.WebApi.Shared.Models.Configuration
{
    public class IdentityProviderInfo
    {
        public string TypeId { get; set; }
        public string Authority { get; set; }
        public string ClientId { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> Scopes { get; set; }
        public string LoginApi { get; set; }
        public IEnumerable<string> KnownAuthorities { get; set; }
    }
}
