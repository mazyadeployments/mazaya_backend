using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.GenericData
{
    /// <summary>
	/// Enables saving (insert or edit) of a single entity.
	/// </summary>
	/// <typeparam name="T">Entity's type</typeparam>
	public interface ISaveableAsync<T>
    {
        Task<T> SaveAsync(T data);
    }
}
