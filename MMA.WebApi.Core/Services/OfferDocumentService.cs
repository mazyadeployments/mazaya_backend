using MMA.WebApi.Shared.Interfaces.OfferDocuments;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Core.Services
{
    public class OfferDocumentService : IOfferDocumentService
    {
        private readonly IOfferDocumentRepository _offerDocumentRepository;
        public OfferDocumentService(IOfferDocumentRepository offerDocumentRepository)
        {
            _offerDocumentRepository = offerDocumentRepository;
        }
        public async Task DeleteOfferDocument(Guid id)
        {
            await _offerDocumentRepository.DeleteOriginalImageAsync(id);
        }

        public IEnumerable<Guid> GetOfferDocumentsByOriginalId(Guid id)
        {
            var offerDocuments = _offerDocumentRepository.GetOfferDocumentsByOriginalId(id);
            return offerDocuments;
        }
    }
}
