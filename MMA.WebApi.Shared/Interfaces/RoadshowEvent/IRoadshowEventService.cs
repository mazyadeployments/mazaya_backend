using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.Roadshow;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.RoadshowEvent
{
    public interface IRoadshowEventService
    {
        Task<PaginationListModel<RoadshowOfferModel>> GetOffersForSelectedEvent(QueryModel queryModel, int eventId);
    }
}
