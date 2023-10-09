using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MMA.WebApi.DataAccess.Models
{
    public class AnnouncementAttachments
    {
        public int Id { get; set; }
        public int AnnouncementId { get; set; }

        [ForeignKey("AnnouncementId")]
        public virtual Announcement Announcement { get; set; }
        public Guid DocumentId { get; set; }

        [ForeignKey("DocumentId")]
        public virtual Document Document { get; set; }
    }
}
