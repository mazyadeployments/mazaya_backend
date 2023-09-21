using MMA.WebApi.Shared.Interfaces.GenericData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MMA.WebApi.DataAccess.Models
{
    public class MailStorage : IChangeable
    {

        public int Id { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser AspNetUser { get; set; }

        public string Subject { get; set; }
        public string Body { get; set; }


        public int? OfferId { get; set; }
        [ForeignKey("OfferId")]
        public virtual Offer Offer { get; set; }
        public DateTime CreatedAt { get; set; }
        public int StatusId { get; set; }
        public DateTime StatusOn { get; set; }
        public string StatusNote { get; set; }
        public string UserEmail { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public int? AnnouncementId { get; set; }
        [ForeignKey("AnnouncementId")]
        public virtual Announcement Announcement { get; set; }
        public virtual ICollection<MailStorageDocument> MailStorageDocuments { get; set; }

    }
}
