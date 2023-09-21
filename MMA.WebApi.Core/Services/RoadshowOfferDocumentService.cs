using MMA.WebApi.Shared.Interfaces.RoadshowDocuments;

namespace MMA.WebApi.Core.Services
{
    public class RoadshowOfferDocumentService : IRoadshowOfferDocumentService
    {
        private readonly IRoadshowOfferDocumentService _roadshowOfferDocumentRepository;
        public RoadshowOfferDocumentService(IRoadshowOfferDocumentService roadshowOfferDocumentRepository)
        {
            _roadshowOfferDocumentRepository = roadshowOfferDocumentRepository;
        }
    }
}
