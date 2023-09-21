using MMA.WebApi.Shared.Models.GenericData;

namespace MMA.WebApi.Shared.Interfaces.GenericData
{
    /// <summary>
    /// Returns server-side filtered data by provided <see cref="FilterOptions{T}"/>
    /// </summary>
    /// <typeparam name="T">Sort, paging and search options for desired data.</typeparam>
    public interface IFilterable<T>
    {
        IFilteredEnumerable<T> Get(FilterOptions<T> filterOptions);
    }
}
