using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Models.PDFModel;
using MMA.WebApi.Shared.Models.Response;
using MMA.WebApi.Shared.Models.Roadshow;
using MMA.WebApi.Shared.Visitor;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace MMA.WebApi.Shared.Interfaces.Offers
{
    public interface IRoadshowProposalRepository : IQueryableRepository<RoadshowProposalModel>
    {
        Task<RoadshowProposalModel> CreateAsync(RoadshowProposalModel model, IVisitor<IChangeable> auditVisitor, string userId);
        IQueryable<RoadshowProposalModel> GetAllRoadshowProposals(string userId, List<Enums.Declares.Roles> roles, QueryModel queryModel);
        Task<RoadshowProposalModel> GetSpecificProposalById(int id, string userId, List<Enums.Declares.Roles> roles);
        bool CheckIfProposalIsValid(int id, int companyId);
        IQueryable<RoadshowProposalModel> GetAllProposalsForCompanyCard(int id);
        Task<ResponseDetailsModel> DeleteProposal(int id);
        Task DeactivateRoadshowProposals(int companyId);
        Task HardOfCompanyDeleteProposals(int companyId);
        Task<AdditionalPdfInfo> GetAdditionalInfoForPdf(int companyId);
    }
}
