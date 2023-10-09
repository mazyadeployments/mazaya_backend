using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Models.Announcement;
using MMA.WebApi.Shared.Visitor;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.WebApi.Shared.Interfaces.Announcement
{
    public interface IAnnouncementRepository
    {
        Task CreateAnnouncementAsync(AnnouncementModel model, IVisitor<IChangeable> auditVisitor);
        Task<AnnouncementModel> GetAnnouncementByStatus(AnnouncementStatus status);
        Task ChangeAnnouncementStatus(
            AnnouncementStatus status,
            int announcementId,
            int? countUsers,
            int? countSent,
            int? countFailed
        );
        Task UpdateCounts(int announcementId, int sentCount, int failedCount);
        Task<AnnouncementModel> GetSpecificAnnouncement(int announcementId);
    }
}
