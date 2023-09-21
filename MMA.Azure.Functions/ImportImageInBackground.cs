using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Interfaces.Image;
using MMA.WebApi.Shared.Interfaces.Offers;
using MMA.WebApi.Shared.Models.Image;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMA.Azure.Functions
{
    public class ImportImageInBackground
    {
        private readonly IImageUtilsService _imageUtilsService;
        public ImportImageInBackground(IImageUtilsService imageUtilsService)
        {
            _imageUtilsService = imageUtilsService;
        }

        [FunctionName("ImportImageInBackground")]
        public async Task Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            try
            {
                log.LogInformation($"ImportImageInBackground started at: {DateTime.Now}");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var model = JsonConvert.DeserializeObject<ImageBackgroundUploadModel>(requestBody);

                await _imageUtilsService.CreateImages(model.CroppedImages.ToList(), model.ImageForType);

                log.LogInformation($"ImportImageInBackground done at: {DateTime.Now}");
            }
            catch (Exception e)
            {
                log.LogInformation($"ImportImageInBackground caused exception: {e.ToString()}");
            }
        }
    }
}
