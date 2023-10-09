using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.GenericData
{
    /// <summary>
    /// Enables bulk updating of list of entities.
    /// </summary>
    /// <typeparam name="T">Entity's type</typeparam>
    public interface IEditableListAsync<T>
    {
        Task EditListAsync(IEnumerable<T> list);
    }
}
