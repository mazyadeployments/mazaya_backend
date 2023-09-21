using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.GenericData
{
    /// <summary>
	/// Enables editing of an existing entity in the persistence store.
	/// </summary>
	/// <typeparam name="T1">Type of entity's identifier. Could be <see cref="int"/>, <see cref="long"/>, <see cref="System.Guid"/>, etc.</typeparam>
	/// <typeparam name="T2">Entity's type</typeparam>
	public interface IEditableAsync<T1, in T2>
    {
        Task<T1> EditAsync(T2 data);
    }
}
