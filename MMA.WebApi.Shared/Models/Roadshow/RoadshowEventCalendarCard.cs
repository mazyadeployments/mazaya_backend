using MMA.WebApi.Shared.Models.Companies;
using MMA.WebApi.Shared.Models.DefaultLocations;
using System;
using System.Collections.Generic;
using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.WebApi.Shared.Models.Roadshow
{
    public class RoadshowEventCalendarCard
    {
        public int Id { get; set; }
        public int RoadshowId { get; set; }
        public string RoadshowName { get; set; }
        public int RoadshowInviteId { get; set; }
        public int OffersCount { get; set; }
        public RoadshowStatus Status { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public SupplierModel FocalPoint { get; set; }
        public ICollection<DefaultLocationModel> RoadshowLocations { get; set; } = new List<DefaultLocationModel>();
        public string Description { get; set; }
        public int? CompanyId { get; set; }
        public string CompanyName { get; set; }
    }
}
