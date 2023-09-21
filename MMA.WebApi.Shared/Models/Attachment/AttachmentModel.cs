using System;

namespace MMA.WebApi.Shared.Models.Attachment
{
    public class AttachmentModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public long? Size { get; set; }
        public string Url { get; set; }
        public DateTime LastModified { get; set; }
    }
}
