using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;

namespace MMA.WebApi.DataAccess.Models
{
    public class Category : IChangeable, IVisitable<IChangeable>
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }
        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime UpdatedOn { get; set; }

        public string UpdatedBy { get; set; }

        public virtual ICollection<OfferCategory> OfferCategories { get; set; }
        public virtual ICollection<CategoryDocument> CategoryDocuments { get; set; }

        public void Accept(IVisitor<IChangeable> visitor)
        {
            if (visitor != null)
            {
                visitor.Visit(this);
            }
        }
    }
}
