using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MMA.WebApi.DataAccess.Extensions;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.Companies;
using MMA.WebApi.Shared.Interfaces.MailStorage;
using MMA.WebApi.Shared.Interfaces.Offers;
using MMA.WebApi.Shared.Interfaces.RoadshowInvite;
using MMA.WebApi.Shared.Interfaces.Roles;
using MMA.WebApi.Shared.Models.ApplicationUser;
using MMA.WebApi.Shared.Models.B2C;
using MMA.WebApi.Shared.Models.Companies;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.Offer;
using MMA.WebApi.Shared.Models.SupplierToApprove;
using MMA.WebApi.Shared.Monads;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.WebApi.Core.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IOfferRepository _offerRepository;
        private readonly IRoadshowProposalRepository _roadshowProposalRepository;
        private readonly IRoadshowInviteRepository _roadshowInviteRepository;
        private readonly IMailStorageService _mailStorageServiceService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRoleService _roleService;
        private readonly IConfiguration _configuration;

        public CompanyService(
            ICompanyRepository companyRepository,
            UserManager<ApplicationUser> userManager,
            IOfferRepository offerRepository,
            IMailStorageService mailStorageServiceService,
            IRoadshowProposalRepository roadshowProposalRepository,
            IRoadshowInviteRepository roadshowInviteRepository,
            IRoleService roleService,
            IConfiguration configuration
        )
        {
            _userManager = userManager;
            _companyRepository = companyRepository;
            _offerRepository = offerRepository;
            _mailStorageServiceService = mailStorageServiceService;
            _roadshowProposalRepository = roadshowProposalRepository;
            _roadshowInviteRepository = roadshowInviteRepository;
            _roleService = roleService;
            _configuration = configuration;
        }

        public async Task<PaginationListModel<CompanyCardModel>> GetAllCompaniesPage(
            QueryModel queryModel
        )
        {
            var companies = _companyRepository.GetAllCompaniesPage(queryModel);
            return await companies.ToPagedListAsync(
                queryModel.PaginationParameters.PageNumber,
                queryModel.PaginationParameters.PageSize
            );
        }

        public async Task<
            PaginationListModel<CompanyCardModel>
        > GetAllRejectedAndDeactivatedCompaniesPage(QueryModel queryModel)
        {
            var companies = _companyRepository.GetAllRejectedAndDeactivatedCompaniesPage(
                queryModel
            );
            return await companies.ToPagedListAsync(
                queryModel.Page,
                queryModel.PaginationParameters.PageSize
            );
        }

        public async Task DeleteCompany(int companyId)
        {
            await _offerRepository.DeactivateOffers(companyId);
            await _roadshowProposalRepository.DeactivateRoadshowProposals(companyId);
            await _roadshowInviteRepository.DeactivateRoadshowInvites(companyId);

            var companyModel = await _companyRepository.DeleteAsync(companyId);
        }

        public async Task HardDeleteCompany(int companyId)
        {
            await _offerRepository.HardOfCompanyDeleteOffers(companyId);
            await _roadshowProposalRepository.HardOfCompanyDeleteProposals(companyId);
            await _roadshowInviteRepository.HardOfCompanyDeleteInvites(companyId);

            await _companyRepository.HardDelete(companyId);
        }

        public async Task<CompanyModel> GetCompanyById(int id, List<Roles> roles, string userId)
        {
            if (
                roles.Contains(Roles.Admin)
                || roles.Contains(Roles.AdnocCoordinator)
                || roles.Contains(Roles.Reviewer)
            )
            {
                return await _companyRepository.GetCompanyById(id);
            }
            else if (roles.Contains(Roles.SupplierAdmin) || roles.Contains(Roles.Supplier))
            {
                var supplierAdminCompany = await _companyRepository.GetMyCompany(userId);
                if (supplierAdminCompany.Id == id)
                    return await _companyRepository.GetCompanyById(id);
            }

            return null;
        }

        public async Task<CompanyModel> GetCompanyById(int id)
        {
            return await _companyRepository.GetCompanyById(id);
        }

        public async Task<IEnumerable<CompanyCardModel>> GetAllCompaniesCard()
        {
            var companies = await _companyRepository.GetAllCompaniesCard().ToListAsync();
            return companies;
        }

        public async Task<Maybe<ApplicationUserModel>> CreateOrUpdateFocalPoint(
            ApplicationUserModel model,
            string supplierAdminId
        )
        {
            var auditVisitor = new CreateAuditVisitor(supplierAdminId, DateTime.UtcNow);
            var focalPointCompany = await _companyRepository.GetMyCompany(model.Id);

            var supplierAdminCompany = await _companyRepository.GetMyCompany(supplierAdminId);
            var zeroId = 0;
            if (model.Id != zeroId.ToString())
            {
                var focalPointCount = await _companyRepository.GetFocalPointsCount(
                    focalPointCompany.Id
                );
                var focalPoints = await _companyRepository.GetFocalPointIds(focalPointCompany.Id);
                var existMoreSupplierAdmins = await CheckSupplierAdminsOnEditSupplier(
                    focalPointCount,
                    focalPoints,
                    model,
                    false
                );

                if (!existMoreSupplierAdmins)
                    return null;
            }

            if (
                (
                    model.Id == "0"
                    || supplierAdminCompany.Id == focalPointCompany.Id
                    || _roleService.CheckIfUserIsAdminOrSupplierAdmin(supplierAdminId)
                )
                || model.CompanyId != 0
            )
            {
                var focalPoint = await _companyRepository.CreateOrUpdateFocalPoint(
                    model,
                    auditVisitor,
                    supplierAdminId
                );
                return focalPoint;
            }

            return null;
        }

        public async Task<bool> CheckSupplierAdminsOnEditSupplier(
            int focalPointCount,
            List<string> supplierIds,
            ApplicationUserModel editSupplier,
            bool isDelete
        )
        {
            var user = await _userManager.FindByIdAsync(editSupplier.Id);
            var role = await _userManager.GetRolesAsync(user);

            var supplierAdminRoleString = Declares.Roles.SupplierAdmin.ToString();
            var isChaneSupplierAdminRole =
                role.Contains(supplierAdminRoleString)
                && supplierAdminRoleString != editSupplier.Role.Replace(" ", "");

            if (focalPointCount == 1 && isChaneSupplierAdminRole)
                return false;

            var listRole = new List<string>();
            var listUser = new List<ApplicationUser>();

            foreach (var u in supplierIds)
                listUser.Add(await _userManager.FindByIdAsync(u));

            listUser.Remove(user);

            foreach (var i in listUser)
                listRole.AddRange(await _userManager.GetRolesAsync(i));

            if (
                !listRole.Contains(supplierAdminRoleString)
                && (isChaneSupplierAdminRole || isDelete)
            )
                return false;

            return true;
        }

        public async Task<ApplicationUserModel> GetFocalPointById(
            string userId,
            string loggedInUserId
        )
        {
            if (_roleService.CheckIfUserIsAdmin(loggedInUserId))
            {
                return await _companyRepository.FindFocalPointByIdAsync(userId);
            }
            else
            {
                var supplierAdminCompany = await _companyRepository.GetMyCompany(loggedInUserId);
                var focalPointCompany = await _companyRepository.GetMyCompany(userId);
                if (supplierAdminCompany.Id == focalPointCompany.Id)
                    return await _companyRepository.FindFocalPointByIdAsync(userId);
            }

            return null;
        }

        public async Task<PaginationListModel<CompanyCardModel>> GetAllPendingCompanies(
            QueryModel queryModel
        )
        {
            var pendingCompanies = await _companyRepository.GetAllPendingCompaniesAsync(queryModel);
            return pendingCompanies.ToPagedList(
                queryModel.Page,
                queryModel.PaginationParameters.PageSize
            );
        }

        public async Task<PaginationListModel<CompanyCardModel>> GetAllSuppliersPage(
            QueryModel queryModel
        )
        {
            var suppliers = _companyRepository.GetAllSuppliersPage(queryModel);
            return await suppliers.ToPagedListAsync(
                queryModel.PaginationParameters.PageNumber,
                queryModel.PaginationParameters.PageSize
            );
        }

        public async Task<PaginationListModel<CompanyCardModel>> GetAllMissingLicenseCompanies(
            QueryModel queryModel
        )
        {
            var suppliers = _companyRepository.GetAllMissingLicenseCompanies(queryModel);
            return await suppliers.ToPagedListAsync(
                queryModel.Page,
                queryModel.PaginationParameters.PageSize
            );
        }

        //Including Supplier Admin
        public async Task<PaginationListModel<ApplicationUserModel>> GetMyFocalPointsPage(
            QueryModel queryModel,
            string userId
        )
        {
            var myFocalPoints = await _companyRepository.GetMyFocalPointsPage(queryModel, userId);

            return await myFocalPoints.ToPagedListAsync(
                queryModel.PaginationParameters.PageNumber,
                queryModel.PaginationParameters.PageSize
            );
        }

        public async Task<
            PaginationListModel<ApplicationUserModel>
        > GetAllAdminSupliersAndSuppliersPage(QueryModel queryModel, int companyId)
        {
            var myFocalPoints = await _companyRepository.GetAllAdminSupliersAndSuppliersPage(
                queryModel,
                companyId
            );

            return await myFocalPoints.ToPagedListAsync(
                queryModel.PaginationParameters.PageNumber,
                queryModel.PaginationParameters.PageSize
            );
        }

        public async Task DeleteFocalPoint(string focalPointId, string loggedInUserId)
        {
            ApplicationUser applicationUser = _userManager.FindByIdAsync(loggedInUserId).Result;
            var roleList = await _userManager.GetRolesAsync(applicationUser);
            var supplierAdminCompany = await _companyRepository.GetMyCompany(loggedInUserId);
            var focalPointCompany = await _companyRepository.GetMyCompany(focalPointId);

            var focalPointCount = await _companyRepository.GetFocalPointsCount(
                focalPointCompany.Id
            );
            var focalPoints = await _companyRepository.GetFocalPointIds(focalPointCompany.Id);
            var user = await _userManager.FindByIdAsync(focalPointId);
            var userRoleList = await _userManager.GetRolesAsync(user);
            var existMoreSupplierAdmins = await CheckSupplierAdminsOnEditSupplier(
                focalPointCount,
                focalPoints,
                new ApplicationUserModel() { Id = user.Id, Role = userRoleList.FirstOrDefault() },
                true
            );

            if (!existMoreSupplierAdmins)
                throw new Exception("One focal point must be Supplier Admin.");

            if (
                supplierAdminCompany.Id == focalPointCompany.Id
                || roleList.Contains(Declares.Roles.Admin.ToString())
                || roleList.Contains(Declares.Roles.AdnocCoordinator.ToString())
            )
                await _companyRepository.DeleteFocalPointAsync(focalPointId);
        }

        //Use case when Admin is created Company
        public async Task<Maybe<CompanyModel>> CreateOrUpdateAsync(
            CompanyModel model,
            string userId
        )
        {
            var auditVisitor = new CreateAuditVisitor(userId, DateTime.UtcNow);
            try
            {
                // Decode company website
                model.Website = DecodeBase64String(model.Website);
                model.Facebook = DecodeBase64String(model.Facebook);
                model.Instagram = DecodeBase64String(model.Instagram);
                model.Twitter = DecodeBase64String(model.Twitter);
                model.LegalForm = DecodeBase64String(model.LegalForm);
                return await _companyRepository.CreateOrUpdateAsync(model, auditVisitor, userId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Use case when SupplierAdmin is registating his Company
        public async Task<Maybe<CompanyModel>> RegisterCompanyAsync(
            CompanyModel model,
            string userId
        )
        {
            var auditVisitor = new CreateAuditVisitor(userId, DateTime.UtcNow);
            try
            {
                var company = await _companyRepository.RegisterCompanyAsync(
                    model,
                    auditVisitor,
                    userId
                );
                var coordinators = await _userManager.GetUsersInRoleAsync("ADNOC Coordinator");

                if (company != null && coordinators.Count > 0)
                {
                    var messageTemplate = Declares
                        .MessageTemplateList
                        .Supplier_Registration_Notify_Coordinator;
                    var emailData = _mailStorageServiceService.CreateMailData(
                        coordinators.FirstOrDefault().Id,
                        null,
                        company.NameEnglish,
                        messageTemplate,
                        false
                    );
                    await _mailStorageServiceService.CreateMail(emailData);
                }

                return company;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> ProcessCompany(
            CompanyToApproveModel companyToApprove,
            string userId
        )
        {
            return await _companyRepository.ProcessCompany(companyToApprove, userId);
        }

        public async Task<CompanyModel> GetMySupplierCompany(string userId)
        {
            var companyModel = await _companyRepository.GetMyCompany(userId);
            return companyModel;
        }

        public async Task<PaginationListModel<CompanyCardModel>> GetSuppliersForRoadshowInviteModal(
            QueryModel queryModel,
            int id
        )
        {
            var suppliers = _companyRepository
                .GetSuppliersForRoadshowInviteModal(queryModel, id)
                .ToList();
            return suppliers.ToPagedList(
                queryModel.PaginationParameters.PageNumber,
                queryModel.PaginationParameters.PageSize
            );
        }

        public bool CanManageCompany(List<Roles> roles, CompanyModel companyModel, string userId)
        {
            // Reviewer and Buyer can't do anything with companies
            if (
                roles.Contains(Roles.Reviewer)
                || roles.Contains(Roles.Buyer)
                || roles.Contains(Roles.Supplier)
            )
                return false;
            // Admins can do everything
            if (roles.Contains(Roles.Admin) || roles.Contains(Roles.AdnocCoordinator))
                return true;

            var company = _companyRepository.GetMyCompany(userId).Result;

            // Creating of companies
            if (
                companyModel.Id != 0
                && roles.Contains(Roles.SupplierAdmin)
                && companyModel.Id == company.Id
            )
                return true;

            return false;
        }

        public async Task<PaginationListModel<CompanyCardModel>> GetAllPendingCompaniesPage(
            QueryModel queryModel
        )
        {
            var companies = _companyRepository.GetAllPendingCompaniesPage(queryModel);
            return await companies.ToPagedListAsync(
                queryModel.PaginationParameters.PageNumber,
                queryModel.PaginationParameters.PageSize
            );
        }

        public async Task<bool> CheckIfCompanyWithSameNameExits(string companyName)
        {
            return await _companyRepository.CheckIfCompanyWithSameNameExits(companyName);
        }

        public async Task<bool> CheckIfUserExistsAsCompanySupplier(string userName)
        {
            return await _companyRepository.CheckIfUserExistsAsCompanySupplier(userName);
        }

        public async Task<CompanyRatingModel> GetCompanyRating(int companyId)
        {
            return await _companyRepository.GetCompanyRating(companyId);
        }

        private string DecodeBase64String(string encodedString)
        {
            encodedString ??= "";
            byte[] data = Convert.FromBase64String(encodedString);
            return Encoding.UTF8.GetString(data);
        }

        public async Task<int> GetCompanyIdForCompanyName(string companyName)
        {
            return await _companyRepository.GetCompanyIdForCompanyName(companyName);
        }

        public async Task<string> GetCompanyName(int companyId)
        {
            return await _companyRepository
                .Get()
                .Where(c => c.Id == companyId)
                .Select(c => c.NameEnglish)
                .FirstOrDefaultAsync();
        }

        public async Task AttachSupplierToExistingCompany(
            ApiConnectorDataSupplier dataSupplier,
            string supplierId
        )
        {
            if (
                await _companyRepository.CheckIfCompanyIsAttachedToSysAdmin(
                    dataSupplier.CompanyName
                )
            )
            {
                await _companyRepository.AttachSupplierToExistingCompany(
                    dataSupplier.CompanyName,
                    supplierId
                );
            }
            else
            {
                var supplier = await _userManager.FindByIdAsync(supplierId);
                await _userManager.DeleteAsync(supplier);
            }
        }

        public IEnumerable<OfferModel> GetCompanyOffers(int companyId)
        {
            return _offerRepository.GetCompanyOffers(companyId);
        }

        public async Task<string> GetSupplierAdminForCompany(int companyId)
        {
            return await _companyRepository.GetSupplierAdminForCompany(companyId);
        }

        public async Task<string> GetSupplierAdminUsernameForCompany(int companyId)
        {
            return await _companyRepository.GetSupplierAdminUsernameForCompany(companyId);
        }

        public async Task UpdateTeradeLicence(CompanyRegistrationModel data, string userId)
        {
            await _companyRepository.UpdateTeradeLicence(data, userId);
        }

        public async Task AddFocalPointToCompany(int companyId, string userId)
        {
            await _companyRepository.AddFocalPointToCompany(companyId, userId);
        }
    }
}
