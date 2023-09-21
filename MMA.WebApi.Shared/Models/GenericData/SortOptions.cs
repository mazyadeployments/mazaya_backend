namespace MMA.WebApi.Shared.Models.GenericData
{
    /// <summary>
	/// Sort options for server-side sorting.
	/// </summary>
	public class SortOptions
    {
        public SortType SortType { get; set; }
        public string OrderBy { get; set; }
    }
}
