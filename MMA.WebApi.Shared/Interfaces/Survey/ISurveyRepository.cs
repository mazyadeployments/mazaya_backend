using Microsoft.Extensions.Logging;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Models.Survey;
using System.Linq;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.Survey
{
    public interface ISurveyRepository
    {
        Task<IQueryable<SurveyModel>> GetAll(QueryModel queryModel);
        Task<SurveyModel> GetSurvey(int id);
        Task<int> InsertSurvey(SurveyModel data, string user);
        Task<bool> RemoveSurvey(int id, string user);
        Task<bool> PublishSurvey(int surveyId, string suplierId);

        Task FindAllUsersAndSetNumberOfOpportynity(SurveyModel survey, string suplierId);
        Task CheckSurveysDoBackgroundJobAsync(ILogger logger);
        Task<int> DuplicateSurvey(int id);
        Task SetNumberOfOpportunity(int id, int count);
        Task<int> GetNumberOfOpportunity(int id);
        Task<int> UpdateSurvey(SurveyModel data, string userId);
        Task<SurveyModel> SetToScheduled(int surveyiId, string suplierId);
        Task<SurveyModel> CloseSurvey(int surveyiId, string suplierId);
        Task<SurveyModel> SetToDraft(int surveyiId, string suplierId);
        Task SendNotificationForSurvey(ILogger log);

    }


}

