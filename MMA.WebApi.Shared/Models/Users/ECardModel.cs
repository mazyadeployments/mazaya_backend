namespace MMA.WebApi.Shared.Models.Users
{
    public class ECardModel
    {
        public string UserId { get; set; }
        public string ECardSequence { get; set; }
        public string FormattedECardSequence { get; set; }
        public int UserDomainId { get; set; }
    }
}
