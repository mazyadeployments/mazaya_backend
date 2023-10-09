using MMA.WebApi.Shared.Models.Roadshow;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.RoadshowDocuments
{
    public interface IRoadshowOfferDocumentRepository
    {
        Task<RoadshowOfferDocumentModel> GetByDocumentId(Guid id);
        IQueryable<RoadshowOfferDocumentModel> GetRoadshowImages(Guid id);
        Task UpdateRoadshowOfferImagesCover(Guid id, bool isCover);
        IQueryable<RoadshowOfferDocumentModel> GetRoadshowOfferImages(Guid id);
        Task DeleteAsync(int id);
    }
}