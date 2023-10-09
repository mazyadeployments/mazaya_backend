using System;
using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.WebApi.Shared.Models.Offer
{
    public class OfferReportModel
    {
        public int Id { get; set; }
        public int OfferId { get; set; }
        public OfferReportType ReportType { get; set; }
        public string Description { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool isResolved { get; set; }
        public DateTime? ResolvedOn { get; set; }


    }
}
