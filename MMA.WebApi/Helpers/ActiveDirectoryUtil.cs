using Microsoft.Extensions.Configuration;
using MMA.WebApi.Shared.Interfaces.Logger;
using MMA.WebApi.Shared.Models.Users;
using System;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MMA.WebApi.Helpers
{
    public class ActiveDirectoryUtil
    {

        private string _domainName;
        private string _un;
        private string _p;
        private IAppLogger _logger;

        public ActiveDirectoryUtil(IConfiguration _configurationservice, IAppLogger logger)
        {
            _domainName = _configurationservice.GetSection("LDAP").GetSection("Domain").Value;
            _un = _configurationservice.GetSection("LDAP").GetSection("UN").Value;
            _p = _configurationservice.GetSection("LDAP").GetSection("P").Value;
            _logger = logger;
        }

        private DirectoryEntry FindAccountByEmail(string emailAddress)
        {

            try
            {
                string whitelist = @"[^a-zA-Z0-9_@.]";
                Regex pattern = new Regex(whitelist);
                string cleanEmailAddress = emailAddress.Trim();

                if (!pattern.IsMatch(cleanEmailAddress))
                {
                    using (DirectoryEntry gc = new DirectoryEntry(_domainName, _un, _p))
                    {
                        foreach (DirectoryEntry z in gc.Children)
                        {
                            using (DirectoryEntry root = z)
                            {
                                using (DirectorySearcher searcher = new DirectorySearcher(root, string.Format("(proxyaddresses=SMTP:{0})", cleanEmailAddress), new string[] { "proxyAddresses", "objectGuid", "displayName", "distinguishedName" }))
                                {
                                    searcher.ReferralChasing = ReferralChasingOption.All;
                                    var result = searcher.FindOne();
                                    if (result != null)
                                    {
                                        return result.GetDirectoryEntry();
                                    }
                                }
                            }
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
                return null;
            }
            return null;
        }


        public UserModel ValidateUser(string email)
        {
            DirectoryEntry data = FindAccountByEmail(email);
            if (data != null)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var name in data.Properties.PropertyNames)
                {
                    var value = data.Properties[name.ToString()].Value.ToString();
                    sb.AppendLine($"##### {name}: {value}");
                }
                //_logger.Info(sb.ToString());
                UserModel user = this.getUser(data);
                return user;
            }

            return null;
        }

        private UserModel getUser(DirectoryEntry userDirectoryEntry)
        {
            UserModel user = new UserModel();

            if (userDirectoryEntry.Properties["givenName"].Value != null)
            {
                user.FirstName = userDirectoryEntry.Properties["givenName"].Value.ToString();
            }

            if (userDirectoryEntry.Properties["sn"].Value != null)
            {
                user.LastName = userDirectoryEntry.Properties["sn"].Value.ToString();
            }

            if (userDirectoryEntry.Properties["cn"].Value != null)
            {
                user.FullName = userDirectoryEntry.Properties["cn"].Value.ToString();
                if (user.FullName.Split('(').ToList().Count > 0)
                {
                    user.FullName = user.FullName.Split('(').ToList()[0];
                }
            }

            if (user.FullName != null && user.FirstName != null && string.IsNullOrEmpty(user.LastName))
            {

                var lastName = user.FullName.Replace(user.FirstName + " ", "");
                user.LastName = lastName;
            }

            if (userDirectoryEntry.Properties["title"].Value != null)
            {
                user.Designation = userDirectoryEntry.Properties["title"].Value.ToString();
            }

            if (userDirectoryEntry.Properties["telephoneNumber"].Value != null)
            {
                user.WorkPhone = userDirectoryEntry.Properties["telephoneNumber"].Value.ToString();

            }
            if (userDirectoryEntry.Properties["mobile"].Value != null)
            {
                user.MobilePhone = userDirectoryEntry.Properties["mobile"].Value.ToString();
            }
            if (userDirectoryEntry.Properties["mail"].Value != null)
            {
                user.Email = userDirectoryEntry.Properties["mail"].Value.ToString();
            }
            if (userDirectoryEntry.Properties["initials"].Value != null)
            {
                user.Initials = userDirectoryEntry.Properties["initials"].Value.ToString();
            }
            if (userDirectoryEntry.Properties["company"].Value != null)
            {
                user.CompanyName = userDirectoryEntry.Properties["company"].Value.ToString();
            }
            PropertyValueCollection thumbnailPhoto = userDirectoryEntry.Properties["thumbnailPhoto"];
            if (thumbnailPhoto.Value != null && thumbnailPhoto.Value is byte[])
            {
                byte[] thumbnailInBytes = (byte[])thumbnailPhoto.Value;
                user.Photo = thumbnailInBytes;
                user.PhotoBase64 = Convert.ToBase64String(thumbnailInBytes);
            }

            return user;

        }


    }
}
