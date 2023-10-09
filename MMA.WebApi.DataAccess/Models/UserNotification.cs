using MMA.WebApi.Shared.Interfaces.GenericData;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MMA.WebApi.DataAccess.Models
{
    public class UserNotification : IChangeable
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser AspNetUser { get; set; }
        public int? NotificationTypeId { get; set; }
        //[ForeignKey("NotificationTypeId")]
        //public virtual NotificationType NotificationType { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string URL { get; set; }
        public bool Acknowledged { get; set; }
        public DateTime? AcknowledgedOn { get; set; }

        public int? OfferId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
    }
}
