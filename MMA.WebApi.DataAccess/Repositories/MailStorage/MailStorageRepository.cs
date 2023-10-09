using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.Domain.Document;
using MMA.WebApi.Shared.Interfaces.EmailTemplate;
using MMA.WebApi.Shared.Interfaces.MailStorage;
using MMA.WebApi.Shared.Interfaces.UserNotifications;
using MMA.WebApi.Shared.Models;
using MMA.WebApi.Shared.Models.Announcement;
using MMA.WebApi.Shared.Models.ApplicationUser;
using MMA.WebApi.Shared.Models.Logger;
using MMA.WebApi.Shared.Models.MailStorage;
using MMA.WebApi.Shared.Models.Users;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.WebApi.DataAccess.MailStorage
{
    public class MailStorageRepository : BaseRepository<MailStorageModel>, IMailStorageRepository
    {
        private readonly IEmailTemplateRepository _emailTemplateRepository;
        private readonly IEmailTemplateRootRepository _emailTemplateRootRepository;
        private readonly IConfiguration _configuration;
        private readonly IUserNotificationRepository _userNotificationRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDocumentService _documentService;

        public MailStorageRepository(
            Func<MMADbContext> contextFactory,
            IEmailTemplateRepository emailTemplateRepository,
            IEmailTemplateRootRepository emailTemplateRootRepository,
            IConfiguration configuration,
            IUserNotificationRepository userNotificationRepository,
            UserManager<ApplicationUser> userManager,
            IDocumentService documentService
        )
            : base(contextFactory)
        {
            _emailTemplateRepository = emailTemplateRepository;
            _emailTemplateRootRepository = emailTemplateRootRepository;
            _configuration = configuration;
            _userNotificationRepository = userNotificationRepository;
            _userManager = userManager;
            _documentService = documentService;
        }

        protected override IQueryable<MailStorageModel> GetEntities()
        {
            var context = ContextFactory();
            return from ms in context.MailStorage
                select new MailStorageModel
                {
                    Id = ms.Id,
                    UserId = ms.UserId,
                    Subject = ms.Subject,
                    Body = ms.Body,
                    OfferId = ms.OfferId,
                    Status = ((Declares.MessageStatusList)ms.StatusId).ToString(),
                    StatusId = ms.StatusId,
                    StatusOn = ms.StatusOn,
                    StatusNote = ms.StatusNote,
                    UserEmail = ms.UserEmail
                };
        }

        public async Task<MailStorageModel> GetSingleAsync(
            Expression<Func<MailStorageModel, bool>> query
        )
        {
            return await GetEntities().FirstOrDefaultAsync(query);
        }

        private async Task SetEmailStatus(
            int emailId,
            MessageStatusList status,
            string exception = ""
        )
        {
            var context = ContextFactory();

            var entityModel = context.MailStorage.FirstOrDefault(x => x.Id == emailId);
            entityModel.StatusId = (int)status;
            entityModel.StatusNote = exception;
            entityModel.StatusOn = DateTime.UtcNow;

            context.Update(entityModel);
            await context.SaveChangesAsync();
        }

        private EmailDataModel CreateEmailDataModel(ApplicationUserModel user, int surveyId)
        {
            return new EmailDataModel()
            {
                User = user,
                MailTemplateId = MessageTemplateList.Survey_Notify,
                OfferId = null,
                CompanyName = "",
                IsApproved = true,
                SurveyId = surveyId
            };
        }

        public void DetachAllEntities()
        {
            var context = ContextFactory();
            var changedEntriesCopy = context.ChangeTracker
                .Entries()
                .Where(
                    e =>
                        e.State == EntityState.Added
                        || e.State == EntityState.Modified
                        || e.State == EntityState.Deleted
                )
                .ToList();

            foreach (var entry in changedEntriesCopy)
                entry.State = EntityState.Detached;

            GC.Collect();
        }

        public async Task CreateMailForAllUsers(
            ICollection<ApplicationUserModel> allUsers,
            int surveyId,
            ILogger log
        )
        {
            var builder = new DbContextOptionsBuilder<MMADbContext>();
            builder.UseSqlServer(
                _configuration.GetConnectionString("eMarketOffers"),
                b => b.MigrationsAssembly("MMA.WebApi.DataAccess")
            );

            var mailData = new List<Models.MailStorage>();
            var mailDataForNotification = new List<EmailDataModel>();
            var reviewerRole = Declares.Roles.Reviewer.ToString();
            var coordinatorRole = Declares.Roles.AdnocCoordinator.ToString();
            var adminRole = Declares.Roles.Admin.ToString();
            int counter = 0;
            EmailTemplateUtils template = new EmailTemplateUtils(
                _emailTemplateRepository,
                _emailTemplateRootRepository,
                _configuration
            );
            foreach (var u in allUsers.ToHashSet())
            {
                if (
                    !string.IsNullOrWhiteSpace(u.Email)
                    && (
                        u.Role != null
                        && (
                            !(
                                u.Role.Contains(reviewerRole)
                                || u.Role == adminRole
                                || !u.Email.Contains("@")
                            )
                        )
                    )
                )
                {
                    try
                    {
                        var emailDataModel = CreateEmailDataModel(u, surveyId);
                        List<string> emails = new List<string>();
                        emails.Add(emailDataModel.User.Email);
                        Models.MailStorage email = new Models.MailStorage
                        {
                            UserEmail = string.Join(",", emails),
                            CreatedOn = DateTime.UtcNow,
                            UserId = emailDataModel.User.Id,
                            CreatedAt = DateTime.UtcNow
                        };
                        counter++;

                        template.CreateBodyAndSubject(emailDataModel);
                        mailDataForNotification.Add(emailDataModel);
                        email.Subject = template.subject;
                        email.Body = String.IsNullOrEmpty(template.body)
                            ? "template.body"
                            : template.body;

                        email.StatusId = (int)Declares.MessageStatusList.PENDING;
                        email.StatusOn = email.CreatedOn;
                        mailData.Add(email);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                else
                {
                    //notification
                    var emailDataModel = CreateEmailDataModel(u, surveyId);
                    mailDataForNotification.Add(emailDataModel);
                }
                if (counter >= 1000)
                {
                    using (MMADbContext contextNew = new MMADbContext(builder.Options))
                    {
                        contextNew.ChangeTracker.AutoDetectChangesEnabled = false;
                        await contextNew.MailStorage.AddRangeAsync(mailData);
                        await contextNew.SaveChangesAsync();
                    }

                    await _userNotificationRepository.CreateNotificationsForSurveys(
                        mailDataForNotification
                    );

                    log.LogInformation("Send mail for " + mailData.Count() + " users");
                    log.LogInformation(
                        "Send notif for " + mailDataForNotification.Count() + " users"
                    );

                    mailData.Clear();
                    mailData = null;

                    mailDataForNotification.Clear();
                    mailDataForNotification = null;
                    counter = 0;

                    GC.Collect();
                    mailData = new List<Models.MailStorage>();
                    mailDataForNotification = new List<EmailDataModel>();
                }
            }

            using (MMADbContext contextNew = new MMADbContext(builder.Options))
            {
                contextNew.ChangeTracker.AutoDetectChangesEnabled = false;
                await contextNew.MailStorage.AddRangeAsync(mailData);
                await contextNew.SaveChangesAsync();
            }
            await _userNotificationRepository.CreateNotificationsForSurveys(
                mailDataForNotification
            );
            log.LogInformation("Send mail for " + mailData.Count() + " users");
            log.LogInformation("Send notif for " + mailDataForNotification.Count() + " users");
        }

        public async Task CreateMail(EmailDataModel emailDataModel)
        {
            if (
                emailDataModel.MailTemplateId
                != Declares.MessageTemplateList.Adnoc_Employee_Invited_New_Family_Member
            )
            {
                var reviewerRole = Declares.Roles.Reviewer.ToString();
                var coordinatorRole = Declares.Roles.AdnocCoordinator.ToString();
                var adminRole = Declares.Roles.Admin.ToString();
                var user = await _userManager.FindByIdAsync(emailDataModel.User.Id);
                var userRoles = await _userManager.GetRolesAsync(user);

                if (
                    userRoles.Contains(adminRole)
                    || userRoles.Contains(reviewerRole)
                    || emailDataModel.User.Email == null
                    || !emailDataModel.User.Email.Contains("@")
                )
                    return;
            }

            try
            {
                var context = ContextFactory();
                List<string> emails = new List<string>();
                if (!string.IsNullOrWhiteSpace(emailDataModel.User.Email))
                {
                    emails.Add(emailDataModel.User.Email);
                }
                ;

                Models.MailStorage email = new Models.MailStorage
                {
                    UserEmail = string.Join(",", emails),
                    CreatedOn = DateTime.UtcNow,
                    UserId = emailDataModel.User.Id,
                    CreatedAt = DateTime.UtcNow
                };

                EmailTemplateUtils template = new EmailTemplateUtils(
                    _emailTemplateRepository,
                    _emailTemplateRootRepository,
                    _configuration
                );
                template.CreateBodyAndSubject(emailDataModel);

                email.Subject = template.subject;
                email.Body = String.IsNullOrEmpty(template.body) ? "template.body" : template.body;

                email.StatusId = (int)Declares.MessageStatusList.PENDING;
                email.StatusOn = email.CreatedOn;

                if (emailDataModel.OfferId != null)
                    email.OfferId = emailDataModel.OfferId;

                if (string.IsNullOrWhiteSpace(email.UserId))
                    email.UserId = (
                        from ur in context.UserRoles
                        join r in context.Roles on ur.RoleId equals r.Id
                        where r.Name == Declares.Roles.Admin.ToString()
                        select ur.UserId
                    ).FirstOrDefault();

                context.MailStorage.Add(email);
                await context.SaveChangesAsync();

                if (
                    template.notificationTypeId == NotificationTypeList.Active
                    && emailDataModel.User != null
                    && !string.IsNullOrWhiteSpace(emailDataModel.User.Id)
                )
                {
                    await _userNotificationRepository.CreateUserNotification(
                        emailDataModel,
                        template.subject,
                        template.message
                    );
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task CreateMailsForAnnouncement(
            AnnouncementModel model,
            string userId,
            ICollection<ApplicationUserModel> users
        )
        {
            var builder = new DbContextOptionsBuilder<MMADbContext>();
            builder.UseSqlServer(
                _configuration.GetConnectionString("eMarketOffers"),
                b => b.MigrationsAssembly("MMA.WebApi.DataAccess")
            );

            var mailData = new List<Models.MailStorage>();
            int counter = 0;
            foreach (var u in users.ToHashSet())
            {
                if (!string.IsNullOrWhiteSpace(u.Email) && u.Email.Contains("@"))
                {
                    try
                    {
                        Models.MailStorage email = new Models.MailStorage
                        {
                            UserEmail = u.Email,
                            CreatedOn = DateTime.UtcNow,
                            UserId = u.Id,
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = userId,
                            AnnouncementId = model.Id
                        };
                        counter++;
                        email.Subject = "Announcement";
                        email.Body = model.AnnouncementText;

                        email.StatusId = (int)Declares.MessageStatusList.PENDING;
                        email.StatusOn = email.CreatedOn;

                        if (model.Attachments != null)
                        {
                            email.MailStorageDocuments = model.Attachments
                                .Select(
                                    x =>
                                        new MailStorageDocument
                                        {
                                            DocumentId = Guid.Parse(x.Id),
                                            MailStorageId = email.Id
                                        }
                                )
                                .ToList();
                        }

                        mailData.Add(email);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                if (counter >= 1000)
                {
                    using (MMADbContext contextNew = new MMADbContext(builder.Options))
                    {
                        contextNew.ChangeTracker.AutoDetectChangesEnabled = false;
                        await contextNew.MailStorage.AddRangeAsync(mailData);
                        await contextNew.SaveChangesAsync();
                    }

                    mailData.Clear();
                    mailData = null;

                    counter = 0;

                    GC.Collect();
                    mailData = new List<Models.MailStorage>();
                }
            }

            using (MMADbContext contextNew = new MMADbContext(builder.Options))
            {
                contextNew.ChangeTracker.AutoDetectChangesEnabled = false;
                await contextNew.MailStorage.AddRangeAsync(mailData);
                await contextNew.SaveChangesAsync();
            }
        }

        private async Task UpdateStatus(IEnumerable<Models.MailStorage> emailsToSend, int statusId)
        {
            var context = ContextFactory();
            foreach (var email in emailsToSend)
            {
                email.StatusId = statusId;
                email.StatusOn = DateTime.UtcNow;

                context.MailStorage.Update(email);
            }

            await context.SaveChangesAsync();
        }

        public async Task CheckMessageQueue()
        {
            var context = ContextFactory();
            Logger logger = new Logger();
            logger.Info("******** CheckMessageQueue  ");
            DateTime now = DateTime.UtcNow;

            //'1. get pending emails in chunk of 15 per time interval
            var emailsToSend = context.MailStorage
                .Include(x => x.MailStorageDocuments)
                .ThenInclude(x => x.Document)
                .Where(
                    x =>
                        x.StatusId == (int)Declares.MessageStatusList.PENDING
                        && x.AnnouncementId == null
                )
                .Take(50)
                .ToList();
            emailsToSend.AddRange(
                context.MailStorage
                    .Include(x => x.MailStorageDocuments)
                    .ThenInclude(x => x.Document)
                    .Where(
                        x =>
                            x.StatusId == (int)Declares.MessageStatusList.PENDING
                            && x.AnnouncementId != null
                    )
                    .Take(50 - emailsToSend.Count)
                    .ToList()
            );

            //1. first we set all emails into processing status
            var statusId = (int)Declares.MessageStatusList.PROCESSING;

            await this.UpdateStatus(emailsToSend, statusId);

            //2. then we start sending them
            foreach (var email in emailsToSend)
            {
                SendEmail(email);

                logger.Info("email sent" + email.UserEmail);
            }

            //  3.check if there still are processing emails after one hour
            DateTime _20minutesAgo = now.AddMinutes(-20);

            var emailProcessingTooLong = context.MailStorage
                .Where(
                    x =>
                        x.StatusId == (int)Declares.MessageStatusList.PROCESSING
                        && x.StatusOn < _20minutesAgo
                )
                .ToList();
            //1. change status for all emails that are processing too long into pending again

            statusId = (int)Declares.MessageStatusList.PENDING;
            await this.UpdateStatus(emailProcessingTooLong, statusId);
        }

        public async Task DeleteListAsync(IEnumerable<int> list)
        {
            var context = ContextFactory();
            var dbModels = context.MailStorage.Where(x => list.Contains(x.Id));
            context.MailStorage.RemoveRange(dbModels);
            await context.SaveChangesAsync();
        }

        public IQueryable<MailStorageModel> Get(QueryModel queryModel)
        {
            var filteredStorage = GetEntities();

            foreach (string statusName in queryModel.Filter.Status)
            {
                Declares.MessageStatusList status = (Declares.MessageStatusList)
                    Enum.Parse(typeof(Declares.MessageStatusList), statusName);
                filteredStorage = filteredStorage.Where(x => x.StatusId == (int)status);
            }
            if (queryModel.Filter.Keyword?.Any() == true)
            {
                return filteredStorage.Where(
                    o => o.UserEmail.ToLower().Contains(queryModel.Filter.Keyword.Trim().ToLower())
                );
            }

            return filteredStorage;
        }

        private void SendEmail(Models.MailStorage email)
        {
            string smtpHost = _configuration["Emails:MailHost"];
            string smtpPassword = _configuration["Emails:EmailPassword"];
            int smtpPort = Convert.ToInt32(_configuration["Emails:MailServerPort"]);
            bool EnableSsl = Convert.ToBoolean(_configuration["Emails:EnableSsl"]);
            bool UseDefaultCredentials = Convert.ToBoolean(
                _configuration["Emails:UseDefaultCredentials"]
            );
            string fromEmail = _configuration["Emails:FromAddress"];
            string from = _configuration["Emails:From"];

            using (MailMessage mailMessage = new MailMessage())
            {
                // create mail message
                MailAddress fromAddress = new MailAddress(fromEmail, from);

                mailMessage.From = fromAddress;

                mailMessage.To.Add(new MailAddress(email.UserEmail));

                var sysAdminEmail = _configuration["Emails:SysAdminMail"];
                if (
                    !string.IsNullOrWhiteSpace(sysAdminEmail) && sysAdminEmail.Contains("@adnoc.ae")
                )
                {
                    mailMessage.CC.Add(new MailAddress(sysAdminEmail));
                }
                mailMessage.Subject = email.Subject;
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = email.Body;
                if (email.MailStorageDocuments != null && email.MailStorageDocuments.Count > 0)
                {
                    foreach (var attachment in email.MailStorageDocuments)
                    {
                        var doc = _documentService.Download(attachment.DocumentId).Result;
                        var mimeType = attachment.Document.MimeType.Split('/');
                        try
                        {
                            Attachment at = new Attachment(
                                new MemoryStream(doc.Content),
                                $"{email.Subject}.{mimeType[1]}"
                            );
                            mailMessage.Attachments.Add(at);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }

                using (SmtpClient SmtpServer = new SmtpClient())
                {
                    SmtpServer.Host = smtpHost;
                    SmtpServer.Port = smtpPort;
                    SmtpServer.EnableSsl = EnableSsl;
                    SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                    SmtpServer.UseDefaultCredentials = UseDefaultCredentials;
                    SmtpServer.Credentials = new NetworkCredential(fromEmail, smtpPassword);

                    // set email message status
                    try
                    {
                        SmtpServer.Send(mailMessage);

                        this.SetEmailStatus(email.Id, Declares.MessageStatusList.SENT).Wait();
                    }
                    catch (Exception ex)
                    {
                        this.SetEmailStatus(
                                email.Id,
                                Declares.MessageStatusList.FAILED,
                                ex.ToString()
                            )
                            .Wait();
                    }
                }
            }
        }

        public async Task CreateShareMail(
            string userId,
            string userMail,
            string subject,
            string body
        )
        {
            try
            {
                var context = ContextFactory();

                Models.MailStorage email = new Models.MailStorage
                {
                    UserEmail = userMail,
                    CreatedOn = DateTime.UtcNow,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };

                email.Subject = subject ?? String.Empty;
                email.Body = body ?? String.Empty;

                email.StatusId = (int)Declares.MessageStatusList.PENDING;
                email.StatusOn = email.CreatedOn;

                context.MailStorage.Add(email);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public EmailDataModel CreateMailData(
            InviteUsersModel invitedUser,
            MessageTemplateList messageTemplateList
        )
        {
            var user = new ApplicationUserModel()
            {
                Id = string.Empty,
                UserName = invitedUser.InvitedUserEmail,
                Email = invitedUser.InvitedUserEmail
            };

            var emailData = new EmailDataModel()
            {
                User = user,
                MailTemplateId = messageTemplateList
            };

            return emailData;
        }

        public async Task CreateEmialInvitationForUsersFromSpecificExcelFile(
            IEnumerable<EmailDataModel> mailModels,
            string userId
        )
        {
            var builder = new DbContextOptionsBuilder<MMADbContext>();
            builder.UseSqlServer(
                _configuration.GetConnectionString("Database"),
                b => b.MigrationsAssembly("MMA.WebApi.DataAccess")
            );

            EmailTemplateUtils template = new EmailTemplateUtils(
                _emailTemplateRepository,
                _emailTemplateRootRepository,
                _configuration
            );
            try
            {
                int counter = 0;
                var mailStorageDeata = new List<Models.MailStorage>();
                foreach (var emailDataModel in mailModels)
                {
                    counter++;
                    List<string> emails = new List<string>();
                    if (!string.IsNullOrWhiteSpace(emailDataModel.User.Email))
                    {
                        emails.Add(emailDataModel.User.Email);
                    }
                    ;

                    Models.MailStorage email = new Models.MailStorage
                    {
                        UserEmail = string.Join(",", emails),
                        CreatedOn = DateTime.UtcNow,
                        UserId = userId,
                        CreatedAt = DateTime.UtcNow
                    };

                    template.CreateBodyAndSubject(emailDataModel);

                    email.Subject = template.subject;
                    email.Body = String.IsNullOrEmpty(template.body)
                        ? "template.body"
                        : template.body;

                    email.StatusId = (int)Declares.MessageStatusList.PENDING;
                    email.StatusOn = email.CreatedOn;

                    mailStorageDeata.Add(email);
                    if (counter > 20)
                    {
                        GC.Collect();
                        counter = 0;
                    }
                }

                using (MMADbContext contextNew = new MMADbContext(builder.Options))
                {
                    await contextNew.MailStorage.AddRangeAsync(mailStorageDeata);
                    contextNew.ChangeTracker.AutoDetectChangesEnabled = false;
                    await contextNew.SaveChangesAsync();
                }

                mailStorageDeata.Clear();
                mailStorageDeata = null;
                GC.Collect();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Tuple<int, int>> GetCountSentAnnouncement(int announcementId)
        {
            var context = ContextFactory();
            var pending = await context.MailStorage.FirstOrDefaultAsync(
                x =>
                    x.AnnouncementId == announcementId
                    && (
                        x.StatusId == (int)MessageStatusList.PENDING
                        || x.StatusId == (int)MessageStatusList.PROCESSING
                    )
            );
            if (pending != null)
                return await Task.FromResult<Tuple<int, int>>(null);

            var sent = await context.MailStorage
                .AsNoTracking()
                .Where(
                    x =>
                        x.AnnouncementId == announcementId
                        && x.StatusId == (int)MessageStatusList.SENT
                )
                .CountAsync();
            var failed = await context.MailStorage
                .AsNoTracking()
                .Where(
                    x =>
                        x.AnnouncementId == announcementId
                        && x.StatusId == (int)MessageStatusList.FAILED
                )
                .CountAsync();

            return new Tuple<int, int>(sent, failed);
        }
    }
}
