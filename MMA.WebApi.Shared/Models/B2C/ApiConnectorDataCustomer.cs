using System.Collections.Generic;

namespace MMA.WebApi.Shared.Models.B2C
{
    public class ApiConnectorDataCustomer
    {
        public string Ui_locales { get; set; }
        public string Surname { get; set; }
        public string GivenName { get; set; }
        public List<ApiConnectorDataIdentities> Identities { get; set; }
    }
}
