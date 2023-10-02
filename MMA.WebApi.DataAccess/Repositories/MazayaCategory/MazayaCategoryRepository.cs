using Microsoft.EntityFrameworkCore;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Interfaces.MazayaCategory;
using MMA.WebApi.Shared.Models.MazayaCategory;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMA.WebApi.Shared.Models.Category;
using static MMA.WebApi.Shared.Enums.Document.DocumentDeclares;
using MMA.WebApi.Shared.Models.CategoryDocument;
using MMA.WebApi.Shared.Models.MazayacategoryDocument;
using MMA.WebApi.Shared.Models.Image;
using MMA.WebApi.DataAccess.Extensions;

namespace MMA.WebApi.DataAccess.Repositories.MazayaCategory
{
    public class MazayaCategoryRepository : BaseRepository<MazayaCategoryModel>, IMazayaCategoryRepository
    {
        public MazayaCategoryRepository(Func<MMADbContext> contexFactory) : base(contexFactory) { }

        public async Task<MazayaCategoryModel> CreateOrUpdateAsync(MazayaCategoryModel model, IVisitor<IChangeable> auditVisitor)
        {
            var context = ContextFactory();

            var mazayacategory = context.MazayaCategories.FirstOrDefault(x => x.Id == model.Id);
            if (mazayacategory == null)
                mazayacategory = new Models.MazayaCategory();

            var mazayacategoryDocuments = new List<MazayaCategoryDocumentModel>();
            if (model.ImageSets != null && model.ImageSets.Count > 0)
            {
                foreach (var imageModel in model.ImageSets)
                {
                    mazayacategoryDocuments.Add(new MazayaCategoryDocumentModel
                    {
                        DocumentId = new Guid(imageModel.Id),
                        mazayacategoryId = model.Id,
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

            model.mazayaCategoryDocuments = mazayacategoryDocuments;

            PopulateEntityModel(mazayacategory, model);

            foreach (var mazayacategoryDocument in mazayacategory.mazayaCategoryDocuments)
            {
                mazayacategoryDocument.Accept(auditVisitor);
            }

            if (model.Id == 0)
            {
                mazayacategory.status = true;
                mazayacategory.Accept(auditVisitor);
                context.Add(mazayacategory);
            }
            else
            {
                mazayacategory.image = model.image;
                mazayacategory.UpdatedOn = DateTime.UtcNow;
                context.Update(mazayacategory);
            }

            await context.SaveChangesAsync();

            return projectToMazayaCategoryModel.Compile().Invoke(mazayacategory);
        }

        public async Task<MazayaCategoryModel> DeleteAsync(int id)
        {
            var context = ContextFactory();
            var mazayacategory = await context.MazayaCategories
                        .AsNoTracking()
                        .Select(projectToMazayaCategoryModel)
                        .FirstOrDefaultAsync(x => x.Id == id);

            if (mazayacategory != null)
            {
                var data = new MMA.WebApi.DataAccess.Models.MazayaCategory();
                data.Id = mazayacategory.Id;
                if (mazayacategory.ImageModel != null)
                {
                    context.Document.Remove(new Document { Id = new Guid(mazayacategory.ImageModel.Id) });
                }
                context.Remove(data);
                context.SaveChanges();
            }
            return mazayacategory;
        }

        public async Task<MazayaCategoryModel> Get(int id)
        {
            var context = ContextFactory();

            return await context.MazayaCategories
                    .AsNoTracking()
                    .Select(projectToMazayaCategoryModel)
                    .FirstOrDefaultAsync(x => x.Id == id);
        }

        public IQueryable<MazayaCategoryModel> Get()
        {
            var context = ContextFactory();

            return context.MazayaCategories
                .Select(projectToMazayacategoryCardModel);
        }

        public IQueryable<MazayaCategoryModel> GetAllMazayaCategory()
        {
            var context = ContextFactory();

            var categories = context.MazayaCategories
                               .Select(projectToMazayaCategoryModel);
            return categories.OrderBy(c => c.Id);
        }

        public async Task<int> GetMazayaCategoriesCount()
        {
            var context = ContextFactory();

            return await context.MazayaCategories.CountAsync();
        }

        public IEnumerable<MazayaCategoryModel> GetMazayaCategoriesNumber()
        {
            var context = ContextFactory();

            var categories = context.MazayaCategories
                .AsNoTracking()
                .Select(projectToMazayaCategoryModel);
            
            return categories.OrderBy(c => c.Id);
        }

        public IQueryable<MazayaCategoryModel> GetMazayaCategoriesNumberPage(QueryModel queryModel)
        {
            var context = ContextFactory();

            var mazayacategories = context.MazayaCategories.AsNoTracking();
            var categoryModels = mazayacategories.Select(projectToMazayaCategoryModel);

            return Sort(queryModel.Sort, categoryModels);
        }

        private static IQueryable<MazayaCategoryModel> Sort(SortModel sortModel, IQueryable<MazayaCategoryModel> mazayacategories)
        {
            // Currently sorting needs to be done alphabetically
            return mazayacategories.OrderBy(c => c.Id);
        }
        protected override IQueryable<MazayaCategoryModel> GetEntities()
        {
            var context = ContextFactory();

            return context.MazayaCategories
                .Select(projectToMazayaCategoryModel);
        }

        private Expression<Func<MMA.WebApi.DataAccess.Models.MazayaCategory, MazayaCategoryModel>> projectToMazayaCategoryModel = data =>
        new MazayaCategoryModel()
        {
           

            Id = data.Id,
            Name = data.Name,
            CreatedBy = data.CreatedBy,
            CreatedOn = data.CreatedOn,
            UpdatedBy = data.UpdatedBy,
            UpdatedOn = data.UpdatedOn,
            description = data.description,
            facilities = data.facilities,
            ImageModel = data.mazayaCategoryDocuments.Where(cd => cd.mazayaCategoryId == data.Id).Select
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

        private Expression<Func<MMA.WebApi.DataAccess.Models.MazayaCategory, MazayaCategoryModel>> projectToMazayacategoryCardModel = data =>
           new MazayaCategoryModel()
           {
               Id = data.Id,
               Name = data.Name,
               UpdatedOn = data.UpdatedOn,
               UpdatedBy = data.UpdatedBy,
               CreatedBy = data.CreatedBy,
               CreatedOn = data.CreatedOn,
               description = data.description,
        facilities  = data.facilities,
               ImageUrls = new ImageUrlsModel
               {
                   Large = data.mazayaCategoryDocuments.Where(x => x.mazayaCategoryId == data.Id && x.Type == OfferDocumentType.Large).Select(x => x.DocumentId).FirstOrDefault().ToString(),
                   Original = data.mazayaCategoryDocuments.Where(x => x.mazayaCategoryId == data.Id && x.Type == OfferDocumentType.Original).Select(x => x.DocumentId).FirstOrDefault().ToString(),
                   Thumbnail = data.mazayaCategoryDocuments.Where(x => x.mazayaCategoryId == data.Id && x.Type == OfferDocumentType.Thumbnail).Select(x => x.DocumentId).FirstOrDefault().ToString()
               },
           };

        private void PopulateEntityModel(MMA.WebApi.DataAccess.Models.MazayaCategory data, MazayaCategoryModel model)
        {
            data.Id = model.Id;
            data.Name = model.Name;
            data.description = model.description;
            data.facilities  = model.facilities;
            data.mazayaCategoryDocuments = model.mazayaCategoryDocuments.Select(od => new MazayacategoryDocument
            {
                DocumentId = od.DocumentId,
                mazayaCategoryId = model.Id,
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

    }
}
