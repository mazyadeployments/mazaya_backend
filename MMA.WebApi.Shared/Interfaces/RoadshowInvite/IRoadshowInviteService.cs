using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.Roadshow;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.RoadshowInvite
{
    public interface IRoadshowInviteService
    {
        Task SendRoadshowInvitesFromModal(RoadshowInvitesQueryModel roadshowInvitesQueryModel, int id, string userId);
        Task SendRoadshowInvitesFromForm(RoadshowInvitesQueryModel roadshowInvitesQueryModel, int id, string userId);
        Task<PaginationListModel<RoadshowInviteModel>> GetAllRoadshowInvitesForRoadshow(QueryModel queryModel, string userId, int roadshowId);
        int AddOrUpdateRoadshowEventToRoadshowInvite(RoadshowEventModel roadshowEventModel, int id, int idinvite);
        Task<bool> DeleteRoadshowEventToRoadshowInvite(int idevent);
        void DeleteRoadshowInvites(RoadshowInvitesQueryModel roadshowInvitesQueryModel);
        void UpdateRoadshowInvites(RoadshowInvitesQueryModel roadshowInvitesQueryModel);
        Task<RoadshowInviteDetailsModel> GetRoadshowInviteDetails(int roadshowId, int inviteId, string userId);
        Task UpdateOfRoadshowInviteDetails(RoadshowInviteDetailsModel roadshowInviteDetailsModel, string userId);
        Task<RoadshowInviteModel> Update(RoadshowInviteModel model, string userId);
    }
}
