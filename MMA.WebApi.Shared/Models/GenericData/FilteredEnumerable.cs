using MMA.WebApi.Shared.Interfaces.GenericData;
using System.Collections.Generic;

namespace MMA.WebApi.Shared.Models.GenericData
{
    public class FilteredEnumerable<T> : IFilteredEnumerable<T>
    {
        public FilteredEnumerable() { }

        public FilteredEnumerable(IEnumerable<T> data, long total)
        {
            Data = data;
            TotalItems = total;
        }

        public IEnumerable<T> Data { get; set; }
        public long TotalItems { get; set; }
    }
}
