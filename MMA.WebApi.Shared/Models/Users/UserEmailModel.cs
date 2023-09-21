namespace MMA.WebApi.Shared.Models.Users
{
    public class UserEmailModel
    {

        public int ProfileId { get; set; }
        public string Email { get; set; }
        public string AccountDataId { get; set; }
        public string AlternativeEmail { get; set; }

        public string FullName { get; set; }
    }
}
