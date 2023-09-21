using System.Collections.Generic;
using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.WebApi.Shared.Helpers
{
    public class OfferSearchModel
    {
        public string SearchString { get; set; }
        public string SortBy { get; set; } = "Id";
        public OrderByType OrderBy { get; set; } = OrderByType.Descending;
        public string Filter { get; set; }
        public IEnumerable<DataColumnModel> Columns { get; set; }
        public IEnumerable<string> Include { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string UserId { get; set; }
    }

    public class DataColumnModel
    {
        public string Name { get; set; }
        public string Data { get; set; }
        public DataQueryOperator? Operator { get; set; }
    }

    public enum DataQueryOperator
    {
        Undefined = 0,
        Eq = 1,
        NotEq = 2,
        Or = 3,
        Lt = 4,
        LtOrEq = 5
    }
}
