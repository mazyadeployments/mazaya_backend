using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.GenericData
{
    /// <summary>
    /// Enables inserting new entity to the persistence store.
    /// </summary>
    /// <typeparam name="T1">Type of entity's identifier. Could be <see cref="int"/>, <see cref="long"/>, <see cref="System.Guid"/>, etc.</typeparam>
    /// <typeparam name="T2">Entity's type</typeparam>
    public interface IInsertableAsync<T1, in T2>
    {
        Task<T1> InsertAsync(T2 data);
    }
}
