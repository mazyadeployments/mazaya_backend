using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.GenericData
{
    /// <summary>
	/// Enables deleting of a single entity by given id.
	/// </summary>
	/// <typeparam name="T">Type of entity's identifier. Could be <see cref="int"/>, <see cref="long"/>, <see cref="System.Guid"/>, etc.</typeparam>
	public interface IDeletableAsync<in T>
    {
        Task DeleteAsync(T id);
    }
}
