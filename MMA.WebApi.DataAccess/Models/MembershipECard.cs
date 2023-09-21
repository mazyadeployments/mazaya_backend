using MMA.WebApi.Shared.Interfaces.GenericData;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MMA.WebApi.DataAccess.Models
{
    public class MembershipECard : IChangeable
    {
        public int Id { get; set; }
        public string OwnerId { get; set; }//
        public ApplicationUser Owner { get; set; }
        public string MemberId { get; set; }//
        public string MemberEmail { get; set; }//
        public string Name { get; set; }
        public string Surname { get; set; }
        public string ECardSequence { get; set; }
        public int MembershipType { get; set; }
        public bool IsMembershipCard { get; set; }
        public string PhotoUrl { get; set; }
        public bool isMember { get; set; }
        public Guid? MembershipId { get; set; }

        [ForeignKey("MembershipId")]
        public Membership Membership { get; set; }
        public DateTime ValidTo { get; set; }
        public string Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? sys_updated_on { get; set; }

    }
}
