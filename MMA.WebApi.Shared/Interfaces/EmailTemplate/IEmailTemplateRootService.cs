using MMA.WebApi.Shared.Models.Email;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.EmailTemplate
{
    public interface IEmailTemplateRootService
    {
        Task<IEnumerable<EmailTemplateRootModel>> Get();

        //Task<PaginationListModel<EmailTemplateRootModel>> EmailTemplateRootSearch(SearchModel search);
        Task<int> CreatEmailTemplateRoot(EmailTemplateRootModel model, string userId);
        Task<bool> UpdateEmailTemplateRoot(EmailTemplateRootModel data, string userId);
        Task<IEnumerable<int>> DeleteEmailTemplateRoots(IEnumerable<int> ids);
        Task<EmailTemplateRootModel> GetEmailTemplateRootDetails(int id);
    }
}
