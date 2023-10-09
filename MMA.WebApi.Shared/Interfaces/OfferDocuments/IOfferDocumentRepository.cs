using MMA.WebApi.Shared.Models.OfferDocument;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.OfferDocuments
{
    public interface IOfferDocumentRepository
    {
        Task DeleteOriginalImageAsync(Guid id);
        IQueryable<Guid> GetOfferDocumentsByOriginalId(Guid id);
        Task UpdateOfferImagesCover(Guid id, bool isCover);
        Task<OfferDocumentModel> GetByDocumentId(Guid id);
        IQueryable<OfferDocumentModel> GetOfferImages(Guid id);
        Task DeleteAsync(int id);
    }
}