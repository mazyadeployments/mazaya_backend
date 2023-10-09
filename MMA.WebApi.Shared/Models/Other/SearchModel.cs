using MMA.WebApi.Shared.Models.GenericData;
using System.Collections.Generic;

namespace MMA.WebApi.Shared.Models.Other
{
    public class SearchModel : SearchBaseModel
    {
        public FilterSort Sort { get; set; }
        public IEnumerable<Filter> Filters { get; set; }
    }
}
