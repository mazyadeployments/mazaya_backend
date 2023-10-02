using Microsoft.AspNetCore.Http;
using MMA.WebApi.Shared.Models.CategoryDocument;
using MMA.WebApi.Shared.Models.Image;
using MMA.WebApi.Shared.Models.MazayacategoryDocument;
using System;
using System.Collections.Generic;
using System.Text;

namespace MMA.WebApi.Shared.Models.MazayaCategory
{
    public class MazayaCategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string image { get; set; }
        public string facilities { get; set; }
        public string[] facilitiesarray { get; set; } 
        public string description { get; set; }
        public IFormFile cat_image { get; set; }
        public bool status { get; set; }
        public string ImageUrl { get; set; }
        public string ImageUrlCrop { get; set; }
        public ICollection<MazayaCategoryDocumentModel> mazayaCategoryDocuments { get; set; }
        public ImageModel ImageModel { get; set; }
        public List<ImageModel> ImageSets { get; set; }
        public ImageUrlsModel ImageUrls { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
    }
}
