using MMA.WebApi.Shared.Models.Email;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.EmailTemplate
{
    public interface IEmailTemplateService
    {
        Task<IEnumerable<EmailTemplateModel>> GetEmails();
        Task<int> CreatEmail(EmailTemplateModel model, string userId);
        Task<bool> UpdateEmail(EmailTemplateModel data, string userId);
        Task<int> DeleteEmail(int id);
        Task<EmailTemplateModel> GetEmailDetails(int id);
    }
}
