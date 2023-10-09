using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Models.ApplicationUser;
using MMA.WebApi.Shared.Models.Companies;
using MMA.WebApi.Shared.Models.SupplierToApprove;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.Companies
{
    public interface ICompanyRepository : IQueryableRepository<CompanyModel>
    {
        Task<IQueryable<CompanyModel>> GetCompanies();
        Task<IEnumerable<CompanyExcelModel>> GetCompaniesExcelReport(
            DateTime startDate,
            DateTime endDate
        );
        IQueryable<CompanyCardModel> GetAllCompaniesPage(QueryModel queryModel);
        Task<int> GetCompaniesInStatusCount(string status);
        IQueryable<CompanyCardModel> GetAllPendingCompaniesPage(QueryModel queryModel);
        IQueryable<CompanyCardModel> GetAllRejectedAndDeactivatedCompaniesPage(
            QueryModel queryModel
        );
        IQueryable<CompanyCardModel> GetAllMissingLicenseCompanies(QueryModel queryModel);
        Task<CompanyModel> CreateOrUpdateAsync(
            CompanyModel model,
            IVisitor<IChangeable> auditVisitor,
            string userId
        );
        Task<IEnumerable<CompanyCardModel>> GetAllPendingCompaniesAsync(QueryModel queryModel);
        IQueryable<CompanyCardModel> GetAllSuppliersPage(QueryModel queryModel);
        Task<bool> ProcessCompany(CompanyToApproveModel companyToApprove, string userId);
        Task<CompanyModel> GetCompanyById(int id);
        Task<bool> CheckIfCompanyWithSameNameExits(string companyName);
        Task<int> GetCompanyIdForCompanyName(string companyName);
        Task<bool> CheckIfUserExistsAsCompanySupplier(string userName);
        Task<CompanyRatingModel> GetCompanyRating(int companyId);
        Task<IQueryable<ApplicationUserModel>> GetMyFocalPointsPage(
            QueryModel queryModel,
            string supplierAdminId
        );
        Task DeleteFocalPointAsync(string id);
        Task<ApplicationUserModel> CreateOrUpdateFocalPoint(
            ApplicationUserModel model,
            IVisitor<IChangeable> auditVisitor,
            string supplierAdminId
        );
        Task<ApplicationUserModel> FindFocalPointByIdAsync(string id);
        IQueryable<CompanyCardModel> GetAllCompaniesCard();
        Task<CompanyModel> DeleteAsync(int id);
        Task HardDelete(int id);
        Task<CompanyModel> GetMyCompany(string userId);
        Task<CompanyModel> RegisterCompanyAsync(
            CompanyModel companyModel,
            IVisitor<IChangeable> auditVisitor,
            string userId
        );
        IQueryable<CompanyCardModel> GetSuppliersForRoadshowInviteModal(
            QueryModel queryModel,
            int id
        );
        Task<int> GetMyFocalPointsCount(string userId);
        Task<bool> CheckIfCompanyIsAttachedToSysAdmin(string companyName);
        Task AttachSupplierToExistingCompany(string companyName, string supplierId);
        Task AttachSupplierToExistingCompanyAsFocalPoint(string companyName, string supplierId);
        Task<string> GetSupplierAdminForCompany(int companyId);
        Task<string> GetSupplierAdminUsernameForCompany(int companyId);
        Task<IQueryable<ApplicationUserModel>> GetAllAdminSupliersAndSuppliersPage(
            QueryModel queryModel,
            int companyId
        );
        Task<int> GetFocalPointsCount(int companyId);
        Task<List<string>> GetFocalPointIds(int companyId);
        Task<ApplicationUserModel> GetSupplierAdminModelForCompany(int companyId);
        Task UpdateTeradeLicence(CompanyRegistrationModel data, string userId);
        Task AddFocalPointToCompany(int companyId, string userId);
    }
}
