namespace MMA.WebApi.Shared.Models.Other
{
    public class SearchBaseModel
    {
        public string Query { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int SearchOption { get; set; }
    }
}
