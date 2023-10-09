using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Models.Announcement;
using MMA.WebApi.Shared.Models.ApplicationUser;
using MMA.WebApi.Shared.Models.ApplicationUsers;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.Users;
using MMA.WebApi.Shared.Monads;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.ApplicationUsers
{
    public interface IApplicationUserService
    {
        Task<IEnumerable<ApplicationUserModel>> GetAll();
        Task<IEnumerable<UserCsvModel>> GetAllListUserAsync();
        Task<Maybe<ApplicationUserModel>> CreateOrUpdateUser(ApplicationUserModel model, string userId);
        Task DeleteUser(string id);
        Task DeleteByUsername(string username);
        Task<ApplicationUserModel> GetById(string id);
        Task<Guid?> GetProfilePictureById(string id);
        Task<PaginationListModel<ApplicationUserModel>> GetAllBuyersPage(QueryModel queryModel);
        Task<PaginationListModel<ApplicationUserModel>> GetAllAdnocUsersPage(QueryModel queryModel);
        IEnumerable<string> GetUserRoles(string userId);
        Task<bool> CheckIfUserExists(string email);
        Task<ECardModel> GenerateECardForUser(string userId);
        Task<ECardModel> EditECardForUser(ECardModel model);
        Task CheckForNewAlumniUsers(ILogger logger);
        Task ImportSupplierToAllowedEmailsForRegistration(string email);
        Task<bool> CanUserSendInvitation(string userId);
        Task SetUserInvitation(string userId, string invitedUserEmail);
        Task DeleteOfInvitedUser(string userId, string username);
        Task DeleteInvitedUserByAdmin(string username);
        Task<IEnumerable<InvitedUserModel>> GetInvitedUsers(string userId);
        Task<bool> CheckIfUserIsInvitedOrIsInAcceptedDomains(string username);
        Task<bool> CheckIfUserIsInvitedOrIsInUserDomains(string username);
        Task<IEnumerable<UserInvitationModel>> GetAllUserInvitations();
        Task<bool> DoesUserInvited(string username);
        Task<PaginationListModel<UserInvitationModel>> GetAllUserInvitationsPaginated(QueryModel queryModel);
        int GetInvitedUserType(string username);
        List<UserDomainModel> GetUserDomains();
        Task SetFcmMessagesToken(string userId, string token);
        Task<List<string>> GetFcmDevicesFromUsersIds(List<string> usersId);

        Task RemoveUserFcmMessagesToken(string userId, string token);
        Task SendPushNotificationToSpecificUser(string username);
        Task CreateFcmNotificationForSpecificUser(string userId, string message, string title, string click_action);
        Task<int> CreateFcmNotificationForSpecificRoles(Declares.Roles role, string message, string title, string click_action);
        Task<string> UpdateECardSequence(string username, string eCardSequence);
        Task RemoveRolesFromUser(string userId, IEnumerable<string> roles);
        Task InsertIntoUserInvitationTable(string userId, IEnumerable<InviteUsersModel> invitedEmail);
        Task AddToRoleAsync(string userId, string roles);
        IEnumerable<UserTypeCountModel> GetCountForAllTypeOfUsers();
        Task AddNewDomain(string domain);
        Task<string> GetUserType(string username);
        Task ChangeUserType(string username, int newType);
        Task AddEcardNumber();
        Task<int> GetFcmTokenCount();
        Task<int> GetFcmTokenCount(string role);
        Task<string> UpdateRedCrescentECardData(string username, string eCard, Declares.UserType type);
        IEnumerable<string> GetAllAcceptedDomains();
        Task<bool> CheckIfThereIsDuplicateECard();
        List<ApplicationUserModel> UsersWithDuplicateECard(bool remove);
        Task<int> UpdateInvalidUserTypes();
        Task<int> ClearInvalidECards();
        Task<List<UserDomainModel>> GetUserTypes();
        Task<UserDomainModel> GetSpecificUserType(int id);
        Task AddNewUserDomain(UserDomainModel userDomainModel);
        Task UpdateUserDomain(UserDomainModel userDomainModel);
        Task DeleteUserDomain(string domainName);
        Task DeleteSelfInvitedUsers();
        Task<int> GetLastDataSynchronizationCount(DateTime filterDate);
        Task<ICollection<ApplicationUserModel>> GetUsersByDomain(ICollection<string> domains);
        ICollection<ApplicationUserModel> GetAllUsersForMail();
        ICollection<ApplicationUserModel> GetAllUsersByRolesForMail(ICollection<string> roles);
        ICollection<string> GetUsersForOfferReport();

        Task SendPushNotificationForSurveyToListUser(IEnumerable<string> idlist, int surveyId);
        ICollection<ApplicationUserModel> GetAllUsersByRole(ICollection<string> roleIds);
        Task<ICollection<ApplicationUserModel>> GetAllUsersFromList(ICollection<string> userIds);
        Task<IEnumerable<ApplicationUserModel>> GetUsersByEmailsForECard(ICollection<string> emails);
        ApplicationUserModel GetUserForECard(string Id);
        Task AddUsersInvitationForMobileFromSpecificExcelFile(IFormFileCollection files, string userId);
        Task AddUsersInvitationForEmailFromSpecificExcelFile(IFormFileCollection files, string userId, int userType);

        Task<LoginCounterModel> GetCountersForLogin();
        Task<ICollection<ApplicationUserModel>> FilterUsersForAnnouncement(AnnouncementModel model);
        Task<ApplicationUserModel> ChangeReceivedAnnouncementStatusForUser(bool status, string userId);
    }
}
