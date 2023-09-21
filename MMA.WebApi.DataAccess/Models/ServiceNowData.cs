using System;


namespace MMA.WebApi.DataAccess.Models
{
    public class ServiceNowData
    {
        public Guid Id { get; set; }
        public string JsonData { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Processed { get; set; }
    }

}
