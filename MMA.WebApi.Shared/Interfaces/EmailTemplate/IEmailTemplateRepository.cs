using MMA.WebApi.Shared.Models.Email;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.EmailTemplate
{
    public interface IEmailTemplateRepository
    {
        EmailTemplateModel GetEmailTemplateData(int templateId);

        IQueryable<EmailTemplateModel> Get();
        Task<int> DeleteAsync(int id);
        Task<int> InsertEmail(EmailTemplateModel data);
        Task<int> UpdateEmail(EmailTemplateModel data, string userId);
        Task<EmailTemplateModel> GetEmailDetails(Expression<Func<EmailTemplateModel, bool>> query);

    }
}
