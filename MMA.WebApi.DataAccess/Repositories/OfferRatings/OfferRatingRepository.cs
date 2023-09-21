using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.Companies;
using MMA.WebApi.Shared.Interfaces.OfferRating;
using MMA.WebApi.Shared.Models.Comments;
using MMA.WebApi.Shared.Models.Companies;
using MMA.WebApi.Shared.Models.Ratings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.WebApi.DataAccess.Repositories.OfferRatings
{
    class OfferRatingRepository : BaseRepository<OfferRatingModel>, IOfferRatingRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICompanyRepository _companyRepository;
        public OfferRatingRepository(Func<MMADbContext> contexFactory, UserManager<ApplicationUser> userManager, ICompanyRepository companyRepository) : base(contexFactory)
        {
            _userManager = userManager;
            _companyRepository = companyRepository;
        }

        private Expression<Func<OfferRating, OfferRatingModel>> projectToOfferRatingCardModel = data => new OfferRatingModel()
        {
            OfferId = data.OfferId,
            OfferTitle = data.Offer.Title,
            CreatedOn = data.Offer.CreatedOn,
            ApplicationUserId = data.ApplicationUserId,
            Rating = data.Rating,
            CreatedBy = data.CreatedBy,
            CommentText = data.CommentText,
            Status = data.Status
        };


        public async Task<IEnumerable<OfferRatingModel>> GetAllComments(QueryModel queryModel)
        {
            var context = ContextFactory();

            var comments = await (
                                from o in context.Offer
                                join r in context.OfferRating on o.Id equals r.OfferId
                                join c in context.Company on o.CompanyId equals c.Id
                                join u in context.Users on r.CreatedBy equals u.Id
                                select new OfferRatingModel()
                                {
                                    OfferId = r.OfferId,
                                    OfferTitle = o.Title,
                                    ApplicationUserId = r.ApplicationUserId,
                                    Rating = r.Rating,
                                    CreatedBy = r.CreatedBy,
                                    CreatedOn = r.CreatedOn,
                                    UpdatedBy = r.UpdatedBy,
                                    UpdatedOn = r.UpdatedOn,
                                    CommentText = r.CommentText,
                                    Status = r.Status,
                                    BuyerFirstName = u.FirstName ?? u.UserName,
                                    BuyerLastName = u.LastName ?? u.UserName,
                                    BuyerUsername = u.UserName,
                                    CompanyEnglishName = c.NameEnglish,
                                    CompanyArabicName = c.NameArabic,
                                    IsRoadshowOffer = false,
                                    BuyerEmail = u.Email,
                                    BuyerPhone = u.PhoneNumber
                                }
                           ).ToListAsync();

            var roadshowOfferComments = await (
                                from o in context.RoadshowOffer
                                join r in context.RoadshowOfferRating on o.Id equals r.RoadshowOfferId
                                join p in context.RoadshowProposal on o.RoadshowProposalId equals p.Id
                                join c in context.Company on p.Company.Id equals c.Id
                                join u in context.Users on r.CreatedBy equals u.Id
                                select new OfferRatingModel()
                                {
                                    OfferId = r.RoadshowOfferId,
                                    OfferTitle = o.Title,
                                    ApplicationUserId = r.ApplicationUserId,
                                    Rating = r.Rating,
                                    CreatedBy = r.CreatedBy,
                                    CreatedOn = r.CreatedOn,
                                    UpdatedBy = r.UpdatedBy,
                                    UpdatedOn = r.UpdatedOn,
                                    CommentText = r.CommentText,
                                    Status = r.Status,
                                    BuyerFirstName = u.FirstName ?? u.UserName,
                                    BuyerLastName = u.LastName ?? u.UserName,
                                    BuyerUsername = u.UserName,
                                    CompanyEnglishName = c.NameEnglish,
                                    CompanyArabicName = c.NameArabic,
                                    IsRoadshowOffer = true
                                }
                           ).ToListAsync();

            List<OfferRatingModel> listOfRatings = new List<OfferRatingModel>();
            listOfRatings.AddRange(comments);
            listOfRatings.AddRange(roadshowOfferComments);

            var filteredRatings = Filter(listOfRatings, queryModel);

            return Sort(queryModel.Sort, filteredRatings);
        }

        private static IEnumerable<OfferRatingModel> Filter(List<OfferRatingModel> offerRatings, QueryModel queryModel)
        {
            var filteredCategories = offerRatings
                                     .Where(rating => rating.OfferTitle.ToLower().Contains(queryModel.Filter.Keyword.Trim().ToLower()) ||
                                                      rating.BuyerFirstName.ToLower().Contains(queryModel.Filter.Keyword.Trim().ToLower()) ||
                                                      rating.BuyerLastName.ToLower().Contains(queryModel.Filter.Keyword.Trim().ToLower()) ||
                                                      rating.CommentText.ToLower().Contains(queryModel.Filter.Keyword.Trim().ToLower()));

            return filteredCategories;
        }

        public async Task<IEnumerable<OfferRatingModel>> GetAllRatings(QueryModel queryModel)
        {
            var context = ContextFactory();

            var comments = await (
                                from o in context.Offer
                                join r in context.OfferRating on o.Id equals r.OfferId
                                join c in context.Company on o.CompanyId equals c.Id
                                join u in context.Users on r.CreatedBy equals u.Id
                                select new OfferRatingModel()
                                {
                                    OfferId = r.OfferId,
                                    OfferTitle = o.Title,
                                    ApplicationUserId = r.ApplicationUserId,
                                    Rating = r.Rating,
                                    CreatedBy = r.CreatedBy,
                                    CreatedOn = r.CreatedOn,
                                    UpdatedBy = r.UpdatedBy,
                                    UpdatedOn = r.UpdatedOn,
                                    CommentText = r.CommentText,
                                    Status = r.Status,
                                    BuyerFirstName = u.FirstName,
                                    BuyerLastName = u.LastName,
                                    CompanyEnglishName = c.NameEnglish,
                                    CompanyArabicName = c.NameArabic,
                                    IsRoadshowOffer = false
                                }
                           ).ToListAsync();

            var roadshowOfferComments = await (
                                from o in context.RoadshowOffer
                                join r in context.RoadshowOfferRating on o.Id equals r.RoadshowOfferId
                                join p in context.RoadshowProposal on o.RoadshowProposalId equals p.Id
                                join c in context.Company on p.Company.Id equals c.Id
                                join u in context.Users on r.CreatedBy equals u.Id
                                select new OfferRatingModel()
                                {
                                    OfferId = r.RoadshowOfferId,
                                    OfferTitle = o.Title,
                                    ApplicationUserId = r.ApplicationUserId,
                                    Rating = r.Rating,
                                    CreatedBy = r.CreatedBy,
                                    CreatedOn = r.CreatedOn,
                                    UpdatedBy = r.UpdatedBy,
                                    UpdatedOn = r.UpdatedOn,
                                    CommentText = r.CommentText,
                                    Status = r.Status,
                                    BuyerFirstName = u.FirstName,
                                    BuyerLastName = u.LastName,
                                    CompanyEnglishName = c.NameEnglish,
                                    CompanyArabicName = c.NameArabic,
                                    IsRoadshowOffer = true
                                }
                           ).ToListAsync();

            List<OfferRatingModel> listOfRatings = new List<OfferRatingModel>();
            listOfRatings.AddRange(comments);
            listOfRatings.AddRange(roadshowOfferComments);

            var filteredRatings = Filter(listOfRatings, queryModel);

            return Sort(queryModel.Sort, filteredRatings);
        }

        private static IEnumerable<OfferRatingModel> Sort(SortModel sortModel, IEnumerable<OfferRatingModel> comments)
        {
            if (sortModel.Type == "date")
            {
                if (sortModel.Order == Order.DESC)
                {
                    return comments.OrderByDescending(x => x.UpdatedOn);
                }
                else
                {
                    return comments.OrderBy(x => x.UpdatedOn);
                }
            }
            else
            {
                return comments.OrderByDescending(x => x.UpdatedOn);
            }
        }


        public async Task<CompanyRatingModel> RateOffer(OfferRatingModel offerRating, string userId)
        {
            var context = ContextFactory();

            string offerStatus = OfferCommentStatus.PendingApproval.ToString();
            if (string.IsNullOrEmpty(offerRating.CommentText))
            {
                offerStatus = OfferCommentStatus.Public.ToString();
            }


            if (offerRating.IsRoadshowOffer)
            {
                RoadshowOfferRating selectedOfferRating = (from or in context.RoadshowOfferRating where or.RoadshowOfferId == offerRating.OfferId && or.ApplicationUserId == userId select or).FirstOrDefault();

                if (selectedOfferRating == null)
                {
                    selectedOfferRating = new RoadshowOfferRating
                    {
                        ApplicationUserId = userId,
                        RoadshowOfferId = offerRating.OfferId,
                        Rating = offerRating.Rating,
                        CommentText = offerRating.CommentText,
                        CreatedOn = DateTime.UtcNow,
                        CreatedBy = userId,
                        UpdatedBy = userId,
                        UpdatedOn = DateTime.UtcNow,
                        Status = offerStatus
                    };
                    context.RoadshowOfferRating.Add(selectedOfferRating);
                    await context.SaveChangesAsync();
                }
                SetOfferUpdateOnDate(offerRating.OfferId);
                var roadshowOfferRatingList = context.RoadshowOfferRating.Where(x => offerRating.OfferId == x.RoadshowOfferId).ToList();
                return new CompanyRatingModel()
                {
                    AverageRating = await GetAverageRatingForRoadshowOffer(offerRating.OfferId),
                    TotalRatings = roadshowOfferRatingList.Count > 0 ? roadshowOfferRatingList.Where(x =>
                                                x.Status == OfferCommentStatus.Public.ToString()).Count() : 0,
                    Comments = context.OfferRating.Where(x => x.OfferId == offerRating.OfferId && x.Status == OfferCommentStatus.Public.ToString()).Select(x => new CommentModel
                    {
                        OfferId = x.OfferId,
                        CreatedByName = context.Users.FirstOrDefault(u => x.CreatedBy == u.Id).UserName,
                        CreatedBy = x.CreatedBy,
                        CreatedOn = x.CreatedOn,
                        Text = x.CommentText,
                        Rating = x.Rating
                    }).ToList(),
                };

            }
            else
            {
                OfferRating selectedOfferRating = (from or in context.OfferRating where or.OfferId == offerRating.OfferId && or.ApplicationUserId == userId select or).FirstOrDefault();

                if (selectedOfferRating == null)
                {
                    selectedOfferRating = new OfferRating
                    {
                        ApplicationUserId = userId,
                        OfferId = offerRating.OfferId,
                        Rating = offerRating.Rating,
                        CommentText = offerRating.CommentText,
                        CreatedOn = DateTime.UtcNow,
                        CreatedBy = userId,
                        UpdatedBy = userId,
                        UpdatedOn = DateTime.UtcNow,
                        Status = offerStatus
                    };
                    context.OfferRating.Add(selectedOfferRating);
                    await context.SaveChangesAsync();
                }
                SetOfferUpdateOnDate(offerRating.OfferId);
                var offerRattingList = context.OfferRating.Where(x => offerRating.OfferId == x.OfferId);
                var companyIdByOffers = context.Offer.Where(x => x.Id == offerRating.OfferId).Select(x => x.CompanyId).FirstOrDefault();

                var ratingCompany = new CompanyRatingModel();
                if (companyIdByOffers != null)
                    ratingCompany = await _companyRepository.GetCompanyRating(companyIdByOffers);

                return new CompanyRatingModel()
                {
                    AverageRating = await GetAverageRatingForOffer(offerRating.OfferId),
                    TotalRatings = offerRattingList.ToList().Count > 0 ? offerRattingList.Where(x =>
                                        x.Status == OfferCommentStatus.Public.ToString()).Count() : 0,
                    Comments = context.OfferRating.Where(x => x.OfferId == offerRating.OfferId && x.Status == OfferCommentStatus.Public.ToString()).Select(x => new CommentModel
                    {
                        OfferId = x.OfferId,
                        CreatedByName = context.Users.FirstOrDefault(u => x.CreatedBy == u.Id).UserName,
                        CreatedBy = x.CreatedBy,
                        CreatedOn = x.CreatedOn,
                        Text = x.CommentText,
                        Rating = x.Rating
                    }).ToList(),
                    AverageSupplierRating = ratingCompany.AverageRating,
                    TotalSupplierRatings = ratingCompany.TotalRatings
                };
            }

        }

        public async Task SetOfferUpdateOnDate(int offerId)
        {
            var context = ContextFactory();
            var offer = context.Offer.FirstOrDefault(x => x.Id == offerId);
            offer.UpdatedOn = DateTime.UtcNow;
            context.Offer.Update(offer);
            context.SaveChangesAsync();
        }
        public async Task SetCommentStatus(int offerId, int commentStatus, string userId)
        {
            var context = ContextFactory();

            OfferRating selectedOfferRating = (from or in context.OfferRating where or.OfferId == offerId && or.ApplicationUserId == userId select or).FirstOrDefault();

            if (selectedOfferRating != null)
            {
                selectedOfferRating.Status = ((OfferCommentStatus)commentStatus).ToString();
                await context.SaveChangesAsync();
            }
        }

        public async Task<decimal> GetAverageRatingForOffer(int offerId)
        {
            var context = ContextFactory();

            var listOfRatings = await (from or in context.OfferRating where or.OfferId == offerId && or.Status == OfferCommentStatus.Public.ToString() select or).ToListAsync();

            decimal sum = 0;
            int count = listOfRatings.Count > 0 ? listOfRatings.Count : 1;
            listOfRatings.ForEach(x => sum += x.Rating);
            var rating = sum / count;


            return Math.Round(rating, 2);
        }
        public async Task<decimal> GetAverageRatingForRoadshowOffer(int offerId)
        {
            var context = ContextFactory();

            var listOfRatings = await (from or in context.RoadshowOfferRating where or.RoadshowOfferId == offerId && or.Status == OfferCommentStatus.Public.ToString() select or).ToListAsync();

            decimal sum = 0;
            int count = listOfRatings.Count > 0 ? listOfRatings.Count : 1;
            listOfRatings.ForEach(x => sum += x.Rating);
            var rating = sum / count;


            return Math.Round(rating, 2);
        }

        public async Task<OfferRatingModel> PublishRating(OfferRatingModel offerRating)
        {
            var context = ContextFactory();


            if (offerRating.IsRoadshowOffer)
            {
                var offerRatingToPublish = await (from or in context.RoadshowOfferRating where or.RoadshowOfferId == offerRating.OfferId && or.ApplicationUserId == offerRating.ApplicationUserId select or).FirstOrDefaultAsync();
                offerRatingToPublish.Status = (offerRatingToPublish.Status == OfferCommentStatus.Public.ToString()) ? OfferCommentStatus.Private.ToString() : OfferCommentStatus.Public.ToString();

                var rsOffer = await context.RoadshowOffer.FirstOrDefaultAsync(x => x.Id == offerRatingToPublish.RoadshowOfferId);
                if (rsOffer != null)
                    rsOffer.UpdatedOn = DateTime.UtcNow;

                await context.SaveChangesAsync();
                return roadshowOfferRatingToOfferRatingModel.Compile().Invoke(offerRatingToPublish);
            }
            else
            {
                var offerRatingToPublish = await (from or in context.OfferRating where or.OfferId == offerRating.OfferId && or.ApplicationUserId == offerRating.ApplicationUserId select or).FirstOrDefaultAsync();
                offerRatingToPublish.Status = (offerRatingToPublish.Status == OfferCommentStatus.Public.ToString()) ? OfferCommentStatus.Private.ToString() : OfferCommentStatus.Public.ToString();

                var offer = await context.Offer.FirstOrDefaultAsync(x => x.Id == offerRatingToPublish.OfferId);
                if (offer != null)
                    offer.UpdatedOn = DateTime.UtcNow;

                await context.SaveChangesAsync();
                return offerRatingToOfferRatingModel.Compile().Invoke(offerRatingToPublish);
            }
        }

        protected override IQueryable<OfferRatingModel> GetEntities()
        {
            throw new NotImplementedException();
        }

        public async Task<int> GetCommentsCount()
        {
            var context = ContextFactory();

            return await context.OfferRating.CountAsync();
        }

        public async Task<IEnumerable<OfferRatingModel>> GetOfferRatings()
        {
            var context = ContextFactory();
            var comments = await (
                    from o in context.Offer
                    join r in context.OfferRating on o.Id equals r.OfferId
                    join c in context.Company on o.CompanyId equals c.Id
                    join u in context.Users on r.CreatedBy equals u.Id
                    select new OfferRatingModel()
                    {
                        OfferId = r.OfferId,
                        OfferTitle = o.Title,
                        ApplicationUserId = r.ApplicationUserId,
                        Rating = r.Rating,
                        CreatedBy = r.CreatedBy,
                        CreatedOn = r.CreatedOn,
                        UpdatedBy = r.UpdatedBy,
                        UpdatedOn = r.UpdatedOn,
                        CommentText = r.CommentText,
                        Status = r.Status,
                        BuyerFirstName = u.FirstName,
                        BuyerLastName = u.LastName,
                        CompanyEnglishName = c.NameEnglish,
                        CompanyArabicName = c.NameArabic,
                        IsRoadshowOffer = false
                    }
               ).ToListAsync();

            return comments;
        }

        private readonly Expression<Func<OfferRating, OfferRatingModel>> offerRatingToOfferRatingModel = data =>
           new OfferRatingModel()
           {
               OfferId = data.OfferId,
               Rating = data.Rating,
               CommentText = data.CommentText,
               Status = data.Status.ToString(),
           };

        private readonly Expression<Func<RoadshowOfferRating, OfferRatingModel>> roadshowOfferRatingToOfferRatingModel = data =>
           new OfferRatingModel()
           {
               OfferId = data.RoadshowOfferId,
               Rating = data.Rating,
               CommentText = data.CommentText,
               Status = data.Status.ToString(),
           };

    }
}
