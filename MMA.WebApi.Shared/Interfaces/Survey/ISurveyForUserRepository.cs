using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Models.Survey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.Survey
{
    public interface ISurveyForUserRepository
    {
        IQueryable<SurveyModel> GetAllForUser(QueryModel queryModel, string userId, bool isQuick, bool all);
        Task<SurveyForUserModel> GetSurveyForUser(int id, string user);
        Task<int> RespondToTheSurvey(object data, int id, string userId);
        Task<IEnumerable<Object>> AnswersForSurvey(int id);
        Task<int> CountAnswersForSurvey(int id);




    }
}
