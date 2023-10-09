using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Interfaces.B2C;
using MMA.WebApi.Shared.Interfaces.Companies;
using MMA.WebApi.Shared.Interfaces.MailStorage;
using MMA.WebApi.Shared.Interfaces.Membership;
using MMA.WebApi.Shared.Interfaces.RefreshToken;
using MMA.WebApi.Shared.Models.B2C;
using MMA.WebApi.Shared.Models.Companies;
using MMA.WebApi.Shared.Models.RefreshToken;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.WebApi.Core.Services
{
    public class B2CService : IB2CService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly ICompanyService _companyService;
        private readonly IMembershipECardRepository _membershipECardRepository;
        private readonly IMailStorageService _mailStorageService;

        public B2CService(
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IRefreshTokenService refreshTokenService,
            ICompanyService companyService,
            IMailStorageService mailStorageService,
            IMembershipECardRepository membershipECardRepository
        )
        {
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _refreshTokenService = refreshTokenService;
            _companyService = companyService;
            _mailStorageService = mailStorageService;
            _membershipECardRepository = membershipECardRepository;
        }

        public JwtSecurityToken DecodeJWTToken(string token)
        {
            var stream = token;
            var handler = new JwtSecurityTokenHandler();
            var tokenS = handler.ReadToken(stream) as JwtSecurityToken;

            return tokenS;
        }

        public async Task<JwtSecurityToken> GetJwtSecurityToken(string code, Roles role)
        {
            using (HttpClient client = new HttpClient())
            {
                var requestParams = new Dictionary<string, string>();
                requestParams.Add("client_id", _configuration["B2CSettings:client_id"]);
                requestParams.Add("scope", _configuration["B2CSettings:scope"]);
                requestParams.Add("redirect_uri", _configuration["B2CSettings:redirect_uri"]);
                requestParams.Add("grant_type", _configuration["B2CSettings:grant_type"]);
                requestParams.Add("client_secret", _configuration["B2CSettings:client_secret"]);
                requestParams.Add("code", code);

                string url = String.Empty;

                url =
                    role == Roles.Buyer
                        ? _configuration["B2CSettings:urlBuyer"]
                        : _configuration["B2CSettings:urlSupplier"];

                var req = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = new FormUrlEncodedContent(requestParams)
                };
                var res = await client.SendAsync(req);
                var json = res.Content.ReadAsStringAsync().Result;
                var b2cResponse = JsonConvert.DeserializeObject<B2CResponseModel>(json);

                var stream = b2cResponse.Id_token;
                if (stream == null)
                    return null;

                var handler = new JwtSecurityTokenHandler();
                var tokenS = handler.ReadToken(stream) as JwtSecurityToken;

                return tokenS;
            }
        }

        public async Task<B2CLoginModel> LoginBuyer(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return null;
            }

            //var checkPwd = await _signInManager.CheckPasswordSignInAsync(user, "", false);

            if (!user.Active)
            {
                return null;
            }

            var tokenString = GenerateJSONWebToken(user);

            var refreshToken = new RefreshTokenModel { Username = username };

            await _refreshTokenService.SaveAsync(refreshToken);

            return new B2CLoginModel
            {
                Token = tokenString,
                Refresh = refreshToken.Refreshtoken,
                BaseURL = _configuration["UatURL:Url"]
            };
        }

        public async Task<B2CLoginModel> LoginSupplier(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return null;
            }

            //var checkPwd = await _signInManager.CheckPasswordSignInAsync(user, "", false);

            if (!user.Active)
            {
                return null;
            }

            var tokenString = GenerateJSONWebToken(user);

            var refreshToken = new RefreshTokenModel { Username = username };

            await _refreshTokenService.SaveAsync(refreshToken);

            return new B2CLoginModel
            {
                Token = tokenString,
                Refresh = refreshToken.Refreshtoken,
                BaseURL = _configuration["UatURL:Url"]
            };
        }

        public async Task<B2CLoginModel> RegisterBuyer(
            string email,
            string firstName,
            string lastName,
            string mobileNumber
        )
        {
            ApplicationUser applicationUser = new ApplicationUser();
            applicationUser.UserName = email;
            applicationUser.FirstName = firstName;
            applicationUser.NormalizedUserName = email;
            applicationUser.CreatedOn = DateTime.UtcNow;
            applicationUser.UpdatedOn = DateTime.UtcNow;
            applicationUser.Email = email;
            applicationUser.EmailConfirmed = false;
            applicationUser.PhoneNumber = mobileNumber;
            applicationUser.LastName = lastName;
            applicationUser.Active = true;

            //TODO: See about password
            // We don't use this password anymore, it's just a dummy password (Auth is done on b2c)
            var result = await _userManager.CreateAsync(applicationUser);
            if (result.Succeeded)
            {
                await _membershipECardRepository.FindMembershipCardForUserAndUpdate(
                    new Shared.Models.ServiceNowModels.MemberModel()
                    {
                        Id = applicationUser.Id,
                        mail = applicationUser.Email
                    }
                );
                var resultRole = await _userManager.AddToRoleAsync(
                    applicationUser,
                    Declares.Roles.Buyer.ToString()
                );
                return new B2CLoginModel() { BaseURL = _configuration["UatURL:Url"] };
            }
            else
            {
                return null;
            }
        }

        public async Task<B2CLoginModel> RegisterSupplier(
            string email,
            string companyDescription,
            string companyName,
            string mobileNumber
        )
        {
            ApplicationUser applicationUser = new ApplicationUser();
            applicationUser.UserName = email;
            applicationUser.NormalizedUserName = email;
            applicationUser.CreatedOn = DateTime.UtcNow;
            applicationUser.UpdatedOn = DateTime.UtcNow;
            applicationUser.Email = email;
            applicationUser.EmailConfirmed = false;
            applicationUser.PhoneNumber = mobileNumber;
            applicationUser.Active = true;

            //TODO: See about password
            // We don't use this password anymore, it's just a dummy password (Auth is done on b2c)
            var result = await _userManager.CreateAsync(applicationUser);
            if (result.Succeeded)
            {
                var createdUser = await _userManager.FindByNameAsync(applicationUser.UserName);
                await _membershipECardRepository.FindMembershipCardForUserAndUpdate(
                    new Shared.Models.ServiceNowModels.MemberModel()
                    {
                        Id = createdUser.Id,
                        mail = createdUser.Email
                    }
                );
                CompanyModel company = new CompanyModel();
                company.CompanyDescription = companyDescription;
                company.NameEnglish = companyName;
                company.OfficialEmail = email;
                //company.MobileInternationalNumber = mobileNumber;
                company.ApproveStatus = Declares.SupplierStatus.MissingTradeLicense.ToString();
                company.CreatedBy = createdUser.Id;
                company.UpdatedBy = createdUser.Id;
                company.CreatedOn = DateTime.UtcNow;
                company.UpdatedOn = DateTime.UtcNow;
                company.EstablishDate = DateTime.UtcNow;
                company.ExpiryDate = DateTime.UtcNow.AddDays(365);
                //var resultRole = await _userManager.AddToRoleAsync(applicationUser, "Supplier Admin");

                await _companyService.RegisterCompanyAsync(company, createdUser.Id);

                //Uzmi sve coordinatore
                var coordinators = await _userManager.GetUsersInRoleAsync("ADNOC Coordinator");

                if (coordinators.Count > 0)
                {
                    var messageTemplate = Declares
                        .MessageTemplateList
                        .Supplier_Registration_Notify_Coordinator;
                    var emailData = _mailStorageService.CreateMailData(
                        coordinators.FirstOrDefault().Id,
                        null,
                        company.NameEnglish,
                        messageTemplate,
                        false
                    );
                    await _mailStorageService.CreateMail(emailData);
                }

                return new B2CLoginModel() { BaseURL = _configuration["UatURL:Url"] };
            }
            else
            {
                return null;
            }
        }

        private string GenerateJSONWebToken(ApplicationUser applicationUser)
        {
            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])
            );
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            int tokenDurationInMInutes = 0;
            bool tokenDuration = int.TryParse(
                _configuration["TokenSettings:Duration"],
                out tokenDurationInMInutes
            );
            DateTime tokenExpired =
                (tokenDuration)
                    ? DateTime.Now.AddMinutes(tokenDurationInMInutes)
                    : DateTime.Now.AddMinutes(480);
            var company = _companyService.GetMySupplierCompany(applicationUser.Id).Result;

            var roles = _userManager.GetRolesAsync(applicationUser).GetAwaiter().GetResult();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, applicationUser.UserName),
                new Claim(JwtRegisteredClaimNames.Email, applicationUser.Email),
                new Claim(JwtRegisteredClaimNames.Jti, applicationUser.Id.ToString()),
                new Claim(
                    JwtRegisteredClaimNames.Iat,
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()
                ),
                new Claim(ClaimTypes.Name, applicationUser.Id)
            };

            var header = new JwtHeader(credentials);

            var payload = new JwtPayload(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Issuer"],
                claims: claims,
                notBefore: DateTime.UtcNow.AddSeconds(-1),
                expires: tokenExpired
            );

            payload.Add("permissions", new { permission = "all" });
            payload.Add("roles", roles);
            payload.Add("userId", applicationUser.Id.ToString());
            if (company != null)
            {
                payload.Add("companyId", company.Id.ToString());
                payload.Add("companyStatus", company.ApproveStatus);
            }
            var token = new JwtSecurityToken(header, payload);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private byte[] GetHash(string inputString)
        {
            using (HashAlgorithm algorithm = SHA256.Create())
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        private string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }
    }
}
