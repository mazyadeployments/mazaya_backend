using MMA.WebApi.Shared.Models.Membership;
using MMA.WebApi.Shared.Models.ServiceNowModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.Membership
{
    public interface IMembershipECardRepository
    {
        Task<Dictionary<string, string>> GetMembershipTypes();
        Task<IEnumerable<MembershipEcardModel>> GetMemberCard(string userId);
        Task<IEnumerable<MembershipEcardModel>> GetOwnerCards(string userId);
        Task<IEnumerable<MembershipModel>> GetAllMembershipsModel();
        IEnumerable<MembershipModel> GetMembershipsForUser(string userId, bool isBuyer);
        IEnumerable<MembershipModel> GetMembershipsForOffer(int offerId);
        Task FindMembershipCardForUserAndUpdate(MemberModel data);
        Task DeleteExpiredMembershipECards(DateTime currentTime);
    }
}
