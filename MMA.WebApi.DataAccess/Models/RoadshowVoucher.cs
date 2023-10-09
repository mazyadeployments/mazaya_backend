using System;

namespace MMA.WebApi.DataAccess.Models
{
    public class RoadshowVoucher
    {
        public int Id { get; set; }
        public int? RoadshowOfferId { get; set; }
        public int? RoadshowProposalId { get; set; }
        public int? RoadshowId { get; set; }
        public int Quantity { get; set; }
        public DateTime Validity { get; set; }
        public string Details { get; set; }
    }
}