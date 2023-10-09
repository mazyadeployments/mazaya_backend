using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MMA.WebApi.Shared.Models.ApplicationUser;
using MMA.WebApi.Shared.Models.Membership;
using MMA.WebApi.Shared.Models.ServiceNowModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.WebApi.Shared.Interfaces.Membership
{
    public interface IMembershipECardService
    {
        Task AddMembershipCard(MembershipEcardModel membershipEcard, string userId);
        Task<IEnumerable<MembershipEcardModel>> GetOwnerCards(string userId);
        Task<IEnumerable<MembershipEcardModel>> GetMemberCard(string userId);
        Task UploadPicture(IFormFileCollection files, int type);
        Task<byte[]> CreatePdfForMembershipECard(int ecardId, string userId);
        IEnumerable<MembershipModel> GetMembershipsForUser(string userId, bool isBuyer);
        Task CreateECardForUser(string userId);
        Task<IEnumerable<MembershipModel>> GetMembershipTypes();
        Task CreateECardsForUser(IEnumerable<ServiceNowUserModel> UserDatas, string userId);
        Task<object> UploadForTestuat(IFormFileCollection files);
        Task CheckMembershipECardsValidTo();
        FileContentResult GenerateAppleWalletCard(
            ApplicationUserModel user,
            WalletCardType walletCardType,
            string expireDate = "",
            string userPhotoUrl = ""
        );
        Task<FileContentResult> GenerateMembershipAppleWalletCard(int ecardId, string userId);
        Task<MembershipEcardModel> GetMembershipECardById(int ecardId);
        Task<WalletCardType> GetWalletCardTypeByMembershipId(string membershipId);
    }
}
