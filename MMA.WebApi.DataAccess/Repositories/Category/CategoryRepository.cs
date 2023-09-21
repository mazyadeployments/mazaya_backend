using Microsoft.EntityFrameworkCore;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.Categories;
using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Models.Category;
using MMA.WebApi.Shared.Models.CategoryDocument;
using MMA.WebApi.Shared.Models.Image;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Declares;
using static MMA.WebApi.Shared.Enums.Document.DocumentDeclares;

namespace MMA.WebApi.DataAccess.Categories
{
    public class CategoryRepository : BaseRepository<CategoryModel>, ICategoryRepository
    {
        public CategoryRepository(Func<MMADbContext> contexFactory) : base(contexFactory) { }

        public IQueryable<CategoryModel> Get()
        {
            var context = ContextFactory();

            return context.Category
                .AsNoTracking()
                .Select(projectToCategoryModel);
        }
        public IEnumerable<CategoryModel> GetCategoriesWithOfferNumber()
        {
            var context = ContextFactory();

            var categories = context.Category
                .AsNoTracking()
                .Select(projectToCategoryCardModel);

            return categories.OrderBy(c => c.Title);
        }
        public IQueryable<CategoryModel> GetAllCategoty()
        {
            var context = ContextFactory();

            var categories = context.Category
                               .Select(projectToCategoryModel);
            return categories.OrderBy(c => c.Title);
        }
        public IQueryable<CategoryModel> GetCategoriesWithOfferNumberPage(QueryModel queryModel)
        {
            var context = ContextFactory();

            var categories = context.Category.AsNoTracking();

            var filteredCategory = Filter(categories, queryModel);
            var categoryModels = filteredCategory.Select(projectToCategoryCardModel);

            return Sort(queryModel.Sort, categoryModels);
        }

        private static IQueryable<Category> Filter(IQueryable<Category> categories, QueryModel queryModel)
        {
            var filteredCategories = categories
                                     .Where(category => category.Title.ToLower().Contains(queryModel.Filter.Keyword.Trim().ToLower()));

            return filteredCategories;
        }
        private static IQueryable<CategoryModel> Sort(SortModel sortModel, IQueryable<CategoryModel> categories)
        {
            // Currently sorting needs to be done alphabetically
            return categories.OrderBy(c => c.Title);
        }

        protected override IQueryable<CategoryModel> GetEntities()
        {
            var context = ContextFactory();

            return context.Category
                .Select(projectToCategoryModel);
        }

