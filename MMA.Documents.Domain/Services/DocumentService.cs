using Microsoft.Extensions.Configuration;
using MMA.Documents.Domain.Helpers;
using MMA.WebApi.Shared.Interfaces.Document;
using MMA.WebApi.Shared.Interfaces.Domain.Document;
using MMA.WebApi.Shared.Interfaces.OfferDocuments;
using MMA.WebApi.Shared.Models.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMA.Documents.Domain.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IConfiguration _configuration;
        private readonly IOfferDocumentService _offerDocumentService;

        private readonly Dictionary<string, DocumentProviderFactory.Operator> _supportedProviderOptions = new Dictionary<string, DocumentProviderFactory.Operator>()
        {
            ["filesys"] = DocumentProviderFactory.Operator.filesys,
            ["azureblobstorage"] = DocumentProviderFactory.Operator.azureblobstorage,
            ["database"] = DocumentProviderFactory.Operator.database,
        };

        public DocumentService(IDocumentRepository documentRepository, IConfiguration configuration, IOfferDocumentService offerDocumentService)
        {
            _documentRepository = documentRepository;
            _configuration = configuration;
            _offerDocumentService = offerDocumentService;
        }

        public Guid GetLink()
        {
            Guid guid = Guid.NewGuid();

            var file = new DocumentFileModel
            {
                Id = guid
            };

            return guid;
        }

        public async Task<DocumentFileModel> Download(Guid guid)
        {
            var document = _documentRepository.Get()
                .Where(d => d.Id == guid)
                .Select(d => new { d.Id, d.StorageType, d.Name, d.MimeType })
                .FirstOrDefault();

            if (document == null)
            {
                throw new Exception($"Document with specified id {document.Id} does not exist");
            }

            if (string.IsNullOrWhiteSpace(document.StorageType) || !_supportedProviderOptions.Keys.Contains(document.StorageType))
            {
                throw new Exception($"Storage type not supported {document.StorageType ?? "unknown"} for document {document.Id}");
            }

            var option = _supportedProviderOptions[document.StorageType];

            DocumentProvider provider = DocumentProviderFactory.GetDocumentProvider(option, _documentRepository, _configuration);

            var doc = await Task.FromResult(provider.Download(guid));
            doc.Name = document.Name;
            doc.MimeType = document.MimeType;

            return doc;
        }

        public async Task Delete(Guid guid)
        {
            var document = _documentRepository.Get()
                .Where(d => d.Id == guid)
                .Select(d => new { d.Id, d.StorageType })
                .FirstOrDefault();

            //Get list of all images that have same OriginalId (original + thumbnail + large)
            List<Guid> offerDocumentModelsWithCroppedImages = _offerDocumentService.GetOfferDocumentsByOriginalId(guid).ToList();

            if (document == null)
            {
                throw new Exception($"Document with specified id {document.Id} does not exist");
            }

            if (string.IsNullOrWhiteSpace(document.StorageType) || !_supportedProviderOptions.Keys.Contains(document.StorageType))
            {
                throw new Exception($"Storage type not supported {document.StorageType ?? "unknown"} for document {document.Id}");
            }

            var option = _supportedProviderOptions[document.StorageType];

            DocumentProvider provider = DocumentProviderFactory.GetDocumentProvider(option, _documentRepository, _configuration);

            //If user is removing image before saving the offer, so we only have one image saved to Azure
            if (offerDocumentModelsWithCroppedImages.Count == 0)
            {
                //Remove that image from Azure
                provider.Delete(guid);
            }
            else
            {
                //If user is editing already saved image, delete all 3 images from offer document table, document table and azure
                foreach (var imageGuid in offerDocumentModelsWithCroppedImages)
                {
                    await _offerDocumentService.DeleteOfferDocument(guid);
                }

                foreach (var imageGuid in offerDocumentModelsWithCroppedImages)
                {
                    _documentRepository.Delete(imageGuid);
                }


                foreach (var imageGuid in offerDocumentModelsWithCroppedImages)
                {
                    provider.Delete(imageGuid);
                }
            }

        }

        public async Task<DocumentFileModel> Upload(byte[] content, Guid guid, string fileName, string contentType, Guid? parentId)
        {
            //here you need to change document provider
            var documentProviderOperator = DocumentProviderFactory.Operator.azureblobstorage;
            DocumentProvider provider = DocumentProviderFactory.GetDocumentProvider(documentProviderOperator, _documentRepository, _configuration);
            var result = await Task.FromResult(provider.Upload(content, guid, fileName, contentType, parentId));

            if (documentProviderOperator != DocumentProviderFactory.Operator.database)
                result.Content = null;

            await SaveAsync(result);
            return result;
        }

        public IQueryable<DocumentFileModel> Get()
        {
            return _documentRepository.Get();
        }

        public async Task<DocumentFileModel> SaveAsync(DocumentFileModel data)
        {
            var helpIdExists = _documentRepository.Get().FirstOrDefault(x => x.Id.Equals(data.Id));
            var id = data.Id.ToString();
            if (helpIdExists != null)
            {
                id = await _documentRepository.EditAsync(data);
            }
            else
            {
                id = await _documentRepository.InsertAsync(data);
            }
            return data;
        }

        public async Task DeleteRedundantImages()
        {
            await _documentRepository.DeleteRedundantImages();
        }
    }
}
