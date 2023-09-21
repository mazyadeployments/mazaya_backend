using MMA.WebApi.Shared.Models.ApplicationUser;
using System.Text;
using System.Text.RegularExpressions;

namespace MMA.WebApi.Helpers
{
    public static class ValidCheckerHelper
    {
        public static string isModelValidUser(ApplicationUserModel model)
        {
            StringBuilder sb = new StringBuilder();

            if (string.IsNullOrEmpty(model.FirstName))
            {
                sb.AppendLine("Please fill first name");
            }
            if (string.IsNullOrEmpty(model.LastName))
            {
                sb.AppendLine("Please fill last name");
            }
            if (string.IsNullOrEmpty(model.Email))
            {
                sb.AppendLine("Please fill email");
            }
            //TODO: Ask for list of acceptable email domains
            if (!Regex.IsMatch(model.Email, @"^[a-zA-Z0-9._%+-]+(@gmail\.com|@adnoc\.ae)$"))
            {
                sb.AppendLine("You can only register from certain email domains.");
            }
            return sb.ToString();
        }
    }
}
