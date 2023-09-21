using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MMA.Documents.Domain.Helpers;
using MMA.WebApi.DataAccess.Extensions;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.ApplicationUsers;
using MMA.WebApi.Shared.Interfaces.Categories;
using MMA.WebApi.Shared.Interfaces.Collection;
using MMA.WebApi.Shared.Interfaces.Companies;
using MMA.WebApi.Shared.Interfaces.Document;
using MMA.WebApi.Shared.Interfaces.Domain.Document;
using MMA.WebApi.Shared.Interfaces.LogAnalytics;
using MMA.WebApi.Shared.Interfaces.MailStorage;
using MMA.WebApi.Shared.Interfaces.OfferDocuments;
using MMA.WebApi.Shared.Interfaces.Offers;
using MMA.WebApi.Shared.Interfaces.Tag;
using MMA.WebApi.Shared.Models.ApplicationUser;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.Image;
using MMA.WebApi.Shared.Models.Location;
using MMA.WebApi.Shared.Models.Logger;
using MMA.WebApi.Shared.Models.MailStorage;
using MMA.WebApi.Shared.Models.Mobile;
using MMA.WebApi.Shared.Models.Offer;
using MMA.WebApi.Shared.Models.OfferDocument;
using MMA.WebApi.Shared.Models.OneHubModels;
using MMA.WebApi.Shared.Monads;
using MMA.WebApi.Shared.Visitor;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Declares;
using static MMA.WebApi.Shared.Enums.Document.DocumentDeclares;

namespace MMA.WebApi.Core.Services
{
    public class OfferService : IOfferService
    {
        private readonly IOfferRepository _offerRepository;
        private readonly IDocumentRepository _documentRepository;
        private readonly IDocumentService _documentService;
        private readonly IApplicationUserService _applicationUserService;
        private readonly IConfiguration _configuration;
        private readonly IOfferDocumentRepository _offerDocumentRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICollectionRepository _collectionRepository;
        private readonly ITagRepository _tagRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly ILogAnalyticsService _logAnalyticsService;
        private HttpContext httpcontext;
        private readonly IMailStorageService _mailStorageServiceService;
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IRoadshowLocationService _roadshowLocationService;

        public OfferService(
            IOfferRepository offerRepository,
            ICompanyRepository companyRepository,
            IApplicationUserService applicationUserService,
            IDocumentService documentService,
            IConfiguration configuration,
            IDocumentRepository documentRepository,
            IOfferDocumentRepository offerDocumentRepository,
            IHttpContextAccessor httpaccess,
            UserManager<ApplicationUser> userManager,
            IMailStorageService mailStorageServiceService,
            ICategoryRepository categoryRepository,
            ICollectionRepository collectionRepository,
            ITagRepository tagRepository,
            IRoadshowLocationService roadshowLocationService,
            ILogAnalyticsService logAnalyticsService
        )
        {
            _roadshowLocationService = roadshowLocationService;
            _logAnalyticsService = logAnalyticsService;
            _offerRepository = offerRepository;
            _companyRepository = companyRepository;
            _documentService = documentService;
            _applicationUserService = applicationUserService;
            _configuration = configuration;
            _documentRepository = documentRepository;
            _offerDocumentRepository = offerDocumentRepository;
            _userManager = userManager;
            _mailStorageServiceService = mailStorageServiceService;
            _categoryRepository = categoryRepository;
            _collectionRepository = collectionRepository;
            _tagRepository = tagRepository;
            httpcontext = httpaccess.HttpContext;
        }

        public async Task<IEnumerable<OfferModel>> GetOffers()
        {
            var offers = await _offerRepository.Get().ToListAsync();

            return offers;
        }

        public async Task RateOffer(int rating, int offerId)
        {
            string userId = httpcontext.User.Claims.First(x => x.Type == "userId").Value;
            await _offerRepository.RateOffer(rating, offerId, userId);
        }

        public async Task<PaginationListModel<OfferModel>> GetMyOffers(
            QueryModel queryModel,
            string userId
        )
        {
            var roles = GetUserRoles(userId);
            var offers = _offerRepository.GetMyOffers(userId, roles, queryModel);
            if (roles.Contains(Roles.Buyer))
            {
                BackgroundJob.Enqueue(
                    () =>
                        _logAnalyticsService.LogSearchKeywordAdnOffer(
                            queryModel.Filter.Keyword,
                            offers.Select(x => x.Id).ToList(),
                            userId
                        )
                );
            }

            return await offers.ToPagedListAsync(
                queryModel.PaginationParameters.PageNumber,
                queryModel.PaginationParameters.PageSize
            );
        }

        public async Task<PaginationListModel<OfferModel>> GetAllOffers(
            QueryModel queryModel,
            string userId
        )
        {
            var roles = GetUserRoles(userId);
            var offers = await _offerRepository.GetAllOffers(
                userId,
                roles,
                queryModel,
                roles.Contains(Roles.Buyer)
            );
            if (roles.Contains(Roles.Buyer))
            {
                BackgroundJob.Enqueue(
                    () =>
                        _logAnalyticsService.LogSearchKeywordAdnOffer(
                            queryModel.Filter.Keyword,
                            offers.Select(x => x.Id).ToList(),
                            userId
                        )
                );
            }

            return await offers.ToPagedListAsync(
                queryModel.PaginationParameters.PageNumber,
                queryModel.PaginationParameters.PageSize
            );
        }

        public async Task<PaginationListModel<OfferOneHubCardModel>> GetAllOffersForOneHub(
            OneHubQueryModel queryModel,
            string userId
        )
        {
            var roles = GetUserRoles(userId);
            var offers = await _offerRepository.GetAllOffersForOneHub(userId, roles, queryModel);
            if (roles.Contains(Roles.Buyer))
            {
                var offerIds = offers.Select(x => x.Id).ToList();
                BackgroundJob.Enqueue(
                    () =>
                        _logAnalyticsService.LogSearchKeywordAdnOffer(
                            queryModel.Filter.Keyword,
                            offerIds,
                            userId
                        )
                );
            }

            return await offers.ToPagedListAsync(
                queryModel.PaginationParameters.PageNumber,
                queryModel.PaginationParameters.PageSize
            );
        }

        public async Task<PaginationListModel<OfferModel>> GetSpecificOffersPage(
            QueryModel queryModel,
            string userId
        )
        {
            var roles = GetUserRoles(userId);
            var offers = _offerRepository.GetSpecificOffers(userId, roles, queryModel);

            if (roles.Contains(Roles.Buyer))
            {
                BackgroundJob.Enqueue(
                    () =>
                        _logAnalyticsService.LogSearchKeywordAdnOffer(
                            queryModel.Filter.Keyword,
                            offers.Select(x => x.Id).ToList(),
                            userId
                        )
                );
            }
            return await offers.ToPagedListAsync(
                queryModel.PaginationParameters.PageNumber,
                queryModel.PaginationParameters.PageSize
            );
        }

        private static void ProcessImages(List<OfferModel> offers)
        {
            foreach (var offer in offers)
            {
                offer.ImageUrls = new List<ImageUrlsModel>();

                foreach (var imageGroping in offer.Images.GroupBy(x => x.OriginalImageId))
                {
                    ImageUrlsModel imageUrlsModel = new ImageUrlsModel();
                    foreach (var imageModel in imageGroping)
                    {
                        if (imageModel.Type == OfferDocumentType.QRCode)
                        {
                            imageUrlsModel.QRCode = imageModel.Id;
                            continue;
                        }
                        else if (imageModel.Type == OfferDocumentType.Original)
                        {
                            imageUrlsModel.Original = imageModel.Id;
                        }
                        else if (imageModel.Type == OfferDocumentType.Thumbnail)
                        {
                            imageUrlsModel.Thumbnail = imageModel.Id;
                            offer.MainImage = imageModel.Id;
                        }
                        else if (imageModel.Type == OfferDocumentType.Large)
                        {
                            imageUrlsModel.Large = imageModel.Id;
                        }
                    }
                    if (imageUrlsModel.QRCode == null)
                        offer.ImageUrls.Add(imageUrlsModel);
                }
            }
        }

