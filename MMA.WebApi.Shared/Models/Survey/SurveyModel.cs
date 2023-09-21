using System;
using System.Collections.Generic;
using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.WebApi.Shared.Models.Survey
{
    public class SurveyModel
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public string Description { get; set; }

        public SurveyStatus Status { get; set; }
        public object Questions { get; set; }
        public List<QuestionModel> QuestionsWithAnswers { get; set; }
        public PublishModel Publish { get; set; }
        public ICollection<object> Answers { get; set; }
        public bool IsQuickSurvey { get; set; }
        public int Opportunity { get; set; }

    }
}
