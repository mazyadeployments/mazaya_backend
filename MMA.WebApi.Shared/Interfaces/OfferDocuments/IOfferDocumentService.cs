using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.OfferDocuments
{
    public interface IOfferDocumentService
    {
        Task DeleteOfferDocument(Guid id);
        IEnumerable<Guid> GetOfferDocumentsByOriginalId(Guid id);
    }
}