        public List<Roles> GetUserRoles(string userId)
        {
            ApplicationUser applicationUser = _userManager.FindByIdAsync(userId).Result;

            List<Roles> roles = new List<Roles>();
            foreach (string userRole in _userManager.GetRolesAsync(applicationUser).Result.ToList())
            {
                Enum.TryParse(userRole, out Roles role);
                roles.Add(role);
            }

            return roles;
        }

        private async Task ProcessComment(List<OfferModel> offers)
        {
            // Get text for deleted user from config file
            var deletedUser = _configuration["DeletedUser"];
            foreach (var offer in offers)
            {
                offer.Comments = offer.Comments.OrderByDescending(x => x.CreatedOn).ToList();
                foreach (var comment in offer.Comments)
                {
                    await _mailStorageServiceService.SetCreatedByName(deletedUser, comment);
                }
            }
        }

        public async Task<IEnumerable<OfferModel>> GetWeekendOffers(string userId)
        {
            var roles = GetUserRoles(userId);

            var offers = await _offerRepository
                .GetWeekendOffers(userId, roles.Contains(Roles.Buyer))
                .ToListAsync();

            return offers;
        }

        public async Task<PaginationListModel<OfferModel>> GetWeekendOffers(QueryModel queryModel)
        {
            var offers = _offerRepository.GetAllWeekendOffers(queryModel);

            var roles = GetUserRoles(queryModel.UserId);

            if (roles.Contains(Roles.Buyer))
            {
                BackgroundJob.Enqueue(
                    () =>
                        _logAnalyticsService.LogSearchKeywordAdnOffer(
                            queryModel.Filter.Keyword,
                            offers.Select(x => x.Id).ToList(),
                            queryModel.UserId
                        )
                );
            }
            return await offers.ToPagedListAsync(
                queryModel.PaginationParameters.PageNumber,
                queryModel.PaginationParameters.PageSize
            );
        }

        public async Task<PaginationListModel<OfferModel>> GetOffersByCategoryPage(
            int categoryId,
            QueryModel queryModel
        )
        {
            var roles = GetUserRoles(queryModel.UserId);
            var offers = _offerRepository.GetOffersByCategoryPage(
                categoryId,
                queryModel,
                queryModel.UserId,
                roles.Contains(Roles.Buyer)
            );

            if (roles.Contains(Roles.Buyer))
            {
                BackgroundJob.Enqueue(
                    () =>
                        _logAnalyticsService.LogSearchKeywordAdnOffer(
                            queryModel.Filter.Keyword,
                            offers.Select(x => x.Id).ToList(),
                            queryModel.UserId
                        )
                );
            }
            return await offers.ToPagedListAsync(
                queryModel.PaginationParameters.PageNumber,
                queryModel.PaginationParameters.PageSize
            );
        }

        public async Task<IEnumerable<OfferModel>> GetOffersByCategory(int categoryId)
        {
            return _offerRepository.GetOffersByCategory(categoryId).AsEnumerable();
        }

        public async Task<PaginationListModel<OfferModel>> GetOffersByCollectionPage(
            int collectionId,
            QueryModel queryModel,
            string userId,
            bool isBuyer
        )
        {
            var offers = _offerRepository.GetOffersByCollectionPage(
                collectionId,
                queryModel,
                userId,
                isBuyer
            );
            var roles = GetUserRoles(queryModel.UserId);

            if (roles.Contains(Roles.Buyer))
            {
                BackgroundJob.Enqueue(
                    () =>
                        _logAnalyticsService.LogSearchKeywordAdnOffer(
                            queryModel.Filter.Keyword,
                            offers.Select(x => x.Id).ToList(),
                            queryModel.UserId
                        )
                );
            }

            return await offers.ToPagedListAsync(
                queryModel.PaginationParameters.PageNumber,
                queryModel.PaginationParameters.PageSize
            );
        }

        public async Task<PaginationListModel<OfferModel>> GetOffersByTagPage(
            int tagId,
            QueryModel queryModel
        )
        {
            var offers = _offerRepository.GetOffersByTagPage(tagId, queryModel);
            var roles = GetUserRoles(queryModel.UserId);

            if (roles.Contains(Roles.Buyer))
            {
                BackgroundJob.Enqueue(
                    () =>
                        _logAnalyticsService.LogSearchKeywordAdnOffer(
                            queryModel.Filter.Keyword,
                            offers.Select(x => x.Id).ToList(),
                            queryModel.UserId
                        )
                );
            }
            return await offers.ToPagedListAsync(
                queryModel.PaginationParameters.PageNumber,
                queryModel.PaginationParameters.PageSize
            );
        }

        public async Task<IEnumerable<OfferModel>> GetLatestOffers(int limit)
        {
            var offers = await _offerRepository.GetLatestOffers(limit).ToListAsync();

            return offers;
        }

        public async Task<Maybe<OfferModel>> GetSpecificOfferById(
            int id,
            string userId,
            List<Roles> roles
        )
        {
            var offer = await _offerRepository.GetSpecificOfferById(id, userId, roles);

            // Get average rating of all company offers and roadshow offers
            var companyRating = await _companyRepository.GetCompanyRating(offer.CompanyId);
            offer.AverageRatingOfAllCompanyOffers = Math.Round(companyRating.AverageRating, 2);
            offer.NumberOfVotesOnCompanyOffers = companyRating.TotalRatings;

            if (offer != null)
            {
                var offers = new List<OfferModel>();
                offers.Add(offer);

                ProcessImages(offers);

                List<ImageModel> onlyOriginalImages = new List<ImageModel>();

                foreach (var image in offer.Images)
                {
                    if (image.Type == OfferDocumentType.Original)
                    {
                        onlyOriginalImages.Add(image);
                    }
                }

                // Sort images so that cover is always the first image
                offer.Images = onlyOriginalImages.OrderByDescending(i => i.Cover).ToList();
                var coverImg = onlyOriginalImages.FirstOrDefault(i => i.Cover);
                //if (offer.ImageUrls != null)
                //    offer.ImageUrls = offer.ImageUrls.OrderBy(i => i.Original != coverImg.Id).ToList();

                await ProcessComment(offers);

                // Get text for deleted user from config file
                var deletedUser = _configuration["DeletedUser"];
                foreach (var attachment in offer.Attachments)
                {
                    var a = await _documentRepository.GetSingleAsync(
                        x => x.Id.ToString() == attachment.Id
                    );
                    attachment.Name = a == null ? deletedUser : a.Name;
                }

                offer.Rating = Math.Round(offer.Rating, 2);
                offer.RatingPercent = Math.Round(offer.RatingPercent, 2);
                return offer;
            }
            else
            {
                return null;
            }
        }

