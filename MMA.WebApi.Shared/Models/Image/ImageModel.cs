using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Enums.Document;
using System;
using System.Collections.Generic;

namespace MMA.WebApi.Shared.Models.Image
{
    public class ImageModel
    {
        public CropCoordinates CropCoordinates { get; set; }
        public CropCoordinates CropNGXCoordinates { get; set; }
        public string Id { get; set; }
        public bool Cover { get; set; }
        public DocumentDeclares.OfferDocumentType Type { get; set; }
        public Guid OriginalImageId { get; set; }
    }

    public class ImageBackgroundUploadModel
    {
        public IEnumerable<ImageModel> CroppedImages { get; set; }
        public Declares.ImageForType ImageForType { get; set; }
    }
}
