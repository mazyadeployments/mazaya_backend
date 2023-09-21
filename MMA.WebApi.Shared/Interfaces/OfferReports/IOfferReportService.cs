using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.Offer;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.OfferReports
{
    public interface IOfferReportService
    {
        Task SaveAsync(OfferReportModel data, string userId);
        Task<IEnumerable<OfferReportModel>> GetReportsByOfferId(int offerId);
        Task<PaginationListModel<OfferModel>> GetAllReportedOffer(QueryModel queryModel);
        Task<bool> IsReported(string userId, int reportedOfferId);
        Task<IEnumerable<OfferReportModel>> ResolveOfferReport(int reportId);
    }
}
