namespace MMA.WebApi.Shared.Models.MailStorage
{
    public class MailTemplateModel
    {
        public string Text { get; set; } = string.Empty;
        public string SubjectTitle1 { get; set; } = string.Empty;
        public string SubjectTitle2 { get; set; } = string.Empty;
        public string SubjectTitle3 { get; set; } = string.Empty;
        public string SubjectText1 { get; set; } = string.Empty;
        public string SubjectText2 { get; set; } = string.Empty;
        public string SubjectText3 { get; set; } = string.Empty;
        public string DetailsText { get; set; } = string.Empty;
        public string DetailsLink { get; set; } = string.Empty;
    }
}
