using MMA.WebApi.Shared.Enums;

namespace MMA.WebApi.Shared.Models.Roadshow
{
    public class RoadshowEmailModel
    {
        public int RoadshowId { get; set; }
        public string RoadshowTitle { get; set; }
        public string UserId { get; set; }
        public Declares.MessageTemplateList EmailStatus { get; set; }
    }
}
