using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Configuration;
using MMA.WebApi.Shared.Models.B2C;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Core.Services
{
    public class MicrosofGraphService
    {
        private readonly IConfiguration _configuration;

        public MicrosofGraphService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<MicrosoftGraphAPIResponse> GetUserClaims(string oid)
        {
            var token = await GetAccessToken();
            var queryParams = new Dictionary<string, string>
            {
                { "$select", "identities" }
            };


            var endpointRoot = _configuration["AzureB2C:MicrosoftGraphEndpoint"];
            var url = $"{endpointRoot}/users/{oid}";
            var result = await url.SetQueryParams(queryParams).WithHeader("Authorization", $"Bearer {token}")
            .WithHeader("Content-Type", "application/json").GetJsonAsync<MicrosoftGraphAPIResponse>();

            return result;
        }

        public async Task<string> GetAccessToken()
        {
            var result = await GetAuthorizationEndpoint().PostUrlEncodedAsync(new
            {
                client_id = _configuration["AzureB2C:ApiApplicationId"],
                client_secret = _configuration["AzureB2C:ClientSecret"],
                grant_type = "client_credentials",
                scope = _configuration["AzureB2C:MicrosoftGraphScope"]
            }).ReceiveJson();

            return result.access_token as string;
        }

        private string GetAuthorizationEndpoint()
        {
            var host = _configuration["AzureB2C:OAuthHost"];
            var tenantId = _configuration["AzureB2C:TenantId"];
            var authorizationEndpoint = _configuration["AzureB2C:AuthorizationEndpoint"];

            return $"{host}/{tenantId}/{authorizationEndpoint}";
        }
    }
}
