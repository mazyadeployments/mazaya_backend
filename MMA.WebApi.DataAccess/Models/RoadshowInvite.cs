using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MMA.WebApi.DataAccess.Models
{
    public class RoadshowInvite : IChangeable, IVisitable<IChangeable>
    {
        public int Id { get; set; }
        public virtual ICollection<RoadshowEvent> RoadshowEvents { get; set; } = new List<RoadshowEvent>();
        public Company Company { get; set; }
        [ForeignKey("CompanyId")]
        public int CompanyId { get; set; }
        public Declares.RoadshowInviteStatus Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public Roadshow Roadshow { get; set; }
        [ForeignKey("RoadshowId")]
        public int RoadshowId { get; set; }
        public void Accept(IVisitor<IChangeable> visitor)
        {
            if (visitor != null)
            {
                visitor.Visit(this);
            }
        }
    }
}
