using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Models.Roadshow;
using System.Linq;

namespace MMA.WebApi.Shared.Interfaces.RoadshowEvent
{
    public interface IRoadshowEventRepository : IQueryableRepository<RoadshowEventModel>
    {
        IQueryable<RoadshowOfferModel> GetOffersForSelectedEvent(QueryModel queryModel, int eventId);
    }
}
