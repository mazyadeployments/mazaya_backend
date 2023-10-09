using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MMA.WebApi.DataAccess.Models
{
    public class OffersMemberships
    {
        public Guid Id { get; set; }
        public int OfferId { get; set; }
        [ForeignKey("OfferId")]
        public virtual Offer Offer { get; set; }
        public Guid MembershipId { get; set; }
        [ForeignKey("MembershipId")]
        public virtual Membership Membership { get; set; }

    }
}
