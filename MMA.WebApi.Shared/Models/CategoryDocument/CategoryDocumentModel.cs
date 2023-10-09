using MMA.WebApi.Shared.Models.Category;
using MMA.WebApi.Shared.Models.Document;
using System;
using static MMA.WebApi.Shared.Enums.Document.DocumentDeclares;

namespace MMA.WebApi.Shared.Models.CategoryDocument
{
    public class CategoryDocumentModel
    {
        public int Id { get; set; }

        public int CategoryId { get; set; }

        public virtual CategoryModel Category { get; set; }

        public Guid DocumentId { get; set; }

        public virtual DocumentFileModel Document { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime UpdatedOn { get; set; }

        public string UpdatedBy { get; set; }

        public OfferDocumentType Type { get; set; }
        public Guid OriginalImageId { get; set; }
        public double X1 { get; set; }
        public double Y1 { get; set; }
        public double X2 { get; set; }
        public double Y2 { get; set; }
        public double cropX1 { get; set; }
        public double cropY1 { get; set; }
        public double cropX2 { get; set; }
        public double cropY2 { get; set; }
    }
}
