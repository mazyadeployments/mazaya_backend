using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Models.OfferSuggestions;
using System.Linq;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.OfferSuggestions
{
    public interface IOfferSuggestionsRepository
    {
        IQueryable<OfferSuggestionModel> GetOfferSuggestions(QueryModel query);
        Task CreateAsync(OfferSuggestionModel model);
        IQueryable<OfferSuggestionModel> GetOfferSuggestionsByUserId(string userId);
        Task CompleteOfferSuggestionAsync(int id, string userId);
    }
}
