using Microsoft.Extensions.Logging;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Models.DefaultLocations;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.Roadshow;
using MMA.WebApi.Shared.Monads;
using System.Collections.Generic;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.WebApi.Shared.Interfaces.Offers
{
    public interface IRoadshowService
    {
        Task<RoadshowModel> CreateRoadshow(RoadshowModel model, string userId);
        Task<RoadshowModel> UpdateRoadshow(RoadshowModel model, string userId);
        Task UnpublishRoadshow(int roadshowId);
        Task<IEnumerable<RoadshowModel>> GetRoadshows();
        Task<IEnumerable<RoadshowModel>> GetConfirmedRoadshows();
        Task<PaginationListModel<RoadshowModel>> GetAllRoadshows(
            QueryModel queryModel,
            string userId
        );
        Task<List<RoadshowEventCalendarCard>> GetAllEventsForCalendar(
            QueryModel queryModel,
            string userId
        );
        Task<IEnumerable<RoadshowModel>> GetAllRoadshowsForCalendar(
            CalendarQueryModel queryModel,
            string userId
        );
        Task<RoadshowModel> EditRoadshowForCalendar(RoadshowModel model, string userId);
        IEnumerable<DefaultLocationModel> GetAllDefaultLocations();
        Task<RoadshowModel> GetRoadshow(int id);
        Task CheckExpiredRoadshows(ILogger logger);
        Task DeleteRoadshow(int id);
        Task<RoadshowStatus> GetRoadshowStatusById(int id);
        string DecodeBase64String(string encodedString);
        Task SendMailNotificationIfRoadshowStatusChanged(
            RoadshowStatus currentRoadshowStatus,
            RoadshowStatus newRoadshowStatus,
            RoadshowModel roadshow
        );
        Task<Maybe<RoadshowOfferModel>> GetSpecificRoadshowOfferById(int id);
    }
}
