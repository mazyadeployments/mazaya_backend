using Microsoft.EntityFrameworkCore;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.OfferSuggestions;
using MMA.WebApi.Shared.Models.OfferSuggestions;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.WebApi.DataAccess.Repositories.OfferSuggestions
{
    public class OfferSuggestionsRepository
        : BaseRepository<OfferSuggestionModel>,
            IOfferSuggestionsRepository
    {
        public OfferSuggestionsRepository(Func<MMADbContext> contexFactory)
            : base(contexFactory) { }

        public IQueryable<OfferSuggestionModel> GetOfferSuggestions(QueryModel query)
        {
            var context = ContextFactory();
            var suggestions = context.OfferSuggestions.Include(x => x.Buyer).AsNoTracking();
            var temp = suggestions.ToList();
            suggestions = FilterAndSortSuggestions(suggestions, query);
            IQueryable<OfferSuggestionModel> retVal = suggestions.Select(
                projectToOfferSuggestionCardModel
            );
            return retVal;
        }

        IQueryable<OfferSuggestion> FilterAndSortSuggestions(
            IQueryable<OfferSuggestion> suggestions,
            QueryModel query
        )
        {
            var filteredSuggestions = suggestions;

            if (query.Filter != null)
            {
                var statusList = query.Filter.Status.ToList();
                if (query.Filter.Status?.Any() == true)
                {
                    OfferSuggestionStatus status;
                    Enum.TryParse(query.Filter.Status.ToList()[0], true, out status);
                    filteredSuggestions = filteredSuggestions.Where(x => x.Status == status);
                }
            }
            if (query.Sort.Order == Order.DESC)
            {
                filteredSuggestions = filteredSuggestions.OrderByDescending(x => x.CreatedOn);
                var temp = filteredSuggestions.ToList();
            }
            else
            {
                filteredSuggestions = filteredSuggestions.OrderBy(x => x.CreatedOn);
                var temp = filteredSuggestions.ToList();
            }

            return filteredSuggestions;
        }

        public IQueryable<OfferSuggestionModel> GetOfferSuggestionsByUserId(string userId)
        {
            var context = ContextFactory();
            return context.OfferSuggestions
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .Select(projectToOfferSuggestionCardModel);
        }

        public async Task CreateAsync(OfferSuggestionModel model)
        {
            var context = ContextFactory();
            var user = context.Users.FirstOrDefault(x => x.Id == model.UserId);
            if (user == null)
            {
                throw new Exception("User doesn't exist!");
            }
            var newOfferSuggestion = PopulateDbFromDomainModel(model);
            await context.OfferSuggestions.AddAsync(newOfferSuggestion);
            await context.SaveChangesAsync();
        }

        public async Task CompleteOfferSuggestionAsync(int id, string userId)
        {
            var context = ContextFactory();
            var offerSuggestion = context.OfferSuggestions.Where(x => x.Id == id).FirstOrDefault();
            if (offerSuggestion == null)
            {
                throw new Exception("Offer Suggestion doesn't exist!");
            }
            offerSuggestion.Status = OfferSuggestionStatus.Complete;
            offerSuggestion.UpdatedOn = DateTime.UtcNow;
            offerSuggestion.UpdatedBy = userId;
            await context.SaveChangesAsync();
        }

        private OfferSuggestion PopulateDbFromDomainModel(OfferSuggestionModel model)
        {
            return new OfferSuggestion
            {
                Status = OfferSuggestionStatus.Incomplete,
                UserId = model.UserId,
                UpdatedOn = DateTime.UtcNow,
                UpdatedBy = model.UserId,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = model.UserId,
                Text = model.Text,
            };
        }

        protected override IQueryable<OfferSuggestionModel> GetEntities()
        {
            throw new NotImplementedException();
        }

        private readonly Expression<
            Func<OfferSuggestion, OfferSuggestionModel>
        > projectToOfferSuggestionCardModel = data =>
            new OfferSuggestionModel()
            {
                Id = data.Id,
                User = new UserSuggestionModel
                {
                    Username = data.Buyer.UserName,
                    Email = data.Buyer.Email
                },
                Text = data.Text,
                Status = data.Status,
                CreatedOn = data.CreatedOn
            };
    }
}
