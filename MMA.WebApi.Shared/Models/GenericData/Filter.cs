using System.Collections.Generic;

namespace MMA.WebApi.Shared.Models.GenericData
{
    public class Filter
    {
        public string Field { get; set; }
        public IEnumerable<object> Values { get; set; }
    }
}
