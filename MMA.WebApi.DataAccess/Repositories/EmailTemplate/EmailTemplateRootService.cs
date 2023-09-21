using MMA.WebApi.Shared.Interfaces.EmailTemplate;
using MMA.WebApi.Shared.Models.Email;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMA.WebApi.DataAccess.Repositories.EmailTemplate
{
    public class EmailTemplateRootService : IEmailTemplateRootService
    {
        private readonly IEmailTemplateRootRepository _emailTemplateRootRepository;

        public EmailTemplateRootService(IEmailTemplateRootRepository emailTemplateRootRepository)
        {
            _emailTemplateRootRepository = emailTemplateRootRepository;
        }

        public async Task<IEnumerable<EmailTemplateRootModel>> Get()
        {
            var emailTtemplateRoot = _emailTemplateRootRepository.GetAllEmailTemplateRoot();

            return emailTtemplateRoot;
        }

        //public async Task<PaginationListModel<EmailTemplateRootModel>> EmailTemplateRootSearch(SearchModel search)
        //{
        //    var emails = _templateRootRepository.Get();

        //    PaginationListModel<EmailTemplateRootModel> retVal = new PaginationListModel<EmailTemplateRootModel>();
        //    retVal.List = emails.Skip((search.PageIndex - 1) * search.PageSize).Take(search.PageSize); //*********
        //    if (emails.Count() == 0)
        //    {
        //        retVal.PageIndex = 0;
        //    }
        //    else
        //    {
        //        retVal.PageIndex = search.PageIndex;
        //    }
        //    retVal.PageSize = search.PageSize;
        //    retVal.TotalCount = emails.Count();
        //    retVal.TotalPageCount = (int)Math.Ceiling(retVal.TotalCount / (double)retVal.PageSize);

        //    return await Task.FromResult(retVal);
        //}

        public async Task<int> CreatEmailTemplateRoot(EmailTemplateRootModel model, string userId)
        {
            return await _emailTemplateRootRepository.InsertEmail(model);
        }

        public async Task<bool> UpdateEmailTemplateRoot(EmailTemplateRootModel data, string userId)
        {
            bool dataExists = _emailTemplateRootRepository.Get().Any(x => x.Id == data.Id);

            if (dataExists)
            {
                await _emailTemplateRootRepository.UpdateEmail(data, userId);
                return true;
            }
            else
                return false;
        }

        public async Task<IEnumerable<int>> DeleteEmailTemplateRoots(IEnumerable<int> ids)
        {
            return await _emailTemplateRootRepository.DeleteListAsync(ids);
        }

        public async Task<EmailTemplateRootModel> GetEmailTemplateRootDetails(int id)
        {
            return await _emailTemplateRootRepository.GetEmailDetails(x => x.Id.Equals(id));
        }
    }
}
