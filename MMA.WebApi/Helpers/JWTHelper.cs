using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Models.Companies;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;

namespace MMA.WebApi.Helpers
{
    public class JWTHelper
    {
        public static string GenerateJSONWebToken(
            ApplicationUser userInfo,
            string JwtKey,
            string JwtIssuer,
            string duration,
            string xmppDomain,
            List<string> roles,
            string logoutLink,
            CompanyModel company = null
        )
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            int tokenDurationInMInutes = 0;
            bool tokenDuration = int.TryParse(duration, out tokenDurationInMInutes);
            DateTime tokenExpired =
                (tokenDuration)
                    ? DateTime.Now.AddMinutes(tokenDurationInMInutes)
                    : DateTime.Now.AddMinutes(480);
            //var xmppDomain = _config["Chat:XMPPDomain"];


            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.UserName),
                new Claim("xmpp_jid", string.Format("{0}@{1}", userInfo.Id, xmppDomain)),
                new Claim(JwtRegisteredClaimNames.Email, userInfo.UserName),
                new Claim(
                    JwtRegisteredClaimNames.Iat,
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()
                ),
                new Claim(ClaimTypes.Name, userInfo.Id),
                new Claim(JwtRegisteredClaimNames.Jti, userInfo.Id.ToString())
            };

            var header = new JwtHeader(credentials);

            var payload = new JwtPayload(
                issuer: JwtIssuer,
                audience: JwtIssuer,
                claims: claims,
                notBefore: DateTime.UtcNow.AddSeconds(-1),
                expires: tokenExpired
            );
            payload.Add("permissions", new { permission = "all" });
            payload.Add("userId", userInfo.Id.ToString());
            payload.Add("Name", userInfo.Id.ToString());
            payload.Add("roles", roles);
            payload.Add("logoutLink", logoutLink);
            if (company != null)
            {
                payload.Add("companyId", company.Id.ToString());
                payload.Add("companyStatus", company.ApproveStatus);
            }
            //  IEnumerable<UserPermissionModel> userPermissionsList = usersPermissionsService.GetPermissions(userInfo.Id).Result;
            //  payload.Add("Permissions", userPermissionsList);

            var token = new JwtSecurityToken(header, payload);

            //var token = new JwtSecurityToken(
            //                issuer: _config["Jwt:Issuer"],
            //                audience: _config["Jwt:Issuer"],
            //                claims: claims,
            //                expires: (tokenDuration) ? DateTime.Now.AddMinutes(tokenDurationInMInutes) : DateTime.Now.AddMinutes(480),
            //                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static string GetUserIdFromOneHubToken(
            string accessToken,
            UserManager<ApplicationUser> service
        )
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(accessToken);
            //  Validate(accessToken, jwtToken);

            if (jwtToken.Payload == null)
                return null;

            var email = Convert.ToString(jwtToken.Payload["email"]);

            if (email.IsNullOrEmpty())
                return null;

            var user = service.FindByEmailAsync(email).Result;

            if (user == null)
                return null;

            return user.Id;
        }

        private static JwtSecurityToken Validate(string token, JwtSecurityToken jwtToken)
        {
            string stsDiscoveryEndpoint =
                "https://login.microsoftonline.com/common/v2.0/.well-known/openid-configuration";

            ConfigurationManager<OpenIdConnectConfiguration> configManager =
                new ConfigurationManager<OpenIdConnectConfiguration>(
                    stsDiscoveryEndpoint,
                    new OpenIdConnectConfigurationRetriever()
                );
            OpenIdConnectConfiguration config = configManager.GetConfigurationAsync().Result;

            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                ValidAudience = "8ab18a63-ea76-49c7-bc66-f7d8a1aabfc2",
                ValidateAudience = false,
                //ValidateAudience = true,
                ValidateIssuer = false,
                IssuerSigningKeys = config.SigningKeys,
                ValidateLifetime = true
            };

            JwtSecurityTokenHandler tokendHandler = new JwtSecurityTokenHandler();

            SecurityToken jwt;

            var result = tokendHandler.ValidateToken(token, validationParameters, out jwt);
            return jwt as JwtSecurityToken;
        }

        public static JwtSecurityToken ValidateOneHubToken(
            string token,
            JwtSecurityToken jwtToken,
            IConfiguration _configuration
        )
        {
            HttpClient http = new HttpClient();

            string stsDiscoveryEndpoint =
                "https://login.microsoftonline.com/common/v2.0/.well-known/openid-configuration";

            ConfigurationManager<OpenIdConnectConfiguration> configManager =
                new ConfigurationManager<OpenIdConnectConfiguration>(
                    stsDiscoveryEndpoint,
                    new OpenIdConnectConfigurationRetriever(),
                    http
                );

            OpenIdConnectConfiguration config = configManager.GetConfigurationAsync().Result;

            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                IssuerSigningKeys = config.SigningKeys,
                ValidateLifetime = true,
            };

            JwtSecurityTokenHandler tokendHandler = new JwtSecurityTokenHandler();

            SecurityToken jwt;

            // var appid = jwtToken.Claims.FirstOrDefault(x => x.Type == "appid");
            var audience = jwtToken.Claims.FirstOrDefault(x => x.Type == "aud");
            var iss = jwtToken.Claims.FirstOrDefault(x => x.Type == "iss");

            var audienceFromConfig = _configuration.GetSection("OneHub").GetSection("aud").Value;
            var issFromConfig = _configuration.GetSection("OneHub").GetSection("iss").Value;

            if (audienceFromConfig != null && issFromConfig != null)
            {
                if (iss.Value != issFromConfig || audience.Value != audienceFromConfig)
                {
                    throw new Exception("Invalid token");
                }
            }

            var result = tokendHandler.ValidateToken(token, validationParameters, out jwt);

            return jwt as JwtSecurityToken;
        }
    }
}
