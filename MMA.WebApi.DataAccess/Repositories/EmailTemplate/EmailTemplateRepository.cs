using Microsoft.EntityFrameworkCore;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Interfaces.EmailTemplate;
using MMA.WebApi.Shared.Models.Email;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MMA.WebApi.DataAccess.EmailTemplate
{
    public class EmailTemplateRepository : BaseRepository<EmailTemplateModel>, IEmailTemplateRepository
    {

        public EmailTemplateRepository(Func<MMADbContext> contexFactory) : base(contexFactory)
        {

        }

        public EmailTemplateModel GetEmailTemplateData(int templateId)
        {
            var context = ContextFactory();

            var template = (from t in context.EmailTemplate
                            where t.Id == templateId
                            select new EmailTemplateModel
                            {
                                Id = t.Id,
                                Name = t.Name,
                                Subject = t.Subject,
                                Message = t.Message,
                                Notification = t.Notification,
                                NotificationTypeId = t.NotificationTypeId,
                                Body = t.Body

                            }).FirstOrDefault();
            return template;
        }

        protected override IQueryable<EmailTemplateModel> GetEntities()
        {
            var context = ContextFactory();

            return from t in context.EmailTemplate
                   select new EmailTemplateModel
                   {
                       Id = t.Id,
                       Name = t.Name,
                       Subject = t.Subject,
                       Message = t.Message,
                       Notification = t.Notification,
                       NotificationTypeId = t.NotificationTypeId,
                       Body = t.Body,
                       Sms = t.Sms
                   };
        }

        public async Task<int> DeleteAsync(int id)
        {
            var context = ContextFactory();
            var dbModel = context.EmailTemplate.Where(x => id == x.Id).FirstOrDefault();

            context.Remove(dbModel);
            await context.SaveChangesAsync();
            return id;
        }

        public IQueryable<EmailTemplateModel> Get()
        {
            return GetEntities();

        }

        public async Task<EmailTemplateModel> GetEmailDetails(Expression<Func<EmailTemplateModel, bool>> query)
        {
            return await GetEntities().FirstOrDefaultAsync(query);
        }

        public async Task<int> InsertEmail(EmailTemplateModel data)
        {
            var context = ContextFactory();
            var entityModel = PopulateDbFromDomainModel(new DataAccess.Models.EmailTemplate(), data);
            var emailTemplate = context.EmailTemplate.ToList();
            var lastInsertedId = 0;
            if (emailTemplate.Count > 0)
            {
                lastInsertedId = emailTemplate.OrderByDescending(x => x.Id).FirstOrDefault().Id;
            }
            entityModel.Id = lastInsertedId + 1;
            //ChangeableHelper.Set(entityModel, "sysadmin", true);
            await context.EmailTemplate.AddAsync(entityModel);
            await context.SaveChangesAsync();

            return entityModel.Id;
        }

        protected virtual DataAccess.Models.EmailTemplate PopulateDbFromDomainModel(DataAccess.Models.EmailTemplate entityModel, EmailTemplateModel data)
        {
            entityModel.Name = data.Name;
            entityModel.Subject = data.Subject;
            entityModel.Body = data.Body;
            entityModel.Message = data.Message;
            entityModel.Notification = data.Notification;
            entityModel.CreatedOn = DateTime.UtcNow;
            entityModel.UpdatedOn = DateTime.UtcNow;
            entityModel.NotificationTypeId = data.NotificationTypeId;
            entityModel.Sms = data.Sms;
            //entityModel.MailTemplateType = data.MailTemplateType;

            return entityModel;
        }

        public async Task<int> UpdateEmail(EmailTemplateModel data, string userId)
        {
            var context = ContextFactory();

            var entityModel = await context.EmailTemplate.FirstOrDefaultAsync(x => x.Id.Equals(data.Id));
            data.Notification = DecodeBase64String(data.Notification);
            data.Message = DecodeBase64String(data.Message);
            entityModel = PopulateDbFromDomainModel(entityModel, data);
            //ChangeableHelper.Set(entityModel, userId, false);

            await context.SaveChangesAsync();
            return entityModel.Id;
        }

        private string DecodeBase64String(string encodedString)
        {
            encodedString ??= "";
            byte[] data = Convert.FromBase64String(encodedString);
            return Encoding.UTF8.GetString(data);
        }
    }
}
