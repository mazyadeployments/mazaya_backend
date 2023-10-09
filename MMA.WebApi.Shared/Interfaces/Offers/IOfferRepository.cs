using Microsoft.Extensions.Logging;
using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Models.Document;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.Location;
using MMA.WebApi.Shared.Models.Offer;
using MMA.WebApi.Shared.Models.OneHubModels;
using MMA.WebApi.Shared.Models.Ratings;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace MMA.WebApi.Shared.Interfaces.Offers
{
    public interface IOfferRepository : IQueryableRepository<OfferModel>
    {
        IQueryable<OfferModel> GetSpecificOffers(string userId, List<Enums.Declares.Roles> roles, QueryModel queryModel);
        IQueryable<OfferModel> GetMyOffers(string userId, List<Enums.Declares.Roles> roles, QueryModel queryModel);
        Task<OfferModel> DeleteAsync(int id, string userId);
        int GetBannerOffersCount();
        Task<OfferModel> CreateAsync(OfferModel model, IVisitor<IChangeable> auditVisitor, string userId, List<Declares.Roles> roles);
        Task<OfferModel> DeltaMigration(OfferModel model, IVisitor<IChangeable> auditVisitor, string userId, List<Declares.Roles> roles);
        Task UpdateStatus(OfferModel model, List<Enums.Declares.Roles> roles, string userId);

        Task<OfferModel> UpdateAsync(OfferModel model, IVisitor<IChangeable> auditVisitor, string userId, List<Declares.Roles> roles);

        Task<OfferModel> GetOfferById(int id, string userId, List<Declares.Roles> roles);
        Task<OfferModel> GetOffer(int id);

        Task<OfferModel> GetDraftOfferById(int id, string userId);
        Task<OfferModel> GetSpecificOfferById(int id, string userId, List<Enums.Declares.Roles> roles);
        Task<OfferModelMobile> GetSpecificOfferByIdForMobile(int id, string userId);
        Task<OfferModel> GetReviewOfferById(int id);
        Task<OfferModel> GetPendingOfferById(int id);
        IQueryable<OfferModel> GetLatestOffers(int limit);
        void CreateOrUpdateBanner(List<int> offerIds, IVisitor<IChangeable> auditVisitor);
        IQueryable<OfferModel> GetWeekendOffers(string userId, bool isBuyer);
        IQueryable<OfferModel> GetBannerOffers(QueryModel queryModel);
        List<int> GetBannerOffers();
        IQueryable<OfferModel> GetAllWeekendOffers(QueryModel queryModel);
        IQueryable<OfferModel> GetFavoritesOffersPage(QueryModel queryModel, string userId, bool isBuyer);
        IQueryable<OfferOneHubCardModel> GetFavoritesOffersOneHubPage(OneHubQueryModel queryModel, string userId, bool isBuyer);
        Task<IsOfferFavoriteAndRatedModel> CheckIfOfferIsFavoriteAndRated(int offerId, string userId);
        IQueryable<OfferModel> GetReviewOffers();
        IQueryable<OfferModel> GetPendingOffers();
        IQueryable<OfferModel> GetOffersByCategoryPage(int categoryId, QueryModel queryModel, string userId, bool isBuyer);
        IQueryable<OfferModel> GetOffersByCollectionPage(int collectionId, QueryModel queryModel, string userId, bool isBuyer);
        IQueryable<OfferModel> GetOffersByTagPage(int tagId, QueryModel queryModel);
        IQueryable<OfferModel> GetOffersSearchPage(List<Enums.Declares.Roles> roles, QueryModel queryModel, string userId);
        IQueryable<OfferModelMobile> SelectValidOffers(DateTime lastUpdateOn, string userId);
        Task<IQueryable<OfferModelMobile>> SelectValidOffers();
        IQueryable<ADNOCOneOfferModel> SelectValidOfferView();
        IQueryable<OneHubOfferModel> SelectValidOfferViewOneHub();
        IQueryable<int> SelectValidAndLiveOffersforUser(string userId, bool isBuyer);
        IQueryable<int> SelectValidAndLiveOffers();
        Task SetOfferAsFavourite(OfferFavoriteModel offerFavorite, string userId);
        Task RateOffer(int rating, int offerId, string userId);
        Task<DocumentFileModel> GenerateQRCodeWithLogo(int offerId, string userId);
        Task<Guid> GetQRCodeForOffer(int offerId);
        Task<byte[]> GetQRCodeData(int offerId);
        Task DeactivateOffers(int companyId);
        Task HardOfCompanyDeleteOffers(int companyId);
        Task<IEnumerable<OfferModel>> CheckExpiredOffers(ILogger logger);
        Task<OffersAboutToExpireModel> OffersAboutToExpire();
        IQueryable<DefaultAreaModel> GetAllDefaultAreas();
        Task<bool> PostDefaultArea(DefaultAreaModel model);
        Task DeleteDefaultArea(int id);
        Task PutDefaultArea(DefaultAreaModel model);
        DefaultAreaModel GetDefaultAreaById(int id);
        Task<IQueryable<OfferModel>> GetAllOffers(string userId, List<Declares.Roles> roles, QueryModel queryModel, bool isUser);
        Task<IQueryable<OfferOneHubCardModel>> GetAllOffersForOneHub(string userId, List<Declares.Roles> roles, OneHubQueryModel queryModel);
        Task<int> GetAllOffersCount(string userId, List<Declares.Roles> roles);
        //Supplier and Supllier Admin can see all offer agreements from their company, regardless of their status
        Task<int> GetMyOffersCount(string userId, List<Declares.Roles> roles);
        Task<int> GetAssignedOffersCountForAdmin();
        Task<int> GetAssignedOffersCountForReviewer();
        Task<int> CheckIfThereIsNewOffersToSendPushNotification();
        IEnumerable<OfferSupplierCategoryModel> GetOffersThatHaveEmptyAboutCompany();
        Task ReturnToPending(int offerId);
        IEnumerable<OfferModel> GetCompanyOffers(int companyId);
        IEnumerable<OfferModel> GetReportedOffers(QueryModel queryModel);
        Task TransferOffers(TransferOffersModel transferOffersModel);
        IQueryable<OfferModel> GetOffersByCategory(int categoryId);
        Task<PaginationListModel<OfferModel>> GetOffersForMembership(QueryModel queryModel, int membershipType);
        IEnumerable<OfferModelMobile> SelectOffersForSupplier(DateTime lastUpdateOn, string userId);
        Task<IEnumerable<OfferModelMobile>> GetOfferSpecificData(string userId);
        IQueryable<OfferExcelModel> GetOffersForReportByCategory(int categoryId);
        Task RenewOffer(int offerId);
        Task<OfferOneHubModel> GetOneHubOfferById(int id, string userId, List<Declares.Roles> roles);
    }
}
