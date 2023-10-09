using MMA.WebApi.Shared.Interfaces.GenericData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MMA.WebApi.DataAccess.Models
{
    public class Document : IChangeable
    {
        public Document()
        {
            //AgendaItemAttachments = new HashSet<AgendaItemAttachment>();
            //AgendaItemDocuments = new HashSet<AgendaItemDocument>();
            //DefaultAgendaItemDocuments = new HashSet<DefaultAgendaItemDocument>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string MimeType { get; set; }
        public string StorageType { get; set; }
        public long? Size { get; set; }
        public byte[] Content { get; set; }
        public string StoragePath { get; set; }
        //public ICollection<AgendaItemAttachment> AgendaItemAttachments { get; set; }
        //public ICollection<AgendaItemDocument> AgendaItemDocuments { get; set; }
        //public ICollection<DefaultAgendaItemDocument> DefaultAgendaItemDocuments { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        //public ICollection<AgendaItemActionAttachment> AgendaItemActionAttachments { get; set; }

        public Guid? ParentId { get; set; }
        [ForeignKey("ParentId")]
        public virtual Document Parent { get; set; }

        public virtual ICollection<OfferDocument> OfferDocuments { get; set; }
    }
}
