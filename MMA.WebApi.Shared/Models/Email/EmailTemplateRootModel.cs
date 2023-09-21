namespace MMA.WebApi.Shared.Models.Email
{
    public class EmailTemplateRootModel
    {
        public int Id { get; set; }
        public string MailTemplate { get; set; }
        public string MailBodyFooter { get; set; }
        public string MailApplicationLogin { get; set; }
    }
}
