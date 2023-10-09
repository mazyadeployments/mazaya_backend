using Microsoft.EntityFrameworkCore;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Interfaces.OfferDocuments;
using MMA.WebApi.Shared.Models.Offer;
using MMA.WebApi.Shared.Models.OfferDocument;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MMA.WebApi.DataAccess.Repository.Offers
{
    public class OfferDocumentRepository : BaseRepository<OfferDocumentModel>, IOfferDocumentRepository
    {
        public OfferDocumentRepository(Func<MMADbContext> contexFactory) : base(contexFactory) { }

        public async Task DeleteOriginalImageAsync(Guid id)
        {
            var context = ContextFactory();
            var offerDocument = await context.OfferDocument
                    .Select(projectToOfferDocumentModel)
                    .FirstOrDefaultAsync(x => x.OriginalImageId == id);

            var data = new OfferDocument();
            PopulateEntityModel(data, offerDocument);

            if (offerDocument != null)
            {
                context.Remove(data);
                context.SaveChanges();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var context = ContextFactory();
            var offerDocument = await context.OfferDocument
                    .Select(projectToOfferDocumentModel)
                    .FirstOrDefaultAsync(x => x.Id == id);

            var data = new OfferDocument();
            PopulateEntityModel(data, offerDocument);

            if (offerDocument != null)
            {
                context.Remove(data);
                context.SaveChanges();
            }
        }

        private Expression<Func<OfferDocument, OfferDocumentModel>> projectToOfferDocumentModel = data =>
           new OfferDocumentModel()
           {
               CreatedBy = data.CreatedBy,
               CreatedOn = data.CreatedOn,
               DocumentId = data.DocumentId,
               Id = data.Id,
               OfferId = data.OfferId,
               OriginalImageId = data.OriginalImageId,
               Type = data.Type,
               X1 = data.X1,
               X2 = data.X2,
               Y1 = data.Y1,
               Y2 = data.Y2,
               Cover = data.Cover
           };
        private void PopulateEntityModel(OfferDocument data, OfferDocumentModel model)
        {
            data.Id = model.Id;
            data.CreatedBy = model.CreatedBy;
            data.CreatedOn = model.CreatedOn;
            data.DocumentId = model.DocumentId;
            data.OfferId = model.OfferId;
            data.OriginalImageId = model.OriginalImageId;
            data.Type = model.Type;
            data.UpdatedBy = model.UpdatedBy;
            data.UpdatedOn = model.UpdatedOn;
            data.X1 = model.X1;
            data.X2 = model.X2;
            data.Y1 = model.Y1;
            data.Y2 = model.Y2;
        }

        public IQueryable<Guid> GetOfferDocumentsByOriginalId(Guid id)
        {
            var context = ContextFactory();

            return context.OfferDocument
                .AsNoTracking()
                .Where(o => o.OriginalImageId == id)
                .Select(o => o.DocumentId);
        }

        public IQueryable<OfferDocumentModel> GetOfferImages(Guid id)
        {
            var context = ContextFactory();

            return context.OfferDocument
                              .AsNoTracking()
                              .Where(o => o.OriginalImageId == id)
                              .Select(projectToOfferDocumentModel);
        }

        public async Task UpdateOfferImagesCover(Guid id, bool isCover)
        {
            var context = ContextFactory();

            var images = context.OfferDocument.Where(od => od.OriginalImageId == id).ToList();

            foreach (var img in images)
            {
                img.Cover = isCover;
                context.OfferDocument.Update(img);
                await context.SaveChangesAsync();
            }

        }

        public async Task<OfferDocumentModel> GetByDocumentId(Guid id)
        {
            var context = ContextFactory();

            return await context.OfferDocument
                    .Select(projectToOfferDocumentModel)
                    .FirstOrDefaultAsync(x => x.DocumentId == id);
        }

        public IQueryable<OfferLocationModel> Get()
        {
            throw new NotImplementedException();
        }

        protected override IQueryable<OfferDocumentModel> GetEntities()
        {
            throw new NotImplementedException();
        }
    }
}
