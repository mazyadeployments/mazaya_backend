using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using MMA.WebApi.Shared.Interfaces.Survey;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MMA.Azure.Functions
{
    public class SendNotificationForSurvey
    {
        ISurveyService _surveyService;

        public SendNotificationForSurvey(ISurveyService surveyService)
        {
            _surveyService = surveyService;
        }

        [FunctionName("SendNotificationForSurvey")]
        public async Task RunAsync(
            [TimerTrigger("0 */5 * * * *", RunOnStartup = true)] TimerInfo myTimer,
            ILogger log
        )
        {
            log.LogInformation($"SendNotificationForSurvey started at: {DateTime.Now}");
            try
            {
                await _surveyService.SendNotificationForSurvey(log);
            }
            catch (Exception e)
            {
                log.LogError("Azure function error -> " + e.ToString());
            }

            log.LogInformation($"SendNotificationForSurvey done at: {DateTime.Now}");
        }
    }
}
