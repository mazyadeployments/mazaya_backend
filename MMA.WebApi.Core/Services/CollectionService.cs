using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MMA.Documents.Domain.Helpers;
using MMA.WebApi.DataAccess.Extensions;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.Collection;
using MMA.WebApi.Shared.Interfaces.Document;
using MMA.WebApi.Shared.Interfaces.Domain.Document;
using MMA.WebApi.Shared.Interfaces.Image;
using MMA.WebApi.Shared.Models.Collection;
using MMA.WebApi.Shared.Models.Document;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.Image;
using MMA.WebApi.Shared.Monads;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Document.DocumentDeclares;

namespace MMA.WebApi.Core.Services
{
    public class CollectionService : ICollectionService
    {
        private readonly ICollectionRepository _collectionRepository;
        private readonly IDocumentService _documentService;
        private readonly IImageUtilsService _imageUtilsService;
        private readonly IDocumentRepository _documentRepository;
        private readonly IConfiguration _configuration;

        public CollectionService(ICollectionRepository collectionRepository, IDocumentService documentService,
                                 IImageUtilsService imageUtilsService,
                                 IDocumentRepository documentRepository, IConfiguration configuration)
        {
            _collectionRepository = collectionRepository;
            _documentService = documentService;
            _imageUtilsService = imageUtilsService;
            _documentRepository = documentRepository;
            _configuration = configuration;
        }

        public async Task<IEnumerable<CollectionModel>> GetCollections()
        {
            return await _collectionRepository.Get().ToListAsync();
        }

        public async Task<PaginationListModel<CollectionModel>> GetCollectionsPage(QueryModel queryModel)
        {
            var collections = _collectionRepository.GetCollectionsPage(queryModel);
            return await collections.ToPagedListAsync(queryModel.PaginationParameters.PageNumber, queryModel.PaginationParameters.PageSize);
        }

        public async Task<Maybe<CollectionModel>> GetCollection(int id)
        {
            return await _collectionRepository.Get(id);
        }

        public async Task<Maybe<CollectionModel>> CreateCollection(CollectionModel model, string userId)
        {
            var auditVisitor = new CreateAuditVisitor(userId, DateTime.UtcNow);
            var collection = await _collectionRepository.CreateOrUpdateAsync(model, auditVisitor);
            return collection;
        }

        /// <summary>
        /// Upload of images will be run in background (Max automatic retry is set to 1)
        /// </summary>
        /// <param name="croppedImages"></param>
        /// <returns></returns>
        [AutomaticRetry(Attempts = 0)]
        public async Task CreateImages(List<ImageModel> croppedImages)
        {
            if (croppedImages != null && croppedImages.Count > 0)
            {
                foreach (var image in croppedImages)
                {
                    var file = await _documentService.Download(image.OriginalImageId);

                    byte[] bytes = file.Content;
                    byte[] processedImage = _imageUtilsService.ProcessImageForCollection(bytes);

                    var cropedImage = image.CropCoordinates != null ?
                                      _imageUtilsService.CropImage(processedImage,
                                                                   Convert.ToInt32(image.CropCoordinates.X1),
                                                                   Convert.ToInt32(image.CropCoordinates.Y1),
                                                                   Convert.ToInt32(image.CropCoordinates.X2 - image.CropCoordinates.X1),
                                                                   Convert.ToInt32(image.CropCoordinates.Y2 - image.CropCoordinates.Y1)) : processedImage;

                    if (cropedImage != null && cropedImage.LongLength > 0)
                    {
                        //Create Thumbnail Image
                        if (image.Type == OfferDocumentType.Thumbnail)
                            await CreateThumbnailImage(image, null, file, cropedImage);

                        //Create Details Image
                        if (image.Type == OfferDocumentType.Large)
                            await CreateDetailsImage(image, null, file, cropedImage);
                    }
                }
            }
        }
        private async Task<DocumentFileModel> CreateDetailsImage(ImageModel image, Guid? parentId, DocumentFileModel file, byte[] cropedImage)
        {
            var imageDetails = _imageUtilsService.Resize(cropedImage, 650, 520);
            var resultDetails = await _documentService.Upload(imageDetails, Guid.Parse(image.Id), file.Name, file.MimeType, parentId);

            return resultDetails;
        }

        private async Task<DocumentFileModel> CreateThumbnailImage(ImageModel image, Guid? parentId, DocumentFileModel file, byte[] cropedImage)
        {
            var imageThumbnail = _imageUtilsService.Resize(cropedImage, 200, 160);
            var resultThumbnail = await _documentService.Upload(imageThumbnail, Guid.Parse(image.Id), file.Name, file.MimeType, parentId);

            return resultThumbnail;
        }

        public async Task DeleteCollection(int id)
        {
            var collectionModel = await _collectionRepository.DeleteAsync(id);
            if (collectionModel.Image != null)
            {
                DocumentProvider provider = DocumentProviderFactory.GetDocumentProvider(DocumentProviderFactory.Operator.azureblobstorage, _documentRepository, _configuration);
                provider.Delete(new Guid(collectionModel.Image.Id));
            }
        }
    }
}
