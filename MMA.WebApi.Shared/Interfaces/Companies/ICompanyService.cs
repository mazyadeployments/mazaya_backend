using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Models.ApplicationUser;
using MMA.WebApi.Shared.Models.B2C;
using MMA.WebApi.Shared.Models.Companies;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.Offer;
using MMA.WebApi.Shared.Models.SupplierToApprove;
using MMA.WebApi.Shared.Monads;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.Companies
{
    public interface ICompanyService
    {
        Task<PaginationListModel<CompanyCardModel>> GetAllCompaniesPage(QueryModel queryModel);
        Task<PaginationListModel<CompanyCardModel>> GetAllMissingLicenseCompanies(QueryModel queryModel);
        Task<PaginationListModel<CompanyCardModel>> GetAllRejectedAndDeactivatedCompaniesPage(QueryModel queryModel);
        Task<PaginationListModel<CompanyCardModel>> GetAllPendingCompaniesPage(QueryModel queryModel);
        Task<CompanyModel> GetMySupplierCompany(string userId);
        Task<Maybe<CompanyModel>> CreateOrUpdateAsync(CompanyModel model, string userId);
        Task<PaginationListModel<CompanyCardModel>> GetAllPendingCompanies(QueryModel queryModel);
        Task<bool> ProcessCompany(CompanyToApproveModel companyToApprove, string userId);
        Task<CompanyModel> GetCompanyById(int id, List<Declares.Roles> roles, string userId);
        Task<CompanyModel> GetCompanyById(int id);
        Task<bool> CheckIfCompanyWithSameNameExits(string companyName);
        Task<int> GetCompanyIdForCompanyName(string companyName);
        Task<string> GetCompanyName(int companyId);
        Task<bool> CheckIfUserExistsAsCompanySupplier(string userName);
        Task<PaginationListModel<CompanyCardModel>> GetAllSuppliersPage(QueryModel queryModel);
        Task<PaginationListModel<ApplicationUserModel>> GetMyFocalPointsPage(QueryModel queryModel, string userId);
        Task DeleteFocalPoint(string focalPointId, string loggedInUserId);
        Task<Maybe<ApplicationUserModel>> CreateOrUpdateFocalPoint(ApplicationUserModel model, string userId);
        Task<ApplicationUserModel> GetFocalPointById(string id, string loggedInUserId);
        Task<IEnumerable<CompanyCardModel>> GetAllCompaniesCard();
        Task DeleteCompany(int id);
        Task HardDeleteCompany(int id);
        Task<CompanyRatingModel> GetCompanyRating(int companyId);
        IEnumerable<OfferModel> GetCompanyOffers(int companyId);
        Task<Maybe<CompanyModel>> RegisterCompanyAsync(CompanyModel model, string userId);
        Task<PaginationListModel<CompanyCardModel>> GetSuppliersForRoadshowInviteModal(QueryModel queryModel, int id);
        Task AddFocalPointToCompany(int companyId, string userId);
        bool CanManageCompany(List<Declares.Roles> roles, CompanyModel companyModel, string userId);
        Task AttachSupplierToExistingCompany(ApiConnectorDataSupplier dataSupplier, string supplierId);
        Task<string> GetSupplierAdminForCompany(int companyId);
        Task<string> GetSupplierAdminUsernameForCompany(int companyId);
        Task<PaginationListModel<ApplicationUserModel>> GetAllAdminSupliersAndSuppliersPage(QueryModel queryModel, int companyId);
        Task UpdateTeradeLicence(CompanyRegistrationModel data, string userId);
    }
}
