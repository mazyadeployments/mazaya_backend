using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MMA.WebApi.DataAccess.Extensions;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.Offers;
using MMA.WebApi.Shared.Models.DefaultLocations;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.Roadshow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.WebApi.Core.Services
{
    public class RoadshowLocationService : IRoadshowLocationService
    {
        private readonly IRoadshowLocationRepository _roadshowLocationRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public RoadshowLocationService(IRoadshowLocationRepository roadshowLocationRepository, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _roadshowLocationRepository = roadshowLocationRepository;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<IEnumerable<RoadshowLocationModel>> Get()
        {
            var roadshows = await _roadshowLocationRepository.Get().ToListAsync();

            return roadshows;
        }

        public PaginationListModel<RoadshowLocationModel> GetAllRoadshowOffersForAllLocations(QueryModel queryModel, string userId)
        {
            var roadshows = _roadshowLocationRepository.GetAllRoadshowOffersForAllLocations();

            return roadshows.ToPagedList(queryModel.PaginationParameters.PageNumber, queryModel.PaginationParameters.PageSize);
        }
        public PaginationListModel<RoadshowOfferCardModel> GetRoadshowOffersForSpecificLocation(QueryModel queryModel, int locationId, string userId)
        {
            var roadshows = _roadshowLocationRepository.GetRoadshowOffersForSpecificLocation(locationId, userId).ToPagedList(queryModel.PaginationParameters.PageNumber, queryModel.PaginationParameters.PageSize);

            return roadshows;
        }

        public PaginationListModel<RoadshowLocationModel> GetRoadshowOffersForSpecificDates(QueryModel queryModel, string userId)
        {
            var roadshows = _roadshowLocationRepository.GetRoadshowOffersForSpecificDates(queryModel, userId)
                                                       .ToPagedList(queryModel.PaginationParameters.PageNumber, queryModel.PaginationParameters.PageSize);

            return roadshows;
        }

        public List<Roles> GetUserRoles(string userId)
        {
            ApplicationUser applicationUser = _userManager.FindByIdAsync(userId).Result;

            List<Roles> roles = new List<Roles>();
            foreach (string userRole in _userManager.GetRolesAsync(applicationUser).Result.ToList())
            {
                Enum.TryParse(userRole, out Roles role);
                roles.Add(role);
            }

            return roles;
        }

        public IEnumerable<DefaultLocationModel> GetAllDefaultLocations()
        {
            return _roadshowLocationRepository.GetAllDefaultLocations();
        }

        public async Task<DefaultLocationModel> GetDefaultLocationById(int locationId)
        {
            return await _roadshowLocationRepository.GetDefaultLocationById(locationId);
        }

        public async Task<List<DefaultLocationModel>> GetDefaultLocations()
        {
            return await _roadshowLocationRepository.GetDefaultLocations();
        }

        public async Task UpdateDefaultLocations(List<DefaultLocationModel> defaultLocations, string userId)
        {
            await _roadshowLocationRepository.UpdateDefaultLocations(defaultLocations, userId);
        }

        public IEnumerable<RoadshowLocationModel> GetAllRoadshowOffersForAllLocationsMobile(DateTime lastUpdatedOn, string userId)
        {
            return _roadshowLocationRepository.GetAllRoadshowOffersForAllLocationsMobile(lastUpdatedOn, userId);
        }

        public async Task<IEnumerable<int>> GetValidRoadshowOffersIds()
        {
            return await _roadshowLocationRepository.GetValidRoadshowOffersIds();
        }

        public async Task<IEnumerable<RoadshowOfferMobileModel>> GetAllRoadshowOffersMobile(DateTime lastUpdatedOn, string userId)
        {
            return await _roadshowLocationRepository.GetAllRoadshowOffersMobile(lastUpdatedOn, userId);
        }
        public async Task<IEnumerable<RoadshowOfferMobileModel>> GetAllRoadshowOffersMobile()
        {
            return await _roadshowLocationRepository.GetAllRoadshowOffersMobile();
        }
    }
}