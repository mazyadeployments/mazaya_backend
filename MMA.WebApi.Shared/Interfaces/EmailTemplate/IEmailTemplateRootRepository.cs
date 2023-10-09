using MMA.WebApi.Shared.Models.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.EmailTemplate
{
    public interface IEmailTemplateRootRepository
    {

        EmailTemplateRootModel GetEmailTemplateRootData(int mailTemplateId);

        IEnumerable<EmailTemplateRootModel> GetAllEmailTemplateRoot();
        //EmailTemplateRootModel GetEmailTemplateRootData(int mailTemplateId);
        IQueryable<EmailTemplateRootModel> Get();
        Task<IQueryable<int>> DeleteListAsync(IEnumerable<int> list);
        Task<int> InsertEmail(EmailTemplateRootModel data);
        Task<int> UpdateEmail(EmailTemplateRootModel data, string userId);
        Task<EmailTemplateRootModel> GetEmailDetails(Expression<Func<EmailTemplateRootModel, bool>> query);
    }
}