        public async Task<Maybe<OfferModel>> GetOfferById(
            int id,
            string userId,
            List<Declares.Roles> roles
        )
        {
            var offer = await _offerRepository.GetOfferById(id, userId, roles);

            // Get average rating of all company offers and roadshow offers
            var companyRating = await _companyRepository.GetCompanyRating(offer.CompanyId);
            offer.AverageRatingOfAllCompanyOffers = Math.Round(companyRating.AverageRating, 2);
            offer.NumberOfVotesOnCompanyOffers = companyRating.TotalRatings;
            //  offer.CompanyModel.CompanyRating = companyRating.AverageRating;

            // Check if offer is rated and favorite
            var isRatedAndFavorite = await _offerRepository.CheckIfOfferIsFavoriteAndRated(
                offer.Id,
                userId
            );
            offer.IsAlreadyRated = isRatedAndFavorite.IsRated;
            offer.IsFavourite = isRatedAndFavorite.IsFavorite;

            if (offer != null)
            {
                var offers = new List<OfferModel>();
                offers.Add(offer);

                ProcessImages(offers);

                if (offer != null)
                {
                    foreach (var attachment in offer.Attachments)
                    {
                        var a = await _documentRepository.GetSingleAsync(
                            x => x.Id.ToString() == attachment.Id
                        );
                        attachment.Name = a.Name;
                    }
                }

                offer.Rating = Math.Round(offer.Rating, 2);
                offer.RatingPercent = Math.Round(offer.RatingPercent, 2);
                return offer;
            }
            return null;
        }

        public async Task<Maybe<OfferOneHubModel>> GetOnehubOfferById(
            int id,
            string userId,
            List<Declares.Roles> roles
        )
        {
            var offer = await _offerRepository.GetOneHubOfferById(id, userId, roles);

            // Get average rating of all company offers and roadshow offers
            var companyRating = await _companyRepository.GetCompanyRating(offer.CompanyId);
            offer.AverageRatingOfAllCompanyOffers = Math.Round(companyRating.AverageRating, 2);
            offer.NumberOfVotesOnCompanyOffers = companyRating.TotalRatings;
            offer.CompanyModel.CompanyRating = companyRating.AverageRating;

            // Check if offer is rated and favorite
            var isRatedAndFavorite = await _offerRepository.CheckIfOfferIsFavoriteAndRated(
                offer.Id,
                userId
            );
            offer.IsAlreadyRated = isRatedAndFavorite.IsRated;
            offer.IsFavourite = isRatedAndFavorite.IsFavorite;
            var qRcodeImageId = await _offerRepository.GetQRCodeForOffer(id);
            var baseUrl = _configuration["BaseURL:ApiUrl"];

            if (qRcodeImageId == Guid.Empty)
            {
                var qRcodeImagedata = await _offerRepository.GenerateQRCodeWithLogo(id, userId);
                offer.QRcodeImage = baseUrl + "/media/" + qRcodeImagedata.Id + ".jpg";
            }
            else
                offer.QRcodeImage = baseUrl + "/media/" + qRcodeImageId + ".jpg";

            if (offer != null)
            {
                if (offer != null)
                {
                    foreach (var attachment in offer.Attachments)
                    {
                        var a = await _documentRepository.GetSingleAsync(
                            x => x.Id.ToString() == attachment.Id
                        );
                        attachment.Name = a.Name;
                    }
                }

                offer.Rating = Math.Round(offer.Rating, 2);
                offer.RatingPercent = Math.Round(offer.RatingPercent, 2);
                return offer;
            }
            return null;
        }

        public IEnumerable<OfferModelMobile> SelectValidOffers(
            DateTime lastUpdateOn,
            string userId,
            bool isSupplier
        )
        {
            if (isSupplier)
                return _offerRepository.SelectOffersForSupplier(lastUpdateOn, userId);
            return _offerRepository.SelectValidOffers(lastUpdateOn, userId);
        }

        public async Task<IEnumerable<OfferModelMobile>> SelectValidOffers()
        {
            return await _offerRepository.SelectValidOffers();
        }

        public async Task<PaginationListModel<ADNOCOneOfferModel>> SelectValidOfferView(
            OfferSearchModel queryModel
        )
        {
            var offers = _offerRepository.SelectValidOfferView();
            offers = offers.Where(GetSearchPredicate(queryModel));

            if (queryModel.OrderBy == OrderByType.Descending)
            {
                offers = offers.OrderByDescending(queryModel.SortBy);
            }
            else if (queryModel.OrderBy == OrderByType.Ascending)
            {
                offers = offers.OrderBy(queryModel.SortBy);
            }
            else
            {
                offers = offers.OrderByDescending(queryModel.SortBy);
            }

            if (!string.IsNullOrEmpty(queryModel.UserId))
            {
                var roles = GetUserRoles(queryModel.UserId);

                if (roles.Contains(Roles.Buyer))
                {
                    BackgroundJob.Enqueue(
                        () =>
                            _logAnalyticsService.LogSearchKeywordAdnOffer(
                                queryModel.SearchString,
                                offers.Select(x => x.Id).ToList(),
                                queryModel.UserId
                            )
                    );
                }
            }

            return await offers.ToPagedListAsync(queryModel.PageIndex, queryModel.PageSize);
        }

        public async Task<PaginationListModelExt<OneHubOfferModel>> SelectValidOfferViewOneHub(
            OfferSearchModel queryModel
        )
        {
            var offers = _offerRepository.SelectValidOfferViewOneHub();
            offers = offers.Where(GetSearchPredicateOneHub(queryModel));

            if (queryModel.OrderBy == OrderByType.Descending)
            {
                offers = offers.OrderByDescending(queryModel.SortBy);
            }
            else if (queryModel.OrderBy == OrderByType.Ascending)
            {
                offers = offers.OrderBy(queryModel.SortBy);
            }
            else
            {
                offers = offers.OrderByDescending(queryModel.SortBy);
            }

            var result = await offers.ToPagedListAsync(queryModel.PageIndex, queryModel.PageSize);
            var serializedParent = JsonConvert.SerializeObject(result);
            PaginationListModelExt<OneHubOfferModel> resultExt = JsonConvert.DeserializeObject<
                PaginationListModelExt<OneHubOfferModel>
            >(serializedParent);
            resultExt.BaseUrl = _configuration["BaseURL:Url"];

            return resultExt;
        }

        private Expression<Func<ADNOCOneOfferModel, bool>> GetSearchPredicate(
            OfferSearchModel dataQuery
        )
        {
            var predicate = PredicateBuilder.True<ADNOCOneOfferModel>();

            //var criterias = dataQuery.SearchString.Tokenize(" ");
            var criteria = dataQuery.SearchString;

            if (dataQuery.Columns != null)
            {
                foreach (var column in dataQuery.Columns)
                {
                    if (column.Name == "CategoryId")
                    {
                        int categoryId = 0;

                        int.TryParse(column.Data, out categoryId);

                        if (column.Operator.HasValue)
                        {
                            if (column.Operator.Value == DataQueryOperator.Eq)
                            {
                                predicate = predicate.And(c => c.CategoryId == categoryId);
                            }
                            else if (column.Operator.Value == DataQueryOperator.NotEq)
                            {
                                predicate = predicate.And(c => c.CategoryId != categoryId);
                            }
                            else if (column.Operator.Value == DataQueryOperator.Or)
                            {
                                predicate = predicate.Or(c => c.CategoryId == categoryId);
                            }
                        }
                        else
                        {
                            predicate = predicate.And(c => c.CategoryId == categoryId);
                        }

                        continue;
                    }

                    if (column.Name == "CollectionId")
                    {
                        var collectionId = 0;

                        if (!int.TryParse(column.Data, out collectionId))
                        {
                            continue;
                        }

                        predicate = predicate.And(c => c.CollectionId == collectionId);
                    }

                    if (column.Name == "Tag")
                    {
                        var tagId = 0;

                        if (!int.TryParse(column.Data, out tagId))
                        {
                            continue;
                        }

                        var tag = (Declares.Tag)tagId;

                        if (column.Operator == null)
                        {
                            continue;
                        }

                        switch (column.Operator.Value)
                        {
                            case DataQueryOperator.Eq:
                                predicate = predicate.And(c => c.Tag == tag);
                                break;
                            case DataQueryOperator.NotEq:
                                predicate = predicate.And(c => c.Tag == tag);
                                break;
                            case DataQueryOperator.Or:
                                predicate = predicate.Or(c => c.Tag == tag);
                                break;
                        }
                    }
                }
            }
            return predicate;
        }

