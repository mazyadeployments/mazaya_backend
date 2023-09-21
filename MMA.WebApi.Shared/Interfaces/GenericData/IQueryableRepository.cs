using System.Linq;

namespace MMA.WebApi.Shared.Interfaces.GenericData
{
    public interface IQueryableRepository<out T>
    {
        IQueryable<T> Get();
    }
}
