using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MMA.WebApi.DataAccess.Models
{
    public class RoadshowEvent
    {
        public int Id { get; set; }
        public virtual ICollection<RoadshowEventOffer> RoadshowEventOffers { get; set; } = new List<RoadshowEventOffer>();
        [ForeignKey("DefaultLocationId")]
        public int? DefaultLocationId { get; set; }
        public DefaultLocation DefaultLocation { get; set; }

        [ForeignKey("RoadshowInviteId")]
        public int RoadshowInviteId { get; set; }
        public RoadshowInvite RoadshowInvite { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}