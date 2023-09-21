using Microsoft.Extensions.Logging;
using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.Image;
using MMA.WebApi.Shared.Models.Location;
using MMA.WebApi.Shared.Models.Mobile;
using MMA.WebApi.Shared.Models.Offer;
using MMA.WebApi.Shared.Models.OneHubModels;
using MMA.WebApi.Shared.Monads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.Offers
{

    public interface IOfferService
    {
        Task<IEnumerable<OfferModel>> GetOffers();
        Task<PaginationListModel<OfferModel>> GetMyOffers(QueryModel queryModel, string userId);
        Task<PaginationListModel<OfferModel>> GetAllOffers(QueryModel queryModel, string userId);
        Task<PaginationListModel<OfferOneHubCardModel>> GetAllOffersForOneHub(OneHubQueryModel queryModel, string userId);
        Task<PaginationListModel<OfferModel>> GetOffersForMembership(QueryModel queryModel, int membershipType);
        Task<PaginationListModel<OfferModel>> GetSpecificOffersPage(QueryModel queryModel, string userId);
        Task<Maybe<OfferModel>> GetSpecificOfferById(int id, string userId, List<Declares.Roles> roles);
        Task<OfferModelMobile> GetSpecificOfferByIdForMobile(int id, string userId);
        Task<IEnumerable<OfferModel>> GetWeekendOffers(string userId);
        Task<PaginationListModel<OfferModel>> GetWeekendOffers(QueryModel queryModel);
        Task<PaginationListModel<OfferModel>> GetFavoritesOffersPage(QueryModel queryModel, string userId);
        Task<PaginationListModel<OfferModel>> GetOffersByCategoryPage(int categoryId, QueryModel queryModel);
        Task<PaginationListModel<OfferModel>> GetOffersByCollectionPage(int colletionId, QueryModel queryModel, string userId, bool isBuyer);
        Task<PaginationListModel<OfferModel>> GetOffersByTagPage(int tagId, QueryModel queryModel);
        Task<IEnumerable<OfferModel>> GetLatestOffers(int limit);
        Task RateOffer(int rating, int offerId);
        Task<Maybe<OfferModel>> GetOfferById(int id, string userId, List<Declares.Roles> roles);
        Task<Maybe<OfferModel>> GetOffer(int id);
        Task<Maybe<OfferModel>> CreateOrUpdate(OfferModel model, string userId, List<Declares.Roles> roles);
        Task<OfferModel> DeltaMigration(OfferModel model, string userId, List<Declares.Roles> roles);
        Task UpdateStatus(OfferModel model, string userId);
        Task<string> Step(OfferModel model, string userId);
        Task<Maybe<OfferModel>> UpdateOffer(OfferModel model, string userId, List<Declares.Roles> roles);
        Task<PaginationListModel<OfferModel>> GetOffersSearchPageAsync(QueryModel queryModel, string userId);
        Task DeleteOffer(int id, string userId);
        IEnumerable<OfferModelMobile> SelectValidOffers(DateTime lastUpdateOn, string userId, bool isSupplier);
        Task<PaginationListModel<ADNOCOneOfferModel>> SelectValidOfferView(OfferSearchModel queryModel);
        Task<PaginationListModelExt<OneHubOfferModel>> SelectValidOfferViewOneHub(OfferSearchModel queryModel);
        IEnumerable<int> SelectValidAndLiveOffersForUser(string userId, bool isBuyer);
        IEnumerable<int> SelectValidAndLiveOffers();
        Task SetOfferAsFavourite(OfferFavoriteModel offerFavorite, string userId);
        Task ShareOffer(OfferShareModel offerShareModel, string userId);
        Task<ImageModel> GetQRCodeForOffer(int offerId, string userId);

        Task<byte[]> GetPdfQRCodeForOffer(int offerId, string userId);
        Task CheckExpiredOffers(ILogger logger);
        Task OffersAboutToExpire();
        bool CanManageOffers(List<Declares.Roles> roles, OfferModel offerModel, string userId);
        IQueryable<DefaultAreaModel> GetAllDefaultAreas();
        Task<string> SendPushNotification();
        Task<string> SendPushNotification(string title, string message);
        IEnumerable<OfferSupplierCategoryModel> GetOffersThatHaveEmptyAboutCompany();
        Task ReturnToPending(int offerId);
        Task TransferOffers(TransferOffersModel transferOffersModel);
        Task PostDefaultArea(DefaultAreaModel model);
        Task DeleteDefaultArea(int id);
        Task PutDefaultArea(DefaultAreaModel model);
        DefaultAreaModel GetDefaultAreaById(int id);
        Task<IEnumerable<OfferModel>> GetOffersByCategory(int categoryId);

        Task<SynchronizationDataModel> GenerateSynchronizationData();
        Task<IEnumerable<OfferModelMobile>> GetOfferSpecificData(string userId);
        List<Declares.Roles> GetUserRoles(string userId);
        Task RenewOffer(int offerId);
        Task<Maybe<OfferOneHubModel>> GetOnehubOfferById(int id, string userId, List<Declares.Roles> roles);
        Task<PaginationListModel<OfferOneHubCardModel>> GetFavoritesOffersOnehubPage(OneHubQueryModel queryModel, string userId);

    }
}
