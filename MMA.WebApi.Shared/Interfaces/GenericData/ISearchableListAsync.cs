using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces
{
    /// <typeparam name="T">Entity type</typeparam>
	public interface ISearchableListAsync<T>
    {
        /// <summary>
        /// Executes the provided query in data store and returns the retrieved list of records.
        /// If no query is provided, this method will return all records from the data store.
        /// </summary>
        /// <param name="query">Query to perform against the data store.</param>
        /// <returns>List of entities by provided query, or all records if no query provided.</returns>
        Task<IEnumerable<T>> GetListAsync(Expression<Func<T, bool>> query = null);
    }
}
