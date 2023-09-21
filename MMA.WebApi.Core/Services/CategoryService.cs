using Microsoft.Extensions.Configuration;
using MMA.Documents.Domain.Helpers;
using MMA.WebApi.DataAccess.Extensions;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.Categories;
using MMA.WebApi.Shared.Interfaces.Document;
using MMA.WebApi.Shared.Interfaces.Domain.Document;
using MMA.WebApi.Shared.Interfaces.Image;
using MMA.WebApi.Shared.Interfaces.Urls;
using MMA.WebApi.Shared.Models.Category;
using MMA.WebApi.Shared.Models.Document;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.Image;
using MMA.WebApi.Shared.Monads;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Document.DocumentDeclares;

namespace MMA.WebApi.Core.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IDocumentUrlService _documentUrlService;
        private readonly IDocumentService _documentService;
        private readonly IImageUtilsService _imageUtilsService;
        private readonly IDocumentRepository _documentRepository;
        private readonly IConfiguration _configuration;

        public CategoryService(
            ICategoryRepository CategoryRepository,
            IDocumentUrlService documentUrlService,
            IDocumentService documentService,
            IImageUtilsService imageUtilsService,
            IDocumentRepository documentRepository,
            IConfiguration configuration
        )
        {
            _categoryRepository = CategoryRepository;
            _documentUrlService = documentUrlService;
            _documentService = documentService;
            _imageUtilsService = imageUtilsService;
            _documentRepository = documentRepository;
            _configuration = configuration;
        }

        public async Task<IEnumerable<CategoryModel>> GetCategories()
        {
            var query = _categoryRepository.GetCategoriesWithOfferNumber();
            return await Task.FromResult(query.ToList());
        }

        public async Task<PaginationListModel<CategoryModel>> GetCategoriesPage(
            QueryModel queryModel
        )
        {
            var categories = _categoryRepository.GetCategoriesWithOfferNumberPage(queryModel);
            return await categories.ToPagedListAsync(
                queryModel.PaginationParameters.PageNumber,
                queryModel.PaginationParameters.PageSize
            );
        }

        public async Task<Maybe<CategoryModel>> GetCategory(int id)
        {
            return await _categoryRepository.Get(id);
        }

        public async Task<Maybe<CategoryModel>> CreateOrUpdateAsync(
            CategoryModel model,
            string userId
        )
        {
            if (model.Image != null)
            {
                model.ImageSets = new List<ImageModel>();
                model.ImageSets.Add(model.Image);

                var file = await _documentService.Download(new Guid(model.Image.Id.ToUpper()));

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
            var collection = await _categoryRepository.CreateOrUpdateAsync(model, auditVisitor);
            return collection;
        }

        private async Task<DocumentFileModel> CreateDetailsImage(
            CategoryModel model,
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
                    OriginalImageId = new Guid(model.Image.Id.ToUpper()),
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

        private async Task<DocumentFileModel> CreateThumbnailImage(
            CategoryModel model,
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
                    OriginalImageId = new Guid(model.Image.Id.ToUpper()),
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

        public async Task DeleteCategory(int id)
        {
            var categoryModel = await _categoryRepository.DeleteAsync(id);
            DocumentProvider provider = DocumentProviderFactory.GetDocumentProvider(
                DocumentProviderFactory.Operator.azureblobstorage,
                _documentRepository,
                _configuration
            );
            if (categoryModel.Image != null)
            {
                provider.Delete(new Guid(categoryModel.Image.Id));
            }
        }
    }
}
