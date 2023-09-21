using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MMA.WebApi.DataAccess.Models
{
    public class Roadshow : IChangeable, IVisitable<IChangeable>
    {
        public int Id { get; set; }
        public ICollection<RoadshowDocument> Documents { get; set; } = new List<RoadshowDocument>();
        public ICollection<RoadshowVoucher> RoadshowVouchers { get; set; } = new List<RoadshowVoucher>();
        public ICollection<RoadshowLocation> Locations { get; set; } = new List<RoadshowLocation>();
        public ICollection<RoadshowInvite> RoadshowInvites { get; set; } = new List<RoadshowInvite>();
        public virtual ICollection<RoadshowComment> RoadshowComments { get; set; } = new List<RoadshowComment>();
        public Declares.RoadshowStatus Status { get; set; }
        public string Title { get; set; }
        public int? CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public Company Company { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string Description { get; set; }
        public string Activities { get; set; }
        public string InstructionBox { get; set; }
        public string FocalPointName { get; set; }
        public string FocalPointSurname { get; set; }
        public string FocalPointEmail { get; set; }

        //Focal point phone
        public string CountryCode { get; set; }
        public string E164Number { get; set; }
        public string InternationalNumber { get; set; }
        public string Number { get; set; }

        public Guid? EmiratesId { get; set; }

        [ForeignKey("EmiratesId")]
        public virtual Document EmiratesIdDocument { get; set; }
        public DateTime CreatedOn { get; set; }
        [MaxLength(1000)]
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }

        [MaxLength(1000)]
        public string UpdatedBy { get; set; }
        public void Accept(IVisitor<IChangeable> visitor)
        {
            if (visitor != null)
            {
                visitor.Visit(this);
            }
        }
    }
}
