using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MMA.WebApi.Shared.Interfaces.Survey;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MMA.Azure.Functions
{
    public class CheckSurvey
    {
        ISurveyService _surveyService;

        public CheckSurvey(ISurveyService surveyService)
        {
            _surveyService = surveyService;
        }

        [FunctionName("CheckSurveys")]
        public async Task RunAsync(
            [TimerTrigger("0 */2 * * * *", RunOnStartup = true)] TimerInfo myTimer,
            ILogger log
        )
        {
            log.LogInformation($"CheckSurveys started at: {DateTime.Now}");
            try
            {
                await _surveyService.CheckSurvey(log);
            }
            catch (Exception e)
            {
                log.LogError("Azure function error -> " + e.ToString());
            }

            log.LogInformation($"CheckSurveys done at: {DateTime.Now}");
        }
    }
}
