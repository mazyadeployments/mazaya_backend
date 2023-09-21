using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Extensions;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.Categories;
using MMA.WebApi.Shared.Interfaces.Collection;
using MMA.WebApi.Shared.Interfaces.Companies;
using MMA.WebApi.Shared.Interfaces.Offers;
using MMA.WebApi.Shared.Interfaces.RefreshToken;
using MMA.WebApi.Shared.Models.OneAdnoc;
using MMA.WebApi.Shared.Models.RefreshToken;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Document.DocumentDeclares;

namespace MMA.WebApi.Controllers
{
    [Authorize("Bearer")]
    [ApiController]
    public class OneAdnocController : BaseController
    {
        private readonly IConfiguration _config;
        private readonly IOfferService _offerService;
        private readonly ICategoryService _cattegoryService;
        private readonly ICollectionService _collectionService;
        private readonly ICompanyService _companyService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IRefreshTokenService _refreshTokenService;
        private string _baseContentUrl;

        public OneAdnocController(
            IOfferService offerService,
            ICategoryService cattegoryService,
            ICollectionService collectionService,
            ICompanyService companyService,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IRefreshTokenService refreshTokenService,
            IConfiguration config
        )
        {
            _offerService = offerService;
            _cattegoryService = cattegoryService;
            _collectionService = collectionService;
            _companyService = companyService;
            _userManager = userManager;
            _signInManager = signInManager;
            _refreshTokenService = refreshTokenService;
            _config = config;
            _baseContentUrl = _config["BaseURL:ApiUrl"];
        }

        [HttpPost]
        [Route("oneadnoc/login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);

            if (user == null)
            {
                return BadRequest("Your account is not found.");
            }

            var checkPwd = await _signInManager.CheckPasswordSignInAsync(
                user,
                model.Password,
                false
            );

            if (!checkPwd.Succeeded)
            {
                return BadRequest("Your password is not correct.");
            }

            if (!user.Active)
            {
                return BadRequest("Your account is not active.");
            }

            var tokenString = GenerateJSONWebToken(user);

            var refreshToken = new RefreshTokenModel { Username = model.Username };

            await _refreshTokenService.SaveAsync(refreshToken);

            Shared.Models.OneAdnoc.TokenData tokenData = new Shared.Models.OneAdnoc.TokenData()
            {
                AccessToken = tokenString,
                RefreshToken = refreshToken.Refreshtoken,
                ClientId = "UnknownClientId",
                Issued = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(525600)),
                TokenType = "",
                ExpiresInSeconds = 525600 * 60,
                IsValidUser = true
            };

            await _userManager.SetPlatformInfo(user);

            return Ok(tokenData);
        }

        [HttpGet]
        [Route("oneadnoc/categories")]
        public async Task<IActionResult> GetCategoriesViews()
        {
            var categoryList = await _cattegoryService.GetCategories();
            var returnList = categoryList.Select(
                x =>
                    new CategoryViewModel()
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Description = x.Description,
                        ImageUrl = $"{_baseContentUrl}/media/{x.ImageUrls.Thumbnail}.png",
                        OffersCount = x.NumberOfOffers
                    }
            );
            return Ok(returnList);
        }

        [HttpPost]
        [Route("oneadnoc/offerviews/search/paginated")]
        public async Task<IActionResult> SearchOffersViewsAdvancePaginated(
            OfferSearchModel dataQuery
        )
        {
            dataQuery.UserId = UserId;

            var results = await _offerService.SelectValidOfferView(dataQuery);

            return Ok(results);
        }

        [HttpGet]
        [Route("oneadnoc/offerviews/{id}")]
        public async Task<IActionResult> GetOfferView(int id)
        {
            var offer = await _offerService.GetOffer(id);
            var result = MapOffer(offer.Value);

            return Ok(result);
        }

        [HttpGet]
        [Route("oneadnoc/offerviews/category/{categoryId}")]
        public async Task<IActionResult> GetOffersViewsByCategory(int categoryId)
        {
            var categoryList = await _offerService.GetOffersByCategory(categoryId);
            var returnList = categoryList.Select(x => MapOffer(x)).ToList();
            return Ok(returnList);
        }

        [HttpGet]
        [Route("oneadnoc/collections")]
        public async Task<IActionResult> GetCollectionViews()
        {
            var collectionList = await _collectionService.GetCollections();
            var returnList = collectionList.Select(
                x =>
                    new CollectionViewModel()
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Description = x.Description,
                        Tag = null,
                        ValidUntil = x.ValidUntil,
                        Location = "",
                        SponsorLogoImageUrl =
                            $"{_baseContentUrl}/media/{x.ImageUrls.Thumbnail}.jpg",
                        ImageUrl = $"{_baseContentUrl}/media/{x.ImageUrls.Thumbnail}.jpg",
                        OffersCount = x.OffersIds != null ? x.OffersIds.Count() : 0
                    }
            );

