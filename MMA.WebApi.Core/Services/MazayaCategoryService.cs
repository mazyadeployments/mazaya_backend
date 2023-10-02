using Microsoft.Extensions.Configuration;
using MMA.Documents.Domain.Helpers;
using MMA.WebApi.DataAccess.Extensions;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.Document;
using MMA.WebApi.Shared.Interfaces.Domain.Document;
using MMA.WebApi.Shared.Interfaces.Image;
using MMA.WebApi.Shared.Interfaces.MazayaCategory;
using MMA.WebApi.Shared.Interfaces.Urls;
using MMA.WebApi.Shared.Models.Category;
using MMA.WebApi.Shared.Models.Document;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.Image;
using MMA.WebApi.Shared.Models.MazayaCategory;
using MMA.WebApi.Shared.Monads;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Document.DocumentDeclares;

namespace MMA.WebApi.Core.Services
{
    public class MazayaCategoryService : IMazayaCategoryService
    {
        private readonly IMazayaCategoryRepository _mazayacategoryRepository;
        private readonly IConfiguration _configuration;
        private readonly IDocumentUrlService _documentUrlService;
        private readonly IDocumentService _documentService;
        private readonly IImageUtilsService _imageUtilsService;
        private readonly IDocumentRepository _documentRepository;

        public MazayaCategoryService(IMazayaCategoryRepository MazayaCategoryRepository, IConfiguration configuration, IDocumentUrlService documentUrlService,
            IDocumentService documentService,
            IImageUtilsService imageUtilsService,
            IDocumentRepository documentRepository)
        {
            _mazayacategoryRepository = MazayaCategoryRepository;
            _documentUrlService = documentUrlService;
            _documentService = documentService;
            _imageUtilsService = imageUtilsService;
            _documentRepository = documentRepository;
            _configuration = configuration;
        }

        public async Task<Maybe<MazayaCategoryModel>> CreateOrUpdateAsync(MazayaCategoryModel model, string userId)
        {
            if (model.ImageModel != null)
            {
                model.ImageSets = new List<ImageModel>();
                model.ImageSets.Add(model.ImageModel);

                var file = await _documentService.Download(new Guid(model.ImageModel.Id.ToUpper()));

                byte[] bytes = file.Content;

                if (bytes != null && bytes.LongLength > 0)
                {
                    Guid? parentId = null;
                    var documentFileModelImageThumbnail = await CreateThumbnailImage(
                        model,
                        parentId,
                        file,
                        bytes
                    );
                    var documentFileModelImageDetails = await CreateDetailsImage(
                        model,
                        parentId,
                        file,
                        bytes
                    );
                }
            }
            var auditVisitor = new CreateAuditVisitor(userId, DateTime.UtcNow);
            var collection = await _mazayacategoryRepository.CreateOrUpdateAsync(model, auditVisitor);
            return collection;
        }

        public async Task DeleteMazayaCategory(int id)
        {
            var mazayacategoryModel = await _mazayacategoryRepository.DeleteAsync(id);
            DocumentProvider provider = DocumentProviderFactory.GetDocumentProvider(
                DocumentProviderFactory.Operator.azureblobstorage,
                _documentRepository,
                _configuration
            );
            if (mazayacategoryModel.ImageModel != null)
            {
                provider.Delete(new Guid(mazayacategoryModel.ImageModel.Id));
            }
        }

        public async Task<IEnumerable<MazayaCategoryModel>> GetMazayaCategories()
        {
            var query = _mazayacategoryRepository.GetMazayaCategoriesNumber();
            return await Task.FromResult(query.ToList());
        }

        public async Task<PaginationListModel<MazayaCategoryModel>> GetMazayaCategoriesPage(QueryModel queryModel)
        {
            var mazayacategories = _mazayacategoryRepository.GetMazayaCategoriesNumberPage(queryModel);
            return await mazayacategories.ToPagedListAsync(queryModel.PaginationParameters.PageNumber, queryModel.PaginationParameters.PageSize);
        }

        public async Task<Maybe<MazayaCategoryModel>> GetMazayaCategory(int id)
        {
            return await _mazayacategoryRepository.Get(id);
        }

        private async Task<DocumentFileModel> CreateThumbnailImage(
            MazayaCategoryModel model,
            Guid? parentId,
            DocumentFileModel file,
            byte[] cropedImage
        )
        {
            DocumentFileModel documentFileModelImageThumbnail;
            var imageThumbnail = _imageUtilsService.Resize(cropedImage, 200, 200);
            var resultThumbnail = _documentService.Upload(
                imageThumbnail,
                Guid.NewGuid(),
                file.Name,
                file.MimeType,
                parentId
            );
            documentFileModelImageThumbnail = resultThumbnail.Result;

            model.ImageSets.Add(
                new ImageModel
                {
                    Id = documentFileModelImageThumbnail.Id.ToString(),
                    Type = OfferDocumentType.Thumbnail,
                    OriginalImageId = new Guid(model.ImageModel.Id.ToUpper()),
                    CropCoordinates = new CropCoordinates()
                    {
                        X1 = 0,
                        X2 = 0,
                        Y1 = 0,
                        Y2 = 0
                    },
                    CropNGXCoordinates = new CropCoordinates()
                    {
                        X1 = 0,
                        X2 = 0,
                        Y1 = 0,
                        Y2 = 0
                    }
                }
            );
            return documentFileModelImageThumbnail;
        }

        private async Task<DocumentFileModel> CreateDetailsImage(
            MazayaCategoryModel model,
            Guid? parentId,
            DocumentFileModel file,
            byte[] cropedImage
        )
        {
            DocumentFileModel documentFileModelImageDetails;
            var imageDetails = _imageUtilsService.Resize(cropedImage, 650, 650);
            var resultDetails = _documentService.Upload(
                imageDetails,
                Guid.NewGuid(),
                file.Name,
                file.MimeType,
                parentId
            );
            documentFileModelImageDetails = resultDetails.Result;

            model.ImageSets.Add(
                new ImageModel
                {
                    Id = documentFileModelImageDetails.Id.ToString(),
                    Type = OfferDocumentType.Large,
                    OriginalImageId = new Guid(model.ImageModel.Id.ToUpper()),
                    CropCoordinates = new CropCoordinates()
                    {
                        X1 = 0,
                        X2 = 0,
                        Y1 = 0,
                        Y2 = 0
                    },
                    CropNGXCoordinates = new CropCoordinates()
                    {
                        X1 = 0,
                        X2 = 0,
                        Y1 = 0,
                        Y2 = 0
                    }
                }
            );
            return documentFileModelImageDetails;
        }
    }
}
