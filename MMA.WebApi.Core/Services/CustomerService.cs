using Microsoft.AspNetCore.Identity;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Interfaces.ApplicationUsers;
using MMA.WebApi.Shared.Interfaces.Membership;
using MMA.WebApi.Shared.Models.B2C;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MMA.WebApi.Core.Services
{
    public class CustomerService
    {
        private readonly string BUYER = Declares.Roles.Buyer.ToString();
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IApplicationUserService _applicationUserService;
        private readonly IApplicationUsersRepository _applicationUsersRepository;
        private readonly IMembershipECardRepository _membershipECardRepository;

        public CustomerService(
            UserManager<ApplicationUser> userManager,
            IApplicationUserService applicationUserService,
            IApplicationUsersRepository applicationUsersRepository,
            IMembershipECardRepository membershipECardRepository
        )
        {
            _userManager = userManager;
            _applicationUserService = applicationUserService;
            _applicationUsersRepository = applicationUsersRepository;
            _membershipECardRepository = membershipECardRepository;
        }

        public ApplicationUser MapToApplicationCustomer(ApiConnectorDataCustomer apiConnectorData)
        {
            var username = apiConnectorData.Identities
                .Select(x => x.IssuerAssignedId)
                .FirstOrDefault();
            var applicationUser = new ApplicationUser
            {
                UserName = username,
                FirstName = apiConnectorData.GivenName ?? "",
                LastName = apiConnectorData.Surname ?? "",
                NormalizedUserName = username.ToUpper(),
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow,
                Email =
                    apiConnectorData.Identities.Select(x => x.SignInType).FirstOrDefault()
                    == "emailAddress"
                        ? apiConnectorData.Identities
                            .Select(x => x.IssuerAssignedId)
                            .FirstOrDefault()
                        : String.Empty,
                EmailConfirmed = false,
                PhoneNumber =
                    apiConnectorData.Identities.Select(x => x.SignInType).FirstOrDefault()
                    == "phoneNumber"
                        ? apiConnectorData.Identities
                            .Select(x => x.IssuerAssignedId)
                            .FirstOrDefault()
                        : String.Empty,
                Active = true,
                ECardSequence = null,
                UserType = GetUserType(username),
            };

            return applicationUser;
        }

        private int GetUserType(string username)
        {
            var domains = _applicationUserService.GetUserDomains();

            // If it contains @adnoc in email it is adnoc employee
            if (username.Contains("@adnoc"))
                return domains
                    .FirstOrDefault(
                        x => x.DomainName.Equals(Declares.UserType.ADNOCEmployee.ToString())
                    )
                    .Id;

            // If it's in invited table it's family member
            //if (_applicationUserRepository.CheckIfUserIsFamilyMember(username)) return Declares.UserType.ADNOCEmployeeFamilyMember;
            var userType = _applicationUserService.GetInvitedUserType(username);

            if (userType != 0)
            {
                return userType;
            }

            string userDomainName = _applicationUsersRepository.GetDomainNameFromEmail(username);
            int userNameDomainId = _applicationUsersRepository.GetUserDomain(userDomainName);

            if (userNameDomainId != 0)
            {
                return userNameDomainId;
            }

            return domains
                .FirstOrDefault(x => x.DomainName.Equals(Declares.UserType.Other.ToString()))
                .Id;
        }

        public async Task CreateCustomer(ApiConnectorDataCustomer apiConnectorData)
        {
            var user = MapToApplicationCustomer(apiConnectorData);
            await CreateUser(user);
        }

        public async Task CreateUser(ApplicationUser applicationUser)
        {
            var result = await _userManager.CreateAsync(applicationUser);

            if (!result.Succeeded)
                throw new Exception("Cannot create customer");

            var ecard = await _applicationUserService.GenerateECardForUser(applicationUser.Id);
            applicationUser.ECardSequence = ecard.ECardSequence;

            await _userManager.AddToRoleAsync(applicationUser, BUYER);

            await _membershipECardRepository.FindMembershipCardForUserAndUpdate(
                new Shared.Models.ServiceNowModels.MemberModel()
                {
                    Id = applicationUser.Id,
                    mail = applicationUser.Email
                }
            );
        }

        public async Task UpdateUserFirstAndLastName(
            string username,
            string firstName,
            string lastName
        )
        {
            var user = await _userManager.FindByNameAsync(username);
            user.FirstName = firstName;
            user.LastName = lastName;
            user.UserType = GetUserType(username);
            user.UpdatedOn = DateTime.UtcNow;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                throw new Exception("Cannot create customer");
        }
    }
}
