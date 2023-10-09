using MMA.WebApi.Shared.Interfaces.Document;
using MMA.WebApi.Shared.Models.Document;
using System;
using System.Linq;

namespace MMA.Documents.Domain.Helpers
{
    class DocumentDatabase : DocumentProvider
    {

        private IDocumentRepository _documentRepository;

        public DocumentDatabase(IDocumentRepository documentRepository)
        {
            _documentRepository = documentRepository;
        }

        public override DocumentFileModel Upload(byte[] content, Guid guid, string fileName, string contentType, Guid? parentId)
        {
            return GetFile(content, guid, fileName, contentType, parentId);
        }

        private DocumentFileModel GetFile(byte[] content, Guid guid, string fileName, string contentType, Guid? parentId)
        {
            DocumentFileModel fileValue = new DocumentFileModel();
            fileValue.Content = content;
            fileValue.Id = guid;
            fileValue.StorageType = "database";
            fileValue.UploadedOn = DateTime.Now;
            fileValue.StoragePath = null;
            fileValue.Size = content.LongLength;
            fileValue.Name = fileName;
            fileValue.MimeType = contentType;
            fileValue.ParentId = parentId;
            return fileValue;
        }

        public override DocumentFileModel Download(Guid guid)
        {
            return _documentRepository.Get().Where(x => x.Id.Equals(guid)).FirstOrDefault();
        }

        public override bool Delete(Guid guid)
        {
            throw new NotImplementedException();
        }
    }
}
