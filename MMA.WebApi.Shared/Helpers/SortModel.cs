namespace MMA.WebApi.Shared.Helpers
{
    public class SortModel
    {
        public string Type { get; set; }
        public Order Order { get; set; }
        public SortModel()
        {
            Type = "date";
            Order = Order.DESC;
        }
    }
}