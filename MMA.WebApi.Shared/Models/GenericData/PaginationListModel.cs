using System.Collections.Generic;

namespace MMA.WebApi.Shared.Models.GenericData
{
    public class PaginationListModel<T>
    {
        public IEnumerable<T> List { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPageCount { get; set; }
    }

    public class PaginationListModelExt<T> : PaginationListModel<T>
    {
        public string BaseUrl { get; set; }
    }
}
