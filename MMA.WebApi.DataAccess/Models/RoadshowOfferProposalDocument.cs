using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Visitor;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static MMA.WebApi.Shared.Enums.Document.DocumentDeclares;

namespace MMA.WebApi.DataAccess
{
    public class RoadshowOfferProposalDocument : IChangeable, IVisitable<IChangeable>
    {
        public int Id { get; set; }

        public int RoadshowOfferProposalId { get; set; }
        public Guid DocumentId { get; set; }

        [ForeignKey("DocumentId")]
        public virtual Document Document { get; set; }

        public DateTime CreatedOn { get; set; }

        [MaxLength(1000)]
        public string CreatedBy { get; set; }

        public DateTime UpdatedOn { get; set; }

        [MaxLength(1000)]
        public string UpdatedBy { get; set; }

        public OfferDocumentType Type { get; set; }

        public void Accept(IVisitor<IChangeable> visitor)
        {
            if (visitor != null)
            {
                visitor.Visit(this);
            }
        }
    }
}
