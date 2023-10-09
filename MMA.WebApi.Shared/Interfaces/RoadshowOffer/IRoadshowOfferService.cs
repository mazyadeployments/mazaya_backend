using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.Image;
using MMA.WebApi.Shared.Models.Offer;
using MMA.WebApi.Shared.Models.Response;
using MMA.WebApi.Shared.Models.Roadshow;
using MMA.WebApi.Shared.Monads;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.Offers
{

    public interface IRoadshowOfferService
    {
        Task<RoadshowOfferModel> CreateOrUpdate(RoadshowOfferModel model, string userId);
        Task<IEnumerable<RoadshowOfferModel>> GetRoadshowOffers();
        Task<PaginationListModel<RoadshowOfferModel>> GetAllRoadshowOffers(QueryModel queryModel, string userId);
        Task<Maybe<RoadshowOfferModel>> GetSpecificRoadshowOfferById(int id, string userId);
        Task<RoadshowOfferMobileModel> GetSpecificOfferByIdForMobile(int id, string userId);
        Task<PaginationListModel<RoadshowOfferModel>> GetAllRoadshowOffersForMyCompany(QueryModel queryModel, string userId);
        Task<ImageModel> GetQRCodeForRoadshowOffer(int offerId, string userId);
        Task ShareOffer(OfferShareModel offerShareModel, string userId);
        Task SetRoadshowOfferAsFavourite(RoadshowOfferFavoriteModel roadshowOfferFavorite, string userId);
        bool CanManageRoadshowOffers(List<Declares.Roles> roles, RoadshowOfferModel model, string userId);
        Task<ResponseDetailsModel> DeleteRSOffer(int id, string userId);
        Task<byte[]> GetPdfQRCodeForOffer(int offerId, string userId);

    }
}
