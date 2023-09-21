using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MMA.WebApi.DataAccess.Models
{
    public class Tag : IChangeable, IVisitable<IChangeable>
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsPrivate { get; set; }
        public bool IsEditable { get; set; }
        public virtual ICollection<OfferTag> OfferTags { get; set; }
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
