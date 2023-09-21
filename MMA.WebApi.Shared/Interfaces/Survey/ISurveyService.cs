using Microsoft.Extensions.Logging;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.Survey;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.Survey
{
    public interface ISurveyService
    {
        Task<PaginationListModel<SurveyModel>> GetAllSurveysForAdmin(QueryModel queryModel);
        Task<SurveyModel> GetSurvey(int id);
        Task<bool> DeleteSurvey(int id, string user);
        Task<int> InsertSurvey(SurveyModel data, string user);
        Task<int> PublishSurvey(int surveyiId, string suplierId);
        Task<SurveyModel> SetToScheduled(int surveyiId, string suplierId);
        Task<SurveyModel> SetToDraft(int surveyiId, string suplierId);
        Task<SurveyModel> CloseSurvey(int surveyiId, string suplierId);
        Task CheckSurvey(ILogger logger);
        Task<int> DuplicateSurvey(int id);
        Task SetNumberOfOpportunity(int id, int count);
        Task<int> GetNumberOfOpportunity(int id);
        Task<byte[]> CreateExcelFile(SurveyModel data);
        Task<int> UpdateSurvey(SurveyModel data, string user);
        Task SendNotificationForSurvey(ILogger log);

    }
}
