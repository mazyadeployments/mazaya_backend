using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Models;
using MMA.WebApi.Shared.Models.Logger;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.IO.Compression;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MMA.WebApi.Controllers
{
    [Route("[controller]")]
    [Authorize("Bearer")]
    [ApiController]
    public class SSOController : BaseController
    {
        private readonly IConfiguration _config;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly Logger _logger;

        public SSOController(
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            Logger logger
        )
        {
            _config = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        #region Azure AD IdP



        [Route("sso")]
        [HttpGet]
        public IActionResult SSORedirectAzure()
        {
            string AzureWebAppURL = _config["AzureSSO:WebAppURL"];

            string requestXML =
                $"<samlp:AuthnRequest xmlns='urn:oasis:names:tc:SAML:2.0:metadata' ID='id6c1c178c166d486687be4aaf5e482730' Version='2.0' IssueInstant='{DateTime.UtcNow.ToString("O")}' xmlns:samlp='urn:oasis:names:tc:SAML:2.0:protocol'><Issuer xmlns='urn:oasis:names:tc:SAML:2.0:assertion'>{AzureWebAppURL}</Issuer></samlp:AuthnRequest>";

            string samlRequest = DeflateAndEncode(requestXML);

            var urlBuilder = new StringBuilder();

            urlBuilder.Append(_config["AzureSSO:ADEndpoint"]);

            urlBuilder.Append("?SAMLRequest=" + System.Web.HttpUtility.UrlEncode(samlRequest));

            _logger.Info(urlBuilder.ToString());

            var model = new SSOModel();
            model.Url = urlBuilder.ToString();

            return Ok(model);
        }

        [Route("sso")]
        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> SSOLoginAzure([FromForm] ADFSResponseData responseData)
        {
            var xmlResponse = DecodeAndInflate(responseData.SAMLResponse);

            //var xmlResponse = System.Text.ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(responseData.SAMLResponse));

            _logger.Info(xmlResponse);

            TokenData token = new TokenData();

            var email = GetAttributeAzure(xmlResponse, "emailaddress");

            /*********************/


            var user = _userManager.FindByEmailAsync(email).Result;
            _logger.Info($"ADFS - user email {email} ");

            if (user != null)
            {
                _logger.Info("ADFS - user found");
                //var checkPwd =   signInManager.CheckPasswordSignInAsync(user, model.Password, false);
                //if (checkPwd.Succeeded)
                //{

                string JwtKey = _config["Jwt:Key"];
                string JwtIssuer = _config["Jwt:Issuer"];
                string duration = _config["TokenSettings:Duration"];
                string xmppDomain = _config["Chat:XMPPDomain"];

                var tokenString = GenerateJSONWebToken(
                    user,
                    JwtKey,
                    JwtIssuer,
                    duration,
                    xmppDomain
                );

                //TODO Aleksandar
                //var refreshToken = new RefreshTokenModel
                //{
                //    Username = user.UserName, // model.Username,
                //};
                ////todo get UserModel so we can store more info about the user in the token
                //await refreshTokenService.SaveAsync(refreshToken);


                token.Token = tokenString;
                // token.RefreshToken = refreshToken.Refreshtoken;

                // response = Ok(new { token = tokenString, refresh = refreshToken.Refreshtoken });
                //  }

                //_logger.Info($"Token {tokenString} RefreshToken {refreshToken.Refreshtoken}");
            }

            if (
                Request.QueryString.HasValue
                && Request.QueryString.Value.ToLower().Contains("mobile")
            )
            {
                // HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK); //  Request.CreateResponse(HttpStatusCode.OK);
                // response.Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(token));
                // response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/json");
                //// return response; // StatusCode((int)HttpStatusCode.OK, response);

                return Ok(token);
            }
            else
            {
                var webAppUrl = _config["AzureSSO:WebAppURL"];

                string url =
                    $"{webAppUrl}#/sso?"
                    + $"email={email}&"
                    + $"accessToken={System.Web.HttpUtility.UrlEncode(token.Token)}&"
                    + $"refreshToken={System.Web.HttpUtility.UrlEncode(token.RefreshToken)}";

                return Redirect(url);

                //HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Found); //Request.CreateResponse(HttpStatusCode.Found);
                //response.Headers.Location = new Uri(url);
                //return  StatusCode((int)HttpStatusCode.Found, response);
            }
        }

        private static string DeflateAndEncode(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            using (var output = new MemoryStream())
            {
                using (var zip = new DeflateStream(output, CompressionMode.Compress))
                {
                    zip.Write(bytes, 0, bytes.Length);
                }
                var base64 = Convert.ToBase64String(output.ToArray());

                return base64;
            }
        }

        private static string DecodeAndInflate(string str)
        {
            var utf8 = Encoding.UTF8;
            try
            {
                var bytes1 = Convert.FromBase64String(str);

                return utf8.GetString(bytes1);
            }
            catch { }

            try
            {
                var webencode = System.Web.HttpUtility.UrlDecode(str);

                // url encode first

                var bytes = Convert.FromBase64String(webencode);

                return utf8.GetString(bytes);
            }
            catch { }

            return "";

            //using (var output = new MemoryStream())
            //{
            //    using (var input = new MemoryStream(bytes))
            //    {
            //        using (var unzip = new DeflateStream(input, CompressionMode.Decompress))
            //        {
            //            unzip.CopyTo(output, bytes.Length);
            //            unzip.Close();
            //        }
            //        return utf8.GetString(output.ToArray());
            //    }
            //}
        }

        private string GetAttributeAzure(string wresult, string attributeName)
        {
            string result = "";
            var results = Regex.Matches(
                wresult
                    .Replace(" ", string.Empty)
                    .Replace("\"", string.Empty)
                    .Replace("/", string.Empty),
                "<AttributeName=http:schemas.xmlsoap.orgws200505identityclaims"
                    + attributeName
                    + "(.*?)<Attribute>",
                RegexOptions.Singleline
            );

            if (results.Count > 0)
            {
                result = Regex.Replace(results[0].Value, @"<(.|\n)*?>", string.Empty).Trim();
            }

            return result;
        }

        public static string GenerateJSONWebToken(
            ApplicationUser userInfo,
            string JwtKey,
            string JwtIssuer,
            string duration,
            string xmppDomain
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
                new Claim(JwtRegisteredClaimNames.Email, userInfo.Email),
                new Claim(JwtRegisteredClaimNames.Jti, userInfo.Id.ToString()),
                new Claim(ClaimTypes.Name, userInfo.Id),
            };

            var header = new JwtHeader(credentials);

            var payload = new JwtPayload(
                issuer: JwtIssuer,
                audience: JwtIssuer,
                claims: claims,
                notBefore: DateTime.UtcNow.AddSeconds(-1),
                expires: tokenExpired
            );

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

        #endregion
    }
}
