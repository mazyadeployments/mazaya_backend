using System.Linq;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.GenericData
{
    public interface IQueryableRepositoryAsync<T>
    {
        Task<IQueryable<T>> GetAsync();
    }
}
