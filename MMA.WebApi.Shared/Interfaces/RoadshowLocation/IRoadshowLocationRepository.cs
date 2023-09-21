using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Models.DefaultLocations;
using MMA.WebApi.Shared.Models.Roadshow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace MMA.WebApi.Shared.Interfaces.Offers
{
    public interface IRoadshowLocationRepository : IQueryableRepository<RoadshowLocationModel>
    {
        IQueryable<DefaultLocationModel> GetAllDefaultLocations();
        Task<DefaultLocationModel> GetDefaultLocationById(int locationId);
        Task<List<DefaultLocationModel>> GetDefaultLocations();
        Task UpdateDefaultLocations(List<DefaultLocationModel> defaultLocations, string UserId);
        IQueryable<RoadshowLocationModel> GetAllRoadshowOffersForAllLocations();
        IQueryable<RoadshowOfferCardModel> GetRoadshowOffersForSpecificLocation(int locationId, string userId);
        IQueryable<RoadshowLocationModel> GetRoadshowOffersForSpecificDates(QueryModel queryModel, string userId);
        Task<DefaultLocationModel> GetDefaultLocation(int id);
        IQueryable<RoadshowLocationModel> GetRoadshowOffersForSpecificDatesMobile(DateTime minDate, DateTime maxDate);
        IQueryable<RoadshowLocationModel> GetAllRoadshowOffersForAllLocationsMobile(DateTime lastUpdatedOn, string userId);
        Task<IEnumerable<RoadshowOfferMobileModel>> GetAllRoadshowOffersMobile(DateTime lastUpdatedOn, string userId);
        Task<IEnumerable<RoadshowOfferMobileModel>> GetAllRoadshowOffersMobile();
        Task<IQueryable<int>> GetValidRoadshowOffersIds();
    }
}
