using System;

namespace MMA.WebApi.Shared.Models.Domain.Emails
{
    public class MailStorage
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime CreatedAt { get; set; }
        public int StatusId { get; set; }
        public DateTime StatusOn { get; set; }
        public string StatusNote { get; set; }
        public string UserEmail { get; set; }
        // public virtual ICollection<Attachment> Attachments { get; set; }

    }
}
