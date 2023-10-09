namespace MMA.WebApi.Shared.Models.GenericData
{
    /// <summary>
    /// Paging options for server-side pagination.
    /// </summary>
    public class PagingOptions
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }

        public PagingOptions()
        {
            CurrentPage = 1;
        }
    }
}
