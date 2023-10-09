using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Models.Companies;
using MMA.WebApi.Shared.Models.Ratings;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.OfferRating
{
    public interface IOfferRatingRepository
    {
        Task<IEnumerable<OfferRatingModel>> GetAllComments(QueryModel queryModel);
        Task<int> GetCommentsCount();
        Task<IEnumerable<OfferRatingModel>> GetAllRatings(QueryModel queryModel);
        Task<CompanyRatingModel> RateOffer(OfferRatingModel offerRating, string userId);
        Task SetCommentStatus(int offerId, int commentStatus, string userId);
        Task<decimal> GetAverageRatingForOffer(int offerId);
        Task<decimal> GetAverageRatingForRoadshowOffer(int offerId);
        Task<OfferRatingModel> PublishRating(OfferRatingModel offerRating);
        Task<IEnumerable<OfferRatingModel>> GetOfferRatings();
    }
}
