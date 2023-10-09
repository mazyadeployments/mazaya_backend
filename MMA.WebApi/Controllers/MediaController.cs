using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MMA.WebApi.Shared.Interfaces.Domain.Document;
using NLog.Web;
using System;
using System.Threading.Tasks;

namespace MMA.WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MediaController : BaseController
    {
        private const int _cacheImageTimeInSeconds = 600;
        private readonly IDocumentService _documentService;
        private readonly IMemoryCache _memoryCache;

        public MediaController(IDocumentService documentService, IMemoryCache memoryCache)
        {
            _documentService = documentService;
            _memoryCache = memoryCache;
        }

        [HttpGet]
        [Route("{identifier}.{ext}")]
        [ResponseCache(Duration = 120, Location = ResponseCacheLocation.Any, NoStore = false)]

        public async Task<IActionResult> GetMedia(string identifier, string ext)
        {

            try
            {
                var cacheKey = $"media:{identifier}:{ext}";

                // try cache
                TryGetCachedImage(cacheKey, out byte[] image);

                // get from service
                if (image == null)
                {
                    var imageDocument = await _documentService.Download(new Guid(identifier));

                    image = imageDocument.Content;

                    TryAddImageToCache(cacheKey, image, _cacheImageTimeInSeconds);
                }

                if (image == null)
                {
                    return NotFound();
                }

                return File(image, "image/jpeg");
            }
            catch (Exception ex)
            {
                var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
                logger.Error(ex.ToString());


            }

            return null;
        }

        private void TryGetCachedImage(string cacheKey, out byte[] image)
        {
            try
            {
                image = _memoryCache.Get(cacheKey) as byte[];
            }
            catch (Exception ex)
            {
                image = null;
            }
        }

        private void TryAddImageToCache(string cacheKey, byte[] image, int cacheImageInSeconds)
        {
            if (image == null)
            {
                return;
            }

            try
            {
                _memoryCache.Set(cacheKey, image, DateTimeOffset.UtcNow.AddSeconds(cacheImageInSeconds));
            }
            catch (Exception ex)
            {
            }
        }
    }
}
