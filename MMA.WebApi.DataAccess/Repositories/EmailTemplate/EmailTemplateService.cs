using MMA.WebApi.Shared.Interfaces.EmailTemplate;
using MMA.WebApi.Shared.Models.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMA.WebApi.DataAccess.Repositories.EmailTemplate
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly IEmailTemplateRepository _emailTemplateRepository;

        public EmailTemplateService(IEmailTemplateRepository emailTemplateRepository)
        {
            _emailTemplateRepository = emailTemplateRepository;
        }

        public async Task<int> CreatEmail(EmailTemplateModel model, string userId)
        {
            model.Notification = DecodeBase64String(model.Notification);
            model.Message = DecodeBase64String(model.Message);
            return await _emailTemplateRepository.InsertEmail(model);
        }

        public async Task<int> DeleteEmail(int id)
        {
            return await _emailTemplateRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<EmailTemplateModel>> GetEmails()
        {
            return await Task.FromResult(_emailTemplateRepository.Get());
        }

        public async Task<bool> UpdateEmail(EmailTemplateModel data, string userId)
        {
            bool dataExists = _emailTemplateRepository.Get().Any(x => x.Id == data.Id);

            if (dataExists)
            {
                await _emailTemplateRepository.UpdateEmail(data, userId);
                return true;
            }
            else
                return false;
        }

        public async Task<EmailTemplateModel> GetEmailDetails(int id)
        {
            return await _emailTemplateRepository.GetEmailDetails(x => x.Id.Equals(id));
        }
        private string DecodeBase64String(string encodedString)
        {
            byte[] data = Convert.FromBase64String(encodedString);
            return System.Text.Encoding.UTF8.GetString(data);
        }

    }
}
