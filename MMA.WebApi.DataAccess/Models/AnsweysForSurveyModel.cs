using System.Collections.Generic;

namespace MMA.WebApi.DataAccess.Models
{
    public class AnsweysForSurveyModel
    {
        public ICollection<object> AnswersList { get; set; }
        public int NumberOfOpportunit { get; set; }
    }
}
