using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Models.DefaultLocations;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.Roadshow;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.Offers
{

    public interface IRoadshowLocationService
    {
        Task<IEnumerable<RoadshowLocationModel>> Get();
        PaginationListModel<RoadshowLocationModel> GetAllRoadshowOffersForAllLocations(QueryModel queryModel, string userId);
        PaginationListModel<RoadshowOfferCardModel> GetRoadshowOffersForSpecificLocation(QueryModel queryModel, int locationId, string userId);
        PaginationListModel<RoadshowLocationModel> GetRoadshowOffersForSpecificDates(QueryModel queryModel, string userId);
        IEnumerable<DefaultLocationModel> GetAllDefaultLocations();
        Task<DefaultLocationModel> GetDefaultLocationById(int locationId);
        Task<List<DefaultLocationModel>> GetDefaultLocations();
        Task UpdateDefaultLocations(List<DefaultLocationModel> defaultLocations, string UserId);
        IEnumerable<RoadshowLocationModel> GetAllRoadshowOffersForAllLocationsMobile(DateTime lastUpdatedOn, string userId);
        Task<IEnumerable<RoadshowOfferMobileModel>> GetAllRoadshowOffersMobile(DateTime lastUpdatedOn, string userId);
        Task<IEnumerable<RoadshowOfferMobileModel>> GetAllRoadshowOffersMobile();
        Task<IEnumerable<int>> GetValidRoadshowOffersIds();
    }
}
