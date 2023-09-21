namespace MMA.WebApi.Shared.Models.Email
{
    public class EmailTemplateModel
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Message { get; set; }
        public string Notification { get; set; }
        public int NotificationTypeId { get; set; }
        public int MailTemplateType { get; set; }
        public string Sms { get; set; }
    }
}