        private Expression<Func<OneHubOfferModel, bool>> GetSearchPredicateOneHub(
            OfferSearchModel dataQuery
        )
        {
            var predicate = PredicateBuilder.True<OneHubOfferModel>();

            //var criterias = dataQuery.SearchString.Tokenize(" ");
            var criteria = dataQuery.SearchString;

            if (dataQuery.Columns != null)
            {
                foreach (var column in dataQuery.Columns)
                {
                    if (column.Name == "CategoryId")
                    {
                        int categoryId = 0;

                        int.TryParse(column.Data, out categoryId);

                        if (column.Operator.HasValue)
                        {
                            if (column.Operator.Value == DataQueryOperator.Eq)
                            {
                                predicate = predicate.And(c => c.CategoryId == categoryId);
                            }
                            else if (column.Operator.Value == DataQueryOperator.NotEq)
                            {
                                predicate = predicate.And(c => c.CategoryId != categoryId);
                            }
                            else if (column.Operator.Value == DataQueryOperator.Or)
                            {
                                predicate = predicate.Or(c => c.CategoryId == categoryId);
                            }
                        }
                        else
                        {
                            predicate = predicate.And(c => c.CategoryId == categoryId);
                        }

                        continue;
                    }
                }
            }
            return predicate;
        }

        public IEnumerable<int> SelectValidAndLiveOffersForUser(string userId, bool isBuyer)
        {
            return _offerRepository.SelectValidAndLiveOffersforUser(userId, isBuyer);
        }

        public IEnumerable<int> SelectValidAndLiveOffers()
        {
            return _offerRepository.SelectValidAndLiveOffers();
        }

        public async Task UpdateStatus(OfferModel model, string userId)
        {
            var roles = GetUserRoles(userId);
            await _offerRepository.UpdateStatus(model, roles, userId);
        }

        public async Task<string> Step(OfferModel model, string userId)
        {
            var roles = GetUserRoles(userId);

            var savedOffer = await _offerRepository.GetOffer(model.Id);

            foreach (var comment in model.Comments)
            {
                comment.OfferId = model.Id;
                comment.CreatedBy = userId;
                comment.CreatedOn = DateTime.UtcNow;
            }

            savedOffer.Comments = model.Comments;
            savedOffer.Action = model.Action;

            if (
                (
                    roles.Contains(Roles.Supplier)
                    || (roles.Contains(Roles.SupplierAdmin))
                        && model.Status == OfferStatus.Draft.ToString()
                )
            )
            {
                if (IsOfferModelValid(savedOffer))
                {
                    if (savedOffer.Action == "next")
                    {
                        savedOffer.Status = OfferStatus.Review.ToString();
                        await _offerRepository.UpdateStatus(savedOffer, roles, userId);

                        await NotifyReviewerNewOfferCreated(model.Id);

                        return $"Status is now - {OfferStatus.Review}";
                    }
                    else
                    {
                        return "Error - No action";
                    }
                }
                else
                {
                    return "Error - Offer Model not valid";
                }
            }
            else if (
                roles.Contains(Roles.Reviewer) && model.Status == OfferStatus.Review.ToString()
            )
            {
                if (IsOfferModelValid(savedOffer))
                {
                    if (savedOffer.Action == "next")
                    {
                        savedOffer.Status = OfferStatus.PendingApproval.ToString();
                        await _offerRepository.UpdateStatus(savedOffer, roles, userId);

                        await NotifyCoordinatorOfferToProcess(model.Id);

                        return $"Status is now - {OfferStatus.PendingApproval}";
                    }
                    else if (savedOffer.Action == "prev")
                    {
                        savedOffer.Status = OfferStatus.Draft.ToString();
                        await _offerRepository.UpdateStatus(savedOffer, roles, userId);

                        await NotifySupplierOfferReturned(model.Id);

                        return $"Status is now - {OfferStatus.Draft}";
                    }
                    else
                    {
                        return "Action is not appropiate";
                    }
                }
                else
                {
                    return "Error - Offer Model not valid";
                }
            }
            else if (
                (roles.Contains(Roles.AdnocCoordinator) || roles.Contains(Roles.Admin))
                && (
                    model.Status == OfferStatus.PendingApproval.ToString()
                    || model.Status == OfferStatus.Draft.ToString()
                )
            )
            {
                if (IsOfferModelValid(savedOffer))
                {
                    if (savedOffer.Action == "next")
                    {
                        savedOffer.Status = OfferStatus.Approved.ToString();
                        await _offerRepository.UpdateStatus(savedOffer, roles, userId);

                        await NotifySupplierOfferProcessed(model.Id);

                        return $"Status is now - {OfferStatus.Approved}";
                    }
                    else if (savedOffer.Action == "prev")
                    {
                        savedOffer.Status = OfferStatus.Draft.ToString();
                        await _offerRepository.UpdateStatus(savedOffer, roles, userId);

                        await NotifySupplierOfferReturned(model.Id);

                        return $"Status is now - {OfferStatus.Draft}";
                    }
                    else if (savedOffer.Action == "reject")
                    {
                        savedOffer.Status = OfferStatus.Rejected.ToString();
                        await _offerRepository.UpdateStatus(savedOffer, roles, userId);

                        await NotifySupplierOfferProcessed(model.Id);

                        return $"Status is now - {OfferStatus.Rejected}";
                    }
                    else
                    {
                        return "Action is not appropiate";
                    }
                }
                else
                {
                    return "Error - Offer Model not valid";
                }
            }
            else if (
                (roles.Contains(Roles.AdnocCoordinator) || roles.Contains(Roles.Admin))
                && model.Status == OfferStatus.Approved.ToString()
            )
            {
                if (IsOfferModelValid(savedOffer))
                {
                    if (savedOffer.Action == "block")
                    {
                        savedOffer.Status = OfferStatus.Blocked.ToString();
                        await _offerRepository.UpdateStatus(savedOffer, roles, userId);

                        await NotifySupplierOfferProcessed(model.Id);

                        return $"Status is now - {OfferStatus.Blocked}";
                    }
                }
                return "Error - Offer Model not valid";
            }
            else if (
                (roles.Contains(Roles.AdnocCoordinator) || roles.Contains(Roles.Admin))
                && model.Status == OfferStatus.Blocked.ToString()
            )
            {
                if (IsOfferModelValid(savedOffer))
                {
                    if (savedOffer.Action == "unblock")
                    {
                        savedOffer.Status = OfferStatus.Draft.ToString();
                        await _offerRepository.UpdateStatus(savedOffer, roles, userId);

                        await NotifySupplierOfferProcessed(model.Id);

                        return $"Status is now - {OfferStatus.Draft}";
                    }
                }
                return "Error - Offer Model not valid";
            }
            else
            {
                return "You do not have appropiate role";
            }
        }

        private bool IsOfferModelValid(OfferModel model)
        {
            bool isValid = true;
            StringBuilder sb = new StringBuilder();

            if (string.IsNullOrWhiteSpace(model.Title))
            {
                sb.AppendLine("Title is not OK");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(model.Description))
            {
                sb.AppendLine("Description is not OK");
                isValid = false;
            }

            bool priceOK =
                model.PriceFrom != null
                || model.PriceTo != null
                || model.DiscountFrom != null
                || model.DiscountTo != null
                || model.Discount != null
                || model.OriginalPrice != null
                || model.DiscountedPrice != null
                || !string.IsNullOrWhiteSpace(model.PriceCustom);
            if (!priceOK)
            {
                sb.AppendLine("Price field is not OK");
                isValid = false;
            }

            bool notNull =
                model.Categories != null
                && model.ValidFrom != null
                && model.ValidUntil != null
                && model.Images != null;
            if (!notNull)
            {
                sb.AppendLine("Plese select at least one value from drop-down");
                isValid = false;
            }

            if (!isValid)
            {
                throw new Exception(sb.ToString());
            }

            return isValid;
        }

