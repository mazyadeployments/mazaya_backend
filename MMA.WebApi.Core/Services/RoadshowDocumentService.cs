using MMA.WebApi.Shared.Interfaces.RoadshowDocuments;

namespace MMA.WebApi.Core.Services
{
    public class RoadshowDocumentService : IRoadshowDocumentService
    {
        private readonly IRoadshowDocumentRepository _roadshowDocumentRepository;
        public RoadshowDocumentService(IRoadshowDocumentRepository roadshowDocumentRepository)
        {
            _roadshowDocumentRepository = roadshowDocumentRepository;
        }
    }
}
