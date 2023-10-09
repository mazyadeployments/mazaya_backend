using Microsoft.AspNetCore.Http;
using MMA.WebApi.Security.AntiXSS;
using MMA.WebApi.Security.AntiXSS.Excluded;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MMA.WebApi.Security.NewFolder
{
    public class AntiXSSMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly int _statusCode = (int)HttpStatusCode.BadRequest;

        public AntiXSSMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task Invoke(HttpContext context)
        {
            if (ExcludedItems.ExcludeMethodsFlag)
            {
                if (ExcludedItems.isMethodExcluded(context.Request.Method))
                {
                    await _next(context).ConfigureAwait(false);
                    return;
                }
            }
            if (ExcludedItems.ExcludePathsFlag)
            {
                if (ExcludedItems.isPathExcluded(context.Request.Path.Value))
                {
                    await _next(context).ConfigureAwait(false);
                    return;
                }
            }

            var originalBody = context.Request.Body;
            try
            {
                var content = await ReadRequestBody(context);

                if (CrossSiteScriptingValidation.IsDangerousString(content, out _))
                {
                    await RespondWithAnError(context).ConfigureAwait(false);
                    return;
                }

                await _next(context).ConfigureAwait(false);
            }
            finally
            {
                context.Request.Body = originalBody;
            }
        }

        #region PrivateMethods
        private static async Task<string> ReadRequestBody(HttpContext context)
        {
            var buffer = new MemoryStream();
            await context.Request.Body.CopyToAsync(buffer);
            context.Request.Body = buffer;
            buffer.Position = 0;

            var encoding = Encoding.UTF8;

            var requestContent = await new StreamReader(buffer, encoding).ReadToEndAsync();
            context.Request.Body.Position = 0;

            return HttpUtility.HtmlDecode(requestContent);
        }

        private async Task RespondWithAnError(HttpContext context)
        {
            context.Response.Clear();
            context.Response.Headers.AddHeaders();
            context.Response.ContentType = "application/json; charset=utf-8";
            context.Response.StatusCode = _statusCode;

            await context.Response.WriteAsync("Unsupported string detected"); // message to show on frontend in toastr
        }
        #endregion
    }
}
