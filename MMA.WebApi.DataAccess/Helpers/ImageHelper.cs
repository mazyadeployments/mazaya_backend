using Microsoft.Extensions.Configuration;

namespace MMA.WebApi.DataAccess.Helpers
{
    public class ImageHelper
    {
        private readonly IConfiguration _configuration;

        public ImageHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string BaseUrl { get { return _configuration["Images:BaseURL"]; } }

        public string UserPhotoUrl { get { return BaseUrl + "/image/user/"; } }
    }
}
