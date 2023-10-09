using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.WebApi.Shared.Models.GenericData
{
    public class FilterSort
    {
        public string Field { get; set; }
        public FilterSortDirection Direction { get; set; }
    }
}