        private bool OfferBannerValid(OfferModel model)
        {
            bool isValid = true;
            StringBuilder sb = new StringBuilder();

            if (model.BannerActive.Value)
            {
                int bannerCount = _offerRepository.GetBannerOffersCount();

                var banner = _offerRepository.GetBannerOffers().ToList();

                if (bannerCount >= 10 && !banner.Contains(model.Id))
                {
                    sb.AppendLine("There are already 10 offers in banner, please remove some");
                    isValid = false;
                }
            }

            if (!isValid)
            {
                throw new Exception(sb.ToString());
            }

            return isValid;
        }

        public async Task<Maybe<OfferModel>> CreateOrUpdate(
            OfferModel model,
            string userId,
            List<Declares.Roles> roles
        )
        {
            var auditVisitor = new CreateAuditVisitor(userId, DateTime.UtcNow);

            // Decode encoded text
            model.PriceList = DecodeBase64String(model.PriceList);
            model.TermsAndCondition = DecodeBase64String(model.TermsAndCondition);
            model.AboutCompany = DecodeBase64String(model.AboutCompany);
            model.Description = DecodeBase64String(model.Description);

            if (IsOfferModelValid(model) && OfferBannerValid(model))
            {
                var user = await _userManager.FindByIdAsync(userId);
                var rolesForUser = await _userManager.GetRolesAsync(user);

                if (
                    rolesForUser.Contains(Declares.Roles.AdnocCoordinator.ToString())
                    || rolesForUser.Contains(Declares.Roles.Admin.ToString())
                )
                {
                    var supplierAdminId = await _companyRepository.GetSupplierAdminForCompany(
                        model.CompanyId
                    );
                    if (supplierAdminId != null)
                        auditVisitor = new CreateAuditVisitor(supplierAdminId, DateTime.UtcNow);
                }
                var offer = await _offerRepository.CreateAsync(model, auditVisitor, userId, roles);

                if (model.Id == 0)
                {
                    if (
                        rolesForUser.Contains(Declares.Roles.AdnocCoordinator.ToString())
                        || rolesForUser.Contains(Declares.Roles.Admin.ToString())
                    )
                    {
                        await NotifySupplierAdminOfferCreated(offer.Id);
                    }
                }

                //Group images in the sets of 3 - Original, Large and Thumbnail
                GroupImages(model, offer);

                foreach (var category in offer.Categories)
                {
                    var offerCategory = await _categoryRepository.Get(category.Id);
                    category.Title = offerCategory.Title;
                }

                foreach (var collection in offer.Collections)
                {
                    var offerCollection = await _collectionRepository.Get(collection.Id);
                    collection.Title = offerCollection.Title;
                }

                foreach (var tag in offer.Tags)
                {
                    var offerTag = await _tagRepository.Get(tag.Id);
                    tag.Title = offerTag.Title;
                }

                return offer;
            }

            return model;
        }

        public async Task<OfferModel> DeltaMigration(
            OfferModel model,
            string userId,
            List<Declares.Roles> roles
        )
        {
            var auditVisitor = new CreateAuditVisitor(userId, DateTime.UtcNow);

            // Decode encoded text
            model.PriceList = DecodeBase64String(model.PriceList);
            model.TermsAndCondition = DecodeBase64String(model.TermsAndCondition);
            model.AboutCompany = DecodeBase64String(model.AboutCompany);
            model.Description = DecodeBase64String(model.Description);

            var offer = await _offerRepository.DeltaMigration(model, auditVisitor, userId, roles);

            return offer;
        }

        private static void GroupImages(OfferModel model, OfferModel offer)
        {
            offer.ImageUrls = new List<ImageUrlsModel>();
            Guid coverIMG = Guid.Empty;

            if (model.Images != null && model.Images.Count > 0)
            {
                foreach (var image in model.Images)
                {
                    if (image.OriginalImageId == Guid.Empty)
                    {
                        image.OriginalImageId = new Guid(image.Id);
                        image.Cover = image.Cover;
                    }

                    if (image.Cover)
                    {
                        coverIMG = image.OriginalImageId;
                    }
                }

                var grouping = offer.Images.GroupBy(x => x.OriginalImageId);
                foreach (var imageGroping in grouping)
                {
                    ImageUrlsModel imageUrlsModel = new ImageUrlsModel();
                    foreach (var imageModel in imageGroping)
                    {
                        if (
                            imageModel.OriginalImageId == coverIMG
                            && imageModel.Type == OfferDocumentType.Thumbnail
                        )
                        {
                            imageModel.Cover = true;
                            imageUrlsModel.Thumbnail = imageModel.Id;
                            offer.MainImage = imageModel.Id;
                        }
                        else if (imageModel.Type == OfferDocumentType.Thumbnail)
                        {
                            imageUrlsModel.Thumbnail = imageModel.Id;
                        }
                        else if (imageModel.Type == OfferDocumentType.Large)
                        {
                            imageUrlsModel.Large = imageModel.Id;
                        }
                        else
                        {
                            imageUrlsModel.Original = imageModel.Id;
                        }

                        if (imageModel.OriginalImageId == coverIMG)
                        {
                            imageModel.Cover = true;
                        }
                    }
                    offer.ImageUrls.Add(imageUrlsModel);
                }
            }
        }

        //Skip images that are unchanged during Review phase
        private async Task<bool> ShouldProcessImage(ImageModel image)
        {
            var imageDb = await _offerDocumentRepository.GetByDocumentId(new Guid(image.Id));

            if (
                imageDb != null
                && imageDb.X1 == image.CropCoordinates.X1
                && imageDb.X2 == image.CropCoordinates.X2
                && imageDb.Y1 == image.CropCoordinates.Y1
                && imageDb.Y2 == image.CropCoordinates.Y2
            )
            {
                await _offerDocumentRepository.UpdateOfferImagesCover(
                    new Guid(image.Id),
                    image.Cover
                );
                return false;
            }
            else
            {
                //Delete old thumbanail and large image
                var imagesToDelete = _offerDocumentRepository
                    .GetOfferImages(new Guid(image.Id))
                    .Where(
                        x =>
                            x.Type == OfferDocumentType.Thumbnail
                            || x.Type == OfferDocumentType.Large
                    );
                foreach (OfferDocumentModel imageToDelete in imagesToDelete)
                {
                    await _offerDocumentRepository.DeleteAsync(imageToDelete.Id);
                }

                return true;
            }
        }

        public async Task NotifyReviewerNewOfferCreated(int offerId)
        {
            //Get all reviewers
            var result = await GetOffer(offerId);
            OfferModel offerModel = result.Value;

            var reviewers = await _userManager.GetUsersInRoleAsync("Reviewer");
            foreach (var reviewer in reviewers)
            {
                ApplicationUserModel applicationUserModel = new ApplicationUserModel
                {
                    Email = reviewer.Email,
                    Id = reviewer.Id
                };

                var emailData = new EmailDataModel()
                {
                    User = applicationUserModel,
                    MailTemplateId = Declares.MessageTemplateList.Offer_To_Process_Notify_Reviewer,
                    OfferId = offerId,
                    CompanyName = offerModel.CompanyNameEnglish
                };

                await _mailStorageServiceService.CreateMail(emailData);
            }
        }

