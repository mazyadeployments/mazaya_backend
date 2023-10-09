using MMA.WebApi.DataAccess.Extensions;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.RoadshowEvent;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.Roadshow;
using System.Threading.Tasks;

namespace MMA.WebApi.Core.Services
{
    public class RoadshowEventService : IRoadshowEventService
    {
        private readonly IRoadshowEventRepository _roadshowEventRepository;

        public RoadshowEventService(IRoadshowEventRepository roadshowEventRepository)
        {
            _roadshowEventRepository = roadshowEventRepository;
        }

        public async Task<PaginationListModel<RoadshowOfferModel>> GetOffersForSelectedEvent(QueryModel queryModel, int eventId)
        {
            var roadshowOfferModels = _roadshowEventRepository.GetOffersForSelectedEvent(queryModel, eventId);

            return await roadshowOfferModels.ToPagedListAsync(queryModel.PaginationParameters.PageNumber, queryModel.PaginationParameters.PageSize);
        }
    }
}