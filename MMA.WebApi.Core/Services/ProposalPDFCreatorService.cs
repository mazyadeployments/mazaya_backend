using Microsoft.EntityFrameworkCore;
using MMA.WebApi.Shared.Interfaces.Offers;
using MMA.WebApi.Shared.Interfaces.ProposalPDFCreatorService;
using MMA.WebApi.Shared.Models.Roadshow;
using System.Linq;
using System.Threading.Tasks;

namespace MMA.WebApi.Core.Services
{
    public class ProposalPDFCreatorService : IProposalPDFCreatorService
    {
        private readonly IRoadshowProposalRepository _roadshowProposalRepository;
        public ProposalPDFCreatorService(IRoadshowProposalRepository roadshowProposalRepository)
        {
            _roadshowProposalRepository = roadshowProposalRepository;
        }

        public async Task<RoadshowProposalModel> GetProposalsData(int proposalId)
        {
            return await GetProposal(proposalId);
        }

        private async Task<RoadshowProposalModel> GetProposal(int proposalId)
        {
            return await _roadshowProposalRepository.Get().Where(pr => pr.Id == proposalId).FirstOrDefaultAsync();
        }
    }
}
