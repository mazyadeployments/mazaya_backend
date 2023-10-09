using System.Collections.Generic;

namespace MMA.WebApi.Shared.Models.B2C
{
    public class ApiConnectorDataIdentities
    {
        public string SignInType { get; set; }
        public string Issuer { get; set; }
        public string IssuerAssignedId { get; set; }
    }

    public class MicrosoftGraphAPIResponse
    {
        public IEnumerable<ApiConnectorDataIdentities> Identities { get; set; }
    }
}
