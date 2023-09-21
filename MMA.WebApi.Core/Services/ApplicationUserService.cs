using Flurl.Util;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MMA.WebApi.DataAccess.Extensions;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.ApplicationUsers;
using MMA.WebApi.Shared.Interfaces.Helpers;
using MMA.WebApi.Shared.Interfaces.MailStorage;
using MMA.WebApi.Shared.Models.Announcement;
using MMA.WebApi.Shared.Models.ApplicationUser;
using MMA.WebApi.Shared.Models.ApplicationUsers;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.MailStorage;
using MMA.WebApi.Shared.Models.Users;
using MMA.WebApi.Shared.Monads;
using MMA.WebApi.Shared.Visitor;
using OfficeOpenXml;
using Spire.Pdf.Exporting.XPS.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace MMA.WebApi.Core.Services
{
    public class ApplicationUserService : IApplicationUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IApplicationUsersRepository _applicationUserRepository;
        private readonly IMailStorageRepository _mailStorageRepository;
        private readonly IConfiguration _configuration;
        private readonly IDocumentHelper _documentHelper;

        public ApplicationUserService(
            UserManager<ApplicationUser> userManager,
            IApplicationUsersRepository repo,
            IMailStorageRepository mailStorageRepository,
            IConfiguration configuration,
            IDocumentHelper documentHelper
        )
        {
            _userManager = userManager;
            _applicationUserRepository = repo;
            _mailStorageRepository = mailStorageRepository;
            _configuration = configuration;
            _documentHelper = documentHelper;
        }

        public async Task<Maybe<ApplicationUserModel>> CreateOrUpdateUser(
            ApplicationUserModel model,
            string userId
        )
        {
            var auditVisitor = new CreateAuditVisitor(userId, DateTime.UtcNow);
            var updatedUser = await _applicationUserRepository.CreateOrUpdateUser(
                model,
                auditVisitor
            );
            return updatedUser;
        }

        public Task<Guid?> GetProfilePictureById(string id)
        {
            return _applicationUserRepository.FindProfilePictureFromId(id);
        }

        public async Task DeleteUser(string id)
        {
            await _applicationUserRepository.DeleteAsync(id);
        }

        public async Task DeleteByUsername(string username)
        {
            await _applicationUserRepository.DeleteByUsername(username);
        }

        public async Task<IEnumerable<ApplicationUserModel>> GetAll()
        {
            return await _applicationUserRepository.GetAllAsync();
        }

        public async Task<IEnumerable<UserCsvModel>> GetAllListUserAsync()
        {
            return await _applicationUserRepository.GetAllListUserAsync();
        }

        public async Task<bool> CheckIfUserExists(string email)
        {
            return await _applicationUserRepository.CheckIfUserExists(email);
        }

        public async Task<ECardModel> GenerateECardForUser(string userId)
        {
            var eCard = await _applicationUserRepository.GetECardForUser(userId);

            if (String.IsNullOrWhiteSpace(eCard.ECardSequence))
            {
                return await _applicationUserRepository.GenerateECardForUser(userId);
            }

            return eCard;
        }

        public async Task<ECardModel> EditECardForUser(ECardModel model)
        {
            return await _applicationUserRepository.EditECardForUser(model);
        }

        public async Task<ApplicationUserModel> GetById(string id)
        {
            return await _applicationUserRepository.FindByIdAsync(id);
        }

        public async Task<PaginationListModel<ApplicationUserModel>> GetAllBuyersPage(
            QueryModel queryModel
        )
        {
            var applicationUsers = await _applicationUserRepository.GetAllBuyersPage(queryModel);
            var count = applicationUsers.ToList().Count;
            return applicationUsers.ToPagedList(
                queryModel.PaginationParameters.PageNumber,
                queryModel.PaginationParameters.PageSize
            );
        }

        public async Task<PaginationListModel<ApplicationUserModel>> GetAllAdnocUsersPage(
            QueryModel queryModel
        )
        {
            var applicationUsers = await _applicationUserRepository.GetAllAdnocUsersPage(
                queryModel
            );
            return applicationUsers.ToPagedList(
                queryModel.PaginationParameters.PageNumber,
                queryModel.PaginationParameters.PageSize
            );
        }

        public IEnumerable<string> GetUserRoles(string userId)
        {
            return _applicationUserRepository.GetUserRoles(userId);
        }

        private async Task<string> GetTokenForAlumniIntegration(ILogger logger)
        {
            string Bearer = "";

            string url = _configuration["AlumniIntegration:Oauthv2Url"];
            string client_secret = _configuration["AlumniIntegration:client_secret"];
            string grant_type = _configuration["AlumniIntegration:grant_type"];
            string client_id = _configuration["AlumniIntegration:client_id"];
            string scope = _configuration["AlumniIntegration:scope"];

            if (
                url.IsNullOrEmpty()
                || client_secret.IsNullOrEmpty()
                || client_id.IsNullOrEmpty()
                || grant_type.IsNullOrEmpty()
                || scope.IsNullOrEmpty()
            )
            {
                logger.LogError(
                    $"CheckForNewAlumniUsers -> GetTokenForAlumniIntegration There is an error, Property for Alumni Integration does not exist in the configuration"
                );
                throw new Exception(
                    "Property for Alumni Integration does not exist in the configuration"
                );
            }
            HttpClient client = new HttpClient();

            var dict = new Dictionary<string, string>();
            dict["client_secret"] = client_secret;
            dict["grant_type"] = grant_type;
            dict["client_id"] = client_id;
            dict["scope"] = scope;

            dict.Add("Content-Type", "application/x-www-form-urlencoded");

            var response = await client.PostAsync(url, new FormUrlEncodedContent(dict));

            if (response.StatusCode != HttpStatusCode.OK)
            {
                logger.LogError(
                    $"CheckForNewAlumniUsers -> GetTokenForAlumniIntegration There is an error "
                        + response.RequestMessage.ToString()
                );
                throw new Exception(response.RequestMessage.ToString());
            }
            var contents = await response.Content.ReadAsStringAsync();
            Bearer = System.Text.Json.JsonSerializer
                .Deserialize<AlumniTokenData>(contents)
                .access_token;
            return Bearer;
        }

        public async Task CheckForNewAlumniUsers(ILogger logger)
        {
            var token = await GetTokenForAlumniIntegration(logger);
            logger.LogInformation($"CheckForNewAlumniUsers in service -> start");

            HttpClient httpClient = new HttpClient();
            //Get token
            // Setting Authentication header
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(
                    Encoding.ASCII.GetBytes(
                        _configuration["AlumniIntegration:dafaultImportTajRech"] ?? string.Empty
                    )
                )
            );

            // Reading the response
            List<AllowedEmailsForRegistrationModel> allowedEmails =
                new List<AllowedEmailsForRegistrationModel>();
            AdnocUsersDataCarrierModel result = new AdnocUsersDataCarrierModel();
            int startIndex = 0;
            int step = 100;

            ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
            try
            {
                do
                {
                    string url =
                        $"https://www.adnocalumni.ae/api/v1/users/?start={startIndex}&end={startIndex + step}";
                    logger.LogInformation("URL: " + url);

                    var response = httpClient.GetAsync(url).Result;
                    var content = response.Content.ReadAsStringAsync().Result;
                    logger.LogInformation("Content: -> " + content);
                    logger.LogInformation("Response: -> " + response);
                    result = response.Content.ReadAsAsync<AdnocUsersDataCarrierModel>().Result;

                    if (result != null)
                    {
                        foreach (var adnocUser in result.Models)
                        {
                            string email = adnocUser.EmailAddress.Trim();

                            if (
                                !allowedEmails.Any(ae => ae.Email.ToUpper().Equals(email.ToUpper()))
                            )
                            {
                                AllowedEmailsForRegistrationModel allowedEmail =
                                    new AllowedEmailsForRegistrationModel
                                    {
                                        Email = email,
                                        CreatedOn = DateTime.UtcNow,
                                        UpdatedOn = DateTime.UtcNow
                                    };

                                allowedEmails.Add(allowedEmail);
                            }
                        }

                        startIndex += step;
                    }
                    else
                    {
                        logger.LogInformation("THERE IS NOTHING IN RESULT");
                    }
                } while (startIndex < result.Total);
            }
            catch (Exception e)
            {
                logger.LogError("Service Ex: " + e.ToString());
            }

            logger.LogInformation($"AllowedEmails Count -> " + allowedEmails.Count);
            await _applicationUserRepository.InsertNewAlumniUsers(allowedEmails, logger);
            logger.LogInformation($"CheckForNewAlumniUsers in service -> after repo");

            /* new
             * httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                  "Bearer",
                  token
              );

              // Reading the response
              List<AllowedEmailsForRegistrationModel> allowedEmails =
                  new List<AllowedEmailsForRegistrationModel>();
              AdnocUsersDataCarrierModel result = new AdnocUsersDataCarrierModel();
              int dataCounter = 0;
              int page = 1;
              int size = 100;

              string configurationUrl = _configuration["AlumniIntegration:dataUrl"];
              if (configurationUrl.IsNullOrEmpty())
              {
                  logger.LogError(
                      $"CheckForNewAlumniUsers -> GetTokenForAlumniIntegration There is an error, Property AlumniIntegration:dataUrl does not exist in the configuration"
                  );
                  throw new Exception(
                      "Property AlumniIntegration:dataUrl does not exist in the configuration"
                  );
              }
              ServicePointManager.SecurityProtocol =
                  SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
              try
              {
                  do
                  {
                      string url = configurationUrl + $"?page={page}&pageSize={size}";
                      logger.LogInformation("URL: " + url);

                      var response = httpClient.GetAsync(url).Result;
                      var content = response.Content.ReadAsStringAsync().Result;

                      logger.LogInformation("Content: -> " + content);
                      logger.LogInformation("Response: -> " + response);

                      result = response.Content.ReadAsAsync<AdnocUsersDataCarrierModel>().Result;

                      if (result != null)
                      {
                          foreach (var adnocUser in result.Items)
                          {
                              if (adnocUser.Email.IsNullOrEmpty())
                                  continue;
                              string email = adnocUser.Email.Trim();

                              if (
                                  !allowedEmails.Any(ae => ae.Email.ToUpper().Equals(email.ToUpper()))
                              )
                              {
                                  AllowedEmailsForRegistrationModel allowedEmail =
                                      new AllowedEmailsForRegistrationModel
                                      {
                                          Email = email,
                                          CreatedOn = DateTime.UtcNow,
                                          UpdatedOn = DateTime.UtcNow
                                      };

                                  allowedEmails.Add(allowedEmail);
                              }
                          }
                          dataCounter += result.Items.Count();
                          page += 1;
                      }
                      else
                      {
                          logger.LogInformation("THERE IS NOTHING IN RESULT");
                      }
                  } while (dataCounter < result.TotalCount);
              }
              catch (Exception e)
              {
                  logger.LogError("Service Ex: " + e.ToString());
              }

              logger.LogInformation($"AllowedEmails Count -> " + allowedEmails.Count);
              await _applicationUserRepository.InsertNewAlumniUsers(allowedEmails, logger);
              logger.LogInformation($"CheckForNewAlumniUsers in service -> after repo");*/
        }

        public async Task ImportSupplierToAllowedEmailsForRegistration(string email)
        {
            await _applicationUserRepository.ImportSupplierToAllowedEmailsForRegistration(email);
        }

        public async Task<bool> CanUserSendInvitation(string userId)
        {
            return await _applicationUserRepository.CanUserSendInvitation(userId);
        }

        public async Task SetUserInvitation(string userId, string invitedUserEmail)
        {
            await _applicationUserRepository.SetUserInvitation(userId, invitedUserEmail);

            var user = new ApplicationUserModel()
            {
                Id = string.Empty,
                UserName = invitedUserEmail,
                Email = invitedUserEmail
            };

            var emailData = new EmailDataModel()
            {
                User = user,
                MailTemplateId = Declares
                    .MessageTemplateList
                    .Adnoc_Employee_Invited_New_Family_Member
            };

            await _mailStorageRepository.CreateMail(emailData);
        }

        public async Task DeleteOfInvitedUser(string userId, string username)
        {
            var exist = await _applicationUserRepository.DoesUserExist(username);
            if (exist)
            {
                await _applicationUserRepository.DeleteUserInvitation(userId, username);
            }
            else
            {
                throw new Exception("User already registered.");
            }
        }

        public async Task DeleteInvitedUserByAdmin(string username)
        {
            await _applicationUserRepository.DeleteInvitedUserByAdmin(username);
        }

        public async Task<IEnumerable<InvitedUserModel>> GetInvitedUsers(string userId)
        {
            return await _applicationUserRepository.GetInvitedUsers(userId);
        }

        public async Task<bool> CheckIfUserIsInvitedOrIsInAcceptedDomains(string username)
        {
            return await _applicationUserRepository.CheckIfUserIsInvitedOrIsInAcceptedDomains(
                username
            );
        }

        public async Task<bool> CheckIfUserIsInvitedOrIsInUserDomains(string username)
        {
            return await _applicationUserRepository.CheckIfUserIsInvitedOrIsInUserDomains(username);
        }

        public async Task<IEnumerable<UserInvitationModel>> GetAllUserInvitations()
        {
            return await _applicationUserRepository.GetAllUserInvitations();
        }

        public async Task<bool> DoesUserInvited(string username)
        {
            var invited = await _applicationUserRepository.DoesUserInvited(username);
            return invited;
        }

        public async Task<PaginationListModel<UserInvitationModel>> GetAllUserInvitationsPaginated(
            QueryModel queryModel
        )
        {
            var invitations = await _applicationUserRepository.GetAllUserInvitationsPaginated(
                queryModel
            );
            var count = invitations.ToList().Count;
            return invitations.ToPagedList(
                queryModel.PaginationParameters.PageNumber,
                queryModel.PaginationParameters.PageSize
            );
        }

        public async Task<ICollection<ApplicationUserModel>> FilterUsersForAnnouncement(
            AnnouncementModel model
        )
        {
            ICollection<ApplicationUserModel> users = new HashSet<ApplicationUserModel>();

            if (model.AllSuppliers && model.AllBuyers)
            {
                users.AddRange(await _applicationUserRepository.GetAllBuyersAndSuppliersAsync());
            }
            else
            {
                if (model.AllSuppliers)
                {
                    users.AddRange(await _applicationUserRepository.GetAllSuppliersAsync());
                }
                else if (model.CategoriesSupplier != null && model.CategoriesSupplier.Count > 0)
                {
                    users.AddRange(
                        await _applicationUserRepository.GetAllSpecificSuppliersAsync(
                            model.CategoriesSupplier
                        )
                    );
                }

                if (model.AllBuyers)
                {
                    users.AddRange(await _applicationUserRepository.GetAllBuyersAsync());
                }
                else if (model.CategoriesBuyer != null && model.CategoriesBuyer.Count > 0)
                {
                    users.AddRange(
                        await _applicationUserRepository.GetAllSpecificBuyersAsync(
                            model.CategoriesBuyer
                        )
                    );
                }
            }
            return users;
        }

        public int GetInvitedUserType(string username)
        {
            return _applicationUserRepository.GetInvitedUserType(username);
        }

        public List<UserDomainModel> GetUserDomains()
        {
            return _applicationUserRepository.GetUserDomains();
        }

        private void SendMail(string userMail)
        {
            string smtpHost = _configuration["Emails:MailHost"];
            string smtpPassword = _configuration["Emails:EmailPassword"];
            int smtpPort = Convert.ToInt32(_configuration["Emails:MailServerPort"]);
            bool EnableSsl = Convert.ToBoolean(_configuration["Emails:EnableSsl"]);
            bool UseDefaultCredentials = Convert.ToBoolean(
                _configuration["Emails:UseDefaultCredentials"]
            );
            string fromEmail = _configuration["Emails:FromAddress"];
            string from = _configuration["Emails:From"];
            string subject = _configuration["InviteIntegration:Subject"];
            string body = _configuration["InviteIntegration:Body"];

            using (MailMessage mailMessage = new MailMessage())
            {
                MailAddress fromAddress = new MailAddress(userMail, from);
                mailMessage.Subject = subject;
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = body;
                mailMessage.To.Add(new MailAddress(userMail));

                using (SmtpClient SmtpClient = new SmtpClient())
                {
                    mailMessage.From = fromAddress;
                    SmtpClient.Host = smtpHost;
                    SmtpClient.Port = smtpPort;
                    SmtpClient.EnableSsl = EnableSsl;
                    SmtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    SmtpClient.UseDefaultCredentials = UseDefaultCredentials;
                    SmtpClient.Credentials = new NetworkCredential(fromEmail, smtpPassword);
                    SmtpClient.Send(mailMessage);
                }
            }
        }

        public async Task SetFcmMessagesToken(string userId, string token)
        {
            await _applicationUserRepository.SetFcmMessagesToken(userId, token);
        }

        public async Task RemoveUserFcmMessagesToken(string userId, string token)
        {
            await _applicationUserRepository.RemoveUserFcmMessagesToken(userId, token);
        }

        public async Task SendPushNotificationToSpecificUser(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            var title = _configuration["Firebase:OfferPushNotificationTitle"];
            var message = _configuration["Firebase:OfferPushNotificationMessage"];
            var clickAction = _configuration["Firebase:OfferPushNotificationClickAction"];

            await CreateFcmNotificationForSpecificUser(user.Id, message, title, clickAction);
        }

        public async Task SendPushNotificationForSurveyToListUser(
            IEnumerable<string> tokens,
            int surveyId
        )
        {
            var title = _configuration["Firebase:SurveyPushNotificationTitle"];
            var message = _configuration["Firebase:SurveyPushNotificationMessage"];
            var clickAction = _configuration["Firebase:SurveyPushNotificationClickAction"];
            await SendPushNotificationsToUserDevices(
                tokens.ToList(),
                message,
                title,
                clickAction,
                surveyId
            );
        }

        public async Task CreateFcmNotificationForSpecificUser(
            string userId,
            string message,
            string title,
            string click_action
        )
        {
            // Based on userId get deviceIds and send to all user devieces push notification
            var userDevices = await _applicationUserRepository.GetUserFcmDevices(userId);
            await SendPushNotificationsToUserDevices(userDevices, message, title, click_action);
        }

        public async Task<int> CreateFcmNotificationForSpecificRoles(
            Declares.Roles role,
            string message,
            string title,
            string click_action
        )
        {
            // Based on role get deviceIds and send to all user devieces push notification
            var userDevices = await _applicationUserRepository.GetUserFcmDevicesInRole(role);
            await SendPushNotificationsToUserDevices(userDevices, message, title, click_action);
            //await SendPushNotificationsToUserDevicesTask(userDevices, message, title, click_action);

            return userDevices.Count;
        }

        private async Task SendPushNotificationsToUserDevices(
            List<string> userDevices,
            string message,
            string title,
            string click_action,
            int itemId = -1
        )
        {
            int stepsInt = 1;
            const int step = 500;
            if (userDevices.Count > step)
            {
                double steps = userDevices.Count / step;
                stepsInt = (int)Math.Ceiling(steps);
            }

            for (int i = 0; i <= stepsInt; i++)
            {
                var devices = userDevices.Skip(i * step).Take(step).ToList();

                await SendFirebaseMessages(devices, message, title, click_action, itemId);
            }
        }

        private async Task SendFirebaseMessages(
            List<string> fcmTokens,
            string message,
            string title,
            string click_action,
            int itemId = -1
        )
        {
            using (var client = CreateFirebaseHttpClient())
            {
                await SendFirebaseMessage(title, message, fcmTokens, click_action, client, itemId);
            }
        }

        private HttpClient CreateFirebaseHttpClient()
        {
            string firebaseMessageBaseUrl = _configuration["Firebase:FirebaseMessageBaseUrl"];
            string senderId = _configuration["Firebase:FirebaseSenderId"];
            string serverApiKey = _configuration["Firebase:FirebaseServerApiKey"];

            HttpClient httpClient = new HttpClient
            {
                BaseAddress = new Uri(firebaseMessageBaseUrl)
            };

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "key",
                $"={serverApiKey}"
            );
            httpClient.DefaultRequestHeaders.Add("Sender", $"id={senderId}");

            return httpClient;
        }

        private async Task SendFirebaseMessage(
            string title,
            string body,
            List<string> deviceIds,
            string click_action,
            HttpClient httpClient,
            int itemId = -1
        )
        {
            try
            {
                HttpRequestMessage request = CreateSendMessageRequest(
                    title,
                    body,
                    deviceIds,
                    click_action,
                    itemId
                );
                HttpResponseMessage response = await httpClient.SendAsync(request);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private HttpRequestMessage CreateSendMessageRequest(
            string title,
            string body,
            List<string> deviceIds,
            string click_action,
            int itemId = -1
        )
        {
            object content = null;
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "send");
            if (itemId == -1)
            {
                content = new
                {
                    // TODO: Check about this content
                    time_to_live = 108,
                    delay_while_idle = true,
                    notification = new
                    {
                        title,
                        body,
                        click_action
                    },
                    registration_ids = deviceIds.ToArray()
                };
            }
            else
            {
                content = new
                {
                    time_to_live = 108,
                    delay_while_idle = true,
                    data = new { itemId },
                    notification = new
                    {
                        title,
                        body,
                        click_action,
                        itemId
                    },
                    registration_ids = deviceIds.ToArray()
                };
            }
            request.Content = new StringContent(
                Newtonsoft.Json.JsonConvert.SerializeObject(content),
                Encoding.UTF8,
                "application/json"
            );

            return request;
        }

        public async Task<string> UpdateECardSequence(string username, string eCardSequence)
        {
            return await _applicationUserRepository.UpdateECardSequence(username, eCardSequence);
        }

        public async Task RemoveRolesFromUser(string userId, IEnumerable<string> roles)
        {
            await _applicationUserRepository.RemoveRolesFromUser(userId, roles);
        }

        public async Task AddToRoleAsync(string userId, string role)
        {
            if (await _applicationUserRepository.GetCountOfUserRoles(userId) < 1)
            {
                var user = await _userManager.FindByIdAsync(userId);
                await _userManager.AddToRoleAsync(user, role);
            }
        }

        public IEnumerable<UserTypeCountModel> GetCountForAllTypeOfUsers()
        {
            var users = _applicationUserRepository.GetCountForAllTypeOfUsers().ToList();
            foreach (var u in users)
            {
                if (string.IsNullOrWhiteSpace(u.UserType) || u.UserType == "0")
                    u.UserType = "Unspecified";
            }

            return users;
        }

        public async Task InsertIntoUserInvitationTable(
            string userId,
            IEnumerable<InviteUsersModel> invitedEmails
        )
        {
            await _applicationUserRepository.InsertIntoUserInvitationTable(userId, invitedEmails);
        }

        public async Task AddNewDomain(string domain)
        {
            await _applicationUserRepository.AddNewDomain(domain);
        }

        public async Task<string> GetUserType(string username)
        {
            return await _applicationUserRepository.GetUserType(username);
        }

        public async Task ChangeUserType(string username, int newType)
        {
            await _applicationUserRepository.ChangeUserType(username, newType);
        }

        public async Task AddEcardNumber()
        {
            var userIds = await _applicationUserRepository.GetUsersWithEmptyEcard();
            var tasks = userIds.Select(
                userId => _applicationUserRepository.GenerateECardForUser(userId)
            );
            await Task.WhenAll(tasks);
        }

        public async Task<int> GetFcmTokenCount()
        {
            return await _applicationUserRepository.GetFcmTokenCount();
        }

        public async Task<int> GetFcmTokenCount(string role)
        {
            return await _applicationUserRepository.GetFcmTokenCount(role);
        }

        public async Task<string> UpdateRedCrescentECardData(
            string username,
            string eCard,
            Declares.UserType type
        )
        {
            return await _applicationUserRepository.UpdateRedCrescentECardData(
                username,
                eCard,
                type
            );
        }

        public IEnumerable<string> GetAllAcceptedDomains()
        {
            return _applicationUserRepository.GetAllAcceptedDomains();
        }

        public async Task<List<UserDomainModel>> GetUserTypes()
        {
            return await _applicationUserRepository.GetUserTypes();
        }

        public async Task<UserDomainModel> GetSpecificUserType(int id)
        {
            return await _applicationUserRepository.GetSpecificUserType(id);
        }

        public async Task<bool> CheckIfThereIsDuplicateECard()
        {
            return await _applicationUserRepository.CheckIfThereIsDuplicateECard();
        }

        public List<ApplicationUserModel> UsersWithDuplicateECard(bool remove)
        {
            return _applicationUserRepository.UsersWithDuplicateECard(remove);
        }

        public async Task<int> UpdateInvalidUserTypes()
        {
            return await _applicationUserRepository.UpdateInvalidUserTypes();
        }

        public async Task<int> ClearInvalidECards()
        {
            return await _applicationUserRepository.ClearInvalidECards();
        }

        public async Task AddNewUserDomain(UserDomainModel userDomainModel)
        {
            await _applicationUserRepository.AddNewUserDomain(userDomainModel);
        }

        public async Task UpdateUserDomain(UserDomainModel userDomainModel)
        {
            await _applicationUserRepository.UpdateUserDomain(userDomainModel);
        }

        public async Task DeleteUserDomain(string domainName)
        {
            await _applicationUserRepository.DeleteUserDomain(domainName);
        }

        public async Task DeleteSelfInvitedUsers()
        {
            await _applicationUserRepository.DeleteSelfInvitedUsers();
        }

        public async Task<int> GetLastDataSynchronizationCount(DateTime filterDate)
        {
            var listUsers = await _applicationUserRepository.GetAllAsync();
            var filteredList = listUsers.Where(x => x.LastDataSynchronizationOn >= filterDate);
            return filteredList.Count();
        }

        public Task<ICollection<ApplicationUserModel>> GetUsersByDomain(ICollection<string> domains)
        {
            return _applicationUserRepository.GetUsersByDomain(domains);
        }

        public ICollection<ApplicationUserModel> GetAllUsersForMail()
        {
            return _applicationUserRepository.GetAllUsersForMail();
        }

        public ICollection<ApplicationUserModel> GetAllUsersByRolesForMail(
            ICollection<string> roles
        )
        {
            ICollection<ApplicationUserModel> retVal = new HashSet<ApplicationUserModel>();

            retVal = this.GetAllUsersByRole(roles);

            return retVal;
        }

        public ICollection<ApplicationUserModel> GetAllUsersByRole(ICollection<string> roleIds)
        {
            return _applicationUserRepository.GetAllUserByRoleForMail(roleIds);
        }

        public Task<ICollection<ApplicationUserModel>> GetAllUsersFromList(
            ICollection<string> userIds
        )
        {
            return _applicationUserRepository.GetAllUsersFromList(userIds);
        }

        public async Task<List<string>> GetFcmDevicesFromUsersIds(List<string> usersId)
        {
            return await _applicationUserRepository.GetFcmDevicesFromUsersIds(usersId);
        }

        public ICollection<string> GetUsersForOfferReport()
        {
            return _applicationUserRepository.GetAdminsForOfferReportNotifications();
        }

        public async Task<IEnumerable<ApplicationUserModel>> GetUsersByEmailsForECard(
            ICollection<string> emails
        )
        {
            return await _applicationUserRepository.GetUsersByEmailsForECard(emails);
        }

        public ApplicationUserModel GetUserForECard(string Id)
        {
            return _applicationUserRepository.GetUserForECard(Id);
        }

        public async Task AddUsersInvitationForMobileFromSpecificExcelFile(
            IFormFileCollection files,
            string userId
        )
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var file = files[0];

            var allUserInvations = (
                await _applicationUserRepository.GetAllUserInvitations()
            ).Select(x => x.InvitedUserEmail);
            var invationList = new List<InviteUsersModel>();

            using (var pck = new ExcelPackage())
            {
                pck.Load(new MemoryStream(_documentHelper.GetBytes(file)));
                var worksheet = pck.Workbook.Worksheets[0];
                var dataList = new List<string>();
                int counter = 0;
                for (int i = 2; i < worksheet.Dimension.End.Row; i++)
                {
                    counter++;
                    var cellObject = worksheet.Cells[i, 1].Value;
                    if (cellObject == null)
                        continue;
                    var value = cellObject.ToString();
                    var substringOfValue = value.Substring(0, 4);
                    if (value.Length <= 10)
                    {
                        value = "+971" + value;
                    }
                    if (!value.Contains('+'))
                    {
                        value = "+" + value;
                    }
                    if (allUserInvations.Contains(value) || dataList.Contains(value))
                        continue;
                    dataList.Add(value);
                    var userInvation = new InviteUsersModel()
                    {
                        InvitedUserEmail = value,
                        UserId = userId,
                        UserTypeId = 5,
                    };
                    invationList.Add(userInvation);
                    if (counter >= 5000)
                    {
                        await _applicationUserRepository.CreateInvitationForUsersFromSpecificExcelFile(
                            userId,
                            invationList.AsEnumerable()
                        );
                        counter = 0;
                        invationList.Clear();
                        invationList = null;
                        GC.Collect();
                        invationList = new List<InviteUsersModel>();
                    }
                }
                await _applicationUserRepository.CreateInvitationForUsersFromSpecificExcelFile(
                    userId,
                    invationList.AsEnumerable()
                );
            }
        }

        public async Task AddUsersInvitationForEmailFromSpecificExcelFile(
            IFormFileCollection files,
            string userId,
            int userType
        )
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var file = files[0];

            var allUserInvations = (
                await _applicationUserRepository.GetAllUserInvitations()
            ).Select(x => x.InvitedUserEmail);
            var invationList = new List<InviteUsersModel>();

            using (var pck = new ExcelPackage())
            {
                pck.Load(new MemoryStream(_documentHelper.GetBytes(file)));
                var worksheet = pck.Workbook.Worksheets[0];
                var dataList = new List<string>();
                int counter = 0;
                for (int i = 2; i < worksheet.Dimension.End.Row; i++)
                {
                    counter++;
                    var cellObject = worksheet.Cells[i, 1].Value;
                    if (cellObject == null)
                        continue;
                    var value = cellObject.ToString();
                    var substringOfValue = value.Substring(0, 4);
                    if (!value.Contains("@"))
                        continue;

                    if (allUserInvations.Contains(value) || dataList.Contains(value))
                        continue;
                    dataList.Add(value);
                    var userInvation = new InviteUsersModel()
                    {
                        InvitedUserEmail = value,
                        UserId = userId,
                        UserTypeId = userType,
                    };
                    invationList.Add(userInvation);
                    if (counter >= 5000)
                    {
                        await _applicationUserRepository.CreateInvitationForUsersFromSpecificExcelFile(
                            userId,
                            invationList.AsEnumerable(),
                            true
                        );
                        counter = 0;
                        invationList.Clear();
                        invationList = null;
                        GC.Collect();
                        invationList = new List<InviteUsersModel>();
                    }
                }
                await _applicationUserRepository.CreateInvitationForUsersFromSpecificExcelFile(
                    userId,
                    invationList.AsEnumerable(),
                    true
                );
            }
        }

        public async Task<LoginCounterModel> GetCountersForLogin()
        {
            LoginCounterModel loginCounter = new LoginCounterModel();
            int buyersCount = await _applicationUserRepository.GetBuyersCount();
            int suppliersCount = await _applicationUserRepository.GetSuppliersCount();
            loginCounter.BuyersCount = buyersCount;
            loginCounter.SuppliersCount = suppliersCount;
            return loginCounter;
        }

        public async Task<ApplicationUserModel> ChangeReceivedAnnouncementStatusForUser(
            bool status,
            string userId
        )
        {
            return await _applicationUserRepository.ChangeReceivedAnnouncementStatusForUser(
                status,
                userId
            );
        }
    }
}
