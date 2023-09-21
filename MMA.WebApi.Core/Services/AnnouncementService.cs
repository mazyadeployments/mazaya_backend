using Microsoft.AspNetCore.Identity;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Interfaces.Announcement;
using MMA.WebApi.Shared.Interfaces.MailStorage;
using MMA.WebApi.Shared.Models.Announcement;
using MMA.WebApi.Shared.Models.ApplicationUser;
using MMA.WebApi.Shared.Models.MailStorage;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.WebApi.Core.Services
{
    public class AnnouncementService : IAnnouncementService
    {
        private readonly IAnnouncementRepository _announcementRepository;
        private readonly IMailStorageRepository _mailStorageRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public AnnouncementService(
            IAnnouncementRepository announcementRepository,
            IMailStorageRepository mailStorageRepository,
            UserManager<ApplicationUser> userManager
        )
        {
            _announcementRepository = announcementRepository;
            _mailStorageRepository = mailStorageRepository;
            _userManager = userManager;
        }

        public async Task CreateAnnouncement(AnnouncementModel model, string userId)
        {
            var auditVisitor = new CreateAuditVisitor(userId, DateTime.UtcNow);
            await _announcementRepository.CreateAnnouncementAsync(model, auditVisitor);
        }

        public async Task<AnnouncementModel> GetAnnouncementByStatus(AnnouncementStatus status)
        {
            return await _announcementRepository.GetAnnouncementByStatus(status);
        }

        public async Task ChangeAnnouncementStatus(
            AnnouncementStatus status,
            int announcementId,
            int? countUsers,
            int? countSend,
            int? countFailed
        )
        {
            await _announcementRepository.ChangeAnnouncementStatus(
                status,
                announcementId,
                countUsers,
                countSend,
                countFailed
            );
        }

        public async Task SetSentMailsCounts(int announcementId)
        {
            var counters = await _mailStorageRepository.GetCountSentAnnouncement(announcementId);
            if (counters != null)
            {
                await _announcementRepository.UpdateCounts(
                    announcementId,
                    counters.Item1,
                    counters.Item2
                );
                var announcement = await _announcementRepository.GetSpecificAnnouncement(
                    announcementId
                );
                var user = await _userManager.FindByIdAsync(announcement.CreatedBy);
                var userForMail = new ApplicationUserModel()
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email
                };

                var emailData = new EmailDataModel()
                {
                    User = userForMail,
                    MailTemplateId = Declares
                        .MessageTemplateList
                        .Adnoc_Employee_Invited_New_Family_Member
                };
                emailData.MailTemplateId =
                    counters.Item2 > 0
                        ? Declares.MessageTemplateList.Announcement_Failed_To_Sent
                        : Declares.MessageTemplateList.Announcement_Successfully_Sent;
                await _mailStorageRepository.CreateMail(emailData);
                var status =
                    counters.Item2 > 0 ? AnnouncementStatus.Failed : AnnouncementStatus.Success;
                await _announcementRepository.ChangeAnnouncementStatus(
                    status,
                    announcementId,
                    null,
                    counters.Item1,
                    counters.Item2
                );
            }
        }
    }
}
