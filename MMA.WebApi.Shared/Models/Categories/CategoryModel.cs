using MMA.WebApi.Shared.Models.CategoryDocument;
using MMA.WebApi.Shared.Models.Image;
using System;
using System.Collections.Generic;

namespace MMA.WebApi.Shared.Models.Category
{
    public class CategoryModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public string ImageUrlCrop { get; set; }
        public int NumberOfOffers { get; set; }
        public int OffersCount { get; set; }
        public IEnumerable<int> OffersIds { get; set; }
        public ICollection<CategoryDocumentModel> CategoryDocuments { get; set; }
        public ImageModel Image { get; set; }
        public List<ImageModel> ImageSets { get; set; }
        public ImageUrlsModel ImageUrls { get; set; }
        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime UpdatedOn { get; set; }

        public string UpdatedBy { get; set; }
    }
}
