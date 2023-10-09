using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Models.AdnocTermsAndConditions;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.AdnocTermsAndConditions
{
    public interface IAdnocTermsAndConditionsRepository
    {
        Task<AdnocTermsAndConditionsModel> GetAdnocTermsAndConditions(Declares.AdnocTermsAndConditionType type);
        Task UpdateAdnocTermsAndConditions(AdnocTermsAndConditionsModel adnocTermsAndConditionsModel, string UserId);
    }
}
