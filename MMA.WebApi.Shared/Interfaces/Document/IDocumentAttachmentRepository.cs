using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Models.Document;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.Document
{
    public interface IDocumentAttachmentRepository : IQueryableRepository<DocumentAttachment>, ICrudAsync<string, DocumentAttachment>,
        IDeletableListAsync<string>
    {
        Task<string> InsertActionAttachment(DocumentActionAttachment data);
        IQueryable<DocumentActionAttachment> GetActionAttachments();
        Task<string> InsertAsync(DocumentAttachment data, string userId);
        Task<IEnumerable<DocumentAttachment>> GetAgendaItemAttachments(int meetingId, string query);
        Task<IEnumerable<DocumentAttachment>> GetDocumentAttachments(int agendaItemId, string query);
        Task<IEnumerable<DocumentAttachment>> GetSectionAttachments(int agendaItemId);

    }
}
