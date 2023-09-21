using Microsoft.AspNetCore.Http;
using MMA.WebApi.Shared.Interfaces.Urls;
using System;

namespace MMA.WebApi.Core.Services
{
    public class DocumentUrlService : IDocumentUrlService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public string host { get; }

        public DocumentUrlService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;

            host =
                $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host.Host}:{_httpContextAccessor.HttpContext.Request.Host.Port}";
        }

        public string GetHost()
        {
            return host;
        }

        public string GetDocumentUrl(Guid? docoumentId, string extension)
        {
            return docoumentId.HasValue
                ? $"{host}/api/media/{docoumentId.ToString()}.{extension}"
                : null;
        }
    }
}
