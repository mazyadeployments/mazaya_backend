using MMA.WebApi.Shared.Models.CollectionDocument;
using MMA.WebApi.Shared.Models.Image;
using System;
using System.Collections.Generic;

namespace MMA.WebApi.Shared.Models.Collection
{
    public class CollectionModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }
        public DateTime? ValidUntil { get; set; }
        public DateTime? ValidFrom { get; set; }
        public IEnumerable<int> OffersIds { get; set; }
        public ICollection<CollectionDocumentModel> CollectionDocuments { get; set; }
        public ImageModel Image { get; set; }
        public List<ImageModel> ImageSets { get; set; }
        public ImageUrlsModel ImageUrls { get; set; }
        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime UpdatedOn { get; set; }

        public string UpdatedBy { get; set; }
        public bool HomeVisible { get; set; }
    }
}
