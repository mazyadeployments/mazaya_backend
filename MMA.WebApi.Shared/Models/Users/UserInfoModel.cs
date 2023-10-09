namespace MMA.WebApi.Shared.Models.Users
{
    public class UserInfoModel
    {
        public int ProfileId { get; set; }
        public string FullName { get; set; }
        public string Designation { get; set; }
        public string Image { get; set; }
        public string MobilePhone { get; set; }
        public string WorkPhone { get; set; }
        public string Email { get; set; }
        public string UserImageUrl { get; set; }
        public string AccountDataId { get; set; }
        public string AlternativeEmail { get; set; }
    }
}