            return Ok(returnList);
        }

        [HttpGet]
        [Route("oneadnoc/sellers/{id}")]
        public async Task<IActionResult> GetSeller(int id)
        {
            var company = await _companyService.GetCompanyById(id);
            var returnCompany = new SellerModel()
            {
                Id = company.Id,
                Name = company.NameEnglish,
                Address = "",
                PhoneNumber1 = company.Mobile?.InternationalNumber,
                PhoneNumber2 = company.Fax?.InternationalNumber,
                Website = company.Website,
            };

            return Ok(returnCompany);
        }

        private OfferViewModel MapOffer(Shared.Models.Offer.OfferModel o)
        {
            var result = new OfferViewModel()
            {
                Id = o.Id,
                Title = o.Title,
                Brand = o.Brand,
                Description = o.Description,
                Address = o.Address,
                City = o.City,
                Price = o.PriceFrom.HasValue ? o.PriceFrom.Value : 0,
                DiscountPercentage = null,
                ValidFrom = o.ValidFrom,
                ValidUntil = o.ValidUntil,
                CreatedOn = o.CreatedOn,
                WhatYouGet = o.WhatYouGet,
                PriceList = o.PriceList,
                FinePrint = null,
                CategoryId =
                    o.Categories != null && o.Categories.Any()
                        ? o.Categories.Select(x => x.Id).FirstOrDefault()
                        : 0,
                CollectionId =
                    o.Collections != null && o.Collections.Any()
                        ? o.Collections.Select(x => x.Id).FirstOrDefault()
                        : 0,
                SellerId = o.CompanyId,
                SellerName = o.CompanyNameEnglish,
                Longtitude = o.Locations.Any() ? o.Locations.FirstOrDefault().Longitude : "",
                Latitude = o.Locations.Any() ? o.Locations.FirstOrDefault().Latitude : "",
                Tag = Shared.Enums.Declares.Tag.Undefined,
                IsFavorite = o.IsFavourite,
                AverageRemark = o.Rating,
                ReviewsCount = 0,
                Images =
                    o.Images != null
                        ? o.Images.Select(
                            x =>
                                new OfferImageModel()
                                {
                                    Id = x.Id,
                                    Index = 1,
                                    ImageUrl = $"{_baseContentUrl}/media/{x.Id}.jpg",
                                }
                        )
                        : null,
                Attachments =
                    o.OfferDocuments != null
                        ? o.OfferDocuments
                            .Where(od => od.Type == OfferDocumentType.Document)
                            .Select(
                                od =>
                                    new OfferAttachmentModel
                                    {
                                        Id = !string.IsNullOrEmpty(od.DocumentId.ToString())
                                            ? od.DocumentId.ToString()
                                            : string.Empty,
                                        Name = od.Document.Name,
                                        Index = 0,
                                        OfferId = o.Id,
                                        Size = od.Document.Size.HasValue
                                            ? od.Document.Size.Value
                                            : 0,
                                        Extension = "",
                                        ContentUrl = $"{_baseContentUrl}/document/{od.Id}"
                                    }
                            )
                            .ToList()
                        : null
            };

            return result;
        }

        public string Extension { get; set; }
        public string ContentUrl { get; set; }

        private string GenerateJSONWebToken(ApplicationUser applicationUser)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            int tokenDurationInMInutes = 0;
            bool tokenDuration = int.TryParse(
                _config["TokenSettings:Duration"],
                out tokenDurationInMInutes
            );
            DateTime tokenExpired =
                (tokenDuration)
                    ? DateTime.Now.AddMinutes(tokenDurationInMInutes)
                    : DateTime.Now.AddMinutes(480);

            var roles = _userManager.GetRolesAsync(applicationUser).GetAwaiter().GetResult();
            var company = _companyService.GetMySupplierCompany(applicationUser.Id).Result;

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, applicationUser.UserName),
                new Claim(JwtRegisteredClaimNames.Email, applicationUser.Email),
                new Claim(JwtRegisteredClaimNames.Jti, applicationUser.Id.ToString()),
                new Claim(
                    JwtRegisteredClaimNames.Iat,
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()
                ),
                new Claim(ClaimTypes.Name, applicationUser.Id),
            };

            var header = new JwtHeader(credentials);

            var payload = new JwtPayload(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Issuer"],
                claims: claims,
                notBefore: DateTime.UtcNow.AddSeconds(-1),
                expires: tokenExpired
            );

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

        [HttpPost]
        [AllowAnonymous]
        [Route("onehub/offerviews/search/paginated")]
        public async Task<IActionResult> SearchOffersViewsPaginated(OfferSearchModel dataQuery)
        {
            try
            {
                Validate();

                var results = await _offerService.SelectValidOfferViewOneHub(dataQuery);

                return Ok(results);
            }
            catch (Exception) { }

            return Unauthorized();
        }

        private JwtSecurityToken Validate()
        {
            var handler = new JwtSecurityTokenHandler();

            //var id_token = Request.Headers.GetValues("Authorization").FirstOrDefault().Substring("Bearer ".Length);

            var id_token = Request.Headers["Authorization"]
                .FirstOrDefault()
                .Substring("Bearer ".Length);

            var jwtToken = handler.ReadJwtToken(id_token);

            //var proxy = new WebProxy
            //{
            //    Address = new Uri($"http://adnocproxy.adnoc.ae:8080")
            //};
            //HttpClient http = new HttpClient(new HttpClientHandler { Proxy = proxy });

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

            var result = tokendHandler.ValidateToken(id_token, validationParameters, out jwt);

            var appid = jwtToken.Claims.FirstOrDefault(x => x.Type == "appid");
            var audience = jwtToken.Claims.FirstOrDefault(x => x.Type == "aud");

            return jwt as JwtSecurityToken;
        }
    }
}
