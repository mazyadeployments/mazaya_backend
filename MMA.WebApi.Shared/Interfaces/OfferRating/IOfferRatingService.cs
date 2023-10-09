using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Models.Companies;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.Ratings;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.OfferRating
{
    public interface IOfferRatingService
    {
        Task<CompanyRatingModel> RateOffer(OfferRatingModel offerRating, string userId);
        Task<PaginationListModel<OfferRatingModel>> GetAllComments(QueryModel queryModel);
        Task<PaginationListModel<OfferRatingModel>> GetAllRatings(QueryModel queryModel);
        Task SetCommentStatus(int offerId, int commentStatus, string userId);
        Task<decimal> GetAverageRatingForOffer(int offerId);
        Task<decimal> GetAverageRatingForRoadshowOffer(int offerId);
        Task<OfferRatingModel> PublishRating(OfferRatingModel offerRating);
    }
}
