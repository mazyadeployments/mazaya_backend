using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Models.Document;
using System;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.Document
{
    public interface IDocumentRepository : IQueryableRepository<DocumentFileModel>, ICrudAsync<string, DocumentFileModel>,
        IDeletableListAsync<string>
    {
        void Delete(Guid id);
        Task DeleteRedundantImages();

    }
}
