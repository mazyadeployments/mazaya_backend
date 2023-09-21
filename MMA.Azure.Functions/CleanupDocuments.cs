using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using MMA.WebApi.Shared.Interfaces.Domain.Document;
using System;
using System.Threading.Tasks;

namespace MMA.Azure.Functions
{
    public class CleanupDocuments
    {
        private readonly IDocumentService _documentService;

        public CleanupDocuments(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        [FunctionName("CleanupDocuments")]
        public async Task Run(
            [TimerTrigger("0 30 23 * * *", RunOnStartup = true)] TimerInfo myTimer,
            ILogger log
        )
        {
            log.LogInformation($"CleanupDocuments function executed at: {DateTime.Now}");
            await _documentService.DeleteRedundantImages();
            log.LogInformation($"CleanupDocuments function done at: {DateTime.Now}");
        }
    }
}
