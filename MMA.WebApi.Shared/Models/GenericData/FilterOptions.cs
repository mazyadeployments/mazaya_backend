using System.Collections.Generic;

namespace MMA.WebApi.Shared.Models.GenericData
{
    public class FilterOptions<T>
    {
        public SortOptions Sort { get; set; } = new SortOptions();
        public PagingOptions Paging { get; set; } = new PagingOptions();
        public SearchOptions<T> Search { get; set; }

        public IEnumerable<Filter> Filters { get; set; }
        public string Query { get; set; }
    }
}
