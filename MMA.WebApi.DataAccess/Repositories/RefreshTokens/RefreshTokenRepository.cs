using Microsoft.EntityFrameworkCore;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Interfaces.RefreshToken;
using MMA.WebApi.Shared.Models.RefreshToken;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MMA.WebApi.DataAccess.RefreshTokens
{
    public class RefreshTokenRepository : BaseRepository<RefreshTokenModel>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(Func<MMADbContext> contextFactory) : base(contextFactory)
        {

        }

        public async Task<int> EditAsync(RefreshTokenModel data)
        {
            var context = ContextFactory();
            var entityModel = await context.RefreshToken.FirstOrDefaultAsync(x => x.Id.Equals(data.Id));
            entityModel.Refreshtoken = data.Refreshtoken;
            entityModel.Revoked = data.Revoked;
            entityModel.Id = data.Id;

            await context.SaveChangesAsync();

            return entityModel.Id;
        }

        public IQueryable<RefreshTokenModel> Get()
        {
            return GetEntities();
        }

        public async Task<RefreshTokenModel> GetSingleAsync(Expression<Func<RefreshTokenModel, bool>> query)
        {
            return await GetEntities().FirstOrDefaultAsync(query);
        }

        public async Task<int> InsertAsync(RefreshTokenModel data)
        {
            var context = ContextFactory();
            var entitModel = new RefreshToken
            {
                Username = data.Username,
                Refreshtoken = data.Refreshtoken,
                Revoked = data.Revoked
            };
            entitModel.Id = data.Id;
            await context.RefreshToken.AddAsync(entitModel);
            await context.SaveChangesAsync();

            return entitModel.Id;
        }

        protected override IQueryable<RefreshTokenModel> GetEntities()
        {
            var context = ContextFactory();
            return from rt in context.RefreshToken
                   select new RefreshTokenModel
                   {
                       Id = rt.Id,
                       Refreshtoken = rt.Refreshtoken,
                       Username = rt.Username,
                       Revoked = rt.Revoked

                   };
        }
    }
}
