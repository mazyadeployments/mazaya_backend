﻿namespace MMA.WebApi.Shared.Models.CompanyLocations
{
    public class CompanyLocationModel
    {
        public int Id { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string Address { get; set; }
        public string Vicinity { get; set; }
        public string Country { get; set; }
        public int CompanyId { get; set; }
    }
}