        public async Task NotifySupplierOfferReturned(int offerId)
        {
            var result = await GetOffer(offerId);
            OfferModel offerModel = result.Value;

            ApplicationUser offerCreator = await _userManager.FindByIdAsync(offerModel.CreatedBy);
            ApplicationUserModel applicationUserModel = new ApplicationUserModel
            {
                Email = offerCreator.Email,
                Id = offerCreator.Id
            };

            var emailData = new EmailDataModel()
            {
                User = applicationUserModel,
                MailTemplateId = Declares
                    .MessageTemplateList
                    .Offer_Returned_Notify_SupplierAdminOrSupplier,
                OfferId = offerId,
                CompanyName = offerModel.CompanyNameEnglish
            };

            await _mailStorageServiceService.CreateMail(emailData);
        }

        public async Task NotifySupplierOfferProcessed(int offerId)
        {
            var result = await GetOffer(offerId);
            OfferModel offerModel = result.Value;

            ApplicationUser offerCreator = await _userManager.FindByIdAsync(offerModel.CreatedBy);
            if (offerCreator == null)
                return;
            ApplicationUserModel applicationUserModel = new ApplicationUserModel
            {
                Email = offerCreator.Email,
                Id = offerCreator.Id
            };

            var emailData = new EmailDataModel()
            {
                User = applicationUserModel,
                MailTemplateId = Declares
                    .MessageTemplateList
                    .Offer_Processed_Notify_SupplierAdminOrSupplier,
                IsApproved = offerModel.Status == OfferStatus.Approved.ToString(),
                OfferId = offerId,
                CompanyName = offerModel.CompanyNameEnglish
            };

            await _mailStorageServiceService.CreateMail(emailData);
        }

        public async Task NotifySupplierAdminOfferCreated(int offerId)
        {
            var result = await GetOffer(offerId);
            OfferModel offerModel = result.Value;

            ApplicationUser offerCreator = await _userManager.FindByIdAsync(offerModel.CreatedBy);
            ApplicationUserModel applicationUserModel = new ApplicationUserModel
            {
                Email = offerCreator.Email,
                Id = offerCreator.Id
            };

            var emailData = new EmailDataModel()
            {
                User = applicationUserModel,
                MailTemplateId = Declares
                    .MessageTemplateList
                    .Offer_Created_Notify_SupplierAdminOrSupplier,
                IsApproved = offerModel.Status == OfferStatus.Approved.ToString(),
                OfferId = offerId,
                CompanyName = offerModel.CompanyNameEnglish
            };

            await _mailStorageServiceService.CreateMail(emailData);
        }

        public async Task NotifyCoordinatorOfferToProcess(int offerId)
        {
            var coordinators = await _userManager.GetUsersInRoleAsync("ADNOC Coordinator");

            if (!coordinators.Any())
                return;

            var result = await GetOffer(offerId);
            OfferModel offerModel = result.Value;

            var messageTemplate = Declares.MessageTemplateList.Offer_To_Process_Notify_Coordinator;
            var isApproved = offerModel.Status == OfferStatus.Approved.ToString();
            var emailData = _mailStorageServiceService.CreateMailData(
                coordinators.FirstOrDefault().Id,
                offerId,
                offerModel.CompanyNameEnglish,
                messageTemplate,
                isApproved
            );
            await _mailStorageServiceService.CreateMail(emailData);
        }

        public async Task<Maybe<OfferModel>> UpdateOffer(
            OfferModel model,
            string userId,
            List<Roles> roles
        )
        {
            var auditVisitor = new UpdateAuditVisitor(userId, DateTime.UtcNow);

            // Decode encoded text
            model.PriceList = DecodeBase64String(model.PriceList);
            model.TermsAndCondition = DecodeBase64String(model.TermsAndCondition);
            model.AboutCompany = DecodeBase64String(model.AboutCompany);
            model.Description = DecodeBase64String(model.Description);

            return await _offerRepository.UpdateAsync(model, auditVisitor, userId, roles);
        }

        public async Task<PaginationListModel<OfferModel>> GetOffersSearchPageAsync(
            QueryModel queryModel,
            string userId
        )
        {
            var roles = GetUserRoles(userId);

            var offers = _offerRepository.GetOffersSearchPage(roles, queryModel, userId);
            if (roles.Contains(Roles.Buyer))
            {
                BackgroundJob.Enqueue(
                    () =>
                        _logAnalyticsService.LogSearchKeywordAdnOffer(
                            queryModel.Filter.Keyword,
                            offers.Select(x => x.Id).ToList(),
                            userId
                        )
                );
            }

            return await offers.ToPagedListAsync(
                queryModel.PaginationParameters.PageNumber,
                queryModel.PaginationParameters.PageSize
            );
        }

        public async Task DeleteOffer(int id, string userId)
        {
            var offerModel = await _offerRepository.DeleteAsync(id, userId);
            if (offerModel != null && offerModel.Images != null)
            {
                foreach (var image in offerModel.Images)
                {
                    DocumentProvider provider = DocumentProviderFactory.GetDocumentProvider(
                        DocumentProviderFactory.Operator.azureblobstorage,
                        _documentRepository,
                        _configuration
                    );
                    provider.Delete(new Guid(image.Id));
                }
            }
        }

        public async Task SetOfferAsFavourite(OfferFavoriteModel offerFavorite, string userId)
        {
            await _offerRepository.SetOfferAsFavourite(offerFavorite, userId);
        }

        public async Task ShareOffer(OfferShareModel offerShareModel, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return;

            await _mailStorageServiceService.CreateShareMail(
                userId,
                offerShareModel.ShareTo,
                offerShareModel.Subject,
                offerShareModel.Message
            );
        }

        public async Task<PaginationListModel<OfferModel>> GetFavoritesOffersPage(
            QueryModel queryModel,
            string userId
        )
        {
            var roles = GetUserRoles(userId);
            var flag = roles.Contains(Roles.Buyer);
            var offers = _offerRepository.GetFavoritesOffersPage(queryModel, userId, flag);

            if (roles.Contains(Roles.Buyer))
            {
                BackgroundJob.Enqueue(
                    () =>
                        _logAnalyticsService.LogSearchKeywordAdnOffer(
                            queryModel.Filter.Keyword,
                            offers.Select(x => x.Id).ToList(),
                            userId
                        )
                );
            }

            return await offers.ToPagedListAsync(
                queryModel.PaginationParameters.PageNumber,
                queryModel.PaginationParameters.PageSize
            );
        }

        public async Task<PaginationListModel<OfferOneHubCardModel>> GetFavoritesOffersOnehubPage(
            OneHubQueryModel queryModel,
            string userId
        )
        {
            var roles = GetUserRoles(userId);
            var flag = roles.Contains(Roles.Buyer);
            var offers = _offerRepository.GetFavoritesOffersOneHubPage(queryModel, userId, flag);

            if (roles.Contains(Roles.Buyer))
            {
                BackgroundJob.Enqueue(
                    () =>
                        _logAnalyticsService.LogSearchKeywordAdnOffer(
                            queryModel.Filter.Keyword,
                            offers.Select(x => x.Id).ToList(),
                            userId
                        )
                );
            }

            return await offers.ToPagedListAsync(
                queryModel.PaginationParameters.PageNumber,
                queryModel.PaginationParameters.PageSize
            );
        }

        /// <summary>
        /// Checks if QR Code exitst and based on that returns it or generates it
        /// </summary>
        /// <param name="offerId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<ImageModel> GetQRCodeForOffer(int offerId, string userId)
        {
            var qrCodeID = await _offerRepository.GetQRCodeForOffer(offerId);

            if (qrCodeID == Guid.Empty)
            {
                var qrCodeIMG = await _offerRepository.GenerateQRCodeWithLogo(offerId, userId);
                qrCodeID = qrCodeIMG.Id;
            }

            return new ImageModel()
            {
                Id = qrCodeID.ToString(),
                OriginalImageId = qrCodeID,
                Type = OfferDocumentType.QRCode,
                CropCoordinates = new CropCoordinates()
                {
                    X1 = 0,
                    X2 = 0,
                    Y1 = 0,
                    Y2 = 0
                },
                CropNGXCoordinates = new CropCoordinates()
                {
                    X1 = 0,
                    X2 = 0,
                    Y1 = 0,
                    Y2 = 0
                }
            };
        }

