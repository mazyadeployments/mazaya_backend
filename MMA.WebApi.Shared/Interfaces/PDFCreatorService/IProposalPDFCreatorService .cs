using MMA.WebApi.Shared.Models.Roadshow;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.ProposalPDFCreatorService
{
    public interface IProposalPDFCreatorService
    {
        Task<RoadshowProposalModel> GetProposalsData(int offerId);
    }
}