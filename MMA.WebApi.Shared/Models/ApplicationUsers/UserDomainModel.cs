namespace MMA.WebApi.Shared.Models.ApplicationUser
{
    public class UserDomainModel
    {
        public int Id { get; set; }
        public string DomainName { get; set; }
        public string KeyValue { get; set; }
        public string Domains { get; set; }
        public string SequencerName { get; set; }
    }
}