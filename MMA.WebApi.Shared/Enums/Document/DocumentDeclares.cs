using System.ComponentModel.DataAnnotations;

namespace MMA.WebApi.Shared.Enums.Document
{
    public static class DocumentDeclares
    {
        public enum OfferDocumentType
        {
            [Display(Name = "Document")]
            Document = 1,

            [Display(Name = "ImageOriginal")]
            Original = 2,

            [Display(Name = "ImageThumbnail")]
            Thumbnail = 3,

            [Display(Name = "ImageLarge")]
            Large = 4,

            [Display(Name = "QRCode")]
            QRCode = 5,

            [Display(Name = "Video")]
            Video = 6,
        }
    }
}
