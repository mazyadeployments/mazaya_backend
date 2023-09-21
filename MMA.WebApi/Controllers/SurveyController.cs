using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.Roles;
using MMA.WebApi.Shared.Interfaces.Survey;
using MMA.WebApi.Shared.Models.Survey;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Controllers
{
    [Route("[controller]")]
    [Authorize("Bearer")]
    [ApiController]
    public class SurveyController : BaseController
    {
        private readonly ISurveyService _surveyService;
        private readonly IRoleService _roleService;
        private readonly ISurveyForUserService _surveyForUserService;




        public SurveyController(ISurveyService surveyService, IRoleService roleService, ISurveyForUserService surveyForUserService)
        {
            _surveyService = surveyService;
            _roleService = roleService;
            _surveyForUserService = surveyForUserService;
        }

        [HttpPost("surveys")]
        public async Task<IActionResult> GetAllSurveys(QueryModel queryModel)
        {

            return Ok(await _surveyForUserService.GetAllSurveysForUser(queryModel, UserId, false, true));
        }
        [HttpPost("normal-surveys")]
        public async Task<IActionResult> GetAllNormalSurveys(QueryModel queryModel)
        {

            return Ok(await _surveyForUserService.GetAllSurveysForUser(queryModel, UserId, false, false));
        }
        [HttpPost("quick-surveys")]
        public async Task<IActionResult> GetAllQuickSurveys(QueryModel queryModel)
        {

            return Ok(await _surveyForUserService.GetAllSurveysForUser(queryModel, UserId, true, false));
        }

        [HttpPost("admin-surveys")]
        public async Task<IActionResult> GetAllSurveysForAdmin(QueryModel queryModel)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                return Ok(await _surveyService.GetAllSurveysForAdmin(queryModel));
            }

            return BadRequest("You don't have permission");
        }



        [HttpGet("surveys/{id}/admin")]
        public async Task<IActionResult> GetSurveyForAdminById(int id)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                return Ok(await _surveyService.GetSurvey(id));
            }
            return BadRequest("You don't have permission");
        }
        [HttpGet("surveys/{id}")]
        public async Task<IActionResult> GetSurveyById(int id)
        {

            return Ok(await _surveyForUserService.GetSurveyByIdForUser(id, UserId));
        }

        [HttpGet("surveys/{id}/publish")]
        public async Task<IActionResult> PublishSurvey(int id)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {

                int result = await _surveyService.PublishSurvey(id, UserId);
                switch (result)
                {
                    case -2:
                        return BadRequest("Select users for survey");
                    case -1:
                        return BadRequest("Survey is already published");

                    case 0:
                        return BadRequest("Survey is survey is not valid");

                    default:
                        return Ok(true);
                }

            }

            return BadRequest("You don't have permission");
        }

        [HttpPost("save")]
        public async Task<IActionResult> SaveSurvey(SurveyModel data)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                if (data.Id != 0)///update
                {
                    return Ok(await _surveyService.UpdateSurvey(data, UserId));

                }
                else
                    return Ok(await _surveyService.InsertSurvey(data, UserId));
            }

            return BadRequest("You don't have permission");

        }
        [HttpGet("surveys/{SurveyId}/SetToScheduled")]
        public async Task<IActionResult> SetToScheduled(int SurveyId)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                return Ok(await _surveyService.SetToScheduled(SurveyId, UserId));
            }

            return BadRequest("You don't have permission");

        }
        [HttpGet("surveys/{SurveyId}/SetToDraft")]
        public async Task<IActionResult> SetToDraft(int SurveyId)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                return Ok(await _surveyService.SetToDraft(SurveyId, UserId));

            }

            return BadRequest("You don't have permission");

        }

        [HttpGet("surveys/{SurveyId}/CloseSurvey")]
        public async Task<IActionResult> CloseSurvey(int SurveyId)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {

                var closedSurvey = await _surveyService.CloseSurvey(SurveyId, UserId);
                if (closedSurvey.Id != -1)
                {
                    List<object> temp = (List<object>)(await _surveyForUserService.AnswersForSurvey(SurveyId));
                    var num = await _surveyService.GetNumberOfOpportunity(SurveyId);
                    closedSurvey.Answers = temp;

                }
                return Ok(closedSurvey);
            }

            return BadRequest("You don't have permission");

        }
        [HttpPost("surveys/answers/getexcel")]
        public async Task<IActionResult> GetFile(SurveyModel data)
        {
            var tempfile = await _surveyService.CreateExcelFile(data);
            return Ok(tempfile);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSurvey(int id)
        {

            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                if (await _surveyService.DeleteSurvey(id, UserId))
                    return Ok(true);
                else
                    return BadRequest("Survey does not exist");

            }
            return BadRequest("You don't have permission");
        }
        [HttpGet("surveys/{id}/duplicate")]
        public async Task<IActionResult> DuplicateSurvey(int id)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                var ret = await _surveyService.DuplicateSurvey(id);
                if (ret != 0)
                {
                    return Ok(ret);
                }
                else return BadRequest("Survey is not valid");
            }

            return BadRequest("You don't have permission");
        }
        //odgovaranje
        [HttpPost("surveys/{id}/response")]
        public async Task<IActionResult> RespondToTheSurvey(object data, int id)
        {

            return Ok(await _surveyForUserService.RespondToTheSurvey(data, id, UserId));
        }
        [HttpGet("surveys/{id}/answers")]
        public async Task<IActionResult> AnswersForSurvey(int id)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                List<object> temp = (List<object>)(await _surveyForUserService.AnswersForSurvey(id));
                var num = await _surveyService.GetNumberOfOpportunity(id);
                SurveyModel tempSurv = await _surveyService.GetSurvey(id);
                tempSurv.Answers = temp;

                return Ok(tempSurv);
            }


            return BadRequest("You don't have permission");
        }

    }
}
