using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.GenericData
{
    /// <summary>
    /// Enables deleting of a multiple entities by given list id.
    /// </summary>
    /// <typeparam name="T">Type of entity's identifier. Could be <see cref="int"/>, <see cref="long"/>, <see cref="System.Guid"/>, etc.</typeparam>
    public interface IDeletableListAsync<in T>
    {
        Task DeleteListAsync(IEnumerable<T> list);
    }
}
