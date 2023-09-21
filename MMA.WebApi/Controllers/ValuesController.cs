using Microsoft.AspNetCore.Mvc;
using System;

namespace MMA.WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ValuesController : BaseController
    {

        public ValuesController()
        {
        }

        [HttpGet()]
        public IActionResult GetEnvironmentVariables()
        {
            return Ok(new
            {
                Hostname = Environment.GetEnvironmentVariable("COMPUTERNAME") ??
                           Environment.GetEnvironmentVariable("HOSTNAME"),

                Date = DateTime.Now
            });
        }

    }
}