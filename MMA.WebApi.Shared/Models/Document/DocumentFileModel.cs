using System;

namespace MMA.WebApi.Shared.Models.Document
{
    public class DocumentFileModel : DocumentFileBaseModel
    {

        public byte[] Content { get; set; }
        public string StoragePath { get; set; }
        public string MimeType { get; set; }
        public long? Size { get; set; }
        public Guid? ParentId { get; set; }
    }
}
