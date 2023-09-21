using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Interfaces;
using MMA.WebApi.Shared.Interfaces.ApplicationUsers;
using MMA.WebApi.Shared.Interfaces.Categories;
using MMA.WebApi.Shared.Interfaces.Collection;
using MMA.WebApi.Shared.Interfaces.Membership;
using MMA.WebApi.Shared.Interfaces.Offers;
using MMA.WebApi.Shared.Interfaces.Roles;
using MMA.WebApi.Shared.Interfaces.Tag;
using MMA.WebApi.Shared.Models.Membership;
using MMA.WebApi.Shared.Models.Mobile;
using MMA.WebApi.Shared.Models.Offer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.WebApi.Controllers.Mobile
{
    [Authorize("Bearer")]
    [ApiController]
    public class SynchronizationDataController : BaseController
    {
        public static SynchronizationDataModel SynchronizationDataModel;
        private readonly IConfiguration _configuration;
        private readonly IOfferService offerService;
        private readonly ITagService tagService;
        private readonly ICategoryService categoryService;
        private readonly ICollectionService collectionService;
        private readonly IRoadshowLocationService roadshowLocationService;
        private readonly IRoleService _roleService;
        private readonly IMobileCacheDataService _mobileCacheDataService;
        private readonly IMembershipECardRepository _membershipECardRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IServiceNow _serviceNow;
        private readonly IMembershipECardService _membershipECardService;
        private readonly IApplicationUserService _applicationUserService;


        public SynchronizationDataController(IOfferService offerService,
                                             ICategoryService categoryService,
                                             ITagService tagService,
                                             IConfiguration configuration,
                                             ICollectionService collectionService,
                                             UserManager<ApplicationUser> userManager,
                                             IRoadshowLocationService roadshowLocationService,
                                             IRoleService roleService,
                                             IMobileCacheDataService mobileCacheDataService,
                                             IMembershipECardRepository membershipECardRepository,
                                             IServiceNow serviceNow,
                                             IMembershipECardService membershipECardService,
                                             IApplicationUserService applicationUserService)
        {
            _configuration = configuration;
            _userManager = userManager;
            _roleService = roleService;
            _membershipECardRepository = membershipECardRepository;
            _mobileCacheDataService = mobileCacheDataService;
            this.offerService = offerService;
            this.categoryService = categoryService;
            this.collectionService = collectionService;
            this.tagService = tagService;
            this.roadshowLocationService = roadshowLocationService;
            _serviceNow = serviceNow;
            _applicationUserService = applicationUserService;
            _membershipECardService = membershipECardService;
        }

        private async Task<SynchronizationDataModel> GenerateSynchronizationData(DateTime lastUpdateOn, UserData user)
        {

            SynchronizationDataModel synchronizationDataModel = new SynchronizationDataModel
            {
                UpdatedOn = DateTime.UtcNow
            };
            var roles = await _roleService.GetUserRoles(UserId);

            var tasks = new List<Task>
            {
                Task.Run(() => synchronizationDataModel.Offers = offerService.SelectValidOffers(lastUpdateOn, UserId, user.IsSupplier)),
                Task.Run(() => synchronizationDataModel.OffersIds = offerService.SelectValidAndLiveOffersForUser(user.ApplicationUser.Id, roles.Contains(Roles.Buyer))),
                Task.Run(() => _mobileCacheDataService.GetCategories(synchronizationDataModel, lastUpdateOn)),
                Task.Run(() => _mobileCacheDataService.GetCategoriesIds(synchronizationDataModel)),
                Task.Run(() => _mobileCacheDataService.GetCollections(synchronizationDataModel, lastUpdateOn)),
                Task.Run(() => _mobileCacheDataService.GetCollectionsIds(synchronizationDataModel)),
                Task.Run(() => _mobileCacheDataService.GetTags(synchronizationDataModel, lastUpdateOn)),
                Task.Run(() => _mobileCacheDataService.GetTagsIds(synchronizationDataModel)),
                Task.Run(async () => synchronizationDataModel.RoadshowsOffers = (await roadshowLocationService.GetAllRoadshowOffersMobile(lastUpdateOn, UserId)).OrderBy(c => c.Id)),
                Task.Run(async () => synchronizationDataModel.RoadshowsOfferIds = (await roadshowLocationService.GetValidRoadshowOffersIds()).Distinct())
            };

            // Setting last User data synchronization
            HttpContext.Request.Headers.TryGetValue("User-Agent", out var platformType);

            user.ApplicationUser.LastDataSynchronizationOn = DateTime.UtcNow;
            user.ApplicationUser.PlatformType = platformType;

            await Task.WhenAll(tasks);

            await this._userManager.UpdateAsync(user.ApplicationUser);

            return synchronizationDataModel;
        }

        [HttpGet]
        [Route("mobile/synchronizationdata/{lastUpdateOn}")]
        public async Task<SynchronizationDataModel> GetSynchronizationData(string lastUpdateOn)
        {
            DateTime filterDateTimeValue;
            if (!DateTime.TryParseExact(lastUpdateOn, "yyyyMMddHHmmss", null, DateTimeStyles.None, out filterDateTimeValue))
            {
                throw new Exception("DateTime format of the passed value is not correct. Passed value must be in the format 'yyyyMMddHHmmss'");
            }

            BackgroundJob.Enqueue(() => _membershipECardService.CreateECardForUser(UserId));
            var userCheck = await UserCheck();

            var duration = (DateTime.UtcNow - filterDateTimeValue).TotalMinutes;

            if (duration > 10)
            {
                var model = await _mobileCacheDataService.GetSynchronizationDataModel();
                if (model != null) return await SetSpecificValueForUser(model, UserId, filterDateTimeValue);
            }

            return await this.GenerateSynchronizationData(filterDateTimeValue, userCheck);
        }

        [HttpGet("mobile/userinfo")]
        public async Task<IActionResult> UserInfo()
        {
            var roles = await _roleService.GetUserRoles(UserId);
            var UserInfo = new MobileUserInfo()
            {
                Memberships = _membershipECardRepository.GetMembershipsForUser(UserId, roles.Contains(Roles.Buyer)),
                MyCardCount = (await _membershipECardRepository.GetOwnerCards(UserId)).Count(),
                MyFamilyCardCount = (await _membershipECardRepository.GetMemberCard(UserId)).Count()
            };
            return Ok(UserInfo);
        }

        [HttpGet("mobile/disclaimer")]
        public async Task<IActionResult> GetMobileDisclaimer()
        {
            var disclaimer = _configuration["Mobile:DisclaimerText"] ?? "";
            return Ok(new { disclaimer });
        }

        [HttpGet]
        [Route("mobile/synchronizationdata/")]
        public async Task<SynchronizationDataModel> GetSynchronizationData()
        {
            BackgroundJob.Enqueue(() => _membershipECardService.CreateECardForUser(UserId));

            var userId = UserId;

            var userCheck = await UserCheck();
            if (userCheck.IsSupplier) return await this.GenerateSynchronizationData(DateTime.MinValue, userCheck);

            var model = await _mobileCacheDataService.GetSynchronizationDataModel();
            if (model != null) return await SetSpecificValueForUser(model, userId, DateTime.MinValue);

            return await this.GenerateSynchronizationData(DateTime.MinValue, userCheck);
        }

        [HttpGet]
        [Route("mobile/manualstartcaching/")]
        public async Task ManualCachnig()
        {
            BackgroundJob.Enqueue(() => _membershipECardService.CreateECardForUser(UserId));

            var syncData = await offerService.GenerateSynchronizationData();
            await _mobileCacheDataService.SetMobileCacheData(syncData);
        }


        private async Task<UserData> UserCheck()
        {
            ApplicationUser applicationUser = await _userManager.FindByIdAsync(UserId);
            if (applicationUser == null) throw new Exception("User does not exists.");

            var userRoles = await _roleService.GetUserRoles(applicationUser.Id);
            return new UserData()
            {
                IsSupplier = userRoles.ContainsOneOrMore(Roles.SupplierAdmin, Roles.Supplier),
                ApplicationUser = applicationUser
            };
        }

        private async Task<SynchronizationDataModel> SetSpecificValueForUser(string model, string userId, DateTime lastUpdateOn)
        {
            var data = await offerService.GetOfferSpecificData(userId);
            var userRoles = await _roleService.GetUserRoles(userId);

            var modelObject = JsonConvert.DeserializeObject<SynchronizationDataModel>(model);

            MappedOffersSpecificData(data, modelObject, userId, userRoles.Contains(Roles.Buyer));
            modelObject.Offers = modelObject.Offers.Where(o => o.UpdatedOn > lastUpdateOn).ToList();

            return modelObject;
        }

        private void MappedOffersSpecificData(IEnumerable<OfferModelMobile> data, SynchronizationDataModel model, string userId, bool isBuyer)
        {
            var memberships = _membershipECardRepository.GetMembershipsForUser(userId, isBuyer).Select(x => x.Id);

            var membershipOffers = model.Offers.Where(x => !x.IsPrivate || x.MembershipsId.Count() > 0 && (x.MembershipsId.Where(xm => memberships.Contains(xm))).Any());

            foreach (var item in data)
            {
                //   var modelItem = model.Offers.Where(x => x.Id == item.Id).FirstOrDefault();
                var modelItem = membershipOffers.Where(x => x.Id == item.Id).FirstOrDefault();
                if (modelItem != null)
                {
                    modelItem.IsFavourite = item.IsFavourite;
                    modelItem.IsRated = item.IsRated;
                }
            }
            model.Offers = membershipOffers;
            model.OffersIds = model.Offers.Select(x => x.Id).OrderBy(x => x).ToList();
        }
    }
}
