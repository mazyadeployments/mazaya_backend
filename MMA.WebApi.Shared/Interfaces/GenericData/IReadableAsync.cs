using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.GenericData
{
    /// <summary>
    /// Enables fetching of a single data model by given ID type.
    /// </summary>
    /// <typeparam name="T1">Type of recource's ID. Could be <see cref="int"/>, <see cref="long"/>, <see cref="System.Guid"/>, etc. </typeparam>
    /// <typeparam name="T2">Type of the output resource.</typeparam>
    public interface IReadableAsync<in T1, T2>
    {
        Task<T2> GetAsync(T1 id);
    }
}
