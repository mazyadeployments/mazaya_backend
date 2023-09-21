using System;
using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.WebApi.Shared.Models.Document
{
    public class DocumentAttachment
    {
        public int AgendaItemId { get; set; }
        public Guid AttachmentId { get; set; }
        public string AgendaItemName { get; set; }
        public string AgendaItemDocumentName { get; set; }
        public int? AgendaItemParentId { get; set; }
        public string AgendaItemOrderNo { get; set; }
        public string DocumentName { get; set; }
        public string DocumentType { get; set; }
        public int Revision { get; set; }
        public string UserName { get; set; }
        public string UserRole { get; set; }
        public int? AgendaItemDocumentId { get; set; }
        public int? AgendaItemSectionId { get; set; }
        public string SectionName { get; set; }

        public AttachmentType AttachmentType { get; set; }
        public string AttachmentTypeName { get; set; }
    }
}
