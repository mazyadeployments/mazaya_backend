using MMA.WebApi.DataAccess.Extensions;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.Survey;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.Survey;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Core.Services
{
    public class SurveyForUserService : ISurveyForUserService
    {
        private readonly ISurveyForUserRepository _surveyForUserRepository;

        public SurveyForUserService(ISurveyForUserRepository surveyForUserRepository)
        {
            _surveyForUserRepository = surveyForUserRepository;
        }

        public async Task<IEnumerable<object>> AnswersForSurvey(int id)
        {
            return await _surveyForUserRepository.AnswersForSurvey(id);
        }

        public async Task<int> CountAnswersForSurvey(int id)
        {
            return await _surveyForUserRepository.CountAnswersForSurvey(id);
        }

        public async Task<PaginationListModel<SurveyModel>> GetAllSurveysForUser(
            QueryModel queryModel,
            string user,
            bool isQuick,
            bool all
        )
        {
            var allSurveyForUser = _surveyForUserRepository.GetAllForUser(
                queryModel,
                user,
                isQuick,
                all
            );
            return await allSurveyForUser.ToPagedListAsync(
                queryModel.Page,
                queryModel.PaginationParameters.PageSize
            );
        }

        public Task<SurveyForUserModel> GetSurveyByIdForUser(int id, string user)
        {
            return _surveyForUserRepository.GetSurveyForUser(id, user);
        }

        public Task<int> RespondToTheSurvey(object data, int id, string userId)
        {
            return _surveyForUserRepository.RespondToTheSurvey(data, id, userId);
        }
    }
}
