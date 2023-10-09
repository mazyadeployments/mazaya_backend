using System;

namespace MMA.WebApi.Shared.Models.LogModels

{
    public class LogOfferSearch
    {
        public long Id { get; set; }
        public int OfferId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }

    }
}
