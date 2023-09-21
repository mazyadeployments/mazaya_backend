using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Models.ApplicationUser;

namespace MMA.WebApi.Shared.Models.MailStorage
{
    public class EmailDataModel
    {
        public Declares.MessageTemplateList MailTemplateId { get; set; }
        public ApplicationUserModel User { get; set; }
        public bool IsApproved { get; set; }
        public int? OfferId { get; set; }
        public int? ProposalId { get; set; }
        public string CompanyName { get; set; }
        public string RoadshowName { get; set; }
        public string RoadshowLocation { get; set; }
        public string DetailsLink { get; set; }
        public int? SurveyId { get; set; }
        public int? RoadshowId { get; set; }
    }
}
