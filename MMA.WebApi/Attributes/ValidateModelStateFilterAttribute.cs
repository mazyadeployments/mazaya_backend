using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace MMA.WebApi.Attributes
{
    public class ValidateModelStateFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!actionContext.ModelState.IsValid)
            {
                IEnumerable<string> errorMessages = new List<string>();

                if (actionContext.ModelState.Values.SelectMany(v => v.Errors).Any(e => e.Exception == null))
                {
                    errorMessages = errorMessages.Union(actionContext.ModelState.Values.SelectMany(v => v.Errors)
                                                                                .Where(e => e.Exception == null)
                                                                                .Select(e => e.ErrorMessage));
                }

                if (actionContext.ModelState.Values.SelectMany(v => v.Errors).Any(e => e.Exception != null))
                {
                    errorMessages = errorMessages.Union(actionContext.ModelState.Values.SelectMany(v => v.Errors)
                                                                                       .Where(e => e.Exception != null)
                                                                                       .Select(e => e.Exception.Message));
                }

                var errorModel = new
                {
                    Title = "Validation failed",
                    Description = errorMessages.Aggregate((e1, e2) => $"{e1}{Environment.NewLine}{e2}")
                };


                actionContext.Response = actionContext.Request.CreateResponse((HttpStatusCode)422, errorModel);
            }
        }
    }
}
