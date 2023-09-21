using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Extensions;
using MMA.WebApi.Shared.Interfaces.EmailTemplate;
using MMA.WebApi.Shared.Interfaces.UserNotifications;
using MMA.WebApi.Shared.Models.Email;
using MMA.WebApi.Shared.Models.MailStorage;
using MMA.WebApi.Shared.Models.UserNotifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.WebApi.DataAccess.UserNotifications
{
    public class UserNotificationRepository
        : BaseRepository<UserNotificationModel>,
            IUserNotificationRepository
    {
        private readonly IConfiguration _configuration;
        IEmailTemplateRepository _emailTemplateRepository;

        public UserNotificationRepository(
            Func<MMADbContext> contextFactory,
            IConfiguration configuration,
            IEmailTemplateRepository emailTemplateRepository
        )
            : base(contextFactory)
        {
            _configuration = configuration;
            _emailTemplateRepository = emailTemplateRepository;
        }

        public string notification = string.Empty;
        public string subject = string.Empty;
        public string body = string.Empty;

        public async Task<IQueryable<UserNotificationModel>> GetNotificationForUser(string userId)
        {
            var context = ContextFactory();
            return await Task.FromResult(
                from un in context.UserNotification
                //join m in context.Meeting on un.MeetingId equals m.Id into ps
                //from m in ps.DefaultIfEmpty()
                //join c in context.AgendaItemComment on un.CommentId equals c.Id into pc
                //from c in pc.DefaultIfEmpty()
                where un.Acknowledged == false && un.UserId == userId
                orderby un.CreatedOn descending
                select new UserNotificationModel()
                {
                    Id = un.Id,
                    UserId = un.UserId,
                    Url = un.URL,
                    Message = un.Message,
                    NotificationTypeId = un.NotificationTypeId,
                    Title = un.Title,
                    Acknowledged = un.Acknowledged,
                    AcknowledgedOn = un.AcknowledgedOn,
                    CreatedOn = un.CreatedOn.SpecifyKind(DateTimeKind.Utc),
                    Date = un.CreatedOn.SpecifyKind(DateTimeKind.Utc).ToShortDateString(),
                    Time = un.CreatedOn.SpecifyKind(DateTimeKind.Utc).ToShortTimeString(),
                    isToday = un.CreatedOn.Date == DateTime.UtcNow.Date ? true : false,
                    OfferId = un.OfferId
                }
            );
        }

        public async Task<int> GetNotificationCountForUser(string userId)
        {
            var context = ContextFactory();

            return await Task.FromResult(
                context.UserNotification
                    .Where(x => x.UserId == userId && x.Acknowledged == false)
                    .ToList()
                    .Count
            );
        }

        protected override IQueryable<UserNotificationModel> GetEntities()
        {
            throw new NotImplementedException();
        }

        public async Task AcknowledgeNotification(int id)
        {
            var context = ContextFactory();
            var userNotification = context.UserNotification.Where(x => x.Id == id).FirstOrDefault();

            if (userNotification != null)
            {
                userNotification.Acknowledged = true;
                userNotification.AcknowledgedOn = DateTime.UtcNow;
                await context.SaveChangesAsync();
            }
        }

        private EmailTemplateModel GetEmailTemplateData(int templateId)
        {
            EmailTemplateModel template = _emailTemplateRepository.GetEmailTemplateData(templateId);

            return template
                ?? new EmailTemplateModel()
                {
                    Name = "Please add email templates.",
                    Subject = "Please add email templates.",
                    Body = "Please add email templates.",
                    Message = "Please add email templates.",
                    Notification = "Please add email templates.",
                    NotificationTypeId = (int)Declares.NotificationTypeList.Active
                };
        }

        public async Task CreateNotificationsForSurveys(ICollection<EmailDataModel> emailDataModel) //, string title, string message)
        {
            var context = ContextFactory();
            context.ChangeTracker.AutoDetectChangesEnabled = false;
            string url = _configuration["BaseURL:Url"];
            HashSet<UserNotification> userNotifications = new HashSet<UserNotification>();
            foreach (var emaildata in emailDataModel.ToHashSet())
            {
                UserNotification notification = new UserNotification
                {
                    UserId = emaildata.User.Id,
                    NotificationTypeId = (int?)NotificationTypeList.Active,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedOn = DateTime.UtcNow,
                    Acknowledged = false,
                    Title = "Survey",
                    Message = " You have new survey",
                    URL = url
                };
                notification.URL = String.Format(url + "surveys/" + emaildata.SurveyId);

                userNotifications.Add(notification);
            }

            var builder = new DbContextOptionsBuilder<MMADbContext>();
            builder.UseSqlServer(
                _configuration.GetConnectionString("eMarketOffers"),
                b => b.MigrationsAssembly("MMA.WebApi.DataAccess")
            );

            using (MMADbContext contextNew = new MMADbContext(builder.Options))
            {
                contextNew.ChangeTracker.AutoDetectChangesEnabled = false;
                await contextNew.UserNotification.AddRangeAsync(userNotifications);
                await contextNew.SaveChangesAsync();
            }
            userNotifications = null;
            GC.Collect();
        }

        public async Task CreateUserNotification(
            EmailDataModel emailDataModel,
            string title,
            string message
        )
        {
            var context = ContextFactory();

            UserNotification notification = new UserNotification
            {
                UserId = emailDataModel.User.Id,
                NotificationTypeId = (int?)NotificationTypeList.Active,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow,
                Acknowledged = false,
                OfferId = emailDataModel.OfferId,
                Title = title,
                Message = message
            };

            if (
                emailDataModel.OfferId.HasValue
                && (
                    emailDataModel.MailTemplateId
                        == MessageTemplateList.Offer_To_Process_Notify_Reviewer
                    || emailDataModel.MailTemplateId
                        == MessageTemplateList.Offer_Returned_Notify_SupplierAdminOrSupplier
                    || emailDataModel.MailTemplateId
                        == MessageTemplateList.Offer_To_Process_Notify_Coordinator
                    || emailDataModel.MailTemplateId
                        == MessageTemplateList.Offer_Processed_Notify_SupplierAdminOrSupplier
                )
            )
            {
                string url = _configuration["BaseURL:Url"];
                notification.URL = String.Format(url + "offers/assigned/" + emailDataModel.OfferId);
            }
            else if (
                emailDataModel.MailTemplateId
                == MessageTemplateList.Supplier_Registration_Notify_Coordinator
            )
            {
                string url = _configuration["BaseURL:Url"];
                notification.URL = String.Format(url + "suppliers/pending");
            }
            else if (
                emailDataModel.RoadshowId.HasValue
                    && (
                        emailDataModel.MailTemplateId
                            == MessageTemplateList.Roadshow_Submitted_Notify_Coordinator
                        || emailDataModel.MailTemplateId
                            == MessageTemplateList.Roadshow_Returned_To_Supplier_Notify_SupplierAdminOrSupplier
                        || emailDataModel.MailTemplateId
                            == MessageTemplateList.Roadshow_Approved_Notify_SupplierAdminOrSupplier
                        || emailDataModel.MailTemplateId
                            == MessageTemplateList.Roadshow_Reject_Attendance_Notify_Coordinator
                        || emailDataModel.MailTemplateId
                            == MessageTemplateList.Roadshow_Confirmed_Notify_All
                    )
                || emailDataModel.MailTemplateId
                    == MessageTemplateList.Roadshow_Expired_Notify_SupplierAdminOrSupplier
                || emailDataModel.MailTemplateId
                    == MessageTemplateList.Roadshow_Starts_In_1_Day_Notify_Coordinator
                || emailDataModel.MailTemplateId
                    == MessageTemplateList.Roadshow_Starts_In_5_Days_Notify_Coordinator
                || emailDataModel.MailTemplateId
                    == MessageTemplateList.Roadshow_Published_Notify_SupplierAdminOrSupplier
                || emailDataModel.MailTemplateId
                    == MessageTemplateList.Roadshow_Starts_Today_Notify_SupplierAdminOrSupplier
            )
            {
                string url = _configuration["BaseURL:Url"];
                notification.URL = String.Format(
                    url + "roadshows-administration/planning/" + emailDataModel.RoadshowId + "/edit"
                );
            }

            context.UserNotification.Add(notification);
            await context.SaveChangesAsync();
        }

        public async Task CreateNotificationForPublishJob(
            string userId,
            int userCount,
            int surveyId
        )
        {
            var context = ContextFactory();

            UserNotification notification = new UserNotification
            {
                UserId = userId,
                NotificationTypeId = (int?)NotificationTypeList.Active,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow,
                Acknowledged = false,
                Title = "Publish complete for survey " + surveyId,
                Message = "Number of opportunity: " + userCount,
                URL = String.Format(
                    _configuration["BaseURL:Url"] + "surveys/admin/" + surveyId + "/results"
                )
            };
            context.UserNotification.Add(notification);
            await context.SaveChangesAsync();
        }

        public async Task AcknowledgeAllNotifications(string userId)
        {
            var context = ContextFactory();

            (from un in context.UserNotification where un.UserId == userId select un)
                .ToList()
                .ForEach(un =>
                {
                    un.Acknowledged = true;
                    un.AcknowledgedOn = DateTime.UtcNow;
                });

            await context.SaveChangesAsync();
        }

        public async Task CreateNotificationForOfferReport(int offerId, ICollection<string> usersId)
        {
            var context = ContextFactory();
            var listnotification = new HashSet<UserNotification>();
            foreach (var userId in usersId)
            {
                UserNotification notification = new UserNotification
                {
                    UserId = userId,
                    NotificationTypeId = (int?)NotificationTypeList.Active,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedOn = DateTime.UtcNow,
                    Acknowledged = false,
                    Title = "Offer report",
                    Message = "Offer " + offerId + " was reported! ",
                    URL = String.Format(
                        _configuration["BaseURL:Url"] + "offers/reported/" + offerId
                    )
                };
                listnotification.Add(notification);
            }
            context.UserNotification.AddRange(listnotification);
            await context.SaveChangesAsync();
        }

        public async Task CreateNotificationForUserInvitation(string userId)
        {
            var context = ContextFactory();

            UserNotification notification = new UserNotification
            {
                UserId = userId,
                NotificationTypeId = (int?)NotificationTypeList.Active,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow,
                Acknowledged = false,
                Title = "User Invitation",
                Message = "User Invitation completed",
                URL = String.Format(
                    _configuration["BaseURL:Url"] + "administration/user-invitation"
                )
            };
            context.UserNotification.Add(notification);
            await context.SaveChangesAsync();
        }
    }
}
