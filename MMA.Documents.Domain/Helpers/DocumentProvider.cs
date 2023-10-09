using MMA.WebApi.Shared.Models.Document;
using System;

namespace MMA.Documents.Domain.Helpers
{
    public abstract class DocumentProvider
    {
        public abstract DocumentFileModel Upload(byte[] content, Guid guid, string fileName, string contentType, Guid? parentId);
        public abstract DocumentFileModel Download(Guid guid);
        public abstract bool Delete(Guid guid);
    }
}
