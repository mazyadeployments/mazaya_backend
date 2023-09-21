using Flurl.Http;
using Microsoft.Extensions.Configuration;
using MMA.WebApi.Shared.Interfaces;
using MMA.WebApi.Shared.Interfaces.ApplicationUsers;
using MMA.WebApi.Shared.Interfaces.Membership;
using MMA.WebApi.Shared.Models.ServiceNowModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MMA.WebApi.Core.Services
{
    public class ServiceNow : IServiceNow
    {
        public static string AccessToken = null;

        private readonly IConfiguration _configuration;
        private Dictionary<string, string> mappedEmails { get; set; }
        private readonly IApplicationUserService _applicationUserService;
        private readonly IMembershipECardMakerRepository _membershipECardRepository;

        public ServiceNow(
            IApplicationUserService applicationUserService,
            IMembershipECardMakerRepository membershipECardRepository,
            IConfiguration configuration
        )
        {
            _applicationUserService = applicationUserService;
            _membershipECardRepository = membershipECardRepository;
            _configuration = configuration;
        }

        public async Task<IEnumerable<ServiceNowUserModel>> GetDataByMail(string email)
        {
            try
            {
                var ServiceNowData = await FetchDataFromServiceNowByEmail(email);

                HashSet<ServiceNowUserModel> UsersData = new HashSet<ServiceNowUserModel>();
                foreach (var data in ServiceNowData.result)
                {
                    ServiceNowUserModel userData =
                        JsonConvert.DeserializeObject<ServiceNowUserModel>(data);
                    UsersData.Add(userData);
                }
                return UsersData;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<IEnumerable<ServiceNowUserModel>> GetDataByMailwithOAuth(string email)
        {
            HashSet<ServiceNowUserModel> UsersData = null;

            if (AccessToken == null)
            {
                await CreateAccessToken();
            }
            ResultData result = null;

            try
            {
                result = await FetchDataFromServiceNowByEmailWithtoken(email, AccessToken);
                UsersData = CreateUserModel(result.result);
            }
            catch (Exception e)
            {
                await CreateAccessToken();
            }
            if (UsersData == null)
            {
                try
                {
                    result = await FetchDataFromServiceNowByEmailWithtoken(email, AccessToken);
                    UsersData = CreateUserModel(result.result);
                }
                catch
                {
                    return null;
                }
            }
            return UsersData;
        }

        private async Task<ResultData> FetchDataFromServiceNowByEmail(string email)
        {
            string url = (
                _configuration
                    .GetSection("ServiceNow")
                    .GetSection("urlDataByEmail")
                    .Value.ToString()
            );
            string password = _configuration.GetSection("ServiceNow").GetSection("password").Value;
            string username = _configuration.GetSection("ServiceNow").GetSection("username").Value;

            return await url.WithBasicAuth(username, password)
                .SetQueryParam("Requestor_email", email)
                .GetJsonAsync<ResultData>();
        }

        private async Task CreateAccessToken()
        {
            using (HttpClient client = new HttpClient())
            {
                var requestParams = new Dictionary<string, string>();
                requestParams.Add(
                    "client_id",
                    _configuration.GetSection("ServiceNow").GetSection("client_id").Value
                );
                requestParams.Add(
                    "grant_type",
                    _configuration.GetSection("ServiceNow").GetSection("grant_type").Value
                );
                requestParams.Add(
                    "client_secret",
                    _configuration.GetSection("ServiceNow").GetSection("client_secret").Value
                );
                requestParams.Add(
                    "password",
                    _configuration.GetSection("ServiceNow").GetSection("password").Value
                );
                requestParams.Add(
                    "username",
                    _configuration.GetSection("ServiceNow").GetSection("username").Value
                );
                string url = String.Empty;
                url = _configuration.GetSection("ServiceNow").GetSection("urlForOauthToken").Value;
                var req = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = new FormUrlEncodedContent(requestParams)
                };
                var res = await client.SendAsync(req);
                var json = res.Content.ReadAsStringAsync().Result;
                var tokenModel = JsonConvert.DeserializeObject<TokenModel>(json);
                AccessToken = tokenModel.access_token;
            }
        }

        private HashSet<ServiceNowUserModel> CreateUserModel(List<string> datas)
        {
            var retVal = new HashSet<ServiceNowUserModel>();

            foreach (var data in datas)
            {
                ServiceNowUserModel userData = JsonConvert.DeserializeObject<ServiceNowUserModel>(
                    data
                );
                retVal.Add(userData);
            }

            return retVal.ToHashSet();
        }

        private async Task<ResultData> FetchDataFromServiceNowByEmailWithtoken(
            string email,
            string token
        )
        {
            string url = (
                _configuration
                    .GetSection("ServiceNow")
                    .GetSection("urlDataByEmailWithToken")
                    .Value.ToString()
            );

            return await url.WithOAuthBearerToken(token)
                .SetQueryParam("Requestor_email", email)
                .GetJsonAsync<ResultData>();
        }
    }
}
