using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.OfferSuggestions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.OfferSuggestions
{
    public interface IOfferSuggestionsService
    {
        Task<PaginationListModel<OfferSuggestionModel>> GetAllOfferSuggestions(QueryModel query);
        Task CreateOfferSuggestion(OfferSuggestionModel offerSuggestion);
        Task<IEnumerable<OfferSuggestionModel>> GetAllOfferSuggestionsByUserId(string userId);
        Task CompleteOfferSuggestionAsync(int id, string userId);
    }
}
