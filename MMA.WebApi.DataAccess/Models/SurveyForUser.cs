using MMA.WebApi.Shared.Interfaces.GenericData;

using System;

namespace MMA.WebApi.DataAccess.Models
{
    public class SurveyForUser : IChangeable
    {
        public int Id { get; set; }
        public string Answer { get; set; }
        public string UserId { get; set; }
        public int SurveyId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }

    }
}
