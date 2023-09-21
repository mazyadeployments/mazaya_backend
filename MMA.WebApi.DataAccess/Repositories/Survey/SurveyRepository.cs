using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Extensions;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.ApplicationUsers;
using MMA.WebApi.Shared.Interfaces.MailStorage;
using MMA.WebApi.Shared.Interfaces.Survey;
using MMA.WebApi.Shared.Interfaces.UserNotifications;
using MMA.WebApi.Shared.Models.ApplicationUser;
using MMA.WebApi.Shared.Models.Survey;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMA.WebApi.DataAccess.Repositories.Survey
{
    public class SurveyRepository : BaseRepository<SurveyModel>, ISurveyRepository
    {
        private readonly ILogger _logger;
        private readonly IMailStorageService _mailStorageService;
        private readonly IApplicationUserService _applicationUserService;
        private readonly IUserNotificationService _userNotificationService;

        public SurveyRepository(
            Func<MMADbContext> contexFactory,
            IApplicationUserService applicationUserService,
            IMailStorageService mailStorageService,
            ILogger<SurveyRepository> logger,
            IUserNotificationService userNotificationService
        )
            : base(contexFactory)
        {
            _mailStorageService = mailStorageService;
            _applicationUserService = applicationUserService;
            _logger = logger;
            _userNotificationService = userNotificationService;
        }

        #region get
        public async Task<IQueryable<SurveyModel>> GetAll(QueryModel queryModel)
        {
            var context = ContextFactory();
            var temp = context.Surveys.Where(
                survey =>
                    !survey.IsDeleted
                    && survey.Title.ToLower().Contains(queryModel.Filter.Keyword.Trim().ToLower())
            );

            temp = FilterSurvey(temp, queryModel);
            return Sort(temp, queryModel)
                .Select(
                    x =>
                        new SurveyModel()
                        {
                            Title = x.Title,
                            Id = x.Id,
                            Start =
                                x.Start != null
                                    ? x.Start.Value.SpecifyKind(DateTimeKind.Utc)
                                    : x.Start,
                            End = x.End != null ? x.End.Value.SpecifyKind(DateTimeKind.Utc) : x.End,
                            Description = x.Description,
                            Status = Enum.Parse<Declares.SurveyStatus>(x.Status),
                            Questions = JsonConvert.DeserializeObject(x.Questions),
                            Opportunity = x.Opportunity
                        }
                );
        }

        public async Task<SurveyModel> GetSurvey(int id)
        {
            var context = ContextFactory();
            var retVal = await context.Surveys
                .Where(survey => survey.Id == id)
                .FirstOrDefaultAsync();
            if (retVal != null)
            {
                var retVal2 = new SurveyModel();
                CreateSurveyModelfromSurvey(retVal2, retVal);
                return retVal2;
            }
            else
                return null;
        }

        protected override IQueryable<SurveyModel> GetEntities()
        {
            var context = ContextFactory();
            var data = context.Surveys.ToArray();
            ICollection<SurveyModel> retValList = new HashSet<SurveyModel>();
            foreach (var s in data)
            {
                var temp = new SurveyModel();
                CreateSurveyModelfromSurvey(temp, s);
                retValList.Add(temp);
            }
            return (IQueryable<SurveyModel>)retValList;
        }
        #endregion
        #region delete
        public async Task<bool> RemoveSurvey(int id, string user)
        {
            var context = ContextFactory();
            var survey = await context.Surveys.FirstOrDefaultAsync(x => x.Id == id);
            if (survey != null)
            {
                survey.UpdatedBy = user;
                survey.UpdatedOn = DateTime.UtcNow;
                survey.IsDeleted = true;
                context.Update(survey);

                await context.SaveChangesAsync();
                return true;
            }
            else
                return false;
        }
        #endregion
        #region add insert update
        public async Task<int> InsertSurvey(SurveyModel data, string user)
        {
            var context = ContextFactory();
            var survey = new Models.Survey();
            CreateSurveyFromModel(data, survey, user);

            await context.Surveys.AddAsync(survey);
            await context.SaveChangesAsync();
            return survey.Id;
        }

        public async Task<SurveyModel> SetToScheduled(int surveyiId, string suplierId)
        {
            var context = ContextFactory();
            var survey = context.Surveys.Where(x => x.Id == surveyiId).FirstOrDefault();
            if (
                survey == null
                || survey.Status != Declares.SurveyStatus.Draft.ToString()
                || survey.End == null
                || survey.Start == null
            )
                return new SurveyModel() { Id = -1 };

            survey.Status = Declares.SurveyStatus.Scheduled.ToString();
            survey.UpdatedBy = suplierId;
            survey.UpdatedOn = DateTime.UtcNow;
            context.Update(survey);
            await context.SaveChangesAsync();
            var retval = new SurveyModel();

            CreateSurveyModelfromSurvey(retval, survey);

            return retval;
        }

        public async Task<SurveyModel> SetToDraft(int surveyiId, string suplierId)
        {
            var context = ContextFactory();
            var survey = context.Surveys.Where(x => x.Id == surveyiId).FirstOrDefault();
            if (survey == null || survey.Status != Declares.SurveyStatus.Scheduled.ToString())
                return new SurveyModel() { Id = -1 };

            survey.Status = Declares.SurveyStatus.Draft.ToString();
            survey.UpdatedBy = suplierId;
            survey.UpdatedOn = DateTime.UtcNow;
            survey.IsCreateMail = false;

            context.Update(survey);
            await context.SaveChangesAsync();
            var retval = new SurveyModel();

            CreateSurveyModelfromSurvey(retval, survey);

            return retval;
        }

        public async Task<int> DuplicateSurvey(int id)
        {
            var context = ContextFactory();
            var survey = context.Surveys.Where(x => x.Id == id).FirstOrDefault();
            if (survey == null || survey.Status == Declares.SurveyStatus.Draft.ToString())
                return 0;
            var newSurvey = DupicatesurveyAndSetToDraft(survey);
            context.Surveys.Add(newSurvey);
            await context.SaveChangesAsync();

            return newSurvey.Id;
        }

        public async Task<int> UpdateSurvey(SurveyModel data, string userId)
        {
            var context = ContextFactory();
            var survey = context.Surveys.Where(x => x.Id == data.Id).FirstOrDefault();
            if (survey == null)
                return 0;
            UpdateSuervey(data, survey, userId);

            context.Surveys.Update(survey);
            await context.SaveChangesAsync();

            return survey.Id;
        }

        public async Task<SurveyModel> CloseSurvey(int surveyiId, string suplierId)
        {
            var context = ContextFactory();
            var survey = context.Surveys.Where(x => x.Id == surveyiId).FirstOrDefault();
            if (survey == null || survey.Status != Declares.SurveyStatus.Published.ToString())
                return new SurveyModel() { Id = -1 };

            survey.Status = Declares.SurveyStatus.Closed.ToString();
            survey.UpdatedBy = suplierId;
            survey.UpdatedOn = DateTime.UtcNow;
            context.Update(survey);
            await context.SaveChangesAsync();
            var retval = new SurveyModel();

            CreateSurveyModelfromSurvey(retval, survey);

            return retval;
        }
        #endregion

        #region publish
        private async void PublishLocal(Models.Survey survey)
        {
            var context = ContextFactory();
            if (survey == null || survey.Status == Declares.SurveyStatus.Published.ToString())
            {
                return;
            }

            survey.UpdatedOn = DateTime.UtcNow;
            survey.Status = Declares.SurveyStatus.Published.ToString();
            survey.Start = DateTime.UtcNow;
            context.Update(survey);
            await context.SaveChangesAsync();
        }

        public async Task<bool> PublishSurvey(int surveyId, string userId)
        {
            var context = ContextFactory();
            var survey = await context.Surveys.Where(x => x.Id == surveyId).FirstOrDefaultAsync();
            if (survey == null || survey.Status == Declares.SurveyStatus.Published.ToString())
            {
                return false;
            }

            survey.UpdatedBy = userId;
            survey.UpdatedOn = DateTime.UtcNow;
            survey.Status = Declares.SurveyStatus.Published.ToString();
            survey.Start = DateTime.UtcNow;
            context.Surveys.Update(survey);
            await context.SaveChangesAsync();
            var surveyModel = new SurveyModel();
            CreateSurveyModelfromSurvey(surveyModel, survey);
            await FindAllUsersAndSetNumberOfOpportynity(surveyModel, userId);

            return true;
        }
        #endregion

        #region private
        private static IQueryable<Models.Survey> FilterSurvey(
            IQueryable<Models.Survey> Surveys,
            QueryModel queryModel
        )
        {
            Surveys = Surveys.Where(
                survey =>
                    survey.Title.ToLower().Contains(queryModel.Filter.Keyword.Trim().ToLower())
            );

            if (queryModel.Filter.Status.Any())
            {
                Surveys = Surveys.Where(o => queryModel.Filter.Status.Contains(o.Status));
                var temp = Surveys.ToList();
            }

            return Surveys;
        }

        private static IQueryable<Models.Survey> Sort(
            IQueryable<Models.Survey> Surveys,
            QueryModel queryModel
        )
        {
            if (queryModel.Sort.Order == Order.DESC)
            {
                return Surveys.OrderByDescending(s => s.CreatedOn);
            }
            else
            {
                return Surveys.OrderBy(x => x.CreatedOn);
            }
        }

        private void CreateSurveyModelfromSurvey(SurveyModel model, Models.Survey data)
        {
            model.Id = data.Id;
            model.Title = data.Title;
            model.Start =
                data.Start != null ? data.Start.Value.SpecifyKind(DateTimeKind.Utc) : data.Start;
            model.End = data.End != null ? data.End.Value.SpecifyKind(DateTimeKind.Utc) : data.End;

            model.Description = data.Description;
            model.Status = Enum.Parse<Declares.SurveyStatus>(data.Status);
            model.Questions = JsonConvert.DeserializeObject(data.Questions);
            model.Opportunity = data.Opportunity;
            model.Publish = new PublishModel();
            model.Publish.AllUsers = data.ForAllUsers;
            model.Publish.UsersId =
                (data.UsersId != "" && data.UsersId != null) ? data.UsersId.Split(',') : null;
            model.Publish.Roles =
                (data.UserRoles != "" && data.UserRoles != null) ? data.UserRoles.Split(',') : null;
            model.Publish.Types =
                (data.UserTypes != "" && data.UserTypes != null) ? data.UserTypes.Split(',') : null;
            model.IsQuickSurvey = data.IsQuickSurvey;
        }

        private void UpdateSuervey(SurveyModel model, Models.Survey data, string userId)
        {
            data.Title = model.Title;
            data.Start = model.Start;
            data.End = model.End;
            data.Description = model.Description;
            data.ForAllUsers = model.Publish != null ? model.Publish.AllUsers : data.ForAllUsers;
            data.UsersId =
                model.Publish.UsersId != null ? CreateStringFromList(model.Publish.UsersId) : null;
            data.UserRoles =
                model.Publish.Roles != null ? CreateStringFromList(model.Publish.Roles) : null;
            data.UserTypes =
                model.Publish.Types != null ? CreateStringFromList(model.Publish.Types) : null;
            data.Questions = JsonConvert.SerializeObject(model.Questions);
            data.UpdatedOn = DateTime.UtcNow;
            data.UpdatedBy = userId;
            data.IsQuickSurvey = model.IsQuickSurvey;
        }

        private void CreateSurveyFromModel(SurveyModel model, Models.Survey data, string user)
        {
            data.Id = model.Id;
            data.Title = model.Title;

            data.Start = model.Start != null ? model.Start.Value : data.Start;
            data.End = model.End != null ? model.End.Value : data.End;
            data.Description = model.Description;
            data.Status = Declares.SurveyStatus.Draft.ToString();
            data.IsDeleted = false;
            data.Questions = JsonConvert.SerializeObject(model.Questions);
            data.CreatedOn = DateTime.UtcNow;
            data.UpdatedOn = DateTime.UtcNow;
            data.CreatedBy = user;
            data.UpdatedBy = user;
            data.Opportunity = model.Opportunity;
            data.UsersId =
                model.Publish.UsersId != null ? CreateStringFromList(model.Publish.UsersId) : null;
            data.UserRoles =
                model.Publish.Roles != null ? CreateStringFromList(model.Publish.Roles) : null;
            data.UserTypes =
                model.Publish.Types != null ? CreateStringFromList(model.Publish.Types) : null;
            data.ForAllUsers = model.Publish.AllUsers;
            data.IsCreateMail = false;
            data.IsQuickSurvey = model.IsQuickSurvey;
        }

        private string CreateStringFromList(ICollection<string> list)
        {
            string temp = "";
            int i = 0;
            foreach (var str in list.ToHashSet())
            {
                if (str != "")
                    if (i != 0)
                        temp += "," + str;
                    else
                        temp += str;
                i++;
            }

            return temp;
        }

        private Models.Survey DupicatesurveyAndSetToDraft(Models.Survey survey)
        {
            return new Models.Survey()
            {
                Title = survey.Title,
                Start = null,
                End = DateTime.Now.AddMinutes(20),
                AdminId = survey.AdminId,
                Description = survey.Description,
                UsersId = "",
                UserTypes = "",
                UserRoles = "",
                Status = Declares.SurveyStatus.Draft.ToString(),
                Questions = survey.Questions,
                IsDeleted = false,
                ForAllUsers = false,
                CreatedBy = survey.CreatedBy,
                UpdatedBy = survey.UpdatedBy,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow,
                Opportunity = 0,
                IsCreateMail = false,
                IsQuickSurvey = survey.IsQuickSurvey
            };
        }
        #endregion
        private async Task<HashSet<ApplicationUserModel>> GetUsersForSurveyPublishing(
            SurveyModel survey
        )
        {
            int countUser = 0;
            var usersForEmail = new HashSet<ApplicationUserModel>();
            if (
                survey.Publish.AllUsers
                && (survey.Publish.Roles == null)
                && (survey.Publish.Types == null)
                && (survey.Publish.UsersId == null)
            ) //all users
            {
                _logger.LogInformation("AllUsers");
                usersForEmail = _applicationUserService.GetAllUsersForMail().ToHashSet();
                countUser = usersForEmail.Count();
                _logger.LogInformation("number of users: " + countUser);
            }
            else if (
                survey.Publish.Roles != null
                && survey.Publish.Roles.Count > 0
                && (
                    (!survey.Publish.AllUsers)
                    && (survey.Publish.Types == null)
                    && (survey.Publish.UsersId == null)
                )
            ) //roles
            {
                _logger.LogInformation("Roles");
                usersForEmail = _applicationUserService
                    .GetAllUsersByRolesForMail(survey.Publish.Roles)
                    .ToHashSet();
                countUser = usersForEmail.Count();
                _logger.LogInformation("number of users: " + countUser);
            }
            else if (
                survey.Publish.Types != null
                && survey.Publish.Types.Count > 0
                && (
                    (!survey.Publish.AllUsers)
                    && (survey.Publish.Roles == null)
                    && (survey.Publish.UsersId == null)
                )
            ) //types
            {
                _logger.LogInformation("types");
                usersForEmail = (
                    await _applicationUserService.GetUsersByDomain(survey.Publish.Types)
                ).ToHashSet();
                countUser = usersForEmail.Count();
                _logger.LogInformation("number of users: " + countUser);
            }
            else if (
                survey.Publish.UsersId != null
                && survey.Publish.UsersId.Count > 0
                && (
                    (!survey.Publish.AllUsers)
                    && (survey.Publish.Roles == null)
                    && (survey.Publish.Types == null)
                )
            ) //usersId
            {
                _logger.LogInformation("usersId");
                usersForEmail = (
                    await _applicationUserService.GetAllUsersFromList(survey.Publish.UsersId)
                ).ToHashSet();
                countUser = usersForEmail.Count();
                _logger.LogInformation("number of users: " + countUser);
            }
            else
            {
                _logger.LogInformation("Can not find user");
                return null;
            }
            return usersForEmail;
        }

        private async Task SendNotificationsForSurvey(SurveyModel survey, ILogger log)
        {
            var usersForEmail = await GetUsersForSurveyPublishing(survey);

            _logger.LogInformation("Call CreateMailForListUsersForSurvey");
            await _mailStorageService.CreateMailForListUsersForSurvey(
                usersForEmail,
                survey.Id,
                log
            );
            _logger.LogInformation("Call SendPushNotificationForSurveyToListUser");
            foreach (var user in usersForEmail)
            {
                if (user.FcmDevice != null && user.FcmDevice.Count() > 0)
                    await _applicationUserService.SendPushNotificationForSurveyToListUser(
                        user.FcmDevice,
                        survey.Id
                    );
            }
        }

        public async Task FindAllUsersAndSetNumberOfOpportynity(
            SurveyModel survey,
            string suplierId
        )
        {
            int countUser = 0;
            _logger.LogInformation("In background ->Get user data");
            var usersForEmail = await GetUsersForSurveyPublishing(survey);
            countUser = usersForEmail.Count();
            _logger.LogInformation("Call SetNumberOfOpportunity");
            await SetNumberOfOpportunity(survey.Id, countUser);

            _logger.LogInformation("Background job is comleted");
            await _userNotificationService.CreateNotificationForPublishJob(
                suplierId,
                countUser,
                survey.Id
            );
        }

        public async Task CheckSurveysDoBackgroundJobAsync(ILogger logger)
        {
            var currentDate = DateTime.UtcNow;
            var context = ContextFactory();
            int forPublisCount = 0;
            int forExpiredCount = 0;
            var allSurveyForChange = context.Surveys
                .Where(
                    x =>
                        (
                            x.Status == Declares.SurveyStatus.Scheduled.ToString()
                            && x.Start < currentDate
                        )
                        || (
                            x.Status == Declares.SurveyStatus.Published.ToString()
                            && x.End < currentDate
                        )
                )
                .ToHashSet();
            foreach (var survey in allSurveyForChange)
            {
                if (survey.Status == Declares.SurveyStatus.Scheduled.ToString())
                {
                    PublishLocal(survey);
                    var surveyModel = new SurveyModel();
                    CreateSurveyModelfromSurvey(surveyModel, survey);
                    await FindAllUsersAndSetNumberOfOpportynity(surveyModel, survey.CreatedBy);
                    forPublisCount++;
                }
                else if (survey.Status == Declares.SurveyStatus.Published.ToString())
                {
                    survey.Status = Declares.SurveyStatus.Closed.ToString();
                    survey.UpdatedOn = currentDate;
                    forExpiredCount++;
                    context.Surveys.Update(survey);
                    await context.SaveChangesAsync();
                }
            }

            logger.LogInformation("Publised survey models -> " + forPublisCount);
            logger.LogInformation("Expired survey models -> " + forExpiredCount);
        }

        public async Task SetNumberOfOpportunity(int id, int count)
        {
            var context = ContextFactory();
            var survey = context.Surveys.Where(x => x.Id == id).FirstOrDefault();
            if (survey == null)
                return;
            survey.Opportunity = count;
            survey.UpdatedOn = DateTime.UtcNow;
            context.Surveys.Update(survey);

            await context.SaveChangesAsync();
        }

        public async Task<int> GetNumberOfOpportunity(int id)
        {
            var context = ContextFactory();
            var survey = context.Surveys.Where(x => x.Id == id).FirstOrDefault();
            if (survey == null)
                return -1;

            return survey.Opportunity;
        }

        public async Task SendNotificationForSurvey(ILogger log)
        {
            var context = ContextFactory();
            var allSurveysForSendMail = context.Surveys
                .Where(
                    x => (x.Status == Declares.SurveyStatus.Published.ToString() && !x.IsCreateMail)
                )
                .ToList();

            foreach (var survey in allSurveysForSendMail)
            {
                survey.IsCreateMail = true;
                survey.UpdatedOn = DateTime.UtcNow;
            }

            if (allSurveysForSendMail.Count() > 0)
            {
                context.Surveys.UpdateRange(allSurveysForSendMail);
                await context.SaveChangesAsync();
            }

            foreach (var survey in allSurveysForSendMail)
            {
                log.LogInformation("Create notification for survey " + survey.Id);
                var surveyData = new SurveyModel();
                CreateSurveyModelfromSurvey(surveyData, survey);
                await SendNotificationsForSurvey(surveyData, log);
                log.LogInformation("Complete creating notification for survey " + survey.Id);
            }
        }
    }
}
