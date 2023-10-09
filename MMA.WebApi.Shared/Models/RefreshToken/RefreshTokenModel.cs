namespace MMA.WebApi.Shared.Models.RefreshToken
{
    public class RefreshTokenModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Refreshtoken { get; set; }
        public bool Revoked { get; set; }
    }
}
