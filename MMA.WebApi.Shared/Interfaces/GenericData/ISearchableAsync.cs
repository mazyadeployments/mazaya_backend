using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.GenericData
{
    public interface ISearchableAsync<T>
    {
        Task<T> GetSingleAsync(Expression<Func<T, bool>> query);
    }
}
