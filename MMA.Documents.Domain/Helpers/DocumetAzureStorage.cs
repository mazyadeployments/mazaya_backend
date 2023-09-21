using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using MMA.WebApi.Shared.Models.Document;
using System;
using System.IO;

namespace MMA.Documents.Domain.Helpers
{
    public class DocumetAzureStorage : DocumentProvider
    {
        private readonly IConfiguration _configuration;

        private readonly Lazy<BlobContainerClient> _containerClient;

        public const string StorageType = "azureblobstorage";

        public DocumetAzureStorage(IConfiguration configuration)
        {
            _configuration = configuration;

            _containerClient = new Lazy<BlobContainerClient>(() => GetBlobContainerClient(_configuration));
        }

        public override DocumentFileModel Upload(byte[] content, Guid guid, string fileName, string contentType, Guid? parentId)
        {
            var blobName = parentId != null ? $"{parentId}/{guid}" : guid.ToString();

            using (var stream = new MemoryStream(content))
            {
                BlobClient blob = _containerClient.Value.GetBlobClient(blobName);

                if (!string.IsNullOrEmpty(contentType) && contentType.StartsWith("application"))
                {
                    var result = blob.Upload(stream, new BlobHttpHeaders { ContentType = contentType });
                }
                else if (!string.IsNullOrEmpty(contentType) && contentType.StartsWith("video"))
                {
                    var result = blob.Upload(stream, new BlobHttpHeaders { ContentType = contentType });
                }
                else
                {
                    var result = blob.Upload(stream, new BlobHttpHeaders { ContentType = "image/jpeg" });
                }
            }

            return new DocumentFileModel()
            {
                Content = content,
                Id = guid,
                StorageType = StorageType,
                UploadedOn = DateTime.Now,
                StoragePath = $"{_containerClient.Value.Name}/{blobName}",
                Size = content.LongLength,
                Name = fileName,
                MimeType = contentType,
                ParentId = parentId,
            };
        }

        public override DocumentFileModel Download(Guid guid)
        {
            var blobClient = _containerClient.Value.GetBlobClient(guid.ToString());

            if (!blobClient.ExistsAsync().GetAwaiter().GetResult())
            {
                throw new Exception($"Unable to download file from azure storeage. Specified blob {guid} does not exists.");
            }

            BlobDownloadInfo download = blobClient.DownloadAsync()
                .GetAwaiter()
                .GetResult();

            byte[] content = null;

            using (var stream = new MemoryStream())
            {
                download.Content.CopyToAsync(stream)
                    .GetAwaiter()
                    .GetResult();

                stream.Close();

                content = stream.GetBuffer();
            }

            return new DocumentFileModel
            {
                Id = guid,
                StoragePath = $"{_containerClient.Value.Name}/{guid}",
                Size = content.Length,
                StorageType = StorageType,
                Content = content,
                MimeType = download.ContentType
            };
        }


        public override bool Delete(Guid guid)
        {
            var blobClient = _containerClient.Value.GetBlobClient(guid.ToString());

            if (blobClient.ExistsAsync().GetAwaiter().GetResult())
            {
                // TODO: CHECK IF THIS ISN'T INTERFERING WITH OTHER IMPLEMENTATION
                //throw new Exception($"Unable to download file from azure storeage. Specified blob {guid} does not exists.");

                var isBlobDeleted = blobClient.DeleteIfExists();

                return isBlobDeleted;
            }

            return false;
        }

        public BlobContainerClient GetBlobContainerClient(IConfiguration config)
        {
            var connectionString = config.GetConnectionString("AzureBlobStorage");

            var containerName = config.GetSection("AzureBlobStorage")
                .GetValue<string>("ContainerName");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new Exception("Azure blob storage connection string is not set");
            }

            if (string.IsNullOrWhiteSpace(containerName))
            {
                throw new Exception("Azure blob storage container name is not set");
            }

            var blobServiceClient = new BlobServiceClient(connectionString);

            return blobServiceClient.GetBlobContainerClient(containerName);
        }
    }
}
