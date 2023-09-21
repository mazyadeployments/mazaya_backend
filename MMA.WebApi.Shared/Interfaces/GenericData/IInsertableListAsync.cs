using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.GenericData
{
    /// <summary>
	/// Enables inserting a list of new entities to the persistence store.
	/// </summary>
	/// <typeparam name="T">Entity's type</typeparam>
	public interface IInsertableListAsync<in T>
    {
        Task InsertListAsync(IEnumerable<T> data);
    }
}
