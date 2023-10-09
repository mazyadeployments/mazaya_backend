using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MMA.WebApi.Shared.Interfaces.ApplicationUsers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MMA.Azure.Functions
{
    public class CheckForNewAlumniUsers
    {
        private readonly IApplicationUserService _applicationUserService;

        public CheckForNewAlumniUsers(IApplicationUserService applicationUserService)
        {
            _applicationUserService = applicationUserService;
        }

        [FunctionName("HTTPCheckForNewAlumniUsers")]
        public async Task Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]
                HttpRequest req,
            ILogger log
        )
        {
            try
            {
                log.LogInformation($"CheckForNewAlumniUsers started at: {DateTime.Now}");

                await _applicationUserService.CheckForNewAlumniUsers(log);

                log.LogInformation($"CheckForNewAlumniUsers done at: {DateTime.Now}");
            }
            catch (Exception e)
            {
                log.LogError("Azure Function:" + e.ToString());
            }
        }

        [FunctionName("CheckForNewAlumniUsers")]
        public async Task RunAsync(
            [TimerTrigger("0 */10 * * * *", RunOnStartup = true)] TimerInfo myTimer,
            ILogger log
        )
        {
            try
            {
                log.LogInformation($"CheckForNewAlumniUsers started at: {DateTime.Now}");

                await _applicationUserService.CheckForNewAlumniUsers(log);

                log.LogInformation($"CheckForNewAlumniUsers done at: {DateTime.Now}");
            }
            catch (Exception e)
            {
                log.LogError("Azure Function:" + e.ToString());
            }
        }
    }
}
