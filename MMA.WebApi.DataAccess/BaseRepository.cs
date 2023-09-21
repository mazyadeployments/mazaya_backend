using MMA.WebApi.DataAccess.Models;
using System;
using System.Linq;

namespace MMA.WebApi.DataAccess
{
    public abstract class BaseRepository<T>
    {
        protected readonly Func<MMADbContext> ContextFactory;

        protected BaseRepository(Func<MMADbContext> contextFactory)
        {
            ContextFactory = contextFactory;
        }

        protected abstract IQueryable<T> GetEntities();
    }
}
