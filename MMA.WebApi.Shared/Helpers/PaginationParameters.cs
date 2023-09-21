namespace MMA.WebApi.Shared.Helpers
{
    public class PaginationParameters
    {
        public int PageNumber { get; set; } = 1;

        private int _pageIndex = 16;
        public int PageSize
        {
            get
            {
                return _pageIndex;
            }
            set
            {
                _pageIndex = value;
            }
        }
    }
}
