using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Models.Document;
using System;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.Domain.Document
{
    public interface IDocumentService : IQueryableRepository<DocumentFileModel>
    {
        Guid GetLink();
        Task<DocumentFileModel> Upload(byte[] content, Guid guid, string fileName, string contentType, Guid? parentId);
        Task<DocumentFileModel> Download(Guid guid);
        Task Delete(Guid guid);
        Task DeleteRedundantImages();
    }
}
