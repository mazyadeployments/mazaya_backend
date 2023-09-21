using Microsoft.Extensions.Configuration;
using MMA.WebApi.DataAccess.Extensions;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.OfferRating;
using MMA.WebApi.Shared.Models.Companies;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.Ratings;
using System.Threading.Tasks;

namespace MMA.WebApi.Core.Services
{
    public class OfferRatingService : IOfferRatingService
    {
        private readonly IOfferRatingRepository _offerRatingRepository;
        private readonly IConfiguration _configuration;

        public OfferRatingService(IOfferRatingRepository OfferRatingRepository, IConfiguration configuration)
        {
            _offerRatingRepository = OfferRatingRepository;
            _configuration = configuration;
        }

        public async Task<PaginationListModel<OfferRatingModel>> GetAllComments(QueryModel queryModel)
        {
            var comments = await _offerRatingRepository.GetAllComments(queryModel);
            return comments.ToPagedList(queryModel.PaginationParameters.PageNumber, queryModel.PaginationParameters.PageSize);

        }

        public async Task<PaginationListModel<OfferRatingModel>> GetAllRatings(QueryModel queryModel)
        {
            var comments = await _offerRatingRepository.GetAllComments(queryModel);
            return comments.ToPagedList(queryModel.PaginationParameters.PageNumber, queryModel.PaginationParameters.PageSize);

        }

        public async Task<CompanyRatingModel> RateOffer(OfferRatingModel offerRating, string userId)
        {
            return await _offerRatingRepository.RateOffer(offerRating, userId);
        }

        public async Task SetCommentStatus(int offerId, int commentStatus, string userId)
        {
            await _offerRatingRepository.SetCommentStatus(offerId, commentStatus, userId);
        }

        public async Task<decimal> GetAverageRatingForOffer(int offerId)
        {
            return await _offerRatingRepository.GetAverageRatingForOffer(offerId);
        }

        public async Task<decimal> GetAverageRatingForRoadshowOffer(int offerId)
        {
            return await _offerRatingRepository.GetAverageRatingForOffer(offerId);
        }

        public async Task<OfferRatingModel> PublishRating(OfferRatingModel offerRating)
        {
            return await _offerRatingRepository.PublishRating(offerRating);
        }
    }
}
