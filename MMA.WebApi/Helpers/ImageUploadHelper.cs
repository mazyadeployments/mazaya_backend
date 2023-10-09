using Microsoft.Extensions.Configuration;
using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Models.Image;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace MMA.WebApi.Helpers
{
    public class ImageUploadHelper
    {
        private readonly IConfiguration _configuration;

        public ImageUploadHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async void UploadImagesInBackground(IEnumerable<ImageModel> croppedImages, Declares.ImageForType imageForType)
        {
            var env = _configuration["AzureFunctions:Environment"];

            if (env != "Development")
            {
                var json = JsonConvert.SerializeObject(new ImageBackgroundUploadModel { CroppedImages = croppedImages, ImageForType = imageForType });
                //var json = JsonConvert.SerializeObject(croppedImages);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                var url = _configuration["AzureFunctions:Url"];
                url += "/ImportImageInBackground";
                using var client = new HttpClient();
                var response = await client.PostAsync(url, data);
            }
        }
    }
}