        public async Task<CategoryModel> Get(int id)
        {
            var context = ContextFactory();

            return await context.Category
                    .AsNoTracking()
                    .Select(projectToCategoryModel)
                    .FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<CategoryModel> DeleteAsync(int id)
        {
            var context = ContextFactory();
            var category = await context.Category
                        .AsNoTracking()
                        .Select(projectToCategoryModel)
                        .FirstOrDefaultAsync(x => x.Id == id);

            if (category != null)
            {
                var data = new Category();
                data.Id = category.Id;
                if (category.Image != null)
                {
                    context.Document.Remove(new Document { Id = new Guid(category.Image.Id) });
                }
                context.Remove(data);
                context.SaveChanges();
            }
            return category;
        }

        public async Task<CategoryModel> CreateOrUpdateAsync(CategoryModel model, IVisitor<IChangeable> auditVisitor)
        {
            var context = ContextFactory();

            var category = context.Category
                                    .Include(o => o.CategoryDocuments)
                                    .Include(o => o.OfferCategories)
                                    .FirstOrDefault(x => x.Id == model.Id);

            if (category == null)
                category = new Category();

            if (model.OffersIds == null)
            {
                model.OffersIds = new List<int>();
            }

            var categoryDocuments = new List<CategoryDocumentModel>();

            if (model.ImageSets != null && model.ImageSets.Count > 0)
            {
                foreach (var imageModel in model.ImageSets)
                {
                    categoryDocuments.Add(new CategoryDocumentModel
                    {
                        DocumentId = new Guid(imageModel.Id),
                        CategoryId = model.Id,
                        Type = imageModel.Type,
                        OriginalImageId = imageModel.OriginalImageId,
                        X1 = imageModel.CropCoordinates.X1,
                        X2 = imageModel.CropCoordinates.X2,
                        Y1 = imageModel.CropCoordinates.Y1,
                        Y2 = imageModel.CropCoordinates.Y2,
                        cropX1 = imageModel.CropNGXCoordinates.X1,
                        cropX2 = imageModel.CropNGXCoordinates.X2,
                        cropY1 = imageModel.CropNGXCoordinates.Y1,
                        cropY2 = imageModel.CropNGXCoordinates.Y2,
                    });
                }
            }

            model.CategoryDocuments = categoryDocuments;

            PopulateEntityModel(category, model);

            foreach (var categoryDocument in category.CategoryDocuments)
            {
                categoryDocument.Accept(auditVisitor);
            }

            if (model.Id == 0)
            {
                category.Accept(auditVisitor);
                context.Add(category);
            }
            else
            {
                category.UpdatedOn = DateTime.UtcNow;
                context.Update(category);
            }

            await context.SaveChangesAsync();

            return projectToCategoryModel.Compile().Invoke(category);
        }

        private Expression<Func<Category, CategoryModel>> projectToCategoryModel = data =>
           new CategoryModel()
           {
               Id = data.Id,
               Title = data.Title,
               Description = data.Description,
               OffersIds = data.OfferCategories.Select(x => x.OfferId).ToList(),
               CreatedBy = data.CreatedBy,
               CreatedOn = data.CreatedOn,
               UpdatedBy = data.UpdatedBy,
               UpdatedOn = data.UpdatedOn,
               Image = data.CategoryDocuments.Where(cd => cd.CategoryId == data.Id).Select
                                           (od => new ImageModel
                                           {
                                               Id = !string.IsNullOrEmpty(od.DocumentId.ToString()) ? od.DocumentId.ToString() : string.Empty,
                                               Type = od.Type,
                                               OriginalImageId = od.OriginalImageId,
                                               CropCoordinates = new CropCoordinates
                                               {
                                                   X1 = od.X1,
                                                   X2 = od.X2,
                                                   Y1 = od.Y1,
                                                   Y2 = od.Y2
                                               },
                                               CropNGXCoordinates = new CropCoordinates
                                               {
                                                   X1 = od.cropX1,
                                                   X2 = od.cropX2,
                                                   Y1 = od.cropY1,
                                                   Y2 = od.cropY2
                                               },
                                           }).FirstOrDefault(),
           };

        private Expression<Func<Category, CategoryModel>> projectToCategoryCardModel = data =>
           new CategoryModel()
           {
               Id = data.Id,
               Title = data.Title,
               Description = data.Description,
               UpdatedOn = data.UpdatedOn,
               UpdatedBy = data.UpdatedBy,
               CreatedBy = data.CreatedBy,
               CreatedOn = data.CreatedOn,
               ImageUrls = new ImageUrlsModel
               {
                   Large = data.CategoryDocuments.Where(x => x.CategoryId == data.Id && x.Type == OfferDocumentType.Large).Select(x => x.DocumentId).FirstOrDefault().ToString(),
                   Original = data.CategoryDocuments.Where(x => x.CategoryId == data.Id && x.Type == OfferDocumentType.Original).Select(x => x.DocumentId).FirstOrDefault().ToString(),
                   Thumbnail = data.CategoryDocuments.Where(x => x.CategoryId == data.Id && x.Type == OfferDocumentType.Thumbnail).Select(x => x.DocumentId).FirstOrDefault().ToString()
               },
               NumberOfOffers = data.OfferCategories.Count(oc => oc.Offer.Status == OfferStatus.Approved.ToString() && oc.Offer.ValidFrom < DateTime.UtcNow && oc.Offer.ValidUntil > DateTime.UtcNow && oc.CategoryId == data.Id),
           };

        private void PopulateEntityModel(Category data, CategoryModel model)
        {
            data.Id = model.Id;
            data.Title = model.Title;
            data.Description = model.Description;
            if (model.OffersIds != null)
            {
                data.OfferCategories = model.OffersIds.Select(offerId => new OfferCategory { OfferId = offerId, CategoryId = model.Id }).ToList();
            }
            data.CategoryDocuments = model.CategoryDocuments.Select(od => new CategoryDocument
            {
                DocumentId = od.DocumentId,
                CategoryId = model.Id,
                Type = od.Type.ToString() == "0" ? OfferDocumentType.Original : od.Type,
                OriginalImageId = od.OriginalImageId == Guid.Empty ? od.DocumentId : od.OriginalImageId,
                X1 = od.X1,
                X2 = od.X2,
                Y1 = od.Y1,
                Y2 = od.Y2,
                cropX1 = od.cropX1,
                cropX2 = od.cropX2,
                cropY1 = od.cropY1,
                cropY2 = od.cropY2
            }).ToList();
        }

        public async Task<int> GetCategoriesCount()
        {
            var context = ContextFactory();

            return await context.Category.CountAsync();
        }
    }
}
