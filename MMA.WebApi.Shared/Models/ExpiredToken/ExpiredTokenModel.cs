using System;

namespace MMA.WebApi.Shared.Models.ExpiredToken
{
    public class ExpiredTokenModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Token { get; set; }
        public DateTime ExpiredAt { get; set; }
    }
}
