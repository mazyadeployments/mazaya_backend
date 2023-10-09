namespace MMA.WebApi.Shared.Models.DefaultLocations
{
    public class DefaultLocationMobileModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string Address { get; set; }
        public string Vicinity { get; set; }
        public string Country { get; set; }
    }
}
