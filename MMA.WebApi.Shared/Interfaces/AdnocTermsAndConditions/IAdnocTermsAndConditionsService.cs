using MMA.WebApi.Shared.Models.AdnocTermsAndConditions;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.AdnocTermsAndConditions
{
    public interface IAdnocTermsAndConditionsService
    {
        Task<AdnocTermsAndConditionsModel> GetAdnocTermsAndConditionsForOffer();
        Task<AdnocTermsAndConditionsModel> GetAdnocTermsAndConditionsForProposal();
        Task<AdnocTermsAndConditionsModel> GetGlobalAdnocTermsAndConditions();
        Task UpdateAdnocTermsAndConditions(AdnocTermsAndConditionsModel adnocTermsAndConditionsModel, string userId);
    }
}
