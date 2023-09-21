using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMA.WebApi.DataAccess.Repositories.LogAnalytics
{
    public class LogAnalyticsRepository : ILogAnalyticsRepository
    {
        private readonly Func<MMADbContext> _contextFactory;

        public LogAnalyticsRepository(Func<MMADbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
        public async Task LogOfferClick(int offerId, string userId)
        {

            if (offerId == 0 || string.IsNullOrEmpty(userId))
                return;
            var context = _contextFactory();
            await context.LogOfferClick.AddAsync(new LogOfferClick()
            {
                CreatedBy = userId,
                OfferId = offerId,
                CreatedOn = DateTime.UtcNow

            });

            await context.SaveChangesAsync();
        }

        public async Task LogSearchKeyword(string keyword, string userId)
        {
            if (keyword == null || string.IsNullOrEmpty(keyword))
                return;

            var context = _contextFactory();
            await context.LogKeywordSearch.AddAsync(new LogKeywordSearch()
            {
                CreatedBy = userId,
                CreatedOn = DateTime.UtcNow,
                Keyword = keyword.ToLower()
            });

            await context.SaveChangesAsync();
        }

        public async Task LogSearchOffers(IEnumerable<int> offersIds, string userId)
        {
            if (offersIds == null)
                return;
            List<LogOfferSearch> offers = offersIds.Select(x => new LogOfferSearch()
            {
                OfferId = x,
                CreatedBy = userId,
                CreatedOn = DateTime.UtcNow
            }).ToList();
            var context = _contextFactory();
            context.LogOfferSearch.AddRange(offers);
            await context.SaveChangesAsync();

        }


        public async Task LogBannerClick(int offerId, string userId)
        {
            if (offerId == 0 || string.IsNullOrEmpty(userId))
                return;
            var context = _contextFactory();
            await context.LogBannerClicks.AddAsync(new LogBannerClick()
            {
                OfferId = offerId,
                CreatedOn = DateTime.UtcNow

            });

            await context.SaveChangesAsync();
        }
    }
}
