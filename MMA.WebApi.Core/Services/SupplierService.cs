using Microsoft.AspNetCore.Identity;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Interfaces.ApplicationUsers;
using MMA.WebApi.Shared.Interfaces.Companies;
using MMA.WebApi.Shared.Models.B2C;
using MMA.WebApi.Shared.Models.Companies;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MMA.WebApi.Core.Services
{
    public class SupplierService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICompanyService _companyService;
        private readonly IApplicationUserService _applicationUserService;

        public SupplierService(
            UserManager<ApplicationUser> userManager,
            ICompanyService companyService,
            IApplicationUserService applicationUserService
        )
        {
            _userManager = userManager;
            _companyService = companyService;
            _applicationUserService = applicationUserService;
        }

        public ApplicationUser MapToApplicationUser(ApiConnectorData apiConnectorData)
        {
            var applicationUser = new ApplicationUser();
            applicationUser.UserName = apiConnectorData.Email;
            applicationUser.NormalizedUserName = apiConnectorData.Email;
            applicationUser.CreatedOn = DateTime.UtcNow;
            applicationUser.UpdatedOn = DateTime.UtcNow;
            applicationUser.Email = apiConnectorData.Email;
            applicationUser.EmailConfirmed = false;
            applicationUser.PhoneNumber = apiConnectorData.MobilePhone;
            applicationUser.Active = true;

            return applicationUser;
        }

        public ApplicationUser MapToApplicationSupplier(ApiConnectorDataSupplier apiConnectorData)
        {
            var otherUserDomain = _applicationUserService
                .GetUserDomains()
                .FirstOrDefault(x => x.DomainName == Declares.UserType.Other.ToString());

            var applicationUser = new ApplicationUser
            {
                UserName = GetUsername(apiConnectorData),
                NormalizedUserName = apiConnectorData.Email,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow,
                Email = GetUsername(apiConnectorData),
                EmailConfirmed = false,
                PhoneNumber = GetUsername(apiConnectorData),
                Active = true,
                ECardSequence = null,
                UserType = otherUserDomain.Id
            };

            return applicationUser;
        }

        private static string GetUsername(ApiConnectorDataSupplier apiConnectorData)
        {
            return apiConnectorData.Identities
                .Where(x => x.SignInType == "phoneNumber" || x.SignInType == "emailAddress")
                .Select(x => x.IssuerAssignedId)
                .FirstOrDefault();
        }

        public async Task CreateSupplier(ApiConnectorDataSupplier apiConnectorData)
        {
            var applicationUser = MapToApplicationSupplier(apiConnectorData);
            var result = await _userManager.CreateAsync(applicationUser);

            if (!result.Succeeded)
            {
                var message = "";
                foreach (var item in result.Errors)
                {
                    message += item.Description + " " + item.Code + "  ";
                }
                throw new Exception("Cannot create supplier " + message);
            }

            var createdUser = await _userManager.FindByNameAsync(applicationUser.UserName);
            var company = new CompanyModel();
            company.CompanyDescription = apiConnectorData.CompanyDescription;
            company.NameEnglish = apiConnectorData.CompanyName;
            // Official email may not be same as supplier mail
            //company.OfficialEmail = GetUsername(apiConnectorData);
            company.Mobile = new PhoneNumberModel
            {
                InternationalNumber = applicationUser.PhoneNumber
            };
            company.ApproveStatus = Declares.SupplierStatus.MissingTradeLicense.ToString();
            company.CreatedBy = createdUser.Id;
            company.UpdatedBy = createdUser.Id;
            company.CreatedOn = DateTime.UtcNow;
            company.UpdatedOn = DateTime.UtcNow;
            company.EstablishDate = DateTime.UtcNow;
            company.ExpiryDate = DateTime.UtcNow.AddDays(365);

            var companyId = (await _companyService.RegisterCompanyAsync(company, createdUser.Id))
                .Value
                .Id;
            var ecard = await _applicationUserService.GenerateECardForUser(applicationUser.Id);
            applicationUser.ECardSequence = ecard.ECardSequence;
            await _userManager.UpdateAsync(applicationUser);
            await _companyService.AddFocalPointToCompany(companyId, applicationUser.Id);
            await _userManager.AddToRoleAsync(applicationUser, "Supplier Admin");
        }

        public async Task AttachSupplierToExistingCompany(ApiConnectorDataSupplier apiConnectorData)
        {
            var supplier = MapToApplicationSupplier(apiConnectorData);
            var result = await _userManager.CreateAsync(supplier);
            var ecard = await _applicationUserService.GenerateECardForUser(supplier.Id);
            supplier.ECardSequence = ecard.ECardSequence;
            await _userManager.UpdateAsync(supplier);

            if (!result.Succeeded)
                throw new Exception("Cannot create supplier");

            await _companyService.AttachSupplierToExistingCompany(apiConnectorData, supplier.Id);
        }
    }
}
