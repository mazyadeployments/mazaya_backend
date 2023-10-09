using MMA.WebApi.Shared.Models.GenericData;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.GenericData
{
    /// <summary>
    /// Returns server-side filtered data by provided <see cref="FilterOptions{T}"/>
    /// </summary>
    /// <typeparam name="T">Sort, paging and search options for desired data.</typeparam>
    public interface IFilterableAsync<T>
    {
        Task<IFilteredEnumerable<T>> GetAsync(FilterOptions<T> filterOptions);
    }
}
