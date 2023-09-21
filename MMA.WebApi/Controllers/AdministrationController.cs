using Dapper;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.DataAccess.Resources;
using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.Analytics;
using MMA.WebApi.Shared.Interfaces.Announcement;
using MMA.WebApi.Shared.Interfaces.ApplicationUsers;
using MMA.WebApi.Shared.Interfaces.Companies;
using MMA.WebApi.Shared.Interfaces.MailStorage;
using MMA.WebApi.Shared.Interfaces.Offers;
using MMA.WebApi.Shared.Interfaces.Roles;
using MMA.WebApi.Shared.Models.Announcement;
using MMA.WebApi.Shared.Models.ApplicationUser;
using MMA.WebApi.Shared.Models.Location;
using MMA.WebApi.Shared.Models.MailStorage;
using MMA.WebApi.Shared.Models.SupplierToApprove;
using MMA.WebApi.Shared.Models.UserNotifications;
using MMA.WebApi.Shared.Models.Users;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace MMA.WebApi.Controllers
{
    [Route("[controller]")]
    [Authorize("Bearer")]
    [ApiController]
    public class AdministrationController : BaseController
    {
        private readonly IApplicationUserService _applicationUserService;
        private readonly IMailStorageService _mailStorageServiceService;
        private readonly ICompanyService _companyService;
        private readonly IRoadshowService _roadshowService;
        private readonly IOfferService _offerService;
        private readonly IRoleService _roleService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStringLocalizer<DapperResources> _dapperResources;
        private readonly IConfiguration _configuration;
        private readonly IAnalyticsService _analysticsService;
        private readonly IAnnouncementService _announcementService;

        public AdministrationController(
            IApplicationUserService applicationUserService,
            IRoadshowService roadshowService,
            UserManager<ApplicationUser> userManager,
            IOfferService offerService,
            IRoleService roleService,
            ICompanyService companyService,
            IMailStorageService mailStorageServiceService,
            IStringLocalizer<DapperResources> dapperResources,
            IConfiguration configuration,
            IAnalyticsService analysticsService,
            IAnnouncementService announcementService
        )
        {
            _roleService = roleService;
            _offerService = offerService;
            _userManager = userManager;
            _applicationUserService = applicationUserService;
            _companyService = companyService;
            _mailStorageServiceService = mailStorageServiceService;
            _roadshowService = roadshowService;
            _dapperResources = dapperResources;
            _configuration = configuration;
            _analysticsService = analysticsService;
            _announcementService = announcementService;
        }

        //za admina
        [HttpPost("company/page/{pageNumber}")]
        public async Task<IActionResult> GetAllCompaniesPage(QueryModel queryModel, int pageNumber)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                queryModel.PaginationParameters = new PaginationParameters
                {
                    PageNumber = pageNumber
                };
                queryModel.PaginationParameters.PageSize = 15;
                return Ok(await _companyService.GetAllSuppliersPage(queryModel));
            }

            return BadRequest("You don't have permission to do this action.");
        }

        [HttpGet("get-user-list")]
        public async Task<IActionResult> GetUserList()
        {
            var users = await _applicationUserService.GetAllListUserAsync();

            var csv = ExportHelper.ToCsv<UserCsvModel>(users);

            return File(Encoding.UTF8.GetBytes(csv), "text/csv");
        }

        [HttpGet("lastDataSynchronization/{dateTime}")]
        public async Task<IActionResult> GetLastDataSynchronizationCount(string dateTime)
        {
            if (dateTime == null)
                dateTime = "0001-01-01";
            var count = _applicationUserService
                .GetLastDataSynchronizationCount(DateTime.Parse(dateTime))
                .Result;
            return Ok(count);
        }

        [HttpPost("role-check")]
        public async Task<IActionResult> RoleCheck()
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                await _applicationUserService.RemoveRolesFromUser(
                    UserId,
                    new List<string>
                    {
                        Declares.Roles.Buyer.ToString(),
                        Declares.Roles.SupplierAdmin.ToString()
                    }
                );

                return Ok();
            }

            return Unauthorized();
        }

        [HttpPost("supplier-approve-reject")]
        public async Task<IActionResult> ProcessSuppliers(
            [FromBody] CompanyToApproveModel companyToApprove
        )
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                // Find company supplier by company id
                var supplierAdminId = await _companyService.GetSupplierAdminForCompany(
                    companyToApprove.CompanyId
                );
                supplierAdminId ??= companyToApprove.SupplierId;

                var supplierAdmin = await _userManager.FindByIdAsync(supplierAdminId);
                companyToApprove.SupplierId = supplierAdminId;

                if (supplierAdmin == null)
                    return BadRequest("Company doesn't have supplier admin");

                companyToApprove.SupplierId = supplierAdmin.Id;
                bool isApproved = await _companyService.ProcessCompany(companyToApprove, UserId);
                ApplicationUserModel supplierModel = new ApplicationUserModel
                {
                    FirstName = supplierAdmin.FirstName,
                    LastName = supplierAdmin.LastName,
                    Email = supplierAdmin.Email,
                    Id = supplierAdmin.Id
                };

                var companyName = await _companyService.GetCompanyName(companyToApprove.CompanyId);

                var emailData = new EmailDataModel()
                {
                    User = supplierModel,
                    IsApproved = isApproved,
                    MailTemplateId = Declares
                        .MessageTemplateList
                        .Supplier_Processed_Notify_Supplier,
                    CompanyName = companyName
                };

                await _mailStorageServiceService.CreateMail(emailData);

                return Ok();
            }

            return BadRequest("You don't have permission to do this action.");
        }

        [HttpGet("locations")]
        public IActionResult GetAllDefaultLocations()
        {
            return Ok(_roadshowService.GetAllDefaultLocations());
        }

        [HttpGet("areas")]
        public IActionResult GetAllDefaultAreas()
        {
            return Ok(_offerService.GetAllDefaultAreas());
        }

        [HttpGet("areas/{id}")]
        public IActionResult GetDefaultAreaById(int id)
        {
            return Ok(_offerService.GetDefaultAreaById(id));
        }

        [HttpPost("areas")]
        public async Task<IActionResult> PostDefaultArea(DefaultAreaModel model)
        {
            await _offerService.PostDefaultArea(model);
            return Ok();
        }

        [HttpDelete("areas/{id}")]
        public async Task<IActionResult> DeleteDefaultArea(int id)
        {
            await _offerService.DeleteDefaultArea(id);
            return Ok();
        }

        [HttpPut("areas/{id}")]
        public async Task<IActionResult> PutDefaultArea(DefaultAreaModel model)
        {
            await _offerService.PutDefaultArea(model);
            return Ok();
        }

        [HttpGet("type-of-users")]
        public IActionResult GetCountForAllTypeOfUsers()
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
                return Ok(_applicationUserService.GetCountForAllTypeOfUsers());

            return Unauthorized();
        }

        [HttpPost("insert-into-invitation-table")]
        public async Task<IActionResult> InsertIntoUserInvitationTable(
            IEnumerable<InviteUsersModel> emails
        )
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                BackgroundJob.Enqueue(
                    () => _applicationUserService.InsertIntoUserInvitationTable(UserId, emails)
                );
                return Ok();
            }

            return Unauthorized();
        }

        [HttpGet("get-supplier-admin-for-company/{companyId}")]
        public async Task<IActionResult> GetSupplierAdminUsernameForCompany(int companyId)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                var supplierAdmin = await _companyService.GetSupplierAdminUsernameForCompany(
                    companyId
                );
                return Ok(JsonConvert.SerializeObject(supplierAdmin));
            }

            return Unauthorized();
        }

        [HttpGet("offers-send-push-notifications")]
        public async Task<IActionResult> SendPushNotificationForOffers()
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                var response = await _offerService.SendPushNotification();
                return Ok(response);
            }

            return Unauthorized();
        }

        [HttpPost("offers-send-push-notifications")]
        public async Task<IActionResult> SendPushNotifications(PushNotificationModel msg)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                msg.Title = _roadshowService.DecodeBase64String(msg.Title);
                msg.Message = _roadshowService.DecodeBase64String(msg.Message);

                BackgroundJob.Enqueue(
                    () => _offerService.SendPushNotification(msg.Title, msg.Message)
                );
                return Ok();
            }

            return Unauthorized();
        }

        [HttpGet("offers-send-push-notifications/{username}")]
        public async Task<IActionResult> SendPushNotificationToSpecificUser(string username)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                await _applicationUserService.SendPushNotificationToSpecificUser(username);
                return Ok();
            }

            return Unauthorized();
        }

        [HttpPost("invited-users")]
        public async Task<IActionResult> GetListOfInvitedUsers(QueryModel queryModel)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                queryModel.PaginationParameters.PageSize = 20;
                queryModel.PaginationParameters.PageNumber = queryModel.Page;
                var users = await _applicationUserService.GetAllUserInvitationsPaginated(
                    queryModel
                );

                if (users != null)
                    return Ok(users);

                return NoContent();
            }

            return Unauthorized();
        }

        [HttpDelete("invited-user/{username}")]
        public async Task<IActionResult> DeleteInvitedUser(string username)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                await _applicationUserService.DeleteInvitedUserByAdmin(username);
                return Ok();
            }

            return Unauthorized();
        }

        [HttpGet("get-user-roles/{username}")]
        public async Task<IActionResult> GetUserRoles(string username)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                var userRoles = _applicationUserService.GetUserRoles(username);
                return Ok(JsonConvert.SerializeObject(userRoles));
            }

            return Unauthorized();
        }

        [HttpGet("change-role/{username}/{role}")]
        public async Task<IActionResult> ChangeUserRole(string username, string role)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                var user = await _userManager.FindByNameAsync(username);

                if (user != null)
                {
                    var userRoles = await _userManager.GetRolesAsync(user);
                    var newRole = _roleService.GetRoleName(role);

                    if (!userRoles.Contains(role))
                    {
                        await _applicationUserService.RemoveRolesFromUser(user.Id, userRoles);
                        await _userManager.AddToRoleAsync(user, newRole);
                    }

                    return Ok($"Successfully updated role for {username} to {newRole}");
                }

                return BadRequest("That user does not exist.");
            }

            return Unauthorized();
        }

        [HttpGet("add-new-domain/{domain}")]
        public async Task<IActionResult> AddNewAcceptedDomain(string domain)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                await _applicationUserService.AddNewDomain(domain);

                return Ok();
            }

            return Unauthorized();
        }

        [HttpGet("get-user-type/{username}")]
        public async Task<IActionResult> GetUserType(string username)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                return Ok(await _applicationUserService.GetUserType(username));
            }

            return Unauthorized();
        }

        [HttpPost("change-user-type")]
        public async Task<IActionResult> ChangeUserType(UserTypeModel user)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                await _applicationUserService.ChangeUserType(user.Username, user.NewType);

                return Ok();
            }

            return Unauthorized();
        }

        [HttpGet("add-ecard-where-is-empty")]
        public async Task<IActionResult> AddEcardNumber()
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                BackgroundJob.Enqueue(() => _applicationUserService.AddEcardNumber());

                return Ok();
            }

            return Unauthorized();
        }

        [HttpGet("get-fcm-token-count")]
        public async Task<IActionResult> GetFcmTokenCount()
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                return Ok(await _applicationUserService.GetFcmTokenCount());
            }

            return Unauthorized();
        }

        [HttpGet("get-fcm-token-count/{role}")]
        public async Task<IActionResult> GetFcmTokenCount(string role)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                return Ok(await _applicationUserService.GetFcmTokenCount(role));
            }

            return Unauthorized();
        }

        [HttpGet("update-redcrescent-ecard-data/{username}/{eCard}/{type}")]
        public async Task<IActionResult> UpdateRedCrescentECardData(
            string username,
            string eCard,
            Declares.UserType type
        )
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                return Ok(
                    await _applicationUserService.UpdateRedCrescentECardData(username, eCard, type)
                );
            }

            return Unauthorized("You're not authorized for this action");
        }

        [HttpGet("get-user-domains")]
        public async Task<IActionResult> GetUserDomains()
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                return Ok(await _applicationUserService.GetUserTypes());
            }

            return Unauthorized();
        }

        [HttpGet("get-user-domains/{id}")]
        public async Task<IActionResult> GetUserDomains(int id)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                return Ok(await _applicationUserService.GetSpecificUserType(id));
            }

            return Unauthorized();
        }

        [HttpPost("ecard")]
        public async Task<IActionResult> EditECardForUser(ECardModel model)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                return Ok(await _applicationUserService.EditECardForUser(model));
            }

            return Unauthorized();
        }

        [HttpGet("get-all-accepted-domains")]
        public async Task<IActionResult> GetAllAcceptedDomains()
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                return Ok(_applicationUserService.GetAllAcceptedDomains());
            }

            return Unauthorized("You're not authorized for this action");
        }

        [HttpGet("check-if-there-is-duplicate-ecard")]
        public async Task<IActionResult> CheckIfThereIsDuplicateECard()
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                return Ok(await _applicationUserService.CheckIfThereIsDuplicateECard());
            }

            return Unauthorized();
        }

        [HttpGet("users-with-duplicate-e-card/{remove}")]
        public async Task<IActionResult> UsersWithDuplicateECard(bool remove)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                return Ok(_applicationUserService.UsersWithDuplicateECard(remove));
            }
            return Unauthorized();
        }

        [HttpPost("add-user-domain")]
        public async Task<IActionResult> AddNewUserDomain(UserDomainModel userDomainModel)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                await _applicationUserService.AddNewUserDomain(userDomainModel);
                return Ok();
            }

            return Unauthorized();
        }

        [HttpPost("update-user-domain")]
        public async Task<IActionResult> UpdateUserDomain(UserDomainModel userDomainModel)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                await _applicationUserService.UpdateUserDomain(userDomainModel);
                return Ok();
            }

            return Unauthorized();
        }

        [HttpGet("update-invalid-user-types")]
        public async Task<IActionResult> UpdateInvalidUserTypes()
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                return Ok(await _applicationUserService.UpdateInvalidUserTypes());
            }
            return Unauthorized();
        }

        [HttpGet("clear-invalid-ecards")]
        public async Task<IActionResult> ClearInvalidECards()
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                return Ok(await _applicationUserService.ClearInvalidECards());
            }

            return Unauthorized();
        }

        [HttpDelete("user-domain/{domainName}")]
        public async Task<IActionResult> DeleteUserDomain(string domainName)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                await _applicationUserService.DeleteUserDomain(domainName);
                return Ok();
            }

            return Unauthorized();
        }

        [HttpDelete("self-invited")]
        public async Task<IActionResult> DeleteSelfInvitedUsers()
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                await _applicationUserService.DeleteSelfInvitedUsers();
                return Ok();
            }

            return Unauthorized();
        }

        [HttpPost("get-any-type-test")]
        public async Task<IActionResult> GetAnyType(QuerySql query)
        {
            if (!_roleService.CheckIfUserIsAdmin(UserId))
            {
                return Unauthorized();
            }

            using (
                IDbConnection db = new SqlConnection(_configuration.GetConnectionString("Database"))
            )
            {
                var sql = string.Empty;
                sql = DecodeBase64String(query.Query);
                var res = db.Query<object>(sql, null);
                return Ok(res);
            }
        }

        [HttpPost("announcement")]
        public async Task<IActionResult> SendAnnouncement(AnnouncementModel model)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                await _announcementService.CreateAnnouncement(model, UserId);
                return Ok();
            }
            return Unauthorized();
        }

        private string DecodeBase64String(string encodedString)
        {
            encodedString ??= "";
            byte[] data = Convert.FromBase64String(encodedString);
            return Encoding.UTF8.GetString(data);
        }

        public class QuerySql
        {
            public string Query { get; set; }
        }

        //analytics

        [HttpGet("reports/{type}")]
        public async Task<IActionResult> GetAnalyticsReport(string type)
        {
            var excelFile = await _analysticsService.ExportAnalyticsReporttData(type);

            return Ok(excelFile);
        }

        [HttpGet("reports/supplier-report/{startDate}/{endDate}")]
        public async Task<IActionResult> GetAnalyticsReportForSupplier(
            string startDate,
            string endDate
        )
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                DateTime myStartDate;
                DateTime myEndDate;
                if (startDate == "0" && endDate == "0") //namestiti da DO danasnjeg dana pretrazi, pocevsi OD NEDELJE
                {
                    DateTime day = DateTime.UtcNow.Date;
                    myEndDate = day.Date;
                    myStartDate = day.AddDays(-((int)day.DayOfWeek)).Date;
                }
                else
                {
                    myEndDate = DateTime.Parse(endDate);
                    myStartDate = DateTime.Parse(startDate);
                }

                var excelFile = await _analysticsService.ExportSupplierReport(
                    myStartDate,
                    myEndDate
                );

                return Ok(excelFile);
            }
            return BadRequest("You don't have permission to access");
        }

        [HttpGet("logincounter")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCountersForLogin()
        {
            var loginCounters = await _applicationUserService.GetCountersForLogin();
            return Ok(loginCounters);
        }
    }
}
