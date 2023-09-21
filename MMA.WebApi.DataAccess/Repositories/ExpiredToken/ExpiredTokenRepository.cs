using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Interfaces.ExpiredToken;
using MMA.WebApi.Shared.Models.ExpiredToken;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MMA.WebApi.DataAccess.Repositories.ExpiredToken
{
    public class ExpiredTokenRepository : BaseRepository<ExpiredTokenModel>, IExpiredTokenRepository
    {
        public ExpiredTokenRepository(Func<MMADbContext> contextFactory)
            : base(contextFactory) { }

        public async Task DeleteAsync(int id)
        {
            var context = ContextFactory();
            var expiredToken = await context.ExpiredTokens.FirstOrDefaultAsync(x => x.Id == id);
            if (expiredToken != null)
            {
                context.Remove(expiredToken);
                context.SaveChanges();
            }
        }

        public async Task DeleteExpiredTokens(ILogger logger)
        {
            try
            {
                var context = ContextFactory();
                var tokensToDelete = await context.ExpiredTokens
                    .Where(o => o.ExpiredAt < DateTime.UtcNow)
                    .ToListAsync();
                int countOfTokens = tokensToDelete.Count();

                logger.LogInformation("Deleting " + countOfTokens + " tokens from DB..");
                context.RemoveRange(tokensToDelete);
                context.SaveChanges();
                logger.LogInformation("Successfully deleted " + countOfTokens + " tokens from DB");
            }
            catch (Exception e)
            {
                logger.LogError(e.ToString());
            }
        }

        public Task<int> EditAsync(ExpiredTokenModel data)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ExpiredTokenModel>> GetListAsync(
            Expression<Func<ExpiredTokenModel, bool>> query = null
        )
        {
            return await GetEntities().Where(query).ToListAsync();
        }

        public async Task<ExpiredTokenModel> GetSingleAsync(
            Expression<Func<ExpiredTokenModel, bool>> query
        )
        {
            return await GetEntities().FirstOrDefaultAsync(query);
        }

        public async Task<int> InsertAsync(ExpiredTokenModel data)
        {
            using (var context = ContextFactory())
            {
                var tokenModel = new Models.ExpiredToken
                {
                    ExpiredAt = data.ExpiredAt,
                    Token = data.Token,
                    UserId = data.UserId
                };
                await context.ExpiredTokens.AddAsync(tokenModel);
                await context.SaveChangesAsync();

                return tokenModel.Id; // it tracks entity in db so it's possible to return Id
            }
        }

        protected override IQueryable<ExpiredTokenModel> GetEntities()
        {
            var context = ContextFactory();
            return from et in context.ExpiredTokens
                select new ExpiredTokenModel
                {
                    Id = et.Id,
                    UserId = et.UserId,
                    Token = et.Token,
                    ExpiredAt = et.ExpiredAt
                };
        }
    }
}
