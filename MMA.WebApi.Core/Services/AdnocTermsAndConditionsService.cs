using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Interfaces.AdnocTermsAndConditions;
using MMA.WebApi.Shared.Models.AdnocTermsAndConditions;
using System;
using System.Text;
using System.Threading.Tasks;

namespace MMA.WebApi.Core.Services
{
    public class AdnocTermsAndConditionsService : IAdnocTermsAndConditionsService
    {
        private readonly IAdnocTermsAndConditionsRepository _adnocTermsAndConditionsRepository;
        public AdnocTermsAndConditionsService(IAdnocTermsAndConditionsRepository adnocTermsAndConditionsRepository)
        {
            _adnocTermsAndConditionsRepository = adnocTermsAndConditionsRepository;
        }


        public async Task<AdnocTermsAndConditionsModel> GetGlobalAdnocTermsAndConditions()
        {
            return await _adnocTermsAndConditionsRepository.GetAdnocTermsAndConditions(Declares.AdnocTermsAndConditionType.GlobalType);
        }

        public async Task<AdnocTermsAndConditionsModel> GetAdnocTermsAndConditionsForProposal()
        {
            return await _adnocTermsAndConditionsRepository.GetAdnocTermsAndConditions(Declares.AdnocTermsAndConditionType.ProposalType);
        }
        public async Task<AdnocTermsAndConditionsModel> GetAdnocTermsAndConditionsForOffer()
        {
            return await _adnocTermsAndConditionsRepository.GetAdnocTermsAndConditions(Declares.AdnocTermsAndConditionType.OfferType);
        }

        public async Task UpdateAdnocTermsAndConditions(AdnocTermsAndConditionsModel adnocTermsAndConditionsModel, string UserId)
        {
            // Decode content
            adnocTermsAndConditionsModel.Content = DecodeBase64String(adnocTermsAndConditionsModel.Content);
            adnocTermsAndConditionsModel.ContentArabic = DecodeBase64String(adnocTermsAndConditionsModel.ContentArabic);

            await _adnocTermsAndConditionsRepository.UpdateAdnocTermsAndConditions(adnocTermsAndConditionsModel, UserId);
        }

        private string DecodeBase64String(string encodedString)
        {
            encodedString ??= "";
            byte[] data = Convert.FromBase64String(encodedString);
            return Encoding.UTF8.GetString(data);
        }
    }
}
