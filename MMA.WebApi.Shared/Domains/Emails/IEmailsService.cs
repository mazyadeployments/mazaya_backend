using MMA.WebApi.Shared.Models.MailStorage;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.WebApi.Shared.Interfaces.Domain.Emails
{
    public interface IEmailsService
    {

        Task CheckMessageQueue();
        void SendEmail(MailStorageModel email);
        void SetEmailStatus(int emailId, MessageStatusList status, string exception = "");

        //Task CreateMailsFor(Declares.MessageTemplateList mailTemplateId, IEnumerable<UserInfoModel> userIds, MeetingModel meeting, AgendaItemModel agendaItem, ActionModel action, string customInfo);
    }
}
