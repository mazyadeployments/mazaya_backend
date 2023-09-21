using System;

namespace MMA.WebApi.DataAccess.Models.Views
{
    public class OfferImages
    {
        public int OfferId { get; set; }
        public Guid DocumentId { get; set; }
        public int ImageType { get; set; }
        public bool Cover { get; set; }
    }
}
