using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Models.ApplicationUser;
using MMA.WebApi.Shared.Models.Roadshow;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.RoadshowInvite
{
    public interface IRoadshowInviteRepository : IQueryableRepository<RoadshowInviteModel>
    {
        Task SendRoadshowInvitesFromModal(RoadshowInvitesQueryModel roadshowInvitesQueryModel, int id, string userId);
        Task SendRoadshowInvitesFromForm(RoadshowInvitesQueryModel roadshowInvitesQueryModel, int id, string userId);
        IQueryable<RoadshowInviteModel> GetAllRoadshowInvitesForRoadshow(QueryModel queryModel, string userId, int roadshowId);
        int AddOrUpdateRoadshowEventToRoadshowInvite(RoadshowEventModel roadshowEventModel, int id, int idinvite);
        Task<bool> DeleteRoadshowEventToRoadshowInvite(int idevent);
        void DeleteRoadshowInvites(RoadshowInvitesQueryModel roadshowInvitesQueryModel);
        void UpdateRoadshowInvites(RoadshowInvitesQueryModel roadshowInvitesQueryModel);
        Task<RoadshowInviteDetailsModel> GetRoadshowInviteDetails(int roadshowId, int inviteId, List<Declares.Roles> roles, string userId);
        Task UpdateOfRoadshowInviteDetails(RoadshowInviteDetailsModel roadshowInviteDetailsModel, string userId);
        Task<List<ApplicationUserModel>> GetSupplierAdminsForInvite(int inviteId);
        Task<RoadshowInviteModel> Update(RoadshowInviteModel model, string userId);
        Task DeactivateRoadshowInvites(int companyId);
        Task HardOfCompanyDeleteInvites(int companyId);
    }
}
