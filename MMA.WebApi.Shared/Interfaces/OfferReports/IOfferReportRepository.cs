using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Models.Offer;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.OfferReports
{
    public interface IOfferReportRepository
    {
        Task<IEnumerable<OfferReportModel>> GetOfferReportByOfferId(int offerId);
        Task AddNewOfferReport(OfferReportModel offerReport, string userId);
        Task<IEnumerable<OfferModel>> GetAllReportedOffer(QueryModel queryModel);
        Task<bool> IsReported(string userId, int reportedOfferId);
        Task<IEnumerable<OfferReportModel>> ResolveOfferReport(int reportId);


    }
}
