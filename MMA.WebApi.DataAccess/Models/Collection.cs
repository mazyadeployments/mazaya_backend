using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;

namespace MMA.WebApi.DataAccess.Models
{
    public class Collection : IChangeable, IVisitable<IChangeable>
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }
        public bool HomeVisible { get; set; }

        public DateTime? ValidUntil { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime UpdatedOn { get; set; }

        public string UpdatedBy { get; set; }

        public virtual ICollection<OfferCollection> OfferCollections { get; set; }
        public virtual ICollection<CollectionDocument> CollectionDocuments { get; set; }

        /// <summary>
        /// Accepts visit by visitor
        /// </summary>
        /// <param name="visitor">Visitor object can be null</param>
        public void Accept(IVisitor<IChangeable> visitor)
        {
            if (visitor != null)
            {
                visitor.Visit(this);
            }
        }
    }
}
