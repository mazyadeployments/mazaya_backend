namespace MMA.WebApi.Shared.Helpers
{
    public class QueryModel
    {
        public FilterModel Filter { get; set; }
        public SortModel Sort { get; set; }
        public PaginationParameters PaginationParameters { get; set; }
        public int Page { get; set; }
        public string UserId { get; set; }
        public QueryModel()
        {
            Filter = new FilterModel();
            Sort = new SortModel();
            PaginationParameters = new PaginationParameters();
        }
    }

    public class OneHubQueryModel
    {
        public OneHubFilterModel Filter { get; set; }
        public SortModel Sort { get; set; }
        public PaginationParameters PaginationParameters { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; } = 10;
        public string UserId { get; set; }
        public OneHubQueryModel()
        {
            Filter = new OneHubFilterModel();
            Sort = new SortModel();
            PaginationParameters = new PaginationParameters();
        }
    }
}