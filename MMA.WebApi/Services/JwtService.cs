using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Helpers;
using MMA.WebApi.Shared.Interfaces.Companies;
using MMA.WebApi.Shared.Interfaces.RefreshToken;
using MMA.WebApi.Shared.Models.RefreshToken;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _config;
        private readonly ICompanyService _companyService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly UserManager<ApplicationUser> _userManager;

        public JwtService(IConfiguration config, IRefreshTokenService refreshTokenService, UserManager<ApplicationUser> userManager, ICompanyService companyService)
        {
            _config = config;
            _refreshTokenService = refreshTokenService;
            _userManager = userManager;
            _companyService = companyService;
        }

        public Task<RefreshTokenModel> GetRefreshTokenModel(string refreshToken)
        {
            return _refreshTokenService.GetRefreshToken(refreshToken);
        }

        public async Task<(string token, string refresh)> GenerateToken(ApplicationUser user, string logoutLink)
        {
            var jwtKey = _config["Jwt:Key"];
            var jwtIssuer = _config["Jwt:Issuer"];
            var duration = _config["TokenSettings:Duration"];
            var xmppDomain = _config["Chat:XMPPDomain"];

            var roles = new List<string>(_userManager.GetRolesAsync(user).GetAwaiter().GetResult());

            var refreshTokenModel = new RefreshTokenModel
            {
                Username = user.UserName,
            };
            var company = _companyService.GetMySupplierCompany(user.Id).Result;


            // TODO: get UserModel so we can store more info about the user in the token 
            await _refreshTokenService.SaveAsync(refreshTokenModel);

            var token = JWTHelper.GenerateJSONWebToken(user, jwtKey, jwtIssuer, duration, xmppDomain, roles, logoutLink, company);

            return (token, refreshTokenModel.Refreshtoken); //new { token, refresh = refreshTokenModel.Refreshtoken };
        }
    }
}

