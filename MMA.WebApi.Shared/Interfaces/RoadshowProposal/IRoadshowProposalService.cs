using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.PDFModel;
using MMA.WebApi.Shared.Models.Response;
using MMA.WebApi.Shared.Models.Roadshow;
using MMA.WebApi.Shared.Monads;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.Offers
{

    public interface IRoadshowProposalService
    {
        Task<RoadshowProposalModel> CreateOrUpdate(RoadshowProposalModel model, string userId);
        Task<IEnumerable<RoadshowProposalModel>> GetRoadshowProposals();
        Task<PaginationListModel<RoadshowProposalModel>> GetAllRoadshowProposals(QueryModel queryModel, string userId);
        Task<Maybe<RoadshowProposalModel>> GetSpecificProposalById(int id, string userId);
        Task<IEnumerable<RoadshowProposalModel>> GetAllProposalsForCompanyCard(int id);
        Task<ResponseDetailsModel> DeleteProposal(int id);
        bool CheckIfProposalIsValid(int proposalId, int companyId);
        Task<AdditionalPdfInfo> GetAdditionalInfoForPdf(int companyId);
    }
}
