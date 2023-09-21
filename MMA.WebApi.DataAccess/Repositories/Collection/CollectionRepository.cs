using Microsoft.EntityFrameworkCore;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.Collection;
using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Models.Collection;
using MMA.WebApi.Shared.Models.CollectionDocument;
using MMA.WebApi.Shared.Models.Image;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Document.DocumentDeclares;

namespace MMA.WebApi.DataAccess.Collections
{
    public class CollectionRepository : BaseRepository<CollectionModel>, ICollectionRepository
    {
        public CollectionRepository(Func<MMADbContext> contexFactory) : base(contexFactory) { }

        public IQueryable<CollectionModel> Get()
        {
            var context = ContextFactory();

            return context.Collection
                .Select(projectToCollectionCardModel);
        }

        protected override IQueryable<CollectionModel> GetEntities()
        {
            var context = ContextFactory();

            return context.Collection
                .Select(projectToCollectionModel);
        }
        public IQueryable<CollectionModel> GetCollectionsPage(QueryModel queryModel)
        {
            var context = ContextFactory();
            IQueryable<Collection> filteredCollections = null;
            IQueryable<CollectionModel> collectionModels = null;

            var collections = context.Collection;

            filteredCollections = Filter(collections, queryModel);
            collectionModels = filteredCollections.Select(projectToCollectionCardModel);

            return Sort(queryModel.Sort, collectionModels);
        }

        private static IQueryable<Collection> Filter(IQueryable<Collection> collections, QueryModel queryModel)
        {
            var filteredCollections = collections
                                     .Where(collection => collection.Title.ToLower().Contains(queryModel.Filter.Keyword.Trim().ToLower()));

            return filteredCollections;
        }
        private static IQueryable<CollectionModel> Sort(SortModel sortModel, IQueryable<CollectionModel> collection)
        {
            if (sortModel.Type == "date")
            {
                if (sortModel.Order == Order.DESC)
                {
                    return collection.OrderByDescending(x => x.UpdatedOn);
                }
                else
                {
                    return collection.OrderBy(x => x.UpdatedOn);
                }
            }
            else
            {
                return collection.OrderByDescending(x => x.UpdatedOn);
            }
        }

        public async Task<CollectionModel> Get(int id)
        {
            var context = ContextFactory();

            return await context.Collection
                    .Select(projectToCollectionModel)
                    .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<CollectionModel> CreateOrUpdateAsync(CollectionModel model, IVisitor<IChangeable> auditVisitor)
        {
            var context = ContextFactory();

            var collection = context.Collection
                                    .Include(o => o.CollectionDocuments)
                                    .Include(o => o.OfferCollections)
                                    .FirstOrDefault(x => x.Id == model.Id);

            if (collection == null)
                collection = new Collection();

            if (model.OffersIds == null)
            {
                model.OffersIds = new List<int>();
            }

            var collectionDocuments = new List<CollectionDocumentModel>();


            if (model.ImageSets != null && model.ImageSets.Count > 0)
            {
                foreach (var imageModel in model.ImageSets)
                {
                    collectionDocuments.Add(new CollectionDocumentModel
                    {
                        DocumentId = new Guid(imageModel.Id),
                        CollectionId = model.Id,
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

            model.CollectionDocuments = collectionDocuments;

            PopulateEntityModel(collection, model);

            foreach (var collectionDocument in collection.CollectionDocuments)
            {
                collectionDocument.Accept(auditVisitor);
                if (!await context.Document.AnyAsync(d => d.Id == collectionDocument.DocumentId))
                    context.Document.Add(new Document()
                    {
                        Id = collectionDocument.DocumentId,
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow
                    });
            }

            if (model.Id == 0)
            {
                collection.Accept(auditVisitor);
                context.Add(collection);
            }
            else
            {
                collection.UpdatedOn = DateTime.UtcNow;
                context.Update(collection);
            }

            await context.SaveChangesAsync();

            if (collection.OfferCollections != null && collection.OfferCollections.Count > 0)
            {
                var offerIds = collection.OfferCollections.Select(x => x.OfferId);
                await UpdateOffer(context, offerIds);
            }

            return projectToCollectionModel.Compile().Invoke(collection);
        }

        private Expression<Func<Collection, CollectionModel>> projectToCollectionModel = data =>
           new CollectionModel()
           {
               ValidUntil = data.ValidUntil,
               ValidFrom = data.ValidFrom,
               Title = data.Title,
               Id = data.Id,
               Description = data.Description,
               OffersIds = data.OfferCollections.Select(x => x.OfferId).ToList(),
               CreatedBy = data.CreatedBy,
               CreatedOn = data.CreatedOn,
               UpdatedBy = data.UpdatedBy,
               UpdatedOn = data.UpdatedOn,
               Image = data.CollectionDocuments.Where(cd => cd.CollectionId == data.Id).Select
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
               HomeVisible = data.HomeVisible
           };

        private Expression<Func<Collection, CollectionModel>> projectToCollectionCardModel = data =>
           new CollectionModel()
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
                   Large = data.CollectionDocuments.Where(x => x.CollectionId == data.Id && x.Type == OfferDocumentType.Large).Select(x => x.DocumentId).FirstOrDefault().ToString(),
                   Original = data.CollectionDocuments.Where(x => x.CollectionId == data.Id && x.Type == OfferDocumentType.Original).Select(x => x.DocumentId).FirstOrDefault().ToString(),
                   Thumbnail = data.CollectionDocuments.Where(x => x.CollectionId == data.Id && x.Type == OfferDocumentType.Thumbnail).Select(x => x.DocumentId).FirstOrDefault().ToString()
               }
           };

        private void PopulateEntityModel(Collection data, CollectionModel model)
        {
            data.ValidUntil = model.ValidUntil;
            data.ValidFrom = model.ValidFrom;
            data.Title = model.Title;
            data.Id = model.Id;
            data.Description = model.Description;
            if (model.OffersIds != null)
            {
                data.OfferCollections = model.OffersIds.Select(offerId => new OfferCollection { OfferId = offerId, CollectionId = model.Id }).ToList();
            }
            data.CollectionDocuments = model.CollectionDocuments.Select(od => new CollectionDocument
            {
                DocumentId = od.DocumentId,
                CollectionId = model.Id,
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
            data.HomeVisible = model.HomeVisible;
        }

        public async Task<CollectionModel> DeleteAsync(int id)
        {
            var context = ContextFactory();
            var collection = await context.Collection
                        .AsNoTracking()
                        .Select(projectToCollectionModel)
                        .FirstOrDefaultAsync(x => x.Id == id);

            if (collection != null)
            {
                var data = new Collection();
                data.Id = collection.Id;
                if (collection.Image != null)
                {
                    context.Document.Remove(new Document { Id = new Guid(collection.Image.Id) });
                }
                context.Remove(data);
                context.SaveChanges();

            }
            return collection;
        }

        public async Task<int> GetCollectionsCount()
        {
            var context = ContextFactory();

            return await context.Collection.CountAsync();
        }

        private async Task UpdateOffer(MMADbContext context, IEnumerable<int> offerIds)
        {
            var offers = context.Offer.Where(x => offerIds.Contains(x.Id));

            foreach (var offer in offers)
            {
                offer.UpdatedOn = DateTime.UtcNow;
            }

            context.UpdateRange(offers);
            await context.SaveChangesAsync();
        }
    }
}
