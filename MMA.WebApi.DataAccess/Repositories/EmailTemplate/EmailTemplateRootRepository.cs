using Microsoft.EntityFrameworkCore;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Interfaces.EmailTemplate;
using MMA.WebApi.Shared.Models.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MMA.WebApi.DataAccess.EmailTemplate
{
    public class EmailTemplateRootRepository : BaseRepository<EmailTemplateRootModel>, IEmailTemplateRootRepository
    {

        public EmailTemplateRootRepository(Func<MMADbContext> contextFactory) : base(contextFactory)
        {
        }

        protected override IQueryable<EmailTemplateRootModel> GetEntities()
        {
            var context = ContextFactory();

            return from e in context.EmailTemplateRoot
                   select new EmailTemplateRootModel
                   {
                       Id = e.Id,
                       MailTemplate = e.MailTemplate,
                       MailApplicationLogin = e.MailApplicationLogin,
                       MailBodyFooter = e.MailBodyFooter
                   };
        }

        public IEnumerable<EmailTemplateRootModel> GetAllEmailTemplateRoot()
        {
            return GetEntities().AsEnumerable();
        }

        public EmailTemplateRootModel GetEmailTemplateRootData(int mailTemplateId)
        {
            var context = ContextFactory();

            var template = from e in context.EmailTemplateRoot
                           where e.Id == mailTemplateId
                           select new EmailTemplateRootModel
                           {
                               Id = e.Id,
                               MailTemplate = e.MailTemplate
                           };
            return template.FirstOrDefault();
        }

        public IQueryable<EmailTemplateRootModel> Get()
        {
            return GetEntities();
        }

        public async Task<IQueryable<int>> DeleteListAsync(IEnumerable<int> list)
        {
            var context = ContextFactory();
            var dbModels = context.EmailTemplateRoot.Where(x => list.Contains(x.Id));

            context.RemoveRange(dbModels);
            await context.SaveChangesAsync();
            return new List<int>().AsQueryable();
        }

        public async Task<int> InsertEmail(EmailTemplateRootModel data)
        {
            var context = ContextFactory();
            var entityModel = PopulateDbFromDomainModel(new DataAccess.Models.EmailTemplateRoot(), data);
            var emailTemplateRoot = context.EmailTemplateRoot.ToList();
            var lastInsertedId = 0;
            if (emailTemplateRoot.Count > 0)
            {
                lastInsertedId = emailTemplateRoot.OrderByDescending(x => x.Id).FirstOrDefault().Id;
            }
            entityModel.Id = lastInsertedId + 1;
            //ChangeableHelper.Set(entityModel, "sysadmin", true);
            await context.EmailTemplateRoot.AddAsync(entityModel);
            await context.SaveChangesAsync();

            return entityModel.Id;
        }

        public async Task<int> UpdateEmail(EmailTemplateRootModel data, string userId)
        {
            var context = ContextFactory();

            var entityModel = await context.EmailTemplateRoot.FirstOrDefaultAsync(x => x.Id.Equals(data.Id));
            entityModel = PopulateDbFromDomainModel(entityModel, data);
            //ChangeableHelper.Set(entityModel, userId, false);

            await context.SaveChangesAsync();
            return entityModel.Id;
        }

        public async Task<EmailTemplateRootModel> GetEmailDetails(Expression<Func<EmailTemplateRootModel, bool>> query)
        {
            return await GetEntities().FirstOrDefaultAsync(query);
        }

        protected virtual DataAccess.Models.EmailTemplateRoot PopulateDbFromDomainModel(DataAccess.Models.EmailTemplateRoot entityModel, EmailTemplateRootModel data)
        {
            entityModel.MailTemplate = data.MailTemplate;
            entityModel.MailBodyFooter = data.MailBodyFooter;
            entityModel.MailApplicationLogin = data.MailApplicationLogin;

            return entityModel;
        }
    }
}
