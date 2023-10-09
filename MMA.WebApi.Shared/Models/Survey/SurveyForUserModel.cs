using System;
using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.WebApi.Shared.Models.Survey
{
    public class SurveyForUserModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string AdminId { get; set; }
        public int SurveyId { get; set; }
        public string SurveyTitle { get; set; }
        public object Questions { get; set; }
        public UsersSurveyStatus UserStatus { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public string Description { get; set; }
        public SurveyStatus SurveyStatus { get; set; }

    }
}
