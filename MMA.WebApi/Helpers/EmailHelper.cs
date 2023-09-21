using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace MMA.WebApi.Helpers
{
    public class EmailHelper
    {
        public void SendRegistrationEmail(IConfiguration _config, bool approved, string email)
        {
            var host = _config.GetSection("Smtp").GetValue<string>("Host");
            var port = _config.GetSection("Smtp").GetValue<int>("Port");
            var username = _config.GetSection("Smtp").GetValue<string>("Username");
            var loz = _config.GetSection("Smtp").GetValue<string>("Loz");

            var smtpClient = new SmtpClient(host)
            {
                Port = port,
                //UseDefaultCredentials = false,
                Credentials = new NetworkCredential(username, loz),
                EnableSsl = true,
            };

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(username);
            mailMessage.IsBodyHtml = true;

            if (approved)
            {
                mailMessage.Subject = "Your account has been appoved in ADNOC Offers";
                mailMessage.Body = "<h1>Approved</h1>";
            }
            else
            {
                mailMessage.Subject = "Your account has been rejected in ADNOC Offers";
                mailMessage.Body = "<h1>Rejected</h1>";
            }

            mailMessage.To.Add(email);
            smtpClient.Send(mailMessage);
        }
    }
}
