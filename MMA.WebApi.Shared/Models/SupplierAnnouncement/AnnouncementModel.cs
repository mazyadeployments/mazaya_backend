using MMA.WebApi.Shared.Models.SupplierAnnouncement;
using System.Collections.Generic;
using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.WebApi.Shared.Models.Announcement
{
    public class AnnouncementModel
    {
        public int Id { get; set; }
        public ICollection<int> CategoriesBuyer { get; set; }
        public ICollection<int> CategoriesSupplier { get; set; }
        public AnnouncementAttachmentModel[] Attachments { get; set; }
        public string AnnouncementText { get; set; }
        public bool AllSuppliers { get; set; }
        public bool AllBuyers { get; set; }
        public AnnouncementStatus Status { get; set; }
        public string CreatedBy { get; set; }
    }
}
