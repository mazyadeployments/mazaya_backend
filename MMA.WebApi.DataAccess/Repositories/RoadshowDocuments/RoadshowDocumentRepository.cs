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
    public class RoadshowDocumentRepository : BaseRepository<RoadshowDocumentModel>, IRoadshowDocumentRepository
    {
        public RoadshowDocumentRepository(Func<MMADbContext> contexFactory) : base(contexFactory) { }
        public async Task<RoadshowDocumentModel> GetByDocumentId(Guid id)
        {
            var context = ContextFactory();

            return await context.RoadshowDocument
                    .Select(projectToRoadshowDocumentModel)
                    .FirstOrDefaultAsync(x => x.DocumentId == id);
        }

        public IQueryable<RoadshowDocumentModel> GetRoadshowImages(Guid id)
        {
            var context = ContextFactory();

            return context.RoadshowDocument
                              .AsNoTracking()
                              .Where(o => o.OriginalImageId == id)
                              .Select(projectToRoadshowDocumentModel);
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
            var roadshowDocument = await context.RoadshowDocument
                    .Select(projectToRoadshowDocumentModel)
                    .FirstOrDefaultAsync(x => x.Id == id);

            var data = new RoadshowDocument();
            PopulateEntityModel(data, roadshowDocument);

            if (roadshowDocument != null)
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

        private Expression<Func<RoadshowDocument, RoadshowDocumentModel>> projectToRoadshowDocumentModel = data =>
           new RoadshowDocumentModel()
           {
               CreatedBy = data.CreatedBy,
               CreatedOn = data.CreatedOn,
               DocumentId = data.DocumentId,
               Id = data.Id,
               RoadshowId = data.RoadshowId,
               OriginalImageId = data.OriginalImageId,
               Type = data.Type,
               X1 = data.X1,
               X2 = data.X2,
               Y1 = data.Y1,
               Y2 = data.Y2
           };

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
               Y2 = data.Y2,
               Cover = data.Cover
           };
        protected override IQueryable<RoadshowDocumentModel> GetEntities()
        {
            throw new NotImplementedException();
        }
    }
}
