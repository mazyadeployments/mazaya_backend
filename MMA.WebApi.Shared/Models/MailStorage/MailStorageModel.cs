using System;

namespace MMA.WebApi.Shared.Models.MailStorage
{
    public class MailStorageModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int? MeetingId { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public int? OfferId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public int StatusId { get; set; }
        public DateTime StatusOn { get; set; }
        public string StatusNote { get; set; }
        public string UserEmail { get; set; }
    }
}
