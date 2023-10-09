using Microsoft.EntityFrameworkCore;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Interfaces.RoadshowDocuments;
using MMA.WebApi.Shared.Models.Roadshow;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MMA.WebApi.DataAccess.Repository.RoadshowDocuments
{
    public class RoadshowOfferDocumentRepository : BaseRepository<RoadshowOfferDocumentModel>, IRoadshowOfferDocumentRepository
    {
        public RoadshowOfferDocumentRepository(Func<MMADbContext> contexFactory) : base(contexFactory) { }
        public async Task<RoadshowOfferDocumentModel> GetByDocumentId(Guid id)
        {
            var context = ContextFactory();

            return await context.RoadshowOfferDocument
                    .Select(projectToRoadshowOfferDocumentModel)
                    .FirstOrDefaultAsync(x => x.DocumentId == id.ToString());
        }

        public IQueryable<RoadshowOfferDocumentModel> GetRoadshowImages(Guid id)
        {
            var context = ContextFactory();

            return context.RoadshowOfferDocument
                              .AsNoTracking()
                              .Where(o => o.OriginalImageId == id)
                              .Select(projectToRoadshowOfferDocumentModel);
        }


        public IQueryable<RoadshowOfferDocumentModel> GetRoadshowOfferImages(Guid id)
        {
            var context = ContextFactory();

            return context.RoadshowOfferDocument
                              .AsNoTracking()
                              .Where(o => o.OriginalImageId == id)
                              .Select(projectToRoadshowOfferDocumentModel);
        }

        public async Task DeleteAsync(int id)
        {
            var context = ContextFactory();
            var roadshowOfferDocument = await context.RoadshowOfferDocument
                    .FirstOrDefaultAsync(x => x.Id == id);

            var data = new RoadshowDocument();

            if (roadshowOfferDocument != null)
            {
                context.Remove(data);
                context.SaveChanges();
            }
        }

        private void PopulateEntityModel(RoadshowDocument data, RoadshowDocumentModel model)
        {
            data.Id = model.Id;
            data.CreatedBy = model.CreatedBy;
            data.CreatedOn = model.CreatedOn;
            data.DocumentId = model.DocumentId;
            data.RoadshowId = model.RoadshowId;
            data.OriginalImageId = model.OriginalImageId;
            data.Type = model.Type;
            data.UpdatedBy = model.UpdatedBy;
            data.UpdatedOn = model.UpdatedOn;
            data.X1 = model.X1;
            data.X2 = model.X2;
            data.Y1 = model.Y1;
            data.Y2 = model.Y2;
        }

        private Expression<Func<RoadshowOfferDocument, RoadshowOfferDocumentModel>> projectToRoadshowOfferDocumentModel = data =>
           new RoadshowOfferDocumentModel()
           {
               CreatedBy = data.CreatedBy,
               CreatedOn = data.CreatedOn,
               DocumentId = data.DocumentId.ToString(),
               Id = data.Id,
               RoadshowOfferId = data.RoadshowOfferId,
               OriginalImageId = data.OriginalImageId,
               Type = data.Type,
               X1 = data.X1,
               X2 = data.X2,
               Y1 = data.Y1,
               Y2 = data.Y2
           };
        protected override IQueryable<RoadshowOfferDocumentModel> GetEntities()
        {
            throw new NotImplementedException();
        }

        public async Task UpdateRoadshowOfferImagesCover(Guid id, bool isCover)
        {
            var context = ContextFactory();

            var images = context.RoadshowOfferDocument.Where(od => od.OriginalImageId == id).ToList();

            foreach (var img in images)
            {
                img.Cover = isCover;
                context.RoadshowOfferDocument.Update(img);
                await context.SaveChangesAsync();
            }
        }
    }
}
