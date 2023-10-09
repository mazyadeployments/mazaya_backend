namespace MMA.WebApi.Shared.Models.Account
{
    public class LoginModel
    {
        public string Username { get; set; }
        public string Loz { get; set; }
    }

    public class RefreshTokenRequestModel
    {
        public string RefreshToken { get; set; }
    }
}
