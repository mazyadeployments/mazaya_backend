namespace MMA.WebApi.DataAccess.Models
{
    public class RoadshowLocation
    {
        public int Id { get; set; }
        public int DefaultLocationId { get; set; }
        public DefaultLocation DefaultLocation { get; set; }
        public int RoadshowId { get; set; }
        public Roadshow Roadshow { get; set; }
    }
}
