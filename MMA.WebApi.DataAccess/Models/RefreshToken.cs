﻿namespace MMA.WebApi.DataAccess.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Refreshtoken { get; set; }
        public bool Revoked { get; set; }
    }
}
