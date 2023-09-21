using Microsoft.Extensions.Configuration;
using MMA.WebApi.Shared.Interfaces.Document;
using MMA.WebApi.Shared.Models.Document;
using System;
using System.IO;
using System.Linq;

namespace MMA.Documents.Domain.Helpers
{
    public class DocumentFileSystem : DocumentProvider
    {
        private IDocumentRepository _documentRepository;
        private IConfiguration _configuration;

        public DocumentFileSystem(IDocumentRepository documentRepository, IConfiguration configuration)
        {
            _documentRepository = documentRepository;
            _configuration = configuration;
        }

        public override bool Delete(Guid guid)
        {
            throw new NotImplementedException();
        }

        public override DocumentFileModel Download(Guid guid)
        {
            var file = _documentRepository.Get().Where(x => x.Id.Equals(guid)).FirstOrDefault();

            if (guid != file.Id || file.StoragePath == null)
                throw new InvalidOperationException("Invalid file.");

            var path = file.StoragePath;

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                stream.CopyTo(memory);
            }
            memory.Position = 0;

            byte[] content;

            using (BinaryReader br = new BinaryReader(memory))
            {
                try
                {
                    content = br.ReadBytes((int)memory.Length);
                }
                catch (Exception)
                {

                    throw;
                }
            }
            if (file.MimeType.Contains("image/svg"))
            {
                var fileContent = new DocumentFileModel
                {
                    Content = content.ToArray(),
                    Name = file.Name + ".svg",
                    MimeType = file.MimeType
                };
                return fileContent;
            }
            else
            {
                var fileContent = new DocumentFileModel
                {
                    Content = content.ToArray(),
                    Name = file.Name,
                    MimeType = file.MimeType
                };
                return fileContent;
            }
        }

        public override DocumentFileModel Upload(byte[] content, Guid guid, string fileName, string contentType, Guid? parentId)
        {
            return GetFiles(content, guid, fileName, contentType, parentId);
        }

        private DocumentFileModel GetFiles(byte[] content, Guid guid, string fileName, string contentType, Guid? parentId)
        {
            var fileValue = new DocumentFileModel();
            fileValue.Id = guid;
            string path = String.Empty;
            if (content.Length > 0)
            {

                using (var reader = new MemoryStream())
                {

                    try
                    {
                        path = this.MakeFilePath(contentType, guid);
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    File.WriteAllBytes(path, content);
                    fileValue.Content = null;
                    fileValue.MimeType = contentType;
                    fileValue.StoragePath = path;
                    fileValue.Name = fileName;
                    fileValue.Size = content.LongLength;
                    fileValue.StorageType = "filesys";
                    fileValue.UploadedOn = DateTime.Now;
                    fileValue.ParentId = parentId;

                }
            }
            else
                throw new InvalidOperationException("Invalid file.");

            return fileValue;
        }



        private string MakeFilePath(string contentType, Guid fileId)
        {
            string uploadFolder = _configuration.GetSection("Documents").GetSection("UploadFolder").Value;

            if (string.IsNullOrEmpty(uploadFolder))
            {
                uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "upload");
            }
            uploadFolder = Path.Combine(uploadFolder, DateTime.Today.ToString("yyyy-MM"));

            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            };
            string path = string.Empty;
            if (contentType.Contains("image/svg"))
            {
                path = Path.Combine(uploadFolder,
                    fileId.ToString() + ".svg");
            }
            else if (contentType.Contains("image/png"))
            {
                path = Path.Combine(uploadFolder,
                   fileId.ToString() + ".png");
            }
            else
            {
                path = Path.Combine(uploadFolder,
                    fileId.ToString());
            }

            return path;
        }
    }
}
