using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.Survey;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.Survey
{
    public interface ISurveyForUserService
    {
        Task<PaginationListModel<SurveyModel>> GetAllSurveysForUser(QueryModel queryModel, string user, bool isQuick, bool all);
        Task<SurveyForUserModel> GetSurveyByIdForUser(int id, string user);

        Task<int> RespondToTheSurvey(object data, int id, string userId);
        Task<IEnumerable<object>> AnswersForSurvey(int id);
        Task<int> CountAnswersForSurvey(int id);




    }
}
