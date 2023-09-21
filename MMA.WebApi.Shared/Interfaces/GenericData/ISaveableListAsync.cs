using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.GenericData
{
    /// <summary>
    /// Enables saving (insert or edit) of a multiple entities.
    /// </summary>
    /// <typeparam name="T">Entity's type</typeparam>
    public interface ISaveableListAsync<in T>
    {
        Task SaveListAsync(IEnumerable<T> data);
    }
}
