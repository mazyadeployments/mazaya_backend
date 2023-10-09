using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.GenericData
{
    /// <summary>
	/// Enables fetching of all entities of a given type.
	/// </summary>
	/// <typeparam name="T">Type of the output resource.</typeparam>
	public interface IReadableListAsync<T>
    {
        Task<IEnumerable<T>> GetAsync();
    }
}
