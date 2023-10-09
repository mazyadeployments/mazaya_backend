using Microsoft.Extensions.Logging;
using MMA.WebApi.Shared.Models.Membership;
using MMA.WebApi.Shared.Models.ServiceNowModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.Membership
{
    public interface IMembershipECardMakerRepository
    {
        Task AddMembershipCard(MembershipEcardModel membershipEcard, string userId);
        Task UploadPicture(IEnumerable<Guid> filesId, int type);
        Task<DateTime> GetLastDateForServiceNow();
        Task<DateTime> GetLastDateForServiceNowwithlog(ILogger log);
        Task AddServiceNowData(string JsonData, DateTime startDate, DateTime endDate);
        Task<byte[]> CreatePdfForMembershipECard(int ecardId);
        Task<byte[]> CreatePdfForMazayaCard(string userId);
        Task CreateECardForUser(IEnumerable<ServiceNowUserModel> UserDatas, string userId);
        Task<MembershipEcardModel> GetMembershipECardById(int ecardId);
        Task<System.Drawing.Image> LoadPictureForEcard(string pictureGuid);

    }
}
