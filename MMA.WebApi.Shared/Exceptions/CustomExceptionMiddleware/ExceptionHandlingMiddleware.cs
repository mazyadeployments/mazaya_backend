using Microsoft.AspNetCore.Http;
using MMA.WebApi.Shared.Interfaces.Logger;
using MMA.WebApi.Shared.Models.Exception;
using System;
using System.Net;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Exceptions.CustomExceptionMiddleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IAppLogger _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, IAppLogger logger)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.Error($"Something went wrong:");

                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            ErrorDetails error = new ErrorDetails() { Message = exception.Message, StatusCode = context.Response.StatusCode, Description = exception.ToString() };
            var serializedErrorModel = Newtonsoft.Json.JsonConvert.SerializeObject(error);
            return context.Response.WriteAsync(serializedErrorModel);
        }
    }
}
