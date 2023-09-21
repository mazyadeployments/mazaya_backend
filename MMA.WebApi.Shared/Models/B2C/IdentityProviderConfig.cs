namespace MMA.WebApi.Shared.Models.B2C
{
    public class IdentityProviderConfig
    {
        public string TypeId { get; set; }
        public string Audience { get; set; }
        public string Authority { get; set; }
        public string DisplayName { get; set; }
        public string PolicyName { get; set; }
        public string SectionName { get; set; }
        public string LoginApi { get; set; }
    }
}
