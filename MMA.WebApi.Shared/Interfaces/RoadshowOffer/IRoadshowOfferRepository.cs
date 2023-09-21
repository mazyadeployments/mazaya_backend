using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Models.Document;
using MMA.WebApi.Shared.Models.Ratings;
using MMA.WebApi.Shared.Models.Response;
using MMA.WebApi.Shared.Models.Roadshow;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace MMA.WebApi.Shared.Interfaces.Offers
{
    public interface IRoadshowOfferRepository : IQueryableRepository<RoadshowOfferModel>
    {
        Task<RoadshowOfferModel> CreateAsync(RoadshowOfferModel model, IVisitor<IChangeable> auditVisitor, string userId);
        IQueryable<RoadshowOfferModel> GetAllRoadshowOffers(string userId, List<Enums.Declares.Roles> roles, QueryModel queryModel);
        Task<RoadshowOfferModel> GetSpecificRoadshowOfferById(int id, string userId, List<Enums.Declares.Roles> roles);
        Task<RoadshowOfferMobileModel> GetSpecificOfferByIdForMobile(int id, string userId);
        IQueryable<RoadshowOfferModel> GetAllRoadshowOffersForMyCompany(QueryModel queryModel, string userId);
        DocumentFileModel GenerateQRCode(int offerId, string userId);
        Task<Guid> GetQRCodeForRoadshowOffer(int offerId);
        Task<DocumentFileModel> GenerateQRCodeWithLogoForRoadshowOffer(int roadshowOfferId, string userId);
        Task SetRoadshowOfferAsFavourite(RoadshowOfferFavoriteModel roadshowOfferFavorite, string userId);
        IEnumerable<RoadshowOfferLocationModel> GetRoadshowOfferLocation(int roadshowOfferId);
        Task<IsOfferFavoriteAndRatedModel> CheckIfOfferIsFavoriteAndRated(int roadshowOfferId, string userId);
        Task<ResponseDetailsModel> DeleteRSOffer(int id);
        Task<byte[]> GetQRCodeData(int offerId);
    }
}
