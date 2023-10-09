using System.Collections.Generic;

namespace MMA.WebApi.Shared.Models.GenericData
{
    /// <summary>
	/// Search options for data querying.
	/// <see cref="SearchRanges"/> is a dictionary of property names of <see cref="T"/> as keys and <see cref="RangeOptions"/> as values.
	/// </summary>
	/// <typeparam name="T">Type of the fetched entities.</typeparam>
	public class SearchOptions<T>
    {
        public T SearchValues { get; set; }
        public Dictionary<string, RangeOptions> SearchRanges { get; set; }
    }
}
