using System.Collections.Generic;

namespace MMA.WebApi.Shared.Interfaces.GenericData
{
    /// <summary>
	/// Server-side filtered data.
	/// </summary>
	/// <typeparam name="T">Type of the fetched entity.</typeparam>
	public interface IFilteredEnumerable<T>
    {
        IEnumerable<T> Data { get; set; }
        long TotalItems { get; set; }
    }
}
