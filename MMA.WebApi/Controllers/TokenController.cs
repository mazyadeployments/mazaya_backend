using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using MMA.WebApi.Core.Helpers;
using MMA.WebApi.Core.Services;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Extensions;
using MMA.WebApi.Services;
using MMA.WebApi.Shared.Constants;
using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Interfaces.ApplicationUsers;
using MMA.WebApi.Shared.Interfaces.Companies;
using MMA.WebApi.Shared.Interfaces.Configuration;
using MMA.WebApi.Shared.Interfaces.ExpiredToken;
using MMA.WebApi.Shared.Interfaces.MailStorage;
using MMA.WebApi.Shared.Interfaces.Membership;
using MMA.WebApi.Shared.Interfaces.RefreshToken;
using MMA.WebApi.Shared.Models.Account;
using MMA.WebApi.Shared.Models.Companies;
using MMA.WebApi.Shared.Models.RefreshToken;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.WebApi.Controllers
{
    [ApiController]
    public class TokenController : BaseController
    {
        private readonly IConfiguration _config;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly ICompanyService _companyService;
        private readonly IApplicationUserService _applicationUserService;
        private readonly IMailStorageService _mailStorageServiceService;
        private readonly IIdentityProviderService _identityProvidersService;
        private readonly UserService _userService;
        private readonly IJwtService _jwtService;
        private readonly IExpiredTokenService _expiredTokenService;
        private readonly MicrosofGraphService _microsoftGraphService;
        private readonly IApplicationUsersRepository _applicationUserRepository;
        private readonly IMembershipECardRepository _membershipECardRepository;


        public TokenController(IConfiguration configuration, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IApplicationUserService applicationUserService,
                               IRefreshTokenService refreshTokenService,
                               IApplicationUsersRepository applicationUserRepository, ICompanyService companyService, UserService userService, IMailStorageService mailStorageServiceService,
                               IIdentityProviderService identityProvidersService, IJwtService jwtService, MicrosofGraphService microsoftGraphService, IMembershipECardRepository membershipECardRepository,
                               IExpiredTokenService expiredTokenService)
        {
            _config = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _refreshTokenService = refreshTokenService;
            _applicationUserService = applicationUserService;
            _companyService = companyService;
            _mailStorageServiceService = mailStorageServiceService;
            _identityProvidersService = identityProvidersService;
            _jwtService = jwtService;
            _expiredTokenService = expiredTokenService;
            _userService = userService;
            _microsoftGraphService = microsoftGraphService;
            _applicationUserRepository = applicationUserRepository;
            _membershipECardRepository = membershipECardRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);

            if (user == null)
            {
                return BadRequest("Your account is not found.");
            }

            var checkPwd = await _signInManager.CheckPasswordSignInAsync(user, model.Loz, false);

            if (!checkPwd.Succeeded)
            {
                return BadRequest("Your password is not correct.");
            }

            if (!user.Active)
            {
                return BadRequest("Your account is not active.");
            }

            var tokenString = GenerateJSONWebToken(user);

            var refreshToken = new RefreshTokenModel
            {
                Username = model.Username
            };

            await _userManager.SetPlatformInfo(user);

            await _refreshTokenService.SaveAsync(refreshToken);

            return Ok(new { token = tokenString, refresh = refreshToken.Refreshtoken });
        }

        #region Logout
        [HttpPost("logout")]
        [Authorize("Bearer")]
        public async Task<IActionResult> Logout()
        {
            var token = GetTokenFromHeader();

            if (!string.IsNullOrEmpty(token))
            {
                string userId = UserId;
                int expiredTokenId = await _expiredTokenService.AddTokenToExpired(token, UserId);
                return Ok(expiredTokenId);
            }
            return Unauthorized();
        }

        private string GetTokenFromHeader() => Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");

        #endregion


        [HttpPost("registration")]
        public async Task<IActionResult> Registration(RegistrationModel model)
        {
            StringBuilder sb = new StringBuilder();

            if (model.Role == "Supplier")
            {
                //supplier
                if (isModelValidSupplier(model) == String.Empty)
                {
                    ApplicationUser applicationUser = new ApplicationUser();
                    applicationUser.UserName = model.Email;
                    applicationUser.NormalizedUserName = model.Email;
                    applicationUser.CreatedOn = DateTime.UtcNow;
                    applicationUser.UpdatedOn = DateTime.UtcNow;
                    applicationUser.Email = model.Email;
                    applicationUser.EmailConfirmed = false;
                    applicationUser.PhoneNumber = model.MobileNumber;
                    applicationUser.Active = true;
                    applicationUser.UserType = GetUserType(model.Email);

                    var result = await _userManager.CreateAsync(applicationUser, model.Loz);

                    if (result.Succeeded)
                    {
                        var createdUser = await _userManager.FindByNameAsync(applicationUser.UserName);
                        CompanyModel company = new CompanyModel();
                        company.CompanyDescription = model.CompanyDescription;
                        company.NameEnglish = model.CompanyName;
                        company.OfficialEmail = model.Email;
                        //company.MobileInternationalNumber = model.MobileNumber;
                        company.ApproveStatus = Declares.SupplierStatus.MissingTradeLicense.ToString();
                        company.CreatedBy = createdUser.Id;
                        company.UpdatedBy = createdUser.Id;
                        company.CreatedOn = DateTime.UtcNow;
                        company.UpdatedOn = DateTime.UtcNow;
                        company.EstablishDate = DateTime.UtcNow;
                        company.ExpiryDate = DateTime.UtcNow.AddDays(365);

                        await _companyService.RegisterCompanyAsync(company, createdUser.Id);

                        //Uzmi sve coordinatore
                        var coordinators = await _userManager.GetUsersInRoleAsync("ADNOC Coordinator");

                        if (coordinators.Count > 0)
                        {
                            var messageTemplate = Declares.MessageTemplateList.Supplier_Registration_Notify_Coordinator;
                            var emailData = _mailStorageServiceService.CreateMailData(coordinators.FirstOrDefault().Id, null,
                                                                                        company.NameEnglish, messageTemplate, false);
                            await _mailStorageServiceService.CreateMail(emailData);
                        }
                        await GenerateEcardNumber(applicationUser);

                        return Ok(JsonConvert.SerializeObject(MessageConstants.SaveMessages.ItemSuccessfullySaved("Registration Ok")));
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            sb.Append(error.Description);
                        }

                        return BadRequest(sb.ToString());
                    }
                }
                else
                {
                    return BadRequest(isModelValidSupplier(model));
                }
            }
            else
            {
                if (isModelValidBuyer(model) == String.Empty)
                {
                    ApplicationUser applicationUser = new ApplicationUser();
                    applicationUser.UserName = model.Email;
                    applicationUser.FirstName = model.FirstName;
                    applicationUser.NormalizedUserName = model.Email;
                    applicationUser.CreatedOn = DateTime.UtcNow;
                    applicationUser.UpdatedOn = DateTime.UtcNow;
                    applicationUser.Email = model.Email;
                    applicationUser.EmailConfirmed = false;
                    applicationUser.PhoneNumber = model.MobileNumber;
                    applicationUser.LastName = model.LastName;
                    applicationUser.Active = true;
                    applicationUser.UserType = GetUserType(model.Email);

                    var result = await _userManager.CreateAsync(applicationUser, model.Loz);
                    if (result.Succeeded)
                    {
                        await _membershipECardRepository.FindMembershipCardForUserAndUpdate(new Shared.Models.ServiceNowModels.MemberModel()
                        {
                            Id = applicationUser.Id,
                            mail = applicationUser.Email
                        });
                        var resultRole = await _userManager.AddToRoleAsync(applicationUser, model.Role);
                        await GenerateEcardNumber(applicationUser);
                        return Ok(JsonConvert.SerializeObject(MessageConstants.SaveMessages.ItemSuccessfullySaved("Registration Ok")));
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            sb.Append(error.Description);
                        }

                        return BadRequest(sb.ToString());
                    }
                }
                else
                {
                    return BadRequest(isModelValidBuyer(model));
                }
            }
        }


        private async Task GenerateEcardNumber(ApplicationUser applicationUser)
        {
            var ecard = await _applicationUserService.GenerateECardForUser(applicationUser.Id);
            applicationUser.ECardSequence = ecard.ECardSequence;
            await _userManager.UpdateAsync(applicationUser);
        }

        private string isModelValidSupplier(RegistrationModel model)
        {
            StringBuilder sb = new StringBuilder();

            if (string.IsNullOrEmpty(model.CompanyName))
            {
                sb.AppendLine("Please fill company name");
            }
            if (string.IsNullOrEmpty(model.CompanyDescription))
            {
                sb.AppendLine("Please fill description");
            }
            if (string.IsNullOrEmpty(model.Email))
            {
                sb.AppendLine("Please fill email");
            }
            if (string.IsNullOrEmpty(model.MobileNumber))
            {
                sb.AppendLine("Please fill mobile number");
            }

            return sb.ToString();
        }

        private string isModelValidBuyer(RegistrationModel model)
        {
            StringBuilder sb = new StringBuilder();

            if (string.IsNullOrEmpty(model.FirstName))
            {
                sb.AppendLine("Please fill first name");
            }
            if (string.IsNullOrEmpty(model.LastName))
            {
                sb.AppendLine("Please fill last name");
            }
            if (string.IsNullOrEmpty(model.Email))
            {
                sb.AppendLine("Please fill email");
            }
            if (string.IsNullOrEmpty(model.MobileNumber))
            {
                sb.AppendLine("Please fill mobile number");
            }
            //TODO: Ask for list of acceptable email domains
            if (!Regex.IsMatch(model.Email, @"^[a-zA-Z0-9._%+-]+(@gmail\.com|@adnoc\.ae|@rationaletech.com)$"))
            {
                sb.AppendLine("You can only register from certain email domains.");
            }
            return sb.ToString();
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequestModel model)
        {
            var refreshTokenModel = await _refreshTokenService.GetRefreshToken(model.RefreshToken);

            if (refreshTokenModel == null)
            {
                return Unauthorized("Refresh token not found");
            }

            var user = await _userManager.FindByNameAsync(refreshTokenModel.Username);
            var userclaim = new[] { new Claim(JwtRegisteredClaimNames.Sub, refreshTokenModel.Username) };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = GenerateJSONWebToken(user);

            var refreshTokenResponse = await _refreshTokenService.SaveAsync(refreshTokenModel);

            return Ok(new { token = token, refresh = refreshTokenResponse.Refreshtoken });
        }


        private string GenerateJSONWebToken(ApplicationUser applicationUser)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            int tokenDurationInMInutes = 0;
            bool tokenDuration = int.TryParse(_config["TokenSettings:Duration"], out tokenDurationInMInutes);
            DateTime tokenExpired = (tokenDuration) ? DateTime.Now.AddMinutes(tokenDurationInMInutes) : DateTime.Now.AddMinutes(480);

            var roles = _userManager.GetRolesAsync(applicationUser).GetAwaiter().GetResult();
            var company = _companyService.GetMySupplierCompany(applicationUser.Id).Result;

            var claims = new[] { new Claim(JwtRegisteredClaimNames.Sub, applicationUser.UserName),
                                 new Claim(JwtRegisteredClaimNames.Email, applicationUser.Email),
                                 new Claim(JwtRegisteredClaimNames.Jti, applicationUser.Id.ToString()),
                                 new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                                 new Claim(ClaimTypes.Name, applicationUser.Id),
            };

            var header = new JwtHeader(credentials);

            var payload = new JwtPayload(issuer: _config["Jwt:Issuer"],
                                        audience: _config["Jwt:Issuer"],
                                        claims: claims,
                                        notBefore: DateTime.UtcNow.AddSeconds(-1),
                                        expires: tokenExpired);

            payload.Add("permissions", new { permission = "all" });
            payload.Add("roles", roles);
            payload.Add("userId", applicationUser.Id.ToString());
            payload.Add("Name", applicationUser.Id.ToString());
            if (company != null)
            {
                payload.Add("companyId", company.Id.ToString());
                payload.Add("companyStatus", company.ApproveStatus);
            }


            var token = new JwtSecurityToken(header, payload);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        //********************************************************************************************************************************************
        // OAuth IMPLEMENTATION
        //********************************************************************************************************************************************

        [Authorize("B2C_SUPPLIER")]
        [Authorize("B2C_CUSTOMER")]
        [HttpPost("login-b2c-as-supplier")]
        [HttpPost("login-b2c-as-customer")]
        public async Task<IActionResult> Login()
        {
            var oidClaim = GetOIDClaim();
            var graphResponse = await _microsoftGraphService.GetUserClaims(oidClaim.Value);

            var acceptedClaims = _config.AuthenticationClaims();
            var identificationClaim = graphResponse.Identities.FirstOrDefault(c => acceptedClaims.Contains(c.SignInType));

            var logoutLink = _config["LogoutLink:AzureB2C"];

            return await LoginWithToken(identificationClaim.IssuerAssignedId, logoutLink);
        }

        private async Task UpdateB2CUserName(string email)
        {
            var firstNameClaim = User.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname");
            var lastNameClaim = User.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname");

            if (firstNameClaim != null && lastNameClaim != null)
            {
                var user = await _userManager.FindByNameAsync(email);
                if (user != null)
                {
                    user.FirstName = firstNameClaim.Value;
                    user.LastName = lastNameClaim.Value;
                    await _userManager.UpdateAsync(user);
                }
            }
        }

        private Claim GetOIDClaim()
        {
            var identificationClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            return identificationClaim;
        }

        [Authorize("AZURE_ACTIVE_DIRECTORY_POLICY")]
        [HttpPost("login-with-azure-ad")]
        public async Task<IActionResult> LoginWithAzureActiveDirectory()
        {
            var email = User.Identity.Name;
            await CreateIfNewUser(email);
            var logoutLink = _config["LogoutLink:AzureAD"];

            return await LoginWithToken(email, logoutLink);
        }

        private async Task<IActionResult> LoginWithToken(string username, string logoutLink)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user != null)
            {
                var tokenData = await CreateToken(user, logoutLink);
                await _userManager.SetPlatformInfo(user);
                await _membershipECardRepository.FindMembershipCardForUserAndUpdate(new Shared.Models.ServiceNowModels.MemberModel()
                {
                    Id = user.Id,
                    mail = user.Email
                });
                return Ok(tokenData);
            }

            return Unauthorized("There was error during the login phase, please contact system administrator.");
        }

        [HttpGet("identity-providers")]
        public IActionResult GetAzureConfiguration()
        {
            var identityProviders = _identityProvidersService.GetIdentityProviders();
            return Ok(identityProviders);
        }

        [HttpPost("login/refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            var refreshTokenModel = await _jwtService.GetRefreshTokenModel(refreshToken);
            if (refreshTokenModel == null)
            {
                return Unauthorized("Refresh token not found");
            }
            var user = await _userManager.FindByNameAsync(refreshTokenModel.Username);
            var tokenData = await CreateToken(user);
            return Ok(tokenData);
        }




        private async Task<object> CreateToken(ApplicationUser user, string logoutLink = "")
        {
            try
            {
                var (token, refresh) = await _jwtService.GenerateToken(user, logoutLink);
                return new { token, refresh };
            }
            catch
            {
                throw new Exception("There was error during the login phase, please contact system administrator.");
            }
        }

        private async Task CreateIfNewUser(string email)
        {
            var firstName = User.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname");
            var lastName = User.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname");
            if (firstName != null && lastName != null && !await _userService.UserExists(email))
            {
                ApplicationUser applicationUser = new ApplicationUser
                {
                    //Title
                    FirstName = firstName.Value ?? string.Empty,
                    LastName = lastName.Value ?? string.Empty,
                    UserName = email,
                    NormalizedUserName = email,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedOn = DateTime.UtcNow,
                    Email = email,
                    EmailConfirmed = false,
                    Active = true,
                    UserType = GetUserType(email)
                };

                var result = await _userManager.CreateAsync(applicationUser);

                if (!result.Succeeded) throw new Exception("Cannot create user");
                await _membershipECardRepository.FindMembershipCardForUserAndUpdate(new Shared.Models.ServiceNowModels.MemberModel()
                {
                    Id = applicationUser.Id,
                    mail = applicationUser.Email
                });
                await GenerateEcardNumber(applicationUser);
                await _userManager.AddToRoleAsync(applicationUser, Roles.Buyer.ToString());
            }
            else if (firstName != null && lastName != null)
            {
                var user = await _userManager.FindByNameAsync(email);
                user.FirstName = firstName.Value;
                user.LastName = lastName.Value;
                await _userManager.UpdateAsync(user);
            }
        }

        private int GetUserType(string username)
        {
            var domains = _applicationUserService.GetUserDomains();

            // If it contains @adnoc in email it is adnoc employee
            if (username.Contains("@adnoc")) return domains.FirstOrDefault(x => x.DomainName.Equals(Declares.UserType.ADNOCEmployee.ToString())).Id;

            // If it's in invited table it's family member
            //if (_applicationUserRepository.CheckIfUserIsFamilyMember(username)) return Declares.UserType.ADNOCEmployeeFamilyMember;
            var userType = _applicationUserService.GetInvitedUserType(username);

            if (userType != 0)
            {
                return userType;
            }
            else
            {
                string userDomainName = _applicationUserRepository.GetDomainNameFromEmail(username);
                int userNameDomainId = _applicationUserRepository.GetUserDomain(userDomainName);

                if (userNameDomainId != 0)
                {
                    return userNameDomainId;
                }
            }

            return domains.FirstOrDefault(x => x.DomainName.Equals(Declares.UserType.Other.ToString())).Id;
        }

        private ApplicationUser CreateNonRegistratedApplicationUser(string email)
        {
            var newApplicationUser = new ApplicationUser();
            newApplicationUser.Email = email;
            newApplicationUser.UserName = email;
            newApplicationUser.Id = email.Split("@").FirstOrDefault();
            return newApplicationUser;
        }
    }
}
