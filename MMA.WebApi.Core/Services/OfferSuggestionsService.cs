using Microsoft.EntityFrameworkCore;
using MMA.WebApi.DataAccess.Extensions;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.OfferSuggestions;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.OfferSuggestions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Core.Services
{
    public class OfferSuggestionsService : IOfferSuggestionsService
    {
        private readonly IOfferSuggestionsRepository _offerSuggestionsRepository;

        public OfferSuggestionsService(IOfferSuggestionsRepository offerSuggestionsRepository)
        {
            _offerSuggestionsRepository = offerSuggestionsRepository;
        }

        public async Task<PaginationListModel<OfferSuggestionModel>> GetAllOfferSuggestions(
            QueryModel query
        )
        {
            var offerSuggestions = _offerSuggestionsRepository.GetOfferSuggestions(query);

            return await offerSuggestions.ToPagedListAsync(
                query.PaginationParameters.PageNumber,
                query.PaginationParameters.PageSize
            );
        }

        public async Task<IEnumerable<OfferSuggestionModel>> GetAllOfferSuggestionsByUserId(
            string userId
        )
        {
            var offerSuggestions = await _offerSuggestionsRepository
                .GetOfferSuggestionsByUserId(userId)
                .ToListAsync();
            return offerSuggestions;
        }

        public async Task CreateOfferSuggestion(OfferSuggestionModel offerSuggestion)
        {
            await _offerSuggestionsRepository.CreateAsync(offerSuggestion);
        }

        public async Task CompleteOfferSuggestionAsync(int id, string userId)
        {
            await _offerSuggestionsRepository.CompleteOfferSuggestionAsync(id, userId);
        }
    }
}
