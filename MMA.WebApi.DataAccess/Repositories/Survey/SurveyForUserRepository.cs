using Microsoft.EntityFrameworkCore;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Extensions;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.ApplicationUsers;
using MMA.WebApi.Shared.Interfaces.Survey;
using MMA.WebApi.Shared.Models.Survey;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.WebApi.DataAccess.Repositories.Survey
{
    public class SurveyForUserRepository
        : BaseRepository<SurveyForUserModel>,
            ISurveyForUserRepository
    {
        private readonly IApplicationUsersRepository _applicationUsersRepository;

        public SurveyForUserRepository(
            Func<MMADbContext> contexFactory,
            IApplicationUsersRepository applicationUsersRepository
        )
            : base(contexFactory)
        {
            _applicationUsersRepository = applicationUsersRepository;
        }

        #region get
        public IQueryable<SurveyModel> GetAllForUser(
            QueryModel queryModel,
            string userId,
            bool isQuick,
            bool all
        )
        {
            var context = ContextFactory();
            var respondSurveyId = context.SurveyForUsers
                .Where(x => x.UserId == userId)
                .Select(x => x.SurveyId);

            var user = context.Users
                .Include(x => x.UserDomain)
                .Where(x => x.Id == userId)
                .FirstOrDefault();
            var userRole = _applicationUsersRepository.GetUserRolesId(user.Id).FirstOrDefault();

            var retVal = context.Surveys
                .Where(
                    x =>
                        (
                            x.UserTypes.Contains(user.UserDomain.Id.ToString())
                            || x.UserRoles.Contains(userRole)
                            || x.UsersId.Contains(user.Id)
                            || x.ForAllUsers
                        )
                        && x.Status == SurveyStatus.Published.ToString()
                        && !x.IsDeleted
                        && !respondSurveyId.Contains(x.Id)
                        && x.Start <= DateTime.UtcNow
                        && x.End > DateTime.UtcNow
                        && x.Title.ToLower().Contains(queryModel.Filter.Keyword.ToLower())
                        && (all || x.IsQuickSurvey == isQuick)
                )
                .Select(
                    x =>
                        new SurveyModel()
                        {
                            Id = x.Id,
                            Title = x.Title,
                            Start =
                                x.Start != null
                                    ? x.Start.Value.SpecifyKind(DateTimeKind.Utc)
                                    : x.Start,
                            End = x.End != null ? x.End.Value.SpecifyKind(DateTimeKind.Utc) : x.End,
                            Description = x.Description,
                            IsQuickSurvey = x.IsQuickSurvey,
                            Status = Enum.Parse<SurveyStatus>(x.Status),
                            Questions = JsonConvert.DeserializeObject(x.Questions),
                        }
                );
            ;
            retVal = Sort(retVal, queryModel);
            return retVal;
        }

        public async Task<SurveyForUserModel> GetSurveyForUser(int id, string user)
        {
            var context = ContextFactory();
            //proveriti da li je vec odgovorio
            var answer = context.SurveyForUsers
                .Where(x => x.SurveyId == id && x.CreatedBy == user)
                .FirstOrDefault();
            if (answer != null)
            {
                return new SurveyForUserModel() { Id = -1 };
            }
            var temp = context.Surveys.Where(x => x.Id == id).FirstOrDefault();
            var survey = context.Surveys
                .Where(
                    x =>
                        x.Id == id
                        && x.Status == SurveyStatus.Published.ToString()
                        && x.End >= DateTime.UtcNow
                )
                .FirstOrDefault();
            if (survey == null || survey.Start > DateTime.UtcNow)
            {
                return new SurveyForUserModel() { Id = -2 };
            }
            var retval = new SurveyForUserModel()
            {
                Id = survey.Id,
                UserId = user,
                AdminId = survey.AdminId,
                SurveyId = survey.Id,
                SurveyTitle = survey.Title,
                SurveyStatus = Enum.Parse<SurveyStatus>(survey.Status),
                Questions = JsonConvert.DeserializeObject(survey.Questions),
                UserStatus = Declares.UsersSurveyStatus.InProgres,
                Start = survey.Start,
                End = survey.End,
                Description = survey.Description
            };
            return retval;
        }
        #endregion

        public async Task<int> RespondToTheSurvey(object data, int id, string userId)
        {
            var context = ContextFactory();
            var tempRealSurvey = context.Surveys.Where(x => x.Id == id).FirstOrDefault();
            var tempsurvey = context.SurveyForUsers.Where(
                x => x.SurveyId == id && x.UserId == userId
            );
            if (
                tempsurvey.Count() > 0
                || !(tempRealSurvey.Status == SurveyStatus.Published.ToString())
            )
            {
                return 0;
            }

            var newSurvey = new SurveyForUser()
            {
                UserId = userId,
                SurveyId = id,
                Answer = JsonConvert.SerializeObject(data),
                CreatedBy = userId,
                UpdatedBy = userId,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow
            };
            context.SurveyForUsers.Add(newSurvey);
            context.SaveChanges();
            return id;
        }

        protected override IQueryable<SurveyForUserModel> GetEntities()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<object>> AnswersForSurvey(int id)
        {
            var context = ContextFactory();

            var allAnswers = context.SurveyForUsers
                .Where(x => x.SurveyId == id)
                .Select(x => x.Answer)
                .Select(x => JsonConvert.DeserializeObject(x))
                .ToList();
            return allAnswers;
        }

        public async Task<int> CountAnswersForSurvey(int id)
        {
            var context = ContextFactory();

            var allAnswers = context.SurveyForUsers
                .Where(x => x.SurveyId == id)
                .Select(x => x.Answer)
                .Select(x => JsonConvert.DeserializeObject(x))
                .ToList()
                .Count();
            return allAnswers;
        }

        #region private




        private static IQueryable<SurveyModel> Sort(
            IQueryable<SurveyModel> Surveys,
            QueryModel queryModel
        )
        {
            if (queryModel.Sort.Order == Order.DESC)
            {
                return Surveys.OrderByDescending(s => s.Id);
            }
            else
            {
                return Surveys.OrderBy(x => x.Id);
            }
        }

        #endregion
    }
}
