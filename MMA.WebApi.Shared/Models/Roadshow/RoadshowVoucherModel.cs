using System;

namespace MMA.WebApi.Shared.Models.Roadshow
{
    public class RoadshowVoucherModel
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public DateTime Validity { get; set; }
        public string Details { get; set; }
    }
}