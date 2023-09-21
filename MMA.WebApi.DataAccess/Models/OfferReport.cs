using MMA.WebApi.Shared.Interfaces.GenericData;
using System;
using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.WebApi.DataAccess.Models
{
    public class OfferReport : IChangeable
    {
        public int Id { get; set; }
        public int OfferId { get; set; }
        public Offer Offer { get; set; }
        public OfferReportType ReportType { get; set; }
        public string Description { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public bool isResolved { get; set; }
        public DateTime? ResolvedOn { get; set; }
    }
}
