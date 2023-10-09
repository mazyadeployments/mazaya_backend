using System;

namespace MMA.WebApi.Shared.Models.UserNotifications
{
    public class UserNotificationModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int? NotificationTypeId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Url { get; set; }
        public bool Acknowledged { get; set; }
        public DateTime? AcknowledgedOn { get; set; }

        public DateTime CreatedOn { get; set; }
        public string CreatedOnFormatted => $"{CreatedOn:dd.MM.yyyy.}";

        public int? OfferId { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }

        public bool isToday { get; set; }



    }
}
