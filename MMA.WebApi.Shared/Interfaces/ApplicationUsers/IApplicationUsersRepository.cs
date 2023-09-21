using Microsoft.Extensions.Logging;
using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Models.ApplicationUser;
using MMA.WebApi.Shared.Models.ApplicationUsers;
using MMA.WebApi.Shared.Models.Users;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.ApplicationUsers
{
    public interface IApplicationUsersRepository
    {
        Task<IEnumerable<ApplicationUserModel>> GetAllAsync();
        Task<IEnumerable<string>> GetUsersWithEmptyEcard();
        Task<ApplicationUserModel> CreateOrUpdateUser(
            ApplicationUserModel applicationUser,
            IVisitor<IChangeable> auditVisitor
        );
        Task DeleteAsync(string id);
        Task DeleteByUsername(string username);
        Task<ApplicationUserModel> FindByIdAsync(string id);
        Task<Guid?> FindProfilePictureFromId(string id);
        Task<IEnumerable<ApplicationUserModel>> GetAllBuyersPage(QueryModel queryModel);
        Task<IEnumerable<ApplicationUserModel>> GetAllAdnocUsersPage(QueryModel queryModel);
        Task<int> GetUserInRoleCount(string[] roles);
        IEnumerable<string> GetUserRoles(string userId);
        IEnumerable<string> GetUserRolesId(string userId);
        Task<bool> CheckIfUserExists(string email);
        Task<bool> DoesUserInvited(string invitedUserEmail);
        Task<ECardModel> GetECardForUser(string userId);
        Task<ECardModel> GenerateECardForUser(string userId);
        Task<ECardModel> EditECardForUser(ECardModel model);
        Task InsertNewAlumniUsers(
            List<AllowedEmailsForRegistrationModel> allowedEmails,
            ILogger logger
        );
        Task ImportSupplierToAllowedEmailsForRegistration(string email);
        Task ImportUserFromMobileAPIToAllowedEmailsForRegistration(string email);
        Task<bool> CanUserSendInvitation(string userId);
        Task SetUserInvitation(string userId, string invitedUserEmail);
        Task DeleteUserInvitation(string userId, string username);
        Task DeleteInvitedUserByAdmin(string username);
        Task DeleteUser(string username);
        Task<bool> DoesUserExist(string username);
        Task<IEnumerable<InvitedUserModel>> GetInvitedUsers(string userId);
        Task<bool> CheckIfUserIsInvitedOrIsInAcceptedDomains(string username);
        Task<bool> CheckIfUserIsInvitedOrIsInUserDomains(string username);
        Task<IEnumerable<UserInvitationModel>> GetAllUserInvitations();
        Task<IEnumerable<UserInvitationModel>> GetAllUserInvitationsPaginated(
            QueryModel queryModel
        );
        Task InsertIntoUserInvitationTable(
            string userId,
            IEnumerable<InviteUsersModel> invitedEmail
        );
        int GetInvitedUserType(string username);
        List<UserDomainModel> GetUserDomains();
        Task SetFcmMessagesToken(string userId, string token);
        Task RemoveUserFcmMessagesToken(string userId, string token);
        Task<List<string>> GetFcmDevicesFromUsersIds(List<string> usersId);
        Task<List<string>> GetUserFcmDevices(string userId);
        Task<List<string>> GetUserFcmDevicesInRole(Declares.Roles roles);
        Task<string> UpdateECardSequence(string username, string eCardSequence);
        Task RemoveRolesFromUser(string userId, IEnumerable<string> roles);
        Task<int> GetCountOfUserRoles(string userId);
        IEnumerable<UserTypeCountModel> GetCountForAllTypeOfUsers();
        Task AddNewDomain(string domain);
        int GetUserDomain(string domain);
        Task<string> GetUserType(string username);
        Task ChangeUserType(string username, int newType);
        Task<int> GetFcmTokenCount();
        Task<int> GetFcmTokenCount(string role);
        Task<string> UpdateRedCrescentECardData(
            string username,
            string eCard,
            Declares.UserType type
        );
        IEnumerable<string> GetAllAcceptedDomains();
        Task<List<UserDomainModel>> GetUserTypes();
        Task<UserDomainModel> GetSpecificUserType(int id);
        Task<bool> CheckIfThereIsDuplicateECard();
        List<ApplicationUserModel> UsersWithDuplicateECard(bool remove);
        Task<int> UpdateInvalidUserTypes();
        Task<int> ClearInvalidECards();
        Task AddNewUserDomain(UserDomainModel userDomainModel);
        Task UpdateUserDomain(UserDomainModel userDomainModel);
        Task DeleteUserDomain(string domainName);
        Task DeleteSelfInvitedUsers();
        Task<IEnumerable<UserCsvModel>> GetAllListUserAsync();
        Task<IEnumerable<UserCsvModel>> GetAllUserListAsync();
        Task<int> GetAllBuyers();
        string GetDomainNameFromEmail(string username);
        Task<ICollection<ApplicationUserModel>> GetUsersByDomain(ICollection<string> domains);
        ICollection<ApplicationUserModel> GetAllUsersForMail();
        ICollection<ApplicationUserModel> GetAllUserByRoleForMail(ICollection<string> roleIds);
        ICollection<string> GetAdminsForOfferReportNotifications();
        Task<ICollection<ApplicationUserModel>> GetAllUsersFromList(ICollection<string> userIds);
        Task<IEnumerable<ApplicationUserModel>> GetUsersByEmailsForECard(
            ICollection<string> emails
        );
        ApplicationUserModel GetUserForECard(string Id);
        Task CreateInvitationForUsersFromSpecificExcelFile(
            string userId,
            IEnumerable<InviteUsersModel> invitedEmails,
            bool flag = false
        );
        Task<int> GetBuyersCount();
        Task<int> GetSuppliersCount();
        Task<ICollection<ApplicationUserModel>> GetAllSpecificSuppliersAsync(
            ICollection<int> categories
        );
        Task<ICollection<ApplicationUserModel>> GetAllSpecificBuyersAsync(
            ICollection<int> categories
        );
        Task<ICollection<ApplicationUserModel>> GetAllSuppliersAsync();
        Task<ICollection<ApplicationUserModel>> GetAllBuyersAndSuppliersAsync();
        Task<ICollection<ApplicationUserModel>> GetAllBuyersAsync();
        Task<ApplicationUserModel> ChangeReceivedAnnouncementStatusForUser(
            bool status,
            string userId
        );
    }
}
