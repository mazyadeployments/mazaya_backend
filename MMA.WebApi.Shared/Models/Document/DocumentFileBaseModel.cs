using System;

namespace MMA.WebApi.Shared.Models.Document
{
    public class DocumentFileBaseModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string StorageType { get; set; }
        public DateTime? UploadedOn { get; set; }
    }
}
