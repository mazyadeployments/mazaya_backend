using System.Collections.Generic;

namespace MMA.WebApi.Shared.Models.B2C
{
    public class ApiConnectorDataSupplier
    {

        public ApiConnectorDataSupplier()
        {
            Identities = new List<ApiConnectorDataIdentities>();
        }
        //Extension_7a33a5bc18794ba4bf654210956f3a57_CompanyName
        public string CompanyName { get; set; }
        //Extension_7a33a5bc18794ba4bf654210956f3a57_CompanyDescription
        public string CompanyDescription { get; set; }
        public string Ui_locales { get; set; }
        public string Email { get; set; }
        public List<ApiConnectorDataIdentities> Identities { get; set; }
    }
}
