using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MMA.WebApi.Controllers
{
    [Route("[controller]")]
    [Authorize("Bearer")]
    [ApiController]
    public class AlumniController : BaseController
    {

        public AlumniController()
        {
        }

        public void ImportAllowedEmailsForRegistration()
        {
            //Logger.Information("Import allowed emails for registration");

            //HttpClient httpClient = new HttpClient();

            //// Setting Authentication header
            //byte[] usernamePassword = Encoding.ASCII.GetBytes("apps.adnoc@adnoc.ae:Adnoc@lkdfjg43984");
            //httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(usernamePassword));

            //// Reading the response
            //List<AllowedEmailsForRegistration> allowedEmails = new List<AllowedEmailsForRegistration>();
            //AdnocUsersDataCarrier responseResult;
            //int startIndex = 0;
            //int step = 100;


            //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
            ////  ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            //do
            //{
            //    string url = $"https://www.adnocalumni.ae/api/v1/users/?start={startIndex}&end={startIndex + step}";

            //    responseResult = httpClient.GetAsync(url).Result.Content.ReadAsAsync<AdnocUsersDataCarrier>().Result;

            //    Logger.Information("Total: " + responseResult.Total);

            //    foreach (var adnocUser in responseResult.Models)
            //    {
            //        string email = adnocUser.EmailAddress.Trim();

            //        if (!allowedEmails.Any(ae => ae.Email.ToUpper().Equals(email.ToUpper())))
            //        {
            //            AllowedEmailsForRegistration allowedEmail = new AllowedEmailsForRegistration
            //            {
            //                Email = email,
            //                CreatedById = this.User.Identity.GetUserId(),
            //                CreatedOn = DateTime.UtcNow,
            //                LastUpdateById = this.User.Identity.GetUserId(),
            //                LastUpdateOn = DateTime.UtcNow
            //            };

            //            allowedEmails.Add(allowedEmail);
            //        }
            //    }

            //    startIndex += step;

            //} while (startIndex < responseResult.Total);

            //this.userRegistrationService.SetAllowedEmailsForRegistration(allowedEmails);
        }
    }
}