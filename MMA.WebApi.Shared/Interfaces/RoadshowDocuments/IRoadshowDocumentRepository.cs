using MMA.WebApi.Shared.Models.Roadshow;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.RoadshowDocuments
{
    public interface IRoadshowDocumentRepository
    {
        Task<RoadshowDocumentModel> GetByDocumentId(Guid id);
        IQueryable<RoadshowDocumentModel> GetRoadshowImages(Guid id);
        IQueryable<RoadshowOfferDocumentModel> GetRoadshowOfferImages(Guid id);
        Task DeleteAsync(int id);
    }
}