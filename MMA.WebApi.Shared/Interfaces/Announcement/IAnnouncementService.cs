using MMA.WebApi.Shared.Models.Announcement;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.WebApi.Shared.Interfaces.Announcement
{
    public interface IAnnouncementService
    {
        Task CreateAnnouncement(AnnouncementModel model, string userId);
        Task<AnnouncementModel> GetAnnouncementByStatus(AnnouncementStatus status);
        Task ChangeAnnouncementStatus(
            AnnouncementStatus status,
            int announcementId,
            int? countUsers,
            int? countSend,
            int? countFailed
        );
        Task SetSentMailsCounts(int announcementId);
    }
}
