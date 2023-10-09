using MMA.WebApi.Shared.Models.Survery.Answers;
using System.Collections.Generic;

namespace MMA.WebApi.Shared.Models.Survey
{
    public class QuestionModel
    {
        public string Id { get; set; }
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }
        public object Render { get; set; }
        public List<MyAnswer> MyAnswers { get; set; }
    }
}
