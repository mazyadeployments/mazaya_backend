using System;
using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.WebApi.Shared.Models.Document
{
    public class DocumentActionAttachment
    {
        public int AgendaItemActionId { get; set; }
        public Guid AttachmentId { get; set; }
        public string AgendaItemActionName { get; set; }
        //public int? AgendaItemParentId { get; set; }
        public string AgendaItemActionOrderNo { get; set; }
        public string DocumentName { get; set; }
        public string DocumentType { get; set; }
        public int Revision { get; set; }
        public string UserName { get; set; }
        public string UserRole { get; set; }

        public AttachmentType AttachmentType { get; set; }
    }
}
