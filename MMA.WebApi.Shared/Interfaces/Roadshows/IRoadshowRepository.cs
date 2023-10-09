using Microsoft.Extensions.Logging;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Models.DefaultLocations;
using MMA.WebApi.Shared.Models.Roadshow;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.WebApi.Shared.Interfaces.Offers
{
    public interface IRoadshowRepository : IQueryableRepository<RoadshowModel>
    {
        Task<RoadshowModel> CreateRoadshowAsync(RoadshowModel model, IVisitor<IChangeable> auditVisitor, string userId, int companyId);
        Task<RoadshowModel> UpdateRoadshowAsync(RoadshowModel model, IVisitor<IChangeable> auditVisitor, string userId);
        IQueryable<DefaultLocationModel> GetAllDefaultLocations();
        IQueryable<RoadshowModel> GetAllRoadshows(string userId, List<Enums.Declares.Roles> roles, QueryModel queryModel);
        List<RoadshowEventCalendarCard> GetAllEventsForCalendar(string userId, List<Enums.Declares.Roles> roles, QueryModel queryModel);
        IQueryable<RoadshowModel> GetAllRoadshowsForCalendar(string userId, List<Enums.Declares.Roles> roles, CalendarQueryModel queryModel);
        Task<RoadshowModel> EditRoadshowForCalendar(string userId, List<Enums.Declares.Roles> roles, RoadshowModel model);
        Task<RoadshowModel> GetRoadshow(int id);
        Task<DefaultLocationModel> GetDefaultLocation(int id);
        Task<RoadshowModel> DeleteAsync(int id);
        int MapOfferCountForRoadshow(int roadshowID);
        Task<Tuple<string, List<string>>> UnpublishRoadshow(int roadshowId);
        Task<List<RoadshowEmailModel>> DoBackgroundJobAsync(ILogger logger);
        Task<int> GetRoadshowCount(string userId, List<Enums.Declares.Roles> roles);
        IQueryable<RoadshowModel> GetConfirmedRoadshows();
        Task<RoadshowStatus> GetRoadshowStatusById(int id);
        Task<RoadshowOfferModel> GetSpecificRoadshowOfferById(int id);
    }
}