        private void SendMail(OfferShareModel shareModel, string userMail)
        {
            string smtpHost = _configuration["Emails:MailHost"];
            string smtpPassword = _configuration["Emails:EmailPassword"];
            int smtpPort = Convert.ToInt32(_configuration["Emails:MailServerPort"]);
            bool EnableSsl = Convert.ToBoolean(_configuration["Emails:EnableSsl"]);
            bool UseDefaultCredentials = Convert.ToBoolean(
                _configuration["Emails:UseDefaultCredentials"]
            );
            string fromEmail = _configuration["Emails:FromAddress"];
            string from = _configuration["Emails:From"];

            using (MailMessage mailMessage = new MailMessage())
            {
                MailAddress fromAddress = new MailAddress(userMail, from);
                mailMessage.Subject = shareModel.Subject;
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = shareModel.Message;
                mailMessage.To.Add(new MailAddress(shareModel.ShareTo));

                using (SmtpClient SmtpServer = new SmtpClient())
                {
                    mailMessage.From = fromAddress;

                    SmtpServer.Host = smtpHost;
                    SmtpServer.Port = smtpPort;
                    SmtpServer.EnableSsl = EnableSsl;
                    SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                    SmtpServer.UseDefaultCredentials = UseDefaultCredentials;
                    SmtpServer.Credentials = new NetworkCredential(fromEmail, smtpPassword);

                    try
                    {
                        SmtpServer.Send(mailMessage);
                    }
                    catch (Exception ex)
                    {
                        Logger _logger = new Logger();
                        _logger.Error("Email Error:" + ex.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Checks what offers are expired, changes there status and sends mail to suppliers and ADNOC Team
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public async Task CheckExpiredOffers(ILogger logger)
        {
            try
            {
                logger.LogInformation($"CheckExpiredOffers in service -> before call to repo");

                var expiredOffers = await _offerRepository.CheckExpiredOffers(logger);

                if (expiredOffers != null)
                {
                    foreach (var o in expiredOffers)
                    {
                        await SendMailToSupplierAndAdnocTeam(
                            o,
                            MessageTemplateList.Offer_Expired_Notify_SupplierAdminOrSupplier,
                            MessageTemplateList.Offer_Expired_Notify_Coordinator
                        );
                    }

                    logger.LogInformation($"CheckExpiredOffers in service -> after call to repo");
                }
            }
            catch (Exception e)
            {
                logger.LogInformation(
                    $"CheckExpiredOffers caused exception in service: {e.ToString()}"
                );
            }
        }

        /// <summary>
        /// Gets all offers that are about to expire (3 weeks, 1 week and 1 day before expiration)
        /// Sends mail to suppliers and ADNOC Team
        /// </summary>
        /// <returns></returns>
        public async Task OffersAboutToExpire()
        {
            try
            {
                var expiredOffers = await _offerRepository.OffersAboutToExpire();

                foreach (var o in expiredOffers.ThreeWeeksBeforeExpiration)
                {
                    await SendMailToSupplierAndAdnocTeam(
                        o,
                        MessageTemplateList.Offer_To_Expire_In_3_Weeks_Notify_SupplierAdminOrSupplier,
                        MessageTemplateList.Offer_To_Expire_In_3_Weeks_Notify_Coordinator
                    );
                }

                foreach (var o in expiredOffers.WeekBeforeExpiration)
                {
                    await SendMailToSupplierAndAdnocTeam(
                        o,
                        MessageTemplateList.Offer_To_Expire_In_A_Week_Notify_SupplierAdminOrSupplier,
                        MessageTemplateList.Offer_To_Expire_In_A_Week_Notify_Coordinator
                    );
                }

                foreach (var o in expiredOffers.DayBeforeExpiration)
                {
                    await SendMailToSupplierAndAdnocTeam(
                        o,
                        MessageTemplateList.Offer_To_Expire_In_A_Day_Notify_SupplierAdminOrSupplier,
                        MessageTemplateList.Offer_To_Expire_In_A_Day_Notify_Coordinator
                    );
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        private async Task SendMailToSupplierAndAdnocTeam(
            OfferModel o,
            MessageTemplateList supplierMessageTemplate,
            MessageTemplateList adnocTeamMessageTemplate
        )
        {
            var emailData = new EmailDataModel();
            var company = await _companyRepository
                .Get()
                .FirstOrDefaultAsync(c => c.Id == o.CompanyId);
            emailData.MailTemplateId = supplierMessageTemplate;
            emailData.OfferId = o.Id;
            emailData.CompanyName = company.NameEnglish;

            var userId = await _companyRepository.GetSupplierAdminForCompany(o.CompanyId);
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                emailData.User = new ApplicationUserModel()
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName
                };

                emailData.MailTemplateId = adnocTeamMessageTemplate;
                await _mailStorageServiceService.CreateMail(emailData);
            }

            await _mailStorageServiceService.CreateMailForAdnocTeam(emailData);
        }

        public bool CanManageOffers(List<Roles> roles, OfferModel offerModel, string userId)
        {
            // buyer false
            // admin true

            // Admins can do everything
            if (roles.Contains(Roles.Admin) || roles.Contains(Roles.AdnocCoordinator))
                return true;
            // Buyer can't do anything with offers
            if (roles.Contains(Roles.Buyer))
                return false;

            var company = _companyRepository.GetMyCompany(userId).Result;
            // 1.  offer se kreira
            //
            // samo supplier && suppliery.companny == offermode.company  + admin
            if (offerModel.Id == 0)
            {
                if (
                    (roles.Contains(Roles.SupplierAdmin) || roles.Contains(Roles.Supplier))
                    && offerModel.CompanyId == company.Id
                )
                    return true;
            }
            // 2. offer update
            // revierw if status=Review return true
            // supplier if status=draft,returned && supplier.compnayid = offer.compmpnayid &&
            else
            {
                var offer = _offerRepository.GetOffer(offerModel.Id).Result;
                if (
                    roles.Contains(Roles.Reviewer)
                    && offerModel.Status == OfferStatus.Review.ToString()
                )
                    return true;
                if (
                    roles.Contains(Roles.Reviewer)
                    && (
                        offerModel.Status == OfferStatus.Approved.ToString()
                        || offerModel.Status == OfferStatus.Rejected.ToString()
                        || offerModel.Status == OfferStatus.Draft.ToString()
                    )
                )
                    return true;
                if (
                    (roles.Contains(Roles.SupplierAdmin) || roles.Contains(Roles.Supplier))
                    && offer.CompanyId == company.Id
                    && (
                        offerModel.Status == OfferStatus.Draft.ToString()
                        || offerModel.Status == OfferStatus.Review.ToString()
                    )
                    && offer.Status == OfferStatus.Draft.ToString()
                )
                    return true;
            }

            return false;
        }

        public async Task<Maybe<OfferModel>> GetOffer(int id)
        {
            return await _offerRepository.GetOffer(id);
        }

        public IQueryable<DefaultAreaModel> GetAllDefaultAreas()
        {
            return _offerRepository.GetAllDefaultAreas();
        }

        public DefaultAreaModel GetDefaultAreaById(int id)
        {
            return _offerRepository.GetDefaultAreaById(id);
        }

        public async Task PostDefaultArea(DefaultAreaModel model)
        {
            var response = await _offerRepository.PostDefaultArea(model);
            if (!response)
            {
                throw new Exception("Area already exists");
            }
        }

        public async Task DeleteDefaultArea(int id)
        {
            await _offerRepository.DeleteDefaultArea(id);
        }

        public async Task PutDefaultArea(DefaultAreaModel model)
        {
            await _offerRepository.PutDefaultArea(model);
        }

        public Task<OfferModelMobile> GetSpecificOfferByIdForMobile(int id, string userId)
        {
            return _offerRepository.GetSpecificOfferByIdForMobile(id, userId);
        }

        public async Task<string> SendPushNotification()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SendPushNotification");

            var offerCnt = await _offerRepository.CheckIfThereIsNewOffersToSendPushNotification();

            if (offerCnt > 0)
            {
                sb.AppendLine("CheckIfThereIsNewOffersToSendPushNotification ok");

                var title = _configuration["Firebase:OfferPushNotificationTitle"];

                // New implementation, for different days of week we send different messages
                var currentDay = DateTime.UtcNow.DayOfWeek;
                var message = currentDay switch
                {
                    DayOfWeek.Sunday
                        => _configuration["Firebase:OfferPushNotificationMessageSunday"],
                    DayOfWeek.Tuesday
                        => _configuration["Firebase:OfferPushNotificationMessageTuesday"],
                    DayOfWeek.Thursday
                        => _configuration["Firebase:OfferPushNotificationMessageThursday"],
                    _ => _configuration["Firebase:OfferPushNotificationMessage"]
                };

                var clickAction = _configuration["Firebase:OfferPushNotificationClickAction"];

                var formattedMessage = message.Replace("@@OFFERS-NUMBER@@", offerCnt.ToString());

                try
                {
                    var buyerCnt =
                        await _applicationUserService.CreateFcmNotificationForSpecificRoles(
                            Roles.Buyer,
                            formattedMessage,
                            title,
                            clickAction
                        );
                    var coordinatorCnt =
                        await _applicationUserService.CreateFcmNotificationForSpecificRoles(
                            Roles.AdnocCoordinator,
                            formattedMessage,
                            title,
                            clickAction
                        );
                    sb.AppendLine(
                        $"CheckIfThereIsNewOffersToSendPushNotification buyer device count: {buyerCnt}"
                    );
                    sb.AppendLine(
                        $"CheckIfThereIsNewOffersToSendPushNotification buyer device count: {coordinatorCnt}"
                    );
                    sb.AppendLine("CreateFcmNotificationForSpecificRoles ok");
                }
                catch (Exception ex)
                {
                    sb.AppendLine($"CreateFcmNotificationForSpecificRoles: {ex.ToString()}");
                }
            }

            return sb.ToString();
        }

        public async Task<string> SendPushNotification(string title, string message)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SendPushNotification");

            var clickAction = _configuration["Firebase:OfferPushNotificationClickAction"];

            sb.AppendLine(
                $"CheckIfThereIsNewOffersToSendPushNotification clickAction {clickAction}"
            );

            try
            {
                var buyerCnt = await _applicationUserService.CreateFcmNotificationForSpecificRoles(
                    Roles.Buyer,
                    message,
                    title,
                    clickAction
                );
                var coordinatorCnt =
                    await _applicationUserService.CreateFcmNotificationForSpecificRoles(
                        Roles.AdnocCoordinator,
                        message,
                        title,
                        clickAction
                    );

                sb.AppendLine($"CreateFcmNotificationForSpecificRoles buyerCnt {buyerCnt}");
                sb.AppendLine(
                    $"CreateFcmNotificationForSpecificRoles coordinatorCnt {coordinatorCnt}"
                );
            }
            catch (Exception ex)
            {
                sb.AppendLine($"CreateFcmNotificationForSpecificRoles: {ex.ToString()}");
            }

            return sb.ToString();
        }

        public IEnumerable<OfferSupplierCategoryModel> GetOffersThatHaveEmptyAboutCompany()
        {
            return _offerRepository.GetOffersThatHaveEmptyAboutCompany();
        }

        public async Task ReturnToPending(int offerId)
        {
            await _offerRepository.ReturnToPending(offerId);
        }

        public async Task TransferOffers(TransferOffersModel transferOffersModel)
        {
            await _offerRepository.TransferOffers(transferOffersModel);
        }

        private string DecodeBase64String(string encodedString)
        {
            byte[] data = Convert.FromBase64String(encodedString);
            return Encoding.UTF8.GetString(data);
        }

        public async Task<PaginationListModel<OfferModel>> GetOffersForMembership(
            QueryModel queryModel,
            int membershipType
        )
        {
            return await _offerRepository.GetOffersForMembership(queryModel, membershipType);
        }

        public async Task<byte[]> GetPdfQRCodeForOffer(int offerId, string userId)
        {
            var img = await _offerRepository.GetQRCodeData(offerId);
            if (img == null)
            {
                var qrCodeIMG = await _offerRepository.GenerateQRCodeWithLogo(offerId, userId);
                img = qrCodeIMG.Content;
            }

            var imgWithBack = myQRCodeGenerator.createQRCodeWithBackgroun(img);
            var pdfArray = myQRCodeGenerator.CreatePdfsReturnArrayFromQRCode(imgWithBack);
            return pdfArray;
        }

        public async Task<SynchronizationDataModel> GenerateSynchronizationData()
        {
            SynchronizationDataModel synchronizationDataModel = new SynchronizationDataModel
            {
                UpdatedOn = DateTime.UtcNow
            };

            var tasks = new List<Task>
            {
                Task.Run(async () => synchronizationDataModel.Offers = await SelectValidOffers()),
                Task.Run(() => synchronizationDataModel.OffersIds = SelectValidAndLiveOffers()),
                Task.Run(() => GetCategories(synchronizationDataModel)),
                Task.Run(() => GetCollections(synchronizationDataModel)),
                Task.Run(() => GetTags(synchronizationDataModel)),
                Task.Run(() => GetTagsIds(synchronizationDataModel)),
                Task.Run(
                    async () =>
                        synchronizationDataModel.RoadshowsOffers = (
                            await _roadshowLocationService.GetAllRoadshowOffersMobile()
                        ).OrderBy(x => x.Id)
                ),
                Task.Run(
                    async () =>
                        synchronizationDataModel.RoadshowsOfferIds = (
                            await _roadshowLocationService.GetValidRoadshowOffersIds()
                        ).Distinct()
                )
            };

            await Task.WhenAll(tasks);

            return synchronizationDataModel;
        }

        private async Task GetCategories(SynchronizationDataModel synchronizationDataModel)
        {
            var query = _categoryRepository.GetCategoriesWithOfferNumber();
            var categories = await Task.FromResult(query.ToList());
            synchronizationDataModel.Categories = categories.OrderBy(c => c.Id);
            synchronizationDataModel.CategoriesIds = categories.Select(c => c.Id);
        }

        private async Task GetCollections(SynchronizationDataModel synchronizationDataModel)
        {
            var collections = await _collectionRepository.Get().ToListAsync();
            synchronizationDataModel.Collections = collections.OrderBy(c => c.Id);
            synchronizationDataModel.CollectionsIds = collections.Select(c => c.Id);
        }

        private async Task GetTagsIds(SynchronizationDataModel synchronizationDataModel)
        {
            var tagIds = await _tagRepository.Get().ToListAsync();
            synchronizationDataModel.TagIds = tagIds.Select(t => t.Id);
        }

        private async Task GetTags(SynchronizationDataModel synchronizationDataModel)
        {
            var tags = await _tagRepository.GetTags();
            synchronizationDataModel.Tags = tags.OrderBy(t => t.Id);
        }

        public async Task<IEnumerable<OfferModelMobile>> GetOfferSpecificData(string userId)
        {
            var data = await _offerRepository.GetOfferSpecificData(userId);
            return data;
        }

        public async Task RenewOffer(int offerId)
        {
            await _offerRepository.RenewOffer(offerId);
        }
    }
}
