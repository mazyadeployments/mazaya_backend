using System;

namespace MMA.WebApi.Shared.Models.OneAdnoc
{
    public class TokenData
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string ClientId { get; set; }
        public DateTime Issued { get; set; }
        public DateTime Expires { get; set; }
        public string Error { get; set; }
        public int ExpiresInSeconds { get; set; }
        public bool IsValidUser { get; set; }
        public string TokenType { get; set; }
    }
}
