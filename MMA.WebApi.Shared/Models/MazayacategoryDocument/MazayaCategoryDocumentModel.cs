using MMA.WebApi.Shared.Models.Category;
using MMA.WebApi.Shared.Models.Document;
using MMA.WebApi.Shared.Models.MazayaCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Document.DocumentDeclares;

namespace MMA.WebApi.Shared.Models.MazayacategoryDocument
{
    public class MazayaCategoryDocumentModel
    {
        public int Id { get; set; }

        public int mazayacategoryId { get; set; }

        public virtual MazayaCategoryModel MazayaCategory { get; set; }

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
