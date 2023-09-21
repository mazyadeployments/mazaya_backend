using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MMA.WebApi.DataAccess.Extensions;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Extensions;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.Companies;
using MMA.WebApi.Shared.Interfaces.Domain.Document;
using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Interfaces.Membership;
using MMA.WebApi.Shared.Interfaces.OfferDocuments;
using MMA.WebApi.Shared.Interfaces.Offers;
using MMA.WebApi.Shared.Models.Attachment;
using MMA.WebApi.Shared.Models.Category;
using MMA.WebApi.Shared.Models.Collection;
using MMA.WebApi.Shared.Models.Comments;
using MMA.WebApi.Shared.Models.Document;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.Image;
using MMA.WebApi.Shared.Models.Location;
using MMA.WebApi.Shared.Models.Offer;
using MMA.WebApi.Shared.Models.OfferDocument;
using MMA.WebApi.Shared.Models.OneHubModels;
using MMA.WebApi.Shared.Models.Ratings;
using MMA.WebApi.Shared.Models.Tag;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Declares;
using static MMA.WebApi.Shared.Enums.Document.DocumentDeclares;
using Document = MMA.WebApi.DataAccess.Models.Document;

namespace MMA.WebApi.DataAccess.Repository.Offers
{
    public class OfferRepository : BaseRepository<OfferModel>, IOfferRepository
    {
        private const string MIGRATION_DATE = "2021-02-05";
        private readonly IOfferDocumentRepository _offerDocumentRepository;
        private readonly IConfiguration _configuration;
        private readonly ICompanyRepository _companyRepository;
        private readonly IMembershipECardRepository _membershipECardRepository;
        private readonly IDocumentService _documentService;
        private static string baseUrl;

        public OfferRepository(
            Func<MMADbContext> contexFactory,
            IOfferDocumentRepository offerDocumentRepository,
            IConfiguration configuration,
            ICompanyRepository companyRepository,
            IMembershipECardRepository membershipECardRepository,
            IDocumentService documentService
        )
            : base(contexFactory)
        {
            _offerDocumentRepository = offerDocumentRepository;
            _configuration = configuration;
            _companyRepository = companyRepository;
            _membershipECardRepository = membershipECardRepository;
            _documentService = documentService;
            baseUrl = _configuration["BaseURL:ApiUrl"];
        }

        public IQueryable<OfferModel> Get()
        {
            var context = ContextFactory();

            return context.Offer.AsNoTracking().Select(projectToOfferCardModel);
        }

        public async Task<IQueryable<OfferModel>> GetAllOffers(
            string userId,
            List<Declares.Roles> roles,
            QueryModel queryModel,
            bool isUser
        )
        {
            var context = ContextFactory();
            IQueryable<Offer> offers = context.Offer.Include(o => o.OfferLocations).AsNoTracking();
            IQueryable<OfferModel> offerModels = null;
            IQueryable<Offer> filteredOffers = null;
            HashSet<string> userMemberships = new HashSet<string>();

            userMemberships = _membershipECardRepository
                .GetMembershipsForUser(userId, false)
                .Select(x => x.Id.ToString())
                .ToHashSet();
            if (roles.Contains(Roles.SupplierAdmin) || roles.Contains(Roles.Supplier))
            {
                //In my tab Supplier or Supplier Admin can see all offers from their company, regardless of status
                var userCompanyId = context.Company
                    .Where(x => x.CompanySuppliers.Any(cs => cs.SupplierId == userId))
                    .FirstOrDefault()
                    .Id;
                offers = offers.Where(o => o.CompanyId == userCompanyId);
            }
            else if (roles.Contains(Roles.Reviewer))
            {
                offers = offers.Where(
                    o =>
                        o.Status != OfferStatus.Draft.ToString()
                        && o.Status != OfferStatus.Review.ToString()
                );
            }
            else if (roles.Contains(Roles.Buyer))
            {
                userMemberships = _membershipECardRepository
                    .GetMembershipsForUser(userId, true)
                    .Select(x => x.Id.ToString())
                    .ToHashSet();

                if (userMemberships.Count() == 0)
                    offers = offers.Where(
                        o =>
                            o.Status == OfferStatus.Approved.ToString()
                            && o.ValidFrom <= DateTime.UtcNow
                            && o.ValidUntil > DateTime.UtcNow
                            && !o.IsPrivate
                    );
                else
                    offers = offers.Where(
                        o =>
                            o.Status == OfferStatus.Approved.ToString()
                            && o.ValidFrom <= DateTime.UtcNow
                            && o.ValidUntil > DateTime.UtcNow
                    );
            }

            var filteredMembershipOffers = FilterForMembership(offers, userMemberships);
            filteredOffers = Filter(filteredMembershipOffers, queryModel);
            offerModels = filteredOffers.Select(projectToOfferCardModel);

            return Sort(queryModel.Sort, offerModels);
        }

        public async Task<IQueryable<OfferOneHubCardModel>> GetAllOffersForOneHub(
            string userId,
            List<Declares.Roles> roles,
            OneHubQueryModel queryModel
        )
        {
            var context = ContextFactory();
            IQueryable<Offer> offers = context.Offer.Include(o => o.OfferLocations).AsNoTracking();
            IQueryable<OfferOneHubCardModel> offerModels = null;
            IQueryable<Offer> filteredOffers = null;
            HashSet<string> userMemberships = new HashSet<string>();

            userMemberships = _membershipECardRepository
                .GetMembershipsForUser(userId, false)
                .Select(x => x.Id.ToString())
                .ToHashSet();
            if (roles.Contains(Roles.SupplierAdmin) || roles.Contains(Roles.Supplier))
            {
                //In my tab Supplier or Supplier Admin can see all offers from their company, regardless of status
                var userCompanyId = context.Company
                    .Where(x => x.CompanySuppliers.Any(cs => cs.SupplierId == userId))
                    .FirstOrDefault()
                    .Id;
                offers = offers.Where(o => o.CompanyId == userCompanyId);
            }
            else if (roles.Contains(Roles.Reviewer))
            {
                offers = offers.Where(
                    o =>
                        o.Status != OfferStatus.Draft.ToString()
                        && o.Status != OfferStatus.Review.ToString()
                );
            }
            else if (roles.Contains(Roles.Buyer))
            {
                userMemberships = _membershipECardRepository
                    .GetMembershipsForUser(userId, true)
                    .Select(x => x.Id.ToString())
                    .ToHashSet();

                if (userMemberships.Count() == 0)
                    offers = offers.Where(
                        o =>
                            o.Status == OfferStatus.Approved.ToString()
                            && o.ValidFrom <= DateTime.UtcNow
                            && o.ValidUntil > DateTime.UtcNow
                            && !o.IsPrivate
                    );
                else
                    offers = offers.Where(
                        o =>
                            o.Status == OfferStatus.Approved.ToString()
                            && o.ValidFrom <= DateTime.UtcNow
                            && o.ValidUntil > DateTime.UtcNow
                    );
            }

            var filteredMembershipOffers = FilterForMembership(offers, userMemberships);
            filteredOffers = OneHubFilter(filteredMembershipOffers, queryModel);
            offerModels = filteredOffers.Select(projectToOfferOneHubCardModel);

            return Sort(queryModel.Sort, offerModels);
        }

        public IQueryable<OfferModel> GetSpecificOffers(
            string userId,
            List<Roles> roles,
            QueryModel queryModel
        )
        {
            var context = ContextFactory();
            IQueryable<Offer> offers = context.Offer.Include(o => o.OfferLocations).AsNoTracking();
            IQueryable<OfferModel> offerModels = null;
            IQueryable<Offer> filteredOffers = null;

            if (roles.Contains(Roles.Reviewer))
            {
                offers = offers.Where(o => o.Status == OfferStatus.Review.ToString());
            }
            else if (roles.Contains(Roles.AdnocCoordinator) || roles.Contains(Roles.Admin))
            {
                offers = offers.Where(o => o.Status == OfferStatus.PendingApproval.ToString());
            }

            filteredOffers = Filter(offers, queryModel);
            offerModels = filteredOffers.Select(projectToOfferCardModel);

            return Sort(queryModel.Sort, offerModels);
        }

        public IQueryable<OfferModel> GetMyOffers(
            string userId,
            List<Roles> roles,
            QueryModel queryModel
        )
        {
            var context = ContextFactory();
            IQueryable<Offer> offers = context.Offer.Include(o => o.OfferLocations).AsNoTracking();
            IQueryable<OfferModel> offerModels = null;
            IQueryable<Offer> filteredOffers = null;

            if (roles.Contains(Roles.Supplier) || roles.Contains(Roles.SupplierAdmin))
            {
                //In my tab Supplier or Supplier Admin sees only offers he created, regardless of status
                offers = offers.Where(o => o.CreatedBy == userId);
            }
            else if (roles.Contains(Roles.Reviewer))
            {
                offers = offers.Where(o => o.ReviewedBy == userId);
            }
            else if (roles.Contains(Roles.AdnocCoordinator) || roles.Contains(Roles.Admin))
            {
                offers = offers.Where(o => o.DecisionBy == userId);
            }

            filteredOffers = Filter(offers, queryModel);
            offerModels = filteredOffers.Select(projectToOfferCardModel);

            return Sort(queryModel.Sort, offerModels);
        }

        private static IQueryable<OfferModel> Sort(
            SortModel sortModel,
            IQueryable<OfferModel> offers
        )
        {
            if (sortModel.Type == "price")
            {
                if (sortModel.Order == Order.DESC)
                {
                    return offers.OrderByDescending(x => x.PriceFrom);
                }
                else
                {
                    return offers.OrderBy(x => x.PriceFrom);
                }
            }
            else if (sortModel.Type == "date")
            {
                if (sortModel.Order == Order.DESC)
                {
                    return offers.OrderByDescending(x => x.UpdatedOn);
                }
                else
                {
                    return offers.OrderBy(x => x.UpdatedOn);
                }
            }
            else
            {
                return offers.OrderByDescending(x => x.UpdatedOn);
            }
        }

        private static IQueryable<OfferOneHubCardModel> Sort(
            SortModel sortModel,
            IQueryable<OfferOneHubCardModel> offers
        )
        {
            if (sortModel.Type == "price")
            {
                if (sortModel.Order == Order.DESC)
                {
                    return offers.OrderByDescending(x => x.PriceFrom);
                }
                else
                {
                    return offers.OrderBy(x => x.PriceFrom);
                }
            }
            else if (sortModel.Type == "date")
            {
                if (sortModel.Order == Order.DESC)
                {
                    return offers.OrderByDescending(x => x.UpdatedOn);
                }
                else
                {
                    return offers.OrderBy(x => x.UpdatedOn);
                }
            }
            else
            {
                return offers.OrderByDescending(x => x.UpdatedOn);
            }
        }

        public IQueryable<OfferModel> GetAllWeekendOffers(QueryModel queryModel)
        {
            var context = ContextFactory();
            IQueryable<Offer> offers = context.Offer.Include(o => o.OfferLocations).AsNoTracking();
            IQueryable<OfferModel> offerModels = null;
            IQueryable<Offer> filteredOffers = null;

            offers = offers.Where(o => o.FlagIsWeekendOffer);

            filteredOffers = Filter(offers, queryModel);
            offerModels = filteredOffers.Select(projectToOfferCardModel);

            return Sort(queryModel.Sort, offerModels);
        }

        public IQueryable<OfferModel> GetFavoritesOffersPage(
            QueryModel queryModel,
            string userId,
            bool isBuyer
        )
        {
            var context = ContextFactory();
            IQueryable<Offer> offers = context.Offer.Include(o => o.OfferLocations).AsNoTracking();
            IQueryable<OfferModel> offerModels = null;
            IQueryable<Offer> filteredOffers = null;

            offers = offers.Where(
                o => o.UserFavouritesOffers.Any(o => o.ApplicationUserId == userId && o.IsFavourite)
            );

            var userMemberships = _membershipECardRepository
                .GetMembershipsForUser(userId, isBuyer)
                .Select(x => x.Id.ToString())
                .ToHashSet();
            var filteredMembershipOffers = FilterForMembership(offers, userMemberships);

            filteredOffers = Filter(offers, queryModel);
            offerModels = filteredOffers.Select(projectToOfferCardModel);

            return Sort(queryModel.Sort, offerModels);
        }

        public IQueryable<OfferOneHubCardModel> GetFavoritesOffersOneHubPage(
            OneHubQueryModel queryModel,
            string userId,
            bool isBuyer
        )
        {
            var context = ContextFactory();
            IQueryable<Offer> offers = context.Offer.Include(o => o.OfferLocations).AsNoTracking();
            IQueryable<OfferOneHubCardModel> offerModels = null;
            IQueryable<Offer> filteredOffers = null;

            offers = offers.Where(
                o => o.UserFavouritesOffers.Any(o => o.ApplicationUserId == userId && o.IsFavourite)
            );

            var userMemberships = _membershipECardRepository
                .GetMembershipsForUser(userId, isBuyer)
                .Select(x => x.Id.ToString())
                .ToHashSet();
            var filteredMembershipOffers = FilterForMembership(offers, userMemberships);

            filteredOffers = OneHubFilter(offers, queryModel);
            offerModels = filteredOffers.Select(projectToOfferOneHubCardModel);

            return Sort(queryModel.Sort, offerModels);
        }

        public static IQueryable<Offer> FilterForMembership(
            IQueryable<Offer> offers,
            HashSet<string> userMemberships
        )
        {
            IQueryable<Offer> offerMembershipFilter = offers;

            if (userMemberships.Count() > 0)
                offerMembershipFilter = offerMembershipFilter.Where(
                    x =>
                        !x.IsPrivate
                        || x.Memberships.Count() > 0
                            && x.Memberships
                                .Where(x => userMemberships.Contains(x.MembershipId.ToString()))
                                .Any()
                );
            else
                offerMembershipFilter = offerMembershipFilter.Where(x => !x.IsPrivate);

            return offerMembershipFilter;
        }

        public static IQueryable<Offer> Filter(IQueryable<Offer> offers, QueryModel queryModel)
        {
            var filteredOffers = offers.Where(
                offer =>
                    offer.Title.ToLower().Contains(queryModel.Filter.Keyword.Trim().ToLower())
                    || offer.Company.NameEnglish
                        .ToLower()
                        .Contains(queryModel.Filter.Keyword.Trim().ToLower())
                    || offer.Brand.ToLower().Contains(queryModel.Filter.Keyword.Trim().ToLower())
                    || offer.Description
                        .ToLower()
                        .Contains(queryModel.Filter.Keyword.Trim().ToLower())
                    || offer.OfferCategories.Any(
                        x =>
                            x.Category.Title
                                .ToLower()
                                .Contains(queryModel.Filter.Keyword.Trim().ToLower())
                    )
            );

            if (queryModel.Filter.Categories?.Any() == true)
            {
                filteredOffers = filteredOffers.Where(
                    o =>
                        o.OfferCategories.Any(
                            oc => queryModel.Filter.Categories.Contains(oc.CategoryId)
                        )
                );
            }

            var membershipIds = queryModel.Filter.Memberships.Select(membership => membership.Id);
            if (membershipIds.Count() > 0)
                filteredOffers = filteredOffers.Where(
                    x =>
                        x.IsPrivate
                        && x.Memberships.Count() > 0
                        && x.Memberships.Where(x => membershipIds.Contains(x.MembershipId)).Any()
                );

            if (queryModel.Filter.Collections?.Any() == true)
            {
                filteredOffers = filteredOffers.Where(
                    o =>
                        o.OfferCollections.Any(
                            oc => queryModel.Filter.Collections.Contains(oc.CollectionId)
                        )
                );
            }

            if (queryModel.Filter.Areas?.Any() == true)
            {
                var areas = GetAreaIdFromObject(queryModel.Filter.Areas);

                filteredOffers = (
                    from o in filteredOffers
                    where o.OfferLocations.Any(ol => areas.Contains(ol.DefaultAreaId))
                    select o
                );
            }

            if (queryModel.Filter.Tags?.Any() == true)
            {
                filteredOffers = filteredOffers.Where(
                    o => o.OfferTags.Any(tag => queryModel.Filter.Tags.Contains(tag.TagId))
                );
            }

            if (queryModel.Filter.Status?.Any() == true)
            {
                filteredOffers = filteredOffers.Where(
                    o => queryModel.Filter.Status.Any(item => string.Equals(item, o.Status))
                );
            }

            if (queryModel.Filter.Ratings?.Any() == true)
            {
                var ratings = ConvertStringRatingToDecimal(queryModel.Filter.Ratings);

                var offerRatings = filteredOffers.SelectMany(o => o.OfferRating);

                var filteredRatings = (
                    from r in offerRatings
                    where r.Status == Declares.OfferCommentStatus.Public.ToString()
                    group r by r.OfferId into g
                    select new { OfferId = g.Key, Rating = g.Average(rt => rt.Rating) }
                ).Where(rt => ratings.Contains(Math.Floor(rt.Rating)));

                filteredOffers = filteredOffers.Where(
                    o => filteredRatings.Select(fr => fr.OfferId).Contains(o.Id)
                );
            }

            if (
                (queryModel.Filter.PriceFrom > 1 || queryModel.Filter.PriceTo < 100000)
                && (queryModel.Filter.DiscountFrom > 0 || queryModel.Filter.DiscountTo < 100)
            )
            {
                filteredOffers = filteredOffers
                    .Where(
                        o =>
                            (
                                (o.DiscountFrom != null && o.DiscountTo != null)
                                || (o.Discount != null && o.Discount != 0)
                                || (
                                    o.PriceFrom > queryModel.Filter.PriceFrom
                                    && o.PriceTo < queryModel.Filter.PriceTo
                                )
                                || (
                                    o.OriginalPrice > queryModel.Filter.PriceFrom
                                    && o.OriginalPrice < queryModel.Filter.PriceTo
                                )
                            )
                    )
                    .Union(
                        filteredOffers.Where(
                            o =>
                                (o.PriceFrom != null && o.PriceTo != null)
                                || (o.OriginalPrice != null && o.OriginalPrice != 0)
                                || (
                                    o.DiscountFrom > queryModel.Filter.DiscountFrom
                                    && o.DiscountTo < queryModel.Filter.DiscountTo
                                )
                                || (
                                    o.Discount != null
                                    && o.Discount != 0
                                    && o.Discount > queryModel.Filter.DiscountFrom
                                    && o.Discount < queryModel.Filter.DiscountTo
                                )
                        )
                    );
            }
            else if (queryModel.Filter.PriceFrom > 1 || queryModel.Filter.PriceTo < 100000)
            {
                filteredOffers = filteredOffers.Where(
                    o =>
                        (
                            (
                                (o.DiscountFrom != null && o.DiscountTo != null)
                                || (o.Discount != null && o.Discount != 0)
                                || (
                                    o.PriceFrom > queryModel.Filter.PriceFrom
                                    && o.PriceTo < queryModel.Filter.PriceTo
                                )
                                || (
                                    o.OriginalPrice > queryModel.Filter.PriceFrom
                                    && o.OriginalPrice < queryModel.Filter.PriceTo
                                )
                            )
                        )
                        && (
                            (
                                o.PriceFrom.HasValue
                                && o.PriceTo.HasValue
                                && o.PriceFrom > 0
                                && o.PriceTo > 0
                            )
                            || (
                                (o.OriginalPrice.HasValue || o.DiscountedPrice.HasValue)
                                && (o.OriginalPrice > 0 || o.DiscountedPrice > 0)
                            )
                        )
                );
            }
            else if (queryModel.Filter.DiscountFrom > 0 || queryModel.Filter.DiscountTo < 100)
            {
                filteredOffers = filteredOffers.Where(
                    o =>
                        (
                            (o.PriceFrom != null && o.PriceTo != null)
                            || (o.OriginalPrice != null && o.OriginalPrice != 0)
                            || (
                                o.DiscountFrom > queryModel.Filter.DiscountFrom
                                && o.DiscountTo < queryModel.Filter.DiscountTo
                            )
                            || (
                                o.Discount != null
                                && o.Discount != 0
                                && o.Discount > queryModel.Filter.DiscountFrom
                                && o.Discount < queryModel.Filter.DiscountTo
                            )
                        )
                        && (
                            (
                                o.DiscountFrom.HasValue
                                && o.DiscountTo.HasValue
                                && o.DiscountFrom > 0
                                && o.DiscountTo > 0
                            ) || (o.Discount.HasValue && o.Discount.Value > 0)
                        )
                );
            }

            return filteredOffers;
        }

        public static IQueryable<Offer> OneHubFilter(
            IQueryable<Offer> offers,
            OneHubQueryModel queryModel
        )
        {
            //get all valid offers
            var filteredOffers = offers.Where(
                x =>
                    x.Status == OfferStatus.Approved.ToString()
                    && x.ValidFrom < DateTime.UtcNow
                    && x.ValidUntil > DateTime.UtcNow
            );

            filteredOffers = filteredOffers.Where(
                offer =>
                    offer.Title.ToLower().Contains(queryModel.Filter.Keyword.Trim().ToLower())
                    || offer.Company.NameEnglish
                        .ToLower()
                        .Contains(queryModel.Filter.Keyword.Trim().ToLower())
                    || offer.Description
                        .ToLower()
                        .Contains(queryModel.Filter.Keyword.Trim().ToLower())
                    || offer.OfferCategories.Any(
                        x =>
                            x.Category.Title
                                .ToLower()
                                .Contains(queryModel.Filter.Keyword.Trim().ToLower())
                    )
            );

            if (queryModel.Filter.Categories?.Any() == true)
            {
                filteredOffers = filteredOffers.Where(
                    o =>
                        o.OfferCategories.Any(
                            oc => queryModel.Filter.Categories.Contains(oc.CategoryId)
                        )
                );
            }

            var membershipIds = queryModel.Filter.Memberships.Select(membership => membership.Id);
            if (membershipIds.Count() > 0)
                filteredOffers = filteredOffers.Where(
                    x =>
                        x.IsPrivate
                        && x.Memberships.Count() > 0
                        && x.Memberships.Where(x => membershipIds.Contains(x.MembershipId)).Any()
                );

            if (queryModel.Filter.Collections?.Any() == true)
            {
                filteredOffers = filteredOffers.Where(
                    o =>
                        o.OfferCollections.Any(
                            oc => queryModel.Filter.Collections.Contains(oc.CollectionId)
                        )
                );
            }

            if (queryModel.Filter.Areas?.Any() == true)
            {
                var areas = GetAreaIdFromObject(queryModel.Filter.Areas);

                filteredOffers = (
                    from o in filteredOffers
                    where o.OfferLocations.Any(ol => areas.Contains(ol.DefaultAreaId))
                    select o
                );
            }

            if (queryModel.Filter.Tags?.Any() == true)
            {
                filteredOffers = filteredOffers.Where(
                    o => o.OfferTags.Any(tag => queryModel.Filter.Tags.Contains(tag.TagId))
                );
            }

            if (queryModel.Filter.Ratings?.Any() == true)
            {
                var ratings = ConvertStringRatingToDecimal(queryModel.Filter.Ratings);

                var offerRatings = filteredOffers.SelectMany(o => o.OfferRating);

                var filteredRatings = (
                    from r in offerRatings
                    where r.Status == Declares.OfferCommentStatus.Public.ToString()
                    group r by r.OfferId into g
                    select new { OfferId = g.Key, Rating = g.Average(rt => rt.Rating) }
                ).Where(rt => ratings.Contains(Math.Floor(rt.Rating)));

                filteredOffers = filteredOffers.Where(
                    o => filteredRatings.Select(fr => fr.OfferId).Contains(o.Id)
                );
            }

            if (queryModel.Filter.PriceFrom > 1 || queryModel.Filter.PriceTo < 100000)
            {
                filteredOffers = filteredOffers.Where(
                    o =>
                        (
                            (o.DiscountFrom != null && o.DiscountTo != null)
                            || (o.Discount != null && o.Discount != 0)
                            || (
                                o.PriceFrom > queryModel.Filter.PriceFrom
                                && o.PriceTo < queryModel.Filter.PriceTo
                            )
                            || (
                                o.OriginalPrice > queryModel.Filter.PriceFrom
                                && o.OriginalPrice < queryModel.Filter.PriceTo
                            )
                        )
                );
            }

            if (queryModel.Filter.DiscountList.Count() > 0)
            {
                IQueryable<Offer> helperList = null;
                var discountOffer = filteredOffers;
                List<int> helperListIds = new List<int>();

                foreach (var discount in queryModel.Filter.DiscountList)
                {
                    discountOffer = (
                        filteredOffers.Where(
                            o =>
                                (
                                    o.PriceFrom == null
                                    && o.PriceTo == null
                                    && o.OriginalPrice == null
                                )
                                && (
                                    (
                                        discount.DiscountFrom <= o.DiscountFrom
                                        && o.DiscountFrom <= discount.DiscountTo
                                    )
                                    || (
                                        discount.DiscountFrom <= o.DiscountTo
                                        && o.DiscountTo <= discount.DiscountTo
                                    )
                                    || (
                                        o.Discount != null
                                        && o.Discount != 0
                                        && o.Discount >= discount.DiscountFrom
                                        && o.Discount <= discount.DiscountTo
                                    )
                                    || (
                                        o.DiscountFrom <= discount.DiscountFrom
                                        && discount.DiscountFrom <= o.DiscountTo
                                    )
                                    || (
                                        o.DiscountFrom <= discount.DiscountTo
                                        && discount.DiscountTo <= o.DiscountTo
                                    )
                                )
                        )
                    );

                    if (helperList == null)
                        helperList = discountOffer;
                    else
                        helperList = helperList.Union(discountOffer);

                    helperListIds.AddRange(helperList.Select(x => x.Id).ToList());
                }
                filteredOffers = helperList;
            }
            return filteredOffers;
        }

        private static List<int> GetAreaIdFromObject(List<DefaultAreaModel> defaultAreas)
        {
            var defaultAreaIds = new List<int>();
            defaultAreas.ForEach(x =>
            {
                defaultAreaIds.Add(x.Id);
            });

            return defaultAreaIds;
        }

        private static List<decimal> ConvertStringRatingToDecimal(List<string> stringRatings)
        {
            var ratings = new List<decimal>();
            stringRatings.ForEach(x =>
            {
                switch (x)
                {
                    case "5":
                        ratings.Add(5);
                        break;
                    case "4":
                        ratings.Add(4);
                        break;
                    case "3":
                        ratings.Add(3);
                        break;
                    case "2":
                        ratings.Add(2);
                        break;
                    case "1":
                        ratings.Add(1);
                        break;
                    default:
                        break;
                }
            });

            return ratings;
        }

        public IQueryable<OfferModel> GetReviewOffers()
        {
            var context = ContextFactory();

            return context.Offer
                .AsNoTracking()
                .Where(o => o.Status == OfferStatus.Review.ToString())
                .Select(projectToOfferCardModel);
        }

        public IQueryable<OfferModel> GetPendingOffers()
        {
            var context = ContextFactory();

            return context.Offer
                .AsNoTracking()
                .Where(o => o.Status == OfferStatus.PendingApproval.ToString())
                .Select(projectToOfferCardModel);
        }

        public IQueryable<OfferModelMobile> SelectValidOffers(DateTime lastUpdateOn, string userId)
        {
            var context = ContextFactory();

            var userMemberships = _membershipECardRepository
                .GetMembershipsForUser(userId, true)
                .Select(x => x.Id);
            //var baseContentUrl = _configuration["BaseURL:ApiUrl"];
            var defaultAreas = context.DefaultArea.ToList();
            defaultAreas.Add(new DefaultArea() { Id = 0, Title = DefaultAreas.Other.ToString() });

            var validOffers = (
                from o in context.Offer
                where
                    (
                        !o.ValidUntil.HasValue
                        || (o.ValidUntil.HasValue && o.ValidUntil.Value >= DateTime.UtcNow)
                    )
                    && (!o.ValidFrom.HasValue || o.ValidFrom.Value <= DateTime.UtcNow)
                    && (
                        o.UpdatedOn >= lastUpdateOn
                        || o.UserFavouritesOffers.Any(
                            ufo => ufo.ApplicationUserId == userId && ufo.UpdatedOn > lastUpdateOn
                        )
                    )
                    && o.Status == OfferStatus.Approved.ToString()
                    && (
                        !o.IsPrivate
                        || o.Memberships.Count() > 0
                            && o.Memberships
                                .Where(x => userMemberships.Contains(x.MembershipId))
                                .Any()
                    )
                select new OfferModelMobile
                {
                    AnnouncementActive = o.AnnouncementActive,
                    Comments = o.OfferRating
                        .Where(x => x.Status == OfferCommentStatus.Public.ToString())
                        .Select(
                            x =>
                                new CommentModel
                                {
                                    OfferId = x.OfferId,
                                    CreatedByName = context.Users
                                        .FirstOrDefault(u => x.CreatedBy == u.Id)
                                        .UserName,
                                    CreatedBy = x.CreatedBy,
                                    CreatedOn = x.CreatedOn,
                                    Text = x.CommentText,
                                    Rating = x.Rating
                                }
                        )
                        .ToList(),
                    CompanyId = o.CompanyId,
                    CompanyName = o.Company.NameEnglish,
                    CompanyNameEnglish = o.Company.NameEnglish,
                    CompanyNameArabic = o.Company.NameArabic,
                    CompanyLogo = o.Company.Logo.DocumentId.ToString(),
                    AboutCompany = o.AboutCompany,
                    CompanyWebsite = o.Company.Website,
                    CompanyPhoneNumber = o.Company.Land,
                    CategoryIds = o.OfferCategories.Select(x => x.CategoryId),
                    CollectionIds = o.OfferCollections.Select(x => x.CollectionId),
                    Description = o.Description,
                    DiscountFrom = o.DiscountFrom,
                    DiscountTo = o.DiscountTo,
                    Status = o.Status.ToString(),
                    BannerActive = o.BannerActive,
                    Id = o.Id,
                    Locations = o.OfferLocations
                        .Select(
                            ol =>
                                new OfferLocationModel
                                {
                                    Id = ol.Id,
                                    Address = ol.Address,
                                    Country = ol.Country,
                                    Latitude = ol.Latitude,
                                    Longitude = ol.Longitude,
                                    Vicinity = ol.Vicinity,
                                    DefaultAreaId = ol.DefaultAreaId,
                                    DefaultAreaTitle = MapDefaultAreaTitle(
                                        defaultAreas,
                                        ol.DefaultAreaId
                                    ),
                                }
                        )
                        .ToList(),
                    MainImage = o.OfferDocuments
                        .Where(od => (int)od.Type == 3 && od.Cover)
                        .Select(od => od.DocumentId.ToString())
                        .FirstOrDefault(),
                    PriceFrom = o.PriceFrom,
                    PriceTo = o.PriceTo,
                    PriceList = o.PriceList,
                    PromotionCode = o.PromotionCode,
                    TagIds = o.OfferTags.Select(x => x.TagId),
                    TermsAndCondition = o.TermsAndCondition,
                    Title = FormatTitle(
                        o.Title,
                        o.CreatedOn,
                        o.Company == null ? o.Title : o.Company.NameEnglish
                    ),
                    Brand = o.Brand,
                    ValidFrom = o.ValidFrom,
                    ValidUntil = o.ValidUntil,
                    Rating =
                        o.OfferRating.Count > 0
                            ? o.OfferRating
                                .Where(x => x.Status == OfferCommentStatus.Public.ToString())
                                .Average(x => x.Rating)
                            : 0,
                    RatingPercent =
                        o.OfferRating.Count > 0
                            ? o.OfferRating
                                .Where(x => x.Status == OfferCommentStatus.Public.ToString())
                                .Average(x => x.Rating) * 20
                            : 0,
                    Votes =
                        o.OfferRating.Count > 0
                            ? o.OfferRating
                                .Where(x => x.Status == OfferCommentStatus.Public.ToString())
                                .Count()
                            : 0,
                    WhatYouGet = o.WhatYouGet,
                    Images = FilterOfferImagesForMobile(o.OfferDocuments),
                    Attachments = o.OfferDocuments
                        .Where(od => od.Type == OfferDocumentType.Document)
                        .Select(
                            od =>
                                new AttachmentModel
                                {
                                    Id = !string.IsNullOrEmpty(od.DocumentId.ToString())
                                        ? od.DocumentId.ToString()
                                        : string.Empty,
                                    Name = od.Document.Name,
                                    Type = od.Document.MimeType
                                }
                        )
                        .ToList(),
                    CreatedBy = o.CreatedBy,
                    CreatedOn = o.CreatedOn,
                    UpdatedBy = o.UpdatedBy,
                    UpdatedOn = o.UpdatedOn,
                    ReviewedBy = o.ReviewedBy,
                    ReviewedOn = o.ReviewedOn,
                    DecisionBy = o.DecisionBy,
                    DecisionOn = o.DecisionOn,
                    OriginalPrice =
                        (o.OriginalPrice == o.DiscountedPrice)
                        || (
                            (o.DiscountedPrice.HasValue && !o.OriginalPrice.HasValue)
                            || (o.OriginalPrice.HasValue && !o.DiscountedPrice.HasValue)
                        )
                            ? null
                            : o.OriginalPrice,
                    DiscountedPrice =
                        (o.OriginalPrice == o.DiscountedPrice)
                        || (
                            (o.DiscountedPrice.HasValue && !o.OriginalPrice.HasValue)
                            || (o.OriginalPrice.HasValue && !o.DiscountedPrice.HasValue)
                        )
                            ? null
                            : o.DiscountedPrice,
                    Discount =
                        (!o.Discount.HasValue && o.Discount.Value <= 0)
                            && (o.OriginalPrice == o.DiscountedPrice)
                        || (
                            (o.DiscountedPrice.HasValue && !o.OriginalPrice.HasValue)
                            || (o.OriginalPrice.HasValue && !o.DiscountedPrice.HasValue)
                        )
                            ? null
                            : o.Discount,
                    PriceCustom = CheckIfCustomPrice(
                        o.Discount,
                        o.DiscountedPrice,
                        o.OriginalPrice,
                        o.PriceCustom
                    ),
                    //IsFavourite = o.UserFavouritesOffers.Where(ufo => ufo.OfferId == o.Id && ufo.ApplicationUserId == userId).FirstOrDefault().IsFavourite,
                    //IsRated = o.OfferRating.Any(x => x.OfferId == o.Id && x.ApplicationUserId == userId),
                    CompanyRating = _companyRepository.GetCompanyRating(o.CompanyId).Result,
                    BannerUrl = o.BannerUrl,
                    SpecialAnnouncement = o.SpecialAnnouncement,
                    phoneNumber = o.InternationalNumber,
                    IsPrivate = o.IsPrivate,
                    MembershipsId = o.Memberships.Select(x => x.MembershipId)
                }
            );

            return validOffers;
        }

        public async Task<IQueryable<OfferModelMobile>> SelectValidOffers()
        {
            var context = ContextFactory();

            var defaultAreas = context.DefaultArea.ToList();
            defaultAreas.Add(new DefaultArea() { Id = 0, Title = DefaultAreas.Other.ToString() });

            var validOffers = (
                from o in context.Offer
                where
                    (
                        !o.ValidUntil.HasValue
                        || (o.ValidUntil.HasValue && o.ValidUntil.Value >= DateTime.UtcNow)
                    )
                    && (!o.ValidFrom.HasValue || o.ValidFrom.Value <= DateTime.UtcNow)
                    && o.Status == OfferStatus.Approved.ToString()
                select new OfferModelMobile
                {
                    AnnouncementActive = o.AnnouncementActive,
                    Comments = o.OfferRating
                        .Where(x => x.Status == OfferCommentStatus.Public.ToString())
                        .Select(
                            x =>
                                new CommentModel
                                {
                                    OfferId = x.OfferId,
                                    CreatedByName = context.Users
                                        .FirstOrDefault(u => x.CreatedBy == u.Id)
                                        .UserName,
                                    CreatedBy = x.CreatedBy,
                                    CreatedOn = x.CreatedOn,
                                    Text = x.CommentText,
                                    Rating = x.Rating
                                }
                        )
                        .ToList(),
                    CompanyId = o.CompanyId,
                    CompanyName = o.Company.NameEnglish,
                    CompanyNameEnglish = o.Company.NameEnglish,
                    CompanyNameArabic = o.Company.NameArabic,
                    CompanyLogo = o.Company.Logo.DocumentId.ToString(),
                    AboutCompany = o.AboutCompany,
                    CompanyWebsite = o.Company.Website,
                    CompanyPhoneNumber = o.Company.Land,
                    CategoryIds = o.OfferCategories.Select(x => x.CategoryId),
                    CollectionIds = o.OfferCollections.Select(x => x.CollectionId),
                    Description = o.Description,
                    DiscountFrom = o.DiscountFrom,
                    DiscountTo = o.DiscountTo,
                    Status = o.Status.ToString(),
                    BannerActive = o.BannerActive,
                    Id = o.Id,
                    Locations = o.OfferLocations
                        .Select(
                            ol =>
                                new OfferLocationModel
                                {
                                    Id = ol.Id,
                                    Address = ol.Address,
                                    Country = ol.Country,
                                    Latitude = ol.Latitude,
                                    Longitude = ol.Longitude,
                                    Vicinity = ol.Vicinity,
                                    DefaultAreaId = ol.DefaultAreaId,
                                    DefaultAreaTitle = MapDefaultAreaTitle(
                                        defaultAreas,
                                        ol.DefaultAreaId
                                    ),
                                }
                        )
                        .ToList(),
                    MainImage = o.OfferDocuments
                        .Where(od => (int)od.Type == 3 && od.Cover)
                        .Select(od => od.DocumentId.ToString())
                        .FirstOrDefault(),
                    PriceFrom = o.PriceFrom,
                    PriceTo = o.PriceTo,
                    PriceList = o.PriceList,
                    PromotionCode = o.PromotionCode,
                    TagIds = o.OfferTags.Select(x => x.TagId),
                    TermsAndCondition = o.TermsAndCondition,
                    Title = FormatTitle(
                        o.Title,
                        o.CreatedOn,
                        o.Company == null ? o.Title : o.Company.NameEnglish
                    ),
                    Brand = o.Brand,
                    ValidFrom = o.ValidFrom,
                    ValidUntil = o.ValidUntil,
                    Rating =
                        o.OfferRating.Count > 0
                            ? o.OfferRating
                                .Where(x => x.Status == OfferCommentStatus.Public.ToString())
                                .Average(x => x.Rating)
                            : 0,
                    RatingPercent =
                        o.OfferRating.Count > 0
                            ? o.OfferRating
                                .Where(x => x.Status == OfferCommentStatus.Public.ToString())
                                .Average(x => x.Rating) * 20
                            : 0,
                    Votes =
                        o.OfferRating.Count > 0
                            ? o.OfferRating
                                .Where(x => x.Status == OfferCommentStatus.Public.ToString())
                                .Count()
                            : 0,
                    WhatYouGet = o.WhatYouGet,
                    Images = FilterOfferImagesForMobile(o.OfferDocuments),
                    Attachments = o.OfferDocuments
                        .Where(od => od.Type == OfferDocumentType.Document)
                        .Select(
                            od =>
                                new AttachmentModel
                                {
                                    Id = !string.IsNullOrEmpty(od.DocumentId.ToString())
                                        ? od.DocumentId.ToString()
                                        : string.Empty,
                                    Name = od.Document.Name,
                                    Type = od.Document.MimeType
                                }
                        )
                        .ToList(),
                    CreatedBy = o.CreatedBy,
                    CreatedOn = o.CreatedOn,
                    UpdatedBy = o.UpdatedBy,
                    UpdatedOn = o.UpdatedOn,
                    ReviewedBy = o.ReviewedBy,
                    ReviewedOn = o.ReviewedOn,
                    DecisionBy = o.DecisionBy,
                    DecisionOn = o.DecisionOn,
                    OriginalPrice =
                        (o.OriginalPrice == o.DiscountedPrice)
                        || (
                            (o.DiscountedPrice.HasValue && !o.OriginalPrice.HasValue)
                            || (o.OriginalPrice.HasValue && !o.DiscountedPrice.HasValue)
                        )
                            ? null
                            : o.OriginalPrice,
                    DiscountedPrice =
                        (o.OriginalPrice == o.DiscountedPrice)
                        || (
                            (o.DiscountedPrice.HasValue && !o.OriginalPrice.HasValue)
                            || (o.OriginalPrice.HasValue && !o.DiscountedPrice.HasValue)
                        )
                            ? null
                            : o.DiscountedPrice,
                    Discount =
                        (!o.Discount.HasValue && o.Discount.Value <= 0)
                            && (o.OriginalPrice == o.DiscountedPrice)
                        || (
                            (o.DiscountedPrice.HasValue && !o.OriginalPrice.HasValue)
                            || (o.OriginalPrice.HasValue && !o.DiscountedPrice.HasValue)
                        )
                            ? null
                            : o.Discount,
                    PriceCustom = CheckIfCustomPrice(
                        o.Discount,
                        o.DiscountedPrice,
                        o.OriginalPrice,
                        o.PriceCustom
                    ),
                    //IsFavourite = o.UserFavouritesOffers.Where(ufo => ufo.OfferId == o.Id && ufo.ApplicationUserId == userId).FirstOrDefault().IsFavourite,
                    //IsRated = o.OfferRating.Any(x => x.OfferId == o.Id && x.ApplicationUserId == userId),
                    CompanyRating = _companyRepository.GetCompanyRating(o.CompanyId).Result,
                    BannerUrl = o.BannerUrl,
                    SpecialAnnouncement = o.SpecialAnnouncement,
                    phoneNumber = o.InternationalNumber,
                    IsPrivate = o.IsPrivate,
                    MembershipsId = o.Memberships.Select(x => x.MembershipId)
                }
            );

            return validOffers;
        }

        public IEnumerable<OfferModelMobile> SelectOffersForSupplier(
            DateTime lastUpdateOn,
            string userId
        )
        {
            var context = ContextFactory();

            var defaultAreas = context.DefaultArea.ToList();
            defaultAreas.Add(new DefaultArea() { Id = 0, Title = DefaultAreas.Other.ToString() });

            var validOffers = (
                from o in context.Offer
                where o.CreatedBy == userId
                select new OfferModelMobile
                {
                    AnnouncementActive = o.AnnouncementActive,
                    Comments = o.OfferRating
                        .Where(x => x.Status == OfferCommentStatus.Public.ToString())
                        .Select(
                            x =>
                                new CommentModel
                                {
                                    OfferId = x.OfferId,
                                    CreatedByName = context.Users
                                        .FirstOrDefault(u => x.CreatedBy == u.Id)
                                        .UserName,
                                    CreatedBy = x.CreatedBy,
                                    CreatedOn = x.CreatedOn,
                                    Text = x.CommentText,
                                    Rating = x.Rating
                                }
                        )
                        .ToList(),
                    CompanyId = o.CompanyId,
                    CompanyName = o.Company.NameEnglish,
                    CompanyNameEnglish = o.Company.NameEnglish,
                    CompanyNameArabic = o.Company.NameArabic,
                    CompanyLogo = o.Company.Logo.DocumentId.ToString(),
                    AboutCompany = o.AboutCompany,
                    CompanyWebsite = o.Company.Website,
                    CompanyPhoneNumber = o.Company.Land,
                    CategoryIds = o.OfferCategories.Select(x => x.CategoryId),
                    CollectionIds = o.OfferCollections.Select(x => x.CollectionId),
                    Description = o.Description,
                    DiscountFrom = o.DiscountFrom,
                    DiscountTo = o.DiscountTo,
                    Status = o.Status.ToString(),
                    BannerActive = o.BannerActive,
                    Id = o.Id,
                    Locations = o.OfferLocations
                        .Select(
                            ol =>
                                new OfferLocationModel
                                {
                                    Id = ol.Id,
                                    Address = ol.Address,
                                    Country = ol.Country,
                                    Latitude = ol.Latitude,
                                    Longitude = ol.Longitude,
                                    Vicinity = ol.Vicinity,
                                    DefaultAreaId = ol.DefaultAreaId,
                                    DefaultAreaTitle = MapDefaultAreaTitle(
                                        defaultAreas,
                                        ol.DefaultAreaId
                                    ),
                                }
                        )
                        .ToList(),
                    MainImage = o.OfferDocuments
                        .Where(od => (int)od.Type == 3 && od.Cover)
                        .Select(od => od.DocumentId.ToString())
                        .FirstOrDefault(),
                    PriceFrom = o.PriceFrom,
                    PriceTo = o.PriceTo,
                    PriceList = o.PriceList,
                    PromotionCode = o.PromotionCode,
                    TagIds = o.OfferTags.Select(x => x.TagId),
                    TermsAndCondition = o.TermsAndCondition,
                    Title = FormatTitle(
                        o.Title,
                        o.CreatedOn,
                        o.Company == null ? o.Title : o.Company.NameEnglish
                    ),
                    Brand = o.Brand,
                    ValidFrom = o.ValidFrom,
                    ValidUntil = o.ValidUntil,
                    Rating =
                        o.OfferRating.Count > 0
                            ? o.OfferRating
                                .Where(x => x.Status == OfferCommentStatus.Public.ToString())
                                .Average(x => x.Rating)
                            : 0,
                    RatingPercent =
                        o.OfferRating.Count > 0
                            ? o.OfferRating
                                .Where(x => x.Status == OfferCommentStatus.Public.ToString())
                                .Average(x => x.Rating) * 20
                            : 0,
                    Votes =
                        o.OfferRating.Count > 0
                            ? o.OfferRating
                                .Where(x => x.Status == OfferCommentStatus.Public.ToString())
                                .Count()
                            : 0,
                    WhatYouGet = o.WhatYouGet,
                    Images = FilterOfferImagesForMobile(o.OfferDocuments),
                    Attachments = o.OfferDocuments
                        .Where(od => od.Type == OfferDocumentType.Document)
                        .Select(
                            od =>
                                new AttachmentModel
                                {
                                    Id = !string.IsNullOrEmpty(od.DocumentId.ToString())
                                        ? od.DocumentId.ToString()
                                        : string.Empty,
                                    Name = od.Document.Name,
                                    Type = od.Document.MimeType
                                }
                        )
                        .ToList(),
                    CreatedBy = o.CreatedBy,
                    CreatedOn = o.CreatedOn,
                    UpdatedBy = o.UpdatedBy,
                    UpdatedOn = o.UpdatedOn,
                    ReviewedBy = o.ReviewedBy,
                    ReviewedOn = o.ReviewedOn,
                    DecisionBy = o.DecisionBy,
                    DecisionOn = o.DecisionOn,
                    OriginalPrice =
                        (o.OriginalPrice == o.DiscountedPrice)
                        || (
                            (o.DiscountedPrice.HasValue && !o.OriginalPrice.HasValue)
                            || (o.OriginalPrice.HasValue && !o.DiscountedPrice.HasValue)
                        )
                            ? null
                            : o.OriginalPrice,
                    DiscountedPrice =
                        (o.OriginalPrice == o.DiscountedPrice)
                        || (
                            (o.DiscountedPrice.HasValue && !o.OriginalPrice.HasValue)
                            || (o.OriginalPrice.HasValue && !o.DiscountedPrice.HasValue)
                        )
                            ? null
                            : o.DiscountedPrice,
                    Discount =
                        (!o.Discount.HasValue && o.Discount.Value <= 0)
                            && (o.OriginalPrice == o.DiscountedPrice)
                        || (
                            (o.DiscountedPrice.HasValue && !o.OriginalPrice.HasValue)
                            || (o.OriginalPrice.HasValue && !o.DiscountedPrice.HasValue)
                        )
                            ? null
                            : o.Discount,
                    PriceCustom = CheckIfCustomPrice(
                        o.Discount,
                        o.DiscountedPrice,
                        o.OriginalPrice,
                        o.PriceCustom
                    ),
                    //IsFavourite = o.UserFavouritesOffers.Where(ufo => ufo.OfferId == o.Id && ufo.ApplicationUserId == userId).FirstOrDefault().IsFavourite,
                    //IsRated = o.OfferRating.Any(x => x.OfferId == o.Id && x.ApplicationUserId == userId),
                    CompanyRating = _companyRepository.GetCompanyRating(o.CompanyId).Result,
                    BannerUrl = o.BannerUrl,
                    SpecialAnnouncement = o.SpecialAnnouncement,
                    phoneNumber = o.InternationalNumber,
                    IsPrivate = o.IsPrivate,
                    MembershipsId = o.Memberships.Select(x => x.MembershipId)
                }
            );

            return validOffers;
        }

        public IQueryable<ADNOCOneOfferModel> SelectValidOfferView()
        {
            var context = ContextFactory();

            var baseContentUrl = _configuration["BaseURL:ApiUrl"];
            var defaultAreas = context.DefaultArea.ToList();
            defaultAreas.Add(new DefaultArea() { Id = 0, Title = DefaultAreas.Other.ToString() });

            var validOffers = (
                from o in context.Offer
                where
                    (
                        !o.ValidUntil.HasValue
                        || (o.ValidUntil.HasValue && o.ValidUntil.Value >= DateTime.Now)
                    )
                    && o.Status == OfferStatus.Approved.ToString()
                select new ADNOCOneOfferModel
                {
                    Id = o.Id,
                    Title = o.Title, // FormatTitle(o.Title, o.CreatedOn, o.Company == null ? o.Title : o.Company.NameEnglish),
                    Description = o.Description,
                    Brand = o.Brand,
                    Address = o.OfferLocations.Select(ol => ol.Address).FirstOrDefault(),
                    City = o.OfferLocations
                        .Select(
                            ol =>
                                String.IsNullOrWhiteSpace(ol.Vicinity)
                                    ? "Unnamed Location"
                                    : ol.Vicinity
                        )
                        .FirstOrDefault(),
                    Price = o.PriceFrom,
                    DiscountPercentage = o.Discount,
                    ValidFrom = o.ValidFrom,
                    ValidUntil = o.ValidUntil,
                    CreatedOn = o.CreatedOn,
                    WhatYouGet = o.WhatYouGet,
                    PriceList = o.PriceList,
                    FinePrint = "",
                    CategoryId = o.OfferCategories.Select(x => x.CategoryId).FirstOrDefault(),
                    CollectionId = o.OfferCollections.Select(x => x.CollectionId).FirstOrDefault(),
                    SellerId = o.CompanyId,
                    SellerName = o.Company.NameEnglish,
                    Longtitude = 0,
                    Latitude = 0,
                    Tag = Declares.Tag.Undefined,
                    IsFavorite = false,
                    AverageRemark = 0,
                    ReviewsCount = 0,
                    LastUpdateOn = context.Offer.Select(x => x.UpdatedOn).Max(),
                    Images = (
                        from oi in context.OfferDocument
                        where oi.OfferId == o.Id
                        select new OfferImageModel()
                        {
                            Id = oi.Id,
                            Index = 0,
                            ImageUrl = $"{baseContentUrl}/media/{oi.DocumentId}.jpg",
                            CreatedById = oi.CreatedBy,
                            CreatedOn = oi.CreatedOn,
                            LastUpdateById = oi.UpdatedBy,
                            LastUpdateOn = o.UpdatedOn
                        }
                    ).ToList(),
                    Attachments = o.OfferDocuments
                        .Where(od => od.Type == OfferDocumentType.Document)
                        .Select(
                            od =>
                                new AttachmentModel
                                {
                                    Id = !string.IsNullOrEmpty(od.DocumentId.ToString())
                                        ? od.DocumentId.ToString()
                                        : string.Empty,
                                    Name = od.Document.Name,
                                    Type = od.Document.MimeType
                                }
                        )
                        .ToList(),
                    ImageUrls = null
                }
            );
            //    AnnouncementActive = o.AnnouncementActive,
            //    //Comments = ProcessComments(o.Comments),
            //    Comments = o.OfferRating.Where(x => x.Status == OfferCommentStatus.Public.ToString()).Select(x => new CommentModel
            //    {
            //        OfferId = x.OfferId,
            //        CreatedByName = context.Users.FirstOrDefault(u => x.CreatedBy == u.Id).UserName,
            //        CreatedBy = x.CreatedBy,
            //        CreatedOn = x.CreatedOn,
            //        Text = x.CommentText,
            //        Rating = x.Rating
            //    }).ToList(),
            //    CompanyId = o.CompanyId,
            //    CompanyName = o.Company.NameEnglish,
            //    CompanyNameEnglish = o.Company.NameEnglish,
            //    CompanyNameArabic = o.Company.NameArabic,
            //    CompanyLogo = o.Company.Logo.DocumentId.ToString(),
            //    AboutCompany = o.AboutCompany,
            //    CompanyWebsite = o.Company.Website,
            //    CompanyPhoneNumber = o.Company.Land,
            //    CategoryIds = o.OfferCategories.Select(x => x.CategoryId),
            //    CollectionIds = o.OfferCollections.Select(x => x.CollectionId),
            //    Description = o.Description,
            //    DiscountFrom = o.DiscountFrom,
            //    DiscountTo = o.DiscountTo,
            //    Status = o.Status.ToString(),
            //    BannerActive = o.BannerActive,
            //    Id = o.Id,
            //    Locations = o.OfferLocations.Select(ol => new OfferLocationModel
            //    {
            //        Id = ol.Id,
            //        Address = ol.Address,
            //        Country = ol.Country,
            //        Latitude = ol.Latitude,
            //        Longitude = ol.Longitude,
            //        Vicinity = ol.Vicinity,
            //        DefaultAreaId = ol.DefaultAreaId,
            //        DefaultAreaTitle = MapDefaultAreaTitle(defaultAreas, ol.DefaultAreaId),
            //    }).ToList(),
            //    MainImage = o.OfferDocuments.Where(od => (int)od.Type == 3 && od.Cover).Select(od => od.DocumentId.ToString()).FirstOrDefault(),
            //    PriceFrom = o.PriceFrom,
            //    PriceTo = o.PriceTo,
            //    PriceList = o.PriceList,
            //    PromotionCode = o.PromotionCode,
            //    TagIds = o.OfferTags.Select(x => x.TagId),
            //    TermsAndCondition = o.TermsAndCondition,
            //    Title = FormatTitle(o.Title, o.CreatedOn, o.Company == null ? o.Title : o.Company.NameEnglish),
            //    ValidFrom = o.ValidFrom,
            //    ValidUntil = o.ValidUntil,
            //    Rating = o.OfferRating.Count > 0 ? o.OfferRating.Where(x => x.Status == OfferCommentStatus.Public.ToString()).Average(x => x.Rating) : 0,
            //    RatingPercent = o.OfferRating.Count > 0 ? o.OfferRating.Where(x => x.Status == OfferCommentStatus.Public.ToString()).Average(x => x.Rating) * 20 : 0,
            //    Votes = o.OfferRating.Count > 0 ? o.OfferRating.Where(x => x.Status == OfferCommentStatus.Public.ToString()).Count() : 0,
            //    WhatYouGet = o.WhatYouGet,
            //    //Images = o.OfferDocuments.Where(od => od.Type != OfferDocumentType.Document && od.Type == OfferDocumentType.Thumbnail)
            //    //                         .Select(od => od.DocumentId.ToString())
            //    //                         .ToList(),
            //    Images = FilterOfferImagesForMobile(o.OfferDocuments),
            //    Attachments = o.OfferDocuments.Where(od => od.Type == OfferDocumentType.Document)
            //                                  .Select(od => new AttachmentModel
            //                                  {
            //                                      Id = !string.IsNullOrEmpty(od.DocumentId.ToString()) ? od.DocumentId.ToString() : string.Empty,
            //                                      Name = od.Document.Name,
            //                                      Type = od.Document.MimeType
            //                                  }).ToList(),
            //    CreatedBy = o.CreatedBy,
            //    CreatedOn = o.CreatedOn,
            //    UpdatedBy = o.UpdatedBy,
            //    UpdatedOn = o.UpdatedOn,
            //    ReviewedBy = o.ReviewedBy,
            //    ReviewedOn = o.ReviewedOn,
            //    DecisionBy = o.DecisionBy,
            //    DecisionOn = o.DecisionOn,
            //    OriginalPrice = (o.OriginalPrice == o.DiscountedPrice) || ((o.DiscountedPrice.HasValue && !o.OriginalPrice.HasValue) || (o.OriginalPrice.HasValue && !o.DiscountedPrice.HasValue)) ? null : o.OriginalPrice,
            //    DiscountedPrice = (o.OriginalPrice == o.DiscountedPrice) || ((o.DiscountedPrice.HasValue && !o.OriginalPrice.HasValue) || (o.OriginalPrice.HasValue && !o.DiscountedPrice.HasValue)) ? null : o.DiscountedPrice,
            //    Discount = (!o.Discount.HasValue && o.Discount.Value <= 0) && (o.OriginalPrice == o.DiscountedPrice) || ((o.DiscountedPrice.HasValue && !o.OriginalPrice.HasValue) || (o.OriginalPrice.HasValue && !o.DiscountedPrice.HasValue)) ? null : o.Discount,
            //    PriceCustom = CheckIfCustomPrice(o.Discount, o.DiscountedPrice, o.OriginalPrice, o.PriceCustom),
            //    IsFavourite = o.UserFavouritesOffers.Where(ufo => ufo.OfferId == o.Id && ufo.ApplicationUserId == userId).FirstOrDefault().IsFavourite,
            //});

            return validOffers;
        }

        public IQueryable<OneHubOfferModel> SelectValidOfferViewOneHub()
        {
            var context = ContextFactory();

            var baseApiUrl = _configuration["BaseURL:ApiUrl"];
            var baseUrl = _configuration["BaseURL:Url"];

            var defaultAreas = context.DefaultArea.ToList();
            defaultAreas.Add(new DefaultArea() { Id = 0, Title = DefaultAreas.Other.ToString() });

            var buffered_now = DateTime.Now.AddMinutes(-15);

            var validOffers = (
                from o in context.Offer
                where
                    (
                        !o.ValidUntil.HasValue
                        || (o.ValidUntil.HasValue && o.ValidUntil.Value >= DateTime.Now)
                    )
                    && (
                        !o.ValidFrom.HasValue
                        || (o.ValidFrom.HasValue && o.ValidFrom.Value <= buffered_now)
                    )
                    && o.Status == OfferStatus.Approved.ToString()
                select new OneHubOfferModel
                {
                    Id = o.Id,
                    Title = o.Title, // FormatTitle(o.Title, o.CreatedOn, o.Company == null ? o.Title : o.Company.NameEnglish),
                    Description = o.Description,
                    Brand = o.Brand,
                    Address = o.OfferLocations.Select(ol => ol.Address).FirstOrDefault(),
                    City = o.OfferLocations
                        .Select(
                            ol =>
                                String.IsNullOrWhiteSpace(ol.Vicinity)
                                    ? "Unnamed Location"
                                    : ol.Vicinity
                        )
                        .FirstOrDefault(),
                    Country = o.OfferLocations.Select(ol => ol.Country).FirstOrDefault(),
                    Price = o.PriceFrom,
                    DiscountPercentage = o.Discount ?? o.DiscountFrom,
                    CategoryId = o.OfferCategories.Select(x => x.CategoryId).FirstOrDefault(),
                    CategoryName = o.OfferCategories.Select(x => x.Category.Title).FirstOrDefault(),
                    SellerId = o.CompanyId,
                    SellerName = o.Company.NameEnglish,
                    Longtitude = o.OfferLocations.Select(ol => ol.Longitude).FirstOrDefault(),
                    Latitude = o.OfferLocations.Select(ol => ol.Latitude).FirstOrDefault(),
                    OfferUrl = $"{baseUrl}offers/{o.Id}",
                    Images = (
                        from oi in context.OfferDocument
                        where oi.OfferId == o.Id && oi.Type == OfferDocumentType.Large
                        orderby oi.Cover descending
                        select new OneHubOfferImageModel()
                        {
                            ImageUrl = $"{baseApiUrl}/media/{oi.DocumentId}.jpg"
                        }
                    ).ToList()
                }
            );

            return validOffers;
        }

        private static List<string> FilterOfferImagesForMobile(
            ICollection<OfferDocument> offerDocuments
        )
        {
            var images = new List<string>();

            var allImages = offerDocuments
                .Where(
                    od =>
                        od.Type != OfferDocumentType.Document
                        && od.Type != OfferDocumentType.QRCode
                        && od.Type != OfferDocumentType.Video
                )
                .ToList();

            //var original = new List<string>();
            var thumbanil = new List<string>();
            //var large = new List<string>();

            var grouped = allImages.GroupBy(x => x.OriginalImageId);

            foreach (var group in grouped)
            {
                var list = group.ToList();
                //original.Add(group.Key.ToString());
                thumbanil.Add(
                    list.Where(x => x.Type == OfferDocumentType.Thumbnail)
                        .Select(x => x.DocumentId.ToString())
                        .FirstOrDefault()
                );

                //large.Add(list.Where(x => x.Type == OfferDocumentType.Large)
                //                                  .Select(x => x.DocumentId.ToString())
                //                                  .FirstOrDefault());
            }

            //images.AddRange(original);
            images.AddRange(thumbanil);
            //images.AddRange(large);

            return images;
        }

        /// <summary>
        /// Check for document type, some attachments after migration got default type
        /// Attachments with defualt type couldn't be opened
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns>If document has default type or text plain pdf is returned</returns>
        private static string GetCorrectMimeType(string mimeType, string name)
        {
            if (
                string.IsNullOrWhiteSpace(mimeType)
                || mimeType == "application/octet-stream"
                || mimeType == "text/plain"
            )
            {
                if (name.Contains(".png"))
                    return "image/png";

                if (name.Contains(".jpg") || name.Contains(".jpeg"))
                    return "image/jpeg";

                return "application/pdf";
            }

            return mimeType;
        }

        private static string CheckIfCustomPrice(
            decimal? discount,
            decimal? discountedPrice,
            decimal? originalPrice,
            string priceCustom
        )
        {
            var price = string.Empty;
            if (!string.IsNullOrWhiteSpace(priceCustom))
                price = priceCustom;

            if (
                (!discount.HasValue || discount <= 0)
                    && (
                        discountedPrice.HasValue
                        && originalPrice.HasValue
                        && discountedPrice.Value > 0
                        && originalPrice.Value > 0
                        && (discountedPrice.Value == originalPrice.Value)
                    )
                || (discountedPrice.HasValue && !originalPrice.HasValue)
                || (originalPrice.HasValue && !discountedPrice.HasValue)
            )
            {
                var value = originalPrice ?? discountedPrice;
                price = "AED " + value.Value.ToString("#.##");
            }

            return price;
        }

        private static string MapDefaultAreaTitle(List<DefaultArea> defaultAreas, int defaultAreaId)
        {
            var title = defaultAreas.FirstOrDefault(dl => dl.Id == defaultAreaId).Title;

            return title ?? DefaultAreas.Other.ToString();
        }

        public IQueryable<int> SelectValidAndLiveOffers()
        {
            var context = ContextFactory();

            var validOffers = context.Offer
                .AsNoTracking()
                .Where(
                    o =>
                        o.Status == OfferStatus.Approved.ToString()
                        && (!o.ValidFrom.HasValue || o.ValidFrom.Value <= DateTime.UtcNow)
                        && (
                            !o.ValidUntil.HasValue
                            || (o.ValidUntil.HasValue && o.ValidUntil.Value >= DateTime.UtcNow)
                        )
                )
                .Select(o => o.Id)
                .OrderBy(o => o);
            return validOffers;
        }

        public IQueryable<int> SelectValidAndLiveOffersforUser(string userId, bool isBuyer)
        {
            var context = ContextFactory();
            var userMemberships = _membershipECardRepository
                .GetMembershipsForUser(userId, isBuyer)
                .Select(x => x.Id);
            var validOffers = context.Offer
                .AsNoTracking()
                .Where(
                    o =>
                        o.Status == OfferStatus.Approved.ToString()
                        && (!o.ValidFrom.HasValue || o.ValidFrom.Value <= DateTime.UtcNow)
                        && (
                            !o.ValidUntil.HasValue
                            || (o.ValidUntil.HasValue && o.ValidUntil.Value >= DateTime.UtcNow)
                        )
                        && (
                            !o.IsPrivate
                            || o.Memberships.Count() > 0
                                && o.Memberships
                                    .Where(x => userMemberships.Contains(x.MembershipId))
                                    .Any()
                        )
                )
                .Select(o => o.Id)
                .OrderBy(o => o);
            return validOffers;
        }

        public IQueryable<OfferModel> GetWeekendOffers(string userId, bool isBuyer)
        {
            var context = ContextFactory();

            var offers = context.Offer
                .AsNoTracking()
                .Where(
                    offer =>
                        offer.FlagIsWeekendOffer && offer.Status == OfferStatus.Approved.ToString()
                );

            var userMemberships = _membershipECardRepository
                .GetMembershipsForUser(userId, isBuyer)
                .Select(x => x.Id.ToString())
                .ToHashSet();
            var filteredMembershipOffers = FilterForMembership(offers, userMemberships);

            return filteredMembershipOffers.Select(projectToOfferCardModel);
        }

        public int GetBannerOffersCount()
        {
            var context = ContextFactory();

            int count = context.Offer
                .AsNoTracking()
                .Where(offer => offer.BannerActive == true)
                .Count();

            return count;
        }

        public IQueryable<OfferModel> GetOffersSearchPage(
            List<Roles> roles,
            QueryModel queryModel,
            string userId
        )
        {
            var context = ContextFactory();
            IQueryable<Offer> offers = context.Offer.Include(o => o.OfferLocations).AsNoTracking();
            IQueryable<OfferModel> offerModels = null;
            IQueryable<Offer> filteredOffers = null;

            if (roles.Contains(Roles.Buyer))
            {
                offers = offers.Where(o => o.Status == OfferStatus.Approved.ToString());
            }
            var memberships = _membershipECardRepository
                .GetMembershipsForUser(userId, roles.Contains(Roles.Buyer))
                .Select(x => x.Id.ToString())
                .ToHashSet();
            var membershipsOffers = FilterForMembership(offers, memberships);
            filteredOffers = Filter(membershipsOffers, queryModel);
            offerModels = filteredOffers.Select(projectToOfferCardModel);

            return Sort(queryModel.Sort, offerModels);
        }

        public IQueryable<OfferModel> GetOffersByCategoryPage(
            int categoryId,
            QueryModel queryModel,
            string userId,
            bool isBuyer
        )
        {
            var context = ContextFactory();
            IQueryable<OfferModel> offerModels = null;
            IQueryable<Offer> filteredOffers = null;

            var offers = context.Offer
                .Include(o => o.OfferLocations)
                .AsNoTracking()
                .Where(o => o.Status == OfferStatus.Approved.ToString())
                .Where(offer => offer.OfferCategories.Any(x => x.CategoryId == categoryId));

            var userMemberships = _membershipECardRepository
                .GetMembershipsForUser(userId, isBuyer)
                .Select(x => x.Id.ToString())
                .ToHashSet();
            var filteredMembershipOffers = FilterForMembership(offers, userMemberships);

            filteredOffers = Filter(filteredMembershipOffers, queryModel);
            offerModels = filteredOffers.Select(projectToOfferCardModel);

            return Sort(queryModel.Sort, offerModels);
        }

        public IQueryable<OfferExcelModel> GetOffersForReportByCategory(int categoryId)
        {
            var context = ContextFactory();
            var offers = (
                from o in context.Offer
                join offerCat in context.OfferCategory on o.Id equals offerCat.OfferId into g1
                from p1 in g1.DefaultIfEmpty()
                join company in context.Company on o.CompanyId equals company.Id into g2
                from p2 in g2.DefaultIfEmpty()
                join user in context.Users on o.CreatedBy equals user.Id into g3
                from p3 in g3.DefaultIfEmpty()
                where p1.CategoryId == categoryId && o.Status == OfferStatus.Approved.ToString()
                select new OfferExcelModel()
                {
                    OfferId = o.Id,
                    CompanyEmail = p2.OfficialEmail,
                    CompanyNameEnglish = p2.NameEnglish,
                    PhoneNumber = p2.LandE164Number,
                    Description = o.Description,
                    PriceFiled = OfferRepository.GetPriceFileValue(o),
                    ValidFrom = o.ValidFrom,
                    ValidUntil = o.ValidUntil,
                    FocalPoint = p3.UserName,
                }
            );
            ;
            var tepm = offers.ToList();
            return offers;
        }

        public IQueryable<OfferModel> GetOffersByCategory(int categoryId)
        {
            var context = ContextFactory();
            IQueryable<OfferModel> offerModels = null;

            var offers = context.Offer
                .Include(o => o.OfferLocations)
                .AsNoTracking()
                .Where(o => o.Status == OfferStatus.Approved.ToString())
                .Where(offer => offer.OfferCategories.Any(x => x.CategoryId == categoryId));

            return offers.Select(projectToOfferCardModel);
        }

        public IQueryable<OfferModel> GetOffersByCollectionPage(
            int collectionId,
            QueryModel queryModel,
            string userId,
            bool isBuyer
        )
        {
            var context = ContextFactory();
            IQueryable<OfferModel> offerModels = null;
            IQueryable<Offer> filteredOffers = null;

            var offers = context.Offer
                .Include(o => o.OfferLocations)
                .AsNoTracking()
                .Where(o => o.Status == OfferStatus.Approved.ToString())
                .Where(offer => offer.OfferCollections.Any(x => x.CollectionId == collectionId));

            var userMemberships = _membershipECardRepository
                .GetMembershipsForUser(userId, isBuyer)
                .Select(x => x.Id.ToString())
                .ToHashSet();
            var filteredMembershipOffers = FilterForMembership(offers, userMemberships);

            filteredOffers = Filter(filteredMembershipOffers, queryModel);
            offerModels = filteredOffers.Select(projectToOfferCardModel);

            return Sort(queryModel.Sort, offerModels);
        }

        public IQueryable<OfferModel> GetOffersByTagPage(int tagId, QueryModel queryModel)
        {
            var context = ContextFactory();
            IQueryable<OfferModel> offerModels = null;
            IQueryable<Offer> filteredOffers = null;

            var offers = context.Offer
                .Include(o => o.OfferLocations)
                .AsNoTracking()
                .Where(o => o.Status == OfferStatus.Approved.ToString())
                .Where(offer => offer.OfferTags.Any(x => x.TagId == tagId));

            filteredOffers = Filter(offers, queryModel);
            offerModels = filteredOffers.Select(projectToOfferCardModel);

            return Sort(queryModel.Sort, offerModels);
        }

        public IQueryable<OfferModel> GetLatestOffers(int limit)
        {
            var context = ContextFactory();

            var query = context.Offer
                .AsNoTracking()
                .Include(o => o.OfferDocuments)
                .Where(o => o.Status == OfferStatus.Approved.ToString())
                .OrderByDescending(o => o.UpdatedOn);

            if (limit > 0)
            {
                return query.Take(limit).Select(projectToOfferCardModel);
            }

            return query.Select(projectToOfferCardModel);
        }

        protected override IQueryable<OfferModel> GetEntities()
        {
            var context = ContextFactory();

            return context.Offer.Select(projectToOfferModel);
        }

        public async Task<OfferModel> GetOfferById(int id, string userId, List<Roles> roles)
        {
            var context = ContextFactory();
            if (roles.Contains(Roles.Admin) || roles.Contains(Roles.AdnocCoordinator))
            {
                return await context.Offer
                    .AsNoTracking()
                    .Include(x => x.Company)
                    .Select(projectToOfferModel)
                    .FirstOrDefaultAsync(x => x.Id == id);
            }
            else if (roles.Contains(Roles.Buyer))
            {
                return await context.Offer
                    .AsNoTracking()
                    .Include(x => x.Company)
                    .Select(projectToOfferModel)
                    .FirstOrDefaultAsync(
                        x => x.Id == id && x.Status == OfferStatus.Approved.ToString()
                    );
            }

            return null;
        }

        public async Task<OfferOneHubModel> GetOneHubOfferById(
            int id,
            string userId,
            List<Roles> roles
        )
        {
            var context = ContextFactory();
            if (roles.Contains(Roles.Admin) || roles.Contains(Roles.AdnocCoordinator))
            {
                return await context.Offer
                    .AsNoTracking()
                    .Include(x => x.Company)
                    .Select(projectToOfferOneHubModel)
                    .FirstOrDefaultAsync(x => x.Id == id);
            }
            else if (roles.Contains(Roles.Buyer))
            {
                return await context.Offer
                    .AsNoTracking()
                    .Include(x => x.Company)
                    .Select(projectToOfferOneHubModel)
                    .FirstOrDefaultAsync(
                        x => x.Id == id && x.Status == OfferStatus.Approved.ToString()
                    );
            }

            return null;
        }

        /// <summary>
        /// Return if offer is added to favorite offers and if offer is rated
        /// </summary>
        /// <param name="offerId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IsOfferFavoriteAndRatedModel> CheckIfOfferIsFavoriteAndRated(
            int offerId,
            string userId
        )
        {
            var isFavoriteAndRated = new IsOfferFavoriteAndRatedModel();
            var context = ContextFactory();

            var offerFavourite = await context.UserFavouritesOffers
                .Where(uo => uo.ApplicationUserId == userId && uo.OfferId == offerId)
                .FirstOrDefaultAsync();
            isFavoriteAndRated.IsFavorite = offerFavourite != null && offerFavourite.IsFavourite;

            // Checks if offer is already rated by this user
            var offerRating = await (
                from o in context.OfferRating
                where o.OfferId == offerId && o.ApplicationUserId == userId
                select o
            ).FirstOrDefaultAsync();
            isFavoriteAndRated.IsRated = offerRating != null;

            return isFavoriteAndRated;
        }

        public async Task<OfferModel> GetOffer(int id)
        {
            var context = ContextFactory();

            var offer = await context.Offer
                .AsNoTracking()
                .Select(projectToOfferModel)
                .FirstOrDefaultAsync(x => x.Id == id);
            offer.MembershipType = _membershipECardRepository.GetMembershipsForOffer(id).ToArray();
            return offer;
        }

        public async Task RateOffer(int rating, int offerId, string userId)
        {
            var context = ContextFactory();
            OfferRating offerRating = context.OfferRating.FirstOrDefault(
                x => x.ApplicationUserId == userId && x.OfferId == offerId
            );

            if (offerRating == null)
            {
                offerRating = new OfferRating
                {
                    ApplicationUserId = userId,
                    OfferId = offerId,
                    Rating = rating
                };
                context.OfferRating.Add(offerRating);
            }
            else
            {
                offerRating = new OfferRating
                {
                    ApplicationUserId = userId,
                    OfferId = offerId,
                    Rating = rating
                };
                context.OfferRating.Update(offerRating);
            }

            await context.SaveChangesAsync();
        }

        public async Task<OfferModel> UpdateAsync(
            OfferModel model,
            IVisitor<IChangeable> auditVisitor,
            string userId,
            List<Roles> roles
        )
        {
            var context = ContextFactory();

            var data = await context.Offer.FirstOrDefaultAsync(x => x.Id == model.Id);

            if (data == null)
                return null;

            PopulateEntityModel(data, model, userId);

            data.Status = HandleStatusChange(roles, model, data);

            data.Accept(auditVisitor);

            await context.SaveChangesAsync();

            return projectToOfferModel.Compile().Invoke(data);
        }

        public async Task UpdateStatus(OfferModel model, List<Roles> roles, string userId)
        {
            var context = ContextFactory();

            var offer = context.Offer.FirstOrDefault(x => x.Id == model.Id);

            if (offer != null)
            {
                offer.Status = model.Status;
                SetSpecialAnnoucment(offer);
                offer.Comments = model.Comments
                    .Select(
                        c =>
                            new Comment
                            {
                                Id = c.Id,
                                Text = c.Text,
                                CreatedBy = c.CreatedBy,
                                CreatedOn = c.CreatedOn,
                                OfferId = c.OfferId
                            }
                    )
                    .ToList();

                if (roles.Contains(Roles.Supplier))
                {
                    offer.CreatedBy = userId;
                    offer.CreatedOn = DateTime.UtcNow;
                    offer.UpdatedBy = userId;
                    offer.UpdatedOn = DateTime.UtcNow;
                }
                else if (roles.Contains(Roles.Reviewer))
                {
                    offer.ReviewedBy = userId;
                    offer.ReviewedOn = DateTime.UtcNow;
                }
                else if (roles.Contains(Roles.AdnocCoordinator) || roles.Contains(Roles.Admin))
                {
                    offer.DecisionBy = userId;
                    offer.DecisionOn = DateTime.UtcNow;
                }

                context.Update(offer);

                await context.SaveChangesAsync();
            }
        }

        public async Task<OfferModel> CreateAsync(
            OfferModel model,
            IVisitor<IChangeable> auditVisitor,
            string userId,
            List<Roles> roles
        )
        {
            var context = ContextFactory();

            var offer = context.Offer
                .Include(o => o.OfferCategories)
                .Include(o => o.OfferCollections)
                .Include(o => o.OfferRating)
                .Include(o => o.OfferTags)
                .Include(o => o.OfferLocations)
                .ThenInclude(ol => ol.DefaultArea)
                .Include(o => o.OfferRating)
                .Include(o => o.UserFavouritesOffers)
                .Include(o => o.OfferDocuments)
                .Include(o => o.Company)
                .ThenInclude(c => c.Logo)
                .Include(o => o.Comments)
                .FirstOrDefault(x => x.Id == model.Id);
            if (offer == null)
            {
                DateTime licenseExpiryDate = await context.Company
                    .Where(x => x.Id == model.CompanyId)
                    .Select(x => x.TradeLicenseExpDate)
                    .FirstOrDefaultAsync();
                DateTime today = DateTime.Now;
                if (licenseExpiryDate.Date <= today.Date)
                {
                    throw new Exception(
                        "Please upload valid trade license and update its expiry date."
                    );
                }
                offer = new Offer();
            }

            // Temporary solution for offer locations
            var offerLocations = await context.OfferLocation
                .Where(ol => ol.OfferId == model.Id)
                .ToListAsync();

            var offerDocuments = new List<OfferDocumentModel>();

            if (model.Images != null && model.Images.Count > 0)
            {
                foreach (var imageModel in model.Images)
                {
                    offerDocuments.Add(
                        new OfferDocumentModel
                        {
                            DocumentId = new Guid(imageModel.Id),
                            OfferId = model.Id,
                            Type = imageModel.Type,
                            OriginalImageId = imageModel.OriginalImageId,
                            X1 = imageModel.CropCoordinates.X1,
                            X2 = imageModel.CropCoordinates.X2,
                            Y1 = imageModel.CropCoordinates.Y1,
                            Y2 = imageModel.CropCoordinates.Y2,
                            cropX1 = imageModel.CropNGXCoordinates.X1,
                            cropX2 = imageModel.CropNGXCoordinates.X2,
                            cropY1 = imageModel.CropNGXCoordinates.Y1,
                            cropY2 = imageModel.CropNGXCoordinates.Y2,
                            Cover = imageModel.Cover
                        }
                    );

                    //If Reviewer didn't change image just add already exisitng thumbnail and large image
                    //if (model.Id != 0)
                    //{
                    //    var existingImagesForThisOffer = _offerDocumentRepository.GetOfferImages(new Guid(imageModel.Id)).ToList();
                    //    foreach (var existingImage in existingImagesForThisOffer)
                    //    {
                    //        if (existingImage.Type == OfferDocumentType.Large || existingImage.Type == OfferDocumentType.Thumbnail)
                    //        {
                    //            offerDocuments.Add(existingImage);
                    //        }
                    //    }
                    //}
                }
            }

            if (model.Attachments != null && model.Attachments.Count > 0)
            {
                foreach (var attachmentModel in model.Attachments)
                {
                    AddOfferDocument(model, offerDocuments, attachmentModel.Id);
                }
            }

            if (model.AnnouncementImage != null)
            {
                AddOfferDocument(model, offerDocuments, model.AnnouncementImage.Id);

                offer.SpecialAnnouncement = Guid.Parse(model.AnnouncementImage.Id);
            }

            model.OfferDocuments = offerDocuments;

            if (model.Categories == null)
            {
                model.Categories = new List<CategoryModel>();
            }

            if (model.Collections == null)
            {
                model.Collections = new List<CollectionModel>();
            }

            if (model.Tags == null)
            {
                model.Tags = new List<TagModel>();
            }

            if (model.Locations == null)
            {
                model.Locations = new List<OfferLocationModel>();
            }
            if (model.Comments == null)
            {
                model.Comments = new List<CommentModel>();
            }
            if (model.OfferRating == null)
            {
                model.OfferRating = new List<OfferRatingModel>();
            }
            if (model.MembershipType != null && model.MembershipType.Count() > 0)
                model.isPrivate = true;
            else
                model.isPrivate = false;

            PopulateEntityModel(offer, model, userId);

            offer.Status = HandleStatusChange(roles, model, offer);

            foreach (var offerDocument in offer.OfferDocuments)
            {
                offerDocument.Accept(auditVisitor);
                var document = await context.Document.FirstOrDefaultAsync(
                    d => d.Id == offerDocument.DocumentId
                );
                if (document == null)
                {
                    context.Document.Add(
                        new Document()
                        {
                            Id = offerDocument.DocumentId,
                            CreatedOn = DateTime.UtcNow,
                            UpdatedOn = DateTime.UtcNow,
                        }
                    );
                }
            }

            if (model.Id == 0)
            {
                offer.OfferRating = new List<OfferRating>();
                offer.UserFavouritesOffers = new List<UserFavouritesOffer>();

                offer.Accept(auditVisitor);
                // Fix for sync - for offers that are valid in the future
                offer.UpdatedOn =
                    model.ValidFrom.GetValueOrDefault() > DateTime.UtcNow
                        ? model.ValidFrom.GetValueOrDefault()
                        : DateTime.UtcNow;
                context.Add(offer);
            }
            else
            {
                // Temporary solution for offer locations
                context.OfferLocation.RemoveRange(offerLocations);
                // Fix for sync - for offers that are valid in the future
                offer.UpdatedOn =
                    model.ValidFrom.GetValueOrDefault() > DateTime.UtcNow
                        ? model.ValidFrom.GetValueOrDefault()
                        : DateTime.UtcNow;
                context.Update(offer);
            }

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                throw e;
            }
            model.Id = offer.Id;
            await updateMembership(model);
            return projectToOfferModel.Compile().Invoke(offer);
        }

        private async Task updateMembership(OfferModel model)
        {
            var context = ContextFactory();
            var memberships_offers = context.OffersMemberships
                .Where(om => om.OfferId == model.Id)
                .ToList();
            //provera postojecih membership-offer veza
            foreach (var membership_offer in memberships_offers)
            {
                var listMembership = model.MembershipType.Select(x => x.Id);
                if (model.isPrivate && !listMembership.Contains(membership_offer.MembershipId))
                {
                    context.OffersMemberships.Remove(membership_offer);
                    await context.SaveChangesAsync();
                }
                else if (!model.isPrivate)
                {
                    context.OffersMemberships.Remove(membership_offer);
                    await context.SaveChangesAsync();
                }
            }
            //dodavanje novih
            if (model.isPrivate)
                foreach (var item in model.MembershipType)
                {
                    var membership = memberships_offers
                        .Where(x => x.MembershipId == item.Id)
                        .FirstOrDefault();
                    if (membership == null)
                    {
                        var newMembershipOffer = new OffersMemberships()
                        {
                            MembershipId = item.Id,
                            OfferId = model.Id
                        };
                        await context.OffersMemberships.AddAsync(newMembershipOffer);
                        await context.SaveChangesAsync();
                    }
                }
        }

        private static void AddOfferDocument(
            OfferModel model,
            List<OfferDocumentModel> offerDocuments,
            string imgId
        )
        {
            var guidImg = Guid.Parse(imgId);
            offerDocuments.Add(
                new OfferDocumentModel
                {
                    DocumentId = guidImg,
                    OfferId = model.Id,
                    Type = OfferDocumentType.Document
                }
            );
        }

        private Expression<Func<Offer, OfferModel>> projectToOfferModel = data =>
            new OfferModel()
            {
                AboutCompany = data.AboutCompany,
                Categories = data.OfferCategories
                    .Select(
                        x =>
                            new CategoryModel
                            {
                                Title = x.Category != null ? x.Category.Title : String.Empty,
                                Id = x.CategoryId
                            }
                    )
                    .ToList(),
                Collections = data.OfferCollections
                    .Select(
                        x =>
                            new CollectionModel
                            {
                                Title = x.Collection != null ? x.Collection.Title : String.Empty,
                                Id = x.CollectionId
                            }
                    )
                    .ToList(),
                Description = data.Description,
                DiscountFrom = data.DiscountFrom,
                DiscountTo = data.DiscountTo,
                FlagIsLatest = data.FlagIsLatest,
                Status = data.Status,
                Id = data.Id,
                Locations = data.OfferLocations
                    .Select(
                        ol =>
                            new OfferLocationModel
                            {
                                Address = ol.Address,
                                Country = ol.Country,
                                Id = ol.Id,
                                Latitude = ol.Latitude,
                                Longitude = ol.Longitude,
                                OfferId = data.Id,
                                Vicinity = ol.Vicinity,
                                DefaultAreaId = ol.DefaultAreaId,
                                //DefaultAreaTitle = ol.DefaultArea.Title
                            }
                    )
                    .ToList(),
                LocationsCount = data.OfferLocations.Count(),
                PriceFrom = data.PriceFrom,
                PriceTo = data.PriceTo,
                PriceList = data.PriceList,
                PromotionCode = data.PromotionCode,
                Tags = data.OfferTags
                    .Select(
                        x =>
                            new TagModel
                            {
                                Title = x.Tag != null ? x.Tag.Title : String.Empty,
                                Id = x.TagId
                            }
                    )
                    .ToList(),
                TermsAndCondition = data.TermsAndCondition,
                Brand = data.Brand,
                Title = FormatTitle(
                    data.Title,
                    data.CreatedOn,
                    data.Company == null ? String.Empty : data.Company.NameEnglish
                ),
                ValidFrom = data.ValidFrom.Value.SpecifyKind(DateTimeKind.Utc),
                ValidUntil = data.ValidUntil.Value.SpecifyKind(DateTimeKind.Utc),
                // 5 star rating
                Rating =
                    data.OfferRating
                        .Where(x => x.Status == OfferCommentStatus.Public.ToString())
                        .Count() > 0
                        ? data.OfferRating
                            .Where(x => x.Status == OfferCommentStatus.Public.ToString())
                            .Average(x => x.Rating)
                        : 0,
                RatingPercent =
                    data.OfferRating
                        .Where(x => x.Status == OfferCommentStatus.Public.ToString())
                        .Count() * 20,
                Votes = data.OfferRating
                    .Where(x => x.Status == OfferCommentStatus.Public.ToString())
                    .Count(),
                WhatYouGet = data.WhatYouGet,
                Images = data.OfferDocuments
                    .Where(od => (int)od.Type != 1)
                    .Select(
                        od =>
                            new ImageModel
                            {
                                Id = !string.IsNullOrEmpty(od.DocumentId.ToString())
                                    ? od.DocumentId.ToString()
                                    : string.Empty,
                                Type = od.Type,
                                OriginalImageId = od.OriginalImageId,
                                CropCoordinates = new CropCoordinates
                                {
                                    X1 = od.X1,
                                    X2 = od.X2,
                                    Y1 = od.Y1,
                                    Y2 = od.Y2
                                },
                                CropNGXCoordinates = new CropCoordinates
                                {
                                    X1 = od.cropX1,
                                    X2 = od.cropX2,
                                    Y1 = od.cropY1,
                                    Y2 = od.cropY2
                                },
                                Cover = od.Cover
                            }
                    )
                    .ToList(),
                Attachments = data.OfferDocuments
                    .Where(od => (int)od.Type == 1)
                    .Select(
                        od =>
                            new AttachmentModel
                            {
                                Id = !string.IsNullOrEmpty(od.DocumentId.ToString())
                                    ? od.DocumentId.ToString()
                                    : string.Empty,
                                Name = od.Document != null ? od.Document.Name : string.Empty,
                                Type = od.Document != null ? od.Document.MimeType : string.Empty,
                            }
                    )
                    .ToList(),
                FlagIsWeekendOffer = data.FlagIsWeekendOffer,
                CreatedBy = data.CreatedBy,
                CreatedOn = data.CreatedOn,
                UpdatedBy = data.UpdatedBy,
                UpdatedOn = data.UpdatedOn,
                Comments = data.Comments
                    .Select(
                        c =>
                            new CommentModel
                            {
                                CreatedBy = c.CreatedBy,
                                CreatedOn = c.CreatedOn.SpecifyKind(DateTimeKind.Utc),
                                Text = c.Text,
                                Id = c.Id
                            }
                    )
                    .ToList(),
                ReviewedBy = data.ReviewedBy,
                ReviewedOn = data.ReviewedOn,
                DecisionBy = data.DecisionBy,
                DecisionOn = data.DecisionOn,
                OriginalPrice =
                    (data.OriginalPrice == data.DiscountedPrice)
                    || (
                        (data.DiscountedPrice.HasValue && !data.OriginalPrice.HasValue)
                        || (data.OriginalPrice.HasValue && !data.DiscountedPrice.HasValue)
                    )
                        ? null
                        : data.OriginalPrice,
                Discount =
                    (!data.Discount.HasValue && data.Discount.GetValueOrDefault() <= 0)
                        && (data.OriginalPrice == data.DiscountedPrice)
                    || (
                        (data.DiscountedPrice.HasValue && !data.OriginalPrice.HasValue)
                        || (data.OriginalPrice.HasValue && !data.DiscountedPrice.HasValue)
                    )
                        ? null
                        : data.Discount,
                DiscountedPrice =
                    (data.OriginalPrice == data.DiscountedPrice)
                    || (
                        (data.DiscountedPrice.HasValue && !data.OriginalPrice.HasValue)
                        || (data.OriginalPrice.HasValue && !data.DiscountedPrice.HasValue)
                    )
                        ? null
                        : data.DiscountedPrice,
                PriceCustom = CheckIfCustomPrice(
                    data.Discount,
                    data.DiscountedPrice,
                    data.OriginalPrice,
                    data.PriceCustom
                ),
                Type = GetPriceType(data),
                PriceFiled = GetPriceFileValue(data),
                AnnouncementActive = data.AnnouncementActive,
                BannerActive = data.BannerActive,
                CompanyId = data.CompanyId,
                CompanyOfficialEmail =
                    data.Company != null ? data.Company.OfficialEmail : string.Empty,
                CompanyNameEnglish = data.Company != null ? data.Company.NameEnglish : string.Empty,
                CompanyNameArabic = data.Company != null ? data.Company.NameArabic : string.Empty,
                CompanyAddress =
                    data.Company != null
                    && data.Company.CompanyLocations != null
                    && data.Company.CompanyLocations.Count() > 0
                        ? data.Company.CompanyLocations.FirstOrDefault().Address
                        : string.Empty,
                CompanyPhoneNumber = data.Company != null ? data.Company.Land : string.Empty,
                CompanyWebsite = data.Company != null ? data.Company.Website : string.Empty,
                CompanyLogo =
                    (data.Company != null && data.Company.Logo != null)
                        ? data.Company.Logo.DocumentId.ToString()
                        : string.Empty,
                BannerUrl = data.BannerUrl,
                ReportCount = data.ReportCount,
                PhoneNumber = new Shared.Models.Companies.PhoneNumberModel()
                {
                    CountryCode = data.CountryCode,
                    E164Number = data.E164Number,
                    InternationalNumber = data.InternationalNumber,
                    Number = data.Number
                },
                isPrivate = data.IsPrivate,
                /*  CompanyModel = data.Company != null ? new OneHubCompanyModel()
                  {
                      CompanyAdders = data.Company.CompanyLocations.FirstOrDefault()!=null? data.Company.CompanyLocations.FirstOrDefault().Address:"",
                      CompanyName = data.Company.NameEnglish,
                      CompanyImageId = data.Company.Logo != null ? data.Company.Logo.DocumentId.ToString() : string.Empty,
                      CompanyPhoneNumber = data.Company.LandNumber,
                       CompanyWebsite= data.Company.Website
                  } : null,*/
                OfferUrl = "https://mazayaoffers.ae/offers/" + data.Id
            };

        private Expression<Func<Offer, OfferOneHubModel>> projectToOfferOneHubModel = data =>
            new OfferOneHubModel()
            {
                AboutCompany = data.AboutCompany,
                Categories = data.OfferCategories
                    .Select(
                        x =>
                            new CategoryModel
                            {
                                Title = x.Category != null ? x.Category.Title : String.Empty,
                                Id = x.CategoryId
                            }
                    )
                    .ToList(),
                Collections = data.OfferCollections
                    .Select(
                        x =>
                            new CollectionModel
                            {
                                Title = x.Collection != null ? x.Collection.Title : String.Empty,
                                Id = x.CollectionId
                            }
                    )
                    .ToList(),
                Description = data.Description,
                DiscountFrom = data.DiscountFrom,
                DiscountTo = data.DiscountTo,
                FlagIsLatest = data.FlagIsLatest,
                Status = data.Status,
                Id = data.Id,
                Locations = data.OfferLocations
                    .Select(
                        ol =>
                            new OfferLocationModel
                            {
                                Address = ol.Address,
                                Country = ol.Country,
                                Id = ol.Id,
                                Latitude = ol.Latitude,
                                Longitude = ol.Longitude,
                                OfferId = data.Id,
                                Vicinity = ol.Vicinity,
                                DefaultAreaId = ol.DefaultAreaId,
                                //DefaultAreaTitle = ol.DefaultArea.Title
                            }
                    )
                    .ToList(),
                LocationsCount = data.OfferLocations.Count(),
                PriceFrom = data.PriceFrom,
                PriceTo = data.PriceTo,
                PriceList = data.PriceList,
                PromotionCode = data.PromotionCode,
                Tags = data.OfferTags
                    .Select(
                        x =>
                            new TagModel
                            {
                                Title = x.Tag != null ? x.Tag.Title : String.Empty,
                                Id = x.TagId
                            }
                    )
                    .ToList(),
                TermsAndCondition = data.TermsAndCondition,
                Title = FormatTitle(
                    data.Title,
                    data.CreatedOn,
                    data.Company == null ? String.Empty : data.Company.NameEnglish
                ),
                ValidFrom = data.ValidFrom.Value.SpecifyKind(DateTimeKind.Utc),
                ValidUntil = data.ValidUntil.Value.SpecifyKind(DateTimeKind.Utc),
                // 5 star rating
                Rating =
                    data.OfferRating
                        .Where(x => x.Status == OfferCommentStatus.Public.ToString())
                        .Count() > 0
                        ? data.OfferRating
                            .Where(x => x.Status == OfferCommentStatus.Public.ToString())
                            .Average(x => x.Rating)
                        : 0,
                RatingPercent =
                    data.OfferRating
                        .Where(x => x.Status == OfferCommentStatus.Public.ToString())
                        .Count() * 20,
                Votes = data.OfferRating
                    .Where(x => x.Status == OfferCommentStatus.Public.ToString())
                    .Count(),
                WhatYouGet = data.WhatYouGet,
                Image = data.OfferDocuments
                    .Where(od => (od.Type == OfferDocumentType.Thumbnail) && od.Cover)
                    .Select(
                        od =>
                            baseUrl
                            + "/media/"
                            + (
                                !string.IsNullOrEmpty(od.DocumentId.ToString())
                                    ? od.DocumentId.ToString()
                                    : string.Empty
                            )
                            + ".jpg"
                    )
                    .FirstOrDefault(),
                QRcodeImage = "",
                Attachments = data.OfferDocuments
                    .Where(od => (int)od.Type == 1)
                    .Select(
                        od =>
                            new AttachmentModel
                            {
                                Id = !string.IsNullOrEmpty(od.DocumentId.ToString())
                                    ? od.DocumentId.ToString()
                                    : string.Empty,
                                Name = od.Document != null ? od.Document.Name : string.Empty,
                                Type = od.Document != null ? od.Document.MimeType : string.Empty,
                                LastModified =
                                    od.Document != null ? od.Document.UpdatedOn : DateTime.UtcNow,
                                Size = od.Document != null ? od.Document.Size : 0,
                                Url =
                                    od.Document != null
                                        ? baseUrl
                                            + "/media/"
                                            + (
                                                !string.IsNullOrEmpty(od.DocumentId.ToString())
                                                    ? od.DocumentId.ToString()
                                                    : string.Empty
                                            )
                                            + "."
                                            + GetMimeType(od.Document.MimeType)
                                        : "",
                            }
                    )
                    .ToList(),
                FlagIsWeekendOffer = data.FlagIsWeekendOffer,
                CreatedBy = data.CreatedBy,
                CreatedOn = data.CreatedOn,
                UpdatedBy = data.UpdatedBy,
                UpdatedOn = data.UpdatedOn,
                Comments = data.Comments
                    .Select(
                        c =>
                            new CommentModel
                            {
                                CreatedBy = c.CreatedBy,
                                CreatedOn = c.CreatedOn.SpecifyKind(DateTimeKind.Utc),
                                Text = c.Text,
                                Id = c.Id
                            }
                    )
                    .ToList(),
                ReviewedBy = data.ReviewedBy,
                ReviewedOn = data.ReviewedOn,
                DecisionBy = data.DecisionBy,
                DecisionOn = data.DecisionOn,
                OriginalPrice =
                    (data.OriginalPrice == data.DiscountedPrice)
                    || (
                        (data.DiscountedPrice.HasValue && !data.OriginalPrice.HasValue)
                        || (data.OriginalPrice.HasValue && !data.DiscountedPrice.HasValue)
                    )
                        ? null
                        : data.OriginalPrice,
                Discount =
                    (!data.Discount.HasValue && data.Discount.GetValueOrDefault() <= 0)
                        && (data.OriginalPrice == data.DiscountedPrice)
                    || (
                        (data.DiscountedPrice.HasValue && !data.OriginalPrice.HasValue)
                        || (data.OriginalPrice.HasValue && !data.DiscountedPrice.HasValue)
                    )
                        ? null
                        : data.Discount,
                DiscountedPrice =
                    (data.OriginalPrice == data.DiscountedPrice)
                    || (
                        (data.DiscountedPrice.HasValue && !data.OriginalPrice.HasValue)
                        || (data.OriginalPrice.HasValue && !data.DiscountedPrice.HasValue)
                    )
                        ? null
                        : data.DiscountedPrice,
                PriceCustom = CheckIfCustomPrice(
                    data.Discount,
                    data.DiscountedPrice,
                    data.OriginalPrice,
                    data.PriceCustom
                ),
                Type = GetPriceType(data),
                PriceFiled = GetPriceFileValue(data),
                AnnouncementActive = data.AnnouncementActive,
                BannerActive = data.BannerActive,
                CompanyId = data.CompanyId,
                CompanyOfficialEmail =
                    data.Company != null ? data.Company.OfficialEmail : string.Empty,
                CompanyNameEnglish = data.Company != null ? data.Company.NameEnglish : string.Empty,
                CompanyNameArabic = data.Company != null ? data.Company.NameArabic : string.Empty,
                CompanyAddress =
                    data.Company != null
                    && data.Company.CompanyLocations != null
                    && data.Company.CompanyLocations.Count() > 0
                        ? data.Company.CompanyLocations.FirstOrDefault().Address
                        : string.Empty,
                CompanyPhoneNumber = data.Company != null ? data.Company.Land : string.Empty,
                CompanyWebsite = data.Company != null ? data.Company.Website : string.Empty,
                CompanyLogo =
                    (data.Company != null && data.Company.Logo != null)
                        ? baseUrl + "/media/" + data.Company.Logo.DocumentId.ToString() + ".jpg"
                        : string.Empty,
                BannerUrl = data.BannerUrl,
                ReportCount = data.ReportCount,
                PhoneNumber = new Shared.Models.Companies.PhoneNumberModel()
                {
                    CountryCode = data.CountryCode,
                    E164Number = data.E164Number,
                    InternationalNumber = data.InternationalNumber,
                    Number = data.Number
                },
                isPrivate = data.IsPrivate,
                CompanyModel =
                    data.Company != null
                        ? new OneHubCompanyModel()
                        {
                            CompanyAdders =
                                data.Company.CompanyLocations.FirstOrDefault() != null
                                    ? data.Company.CompanyLocations.FirstOrDefault().Address
                                    : "",
                            CompanyName = data.Company.NameEnglish,
                            CompanyImageUrl =
                                (data.Company != null && data.Company.Logo != null)
                                    ? baseUrl
                                        + "/media/"
                                        + data.Company.Logo.DocumentId.ToString()
                                        + ".jpg"
                                    : string.Empty,
                            CompanyPhoneNumber = data.Company.LandNumber,
                            CompanyWebsite = data.Company.Website
                        }
                        : null,
                OfferUrl = "https://mazayaoffers.ae/offers/" + data.Id
            };

        private static string GetMimeType(string value)
        {
            return value.Split("/")[1];
        }

        private readonly Expression<Func<Offer, OfferModel>> projectToOfferCardModel = data =>
            new OfferModel()
            {
                Categories = data.OfferCategories
                    .Select(
                        x =>
                            new CategoryModel
                            {
                                Title = x.Category != null ? x.Category.Title : String.Empty,
                                Id = x.CategoryId
                            }
                    )
                    .ToList(),
                Description =
                    data.Description.Length > 150
                        ? $"{data.Description.Substring(0, 140)}..."
                        : data.Description,
                Brand = data.Brand,
                DiscountFrom = data.DiscountFrom,
                DiscountTo = data.DiscountTo,
                Status = data.Status,
                Id = data.Id,
                City = data.OfferLocations
                    .Select(
                        ol =>
                            String.IsNullOrWhiteSpace(ol.Vicinity)
                                ? "Unnamed Location"
                                : ol.Vicinity
                    )
                    .FirstOrDefault(),
                Locations = data.OfferLocations
                    .Select(
                        ol =>
                            new OfferLocationModel
                            {
                                Vicinity = String.IsNullOrWhiteSpace(ol.Vicinity)
                                    ? "Unnamed Location"
                                    : ol.Vicinity,
                                DefaultAreaId = ol.DefaultAreaId,
                                DefaultAreaTitle = ol.DefaultArea.Title,
                            }
                    )
                    .ToList(),
                PriceFrom = data.PriceFrom,
                PriceTo = data.PriceTo,
                Tags = data.OfferTags
                    .Select(
                        x =>
                            new TagModel
                            {
                                Title = x.Tag != null ? x.Tag.Title : String.Empty,
                                Id = x.TagId
                            }
                    )
                    .ToList(),
                Title = FormatTitle(data.Title, data.CreatedOn, data.Company.NameEnglish),
                CompanyNameEnglish = data.Company.NameEnglish,
                CreatedBy = data.CreatedBy,
                CreatedOn = data.CreatedOn,
                UpdatedBy = data.UpdatedBy,
                UpdatedOn = data.UpdatedOn,
                ReviewedBy = data.ReviewedBy,
                ReviewedOn = data.ReviewedOn,
                DecisionBy = data.DecisionBy,
                DecisionOn = data.DecisionOn,
                MainImage = data.OfferDocuments
                    .Where(od => (int)od.Type == 3 && od.Cover)
                    .Select(od => od.DocumentId.ToString())
                    .FirstOrDefault(),
                OriginalPrice =
                    (data.OriginalPrice == data.DiscountedPrice)
                    || (
                        (data.DiscountedPrice.HasValue && !data.OriginalPrice.HasValue)
                        || (data.OriginalPrice.HasValue && !data.DiscountedPrice.HasValue)
                    )
                        ? null
                        : data.OriginalPrice,
                Discount =
                    (!data.Discount.HasValue && data.Discount.Value <= 0)
                        && (data.OriginalPrice == data.DiscountedPrice)
                    || (
                        (data.DiscountedPrice.HasValue && !data.OriginalPrice.HasValue)
                        || (data.OriginalPrice.HasValue && !data.DiscountedPrice.HasValue)
                    )
                        ? null
                        : data.Discount,
                DiscountedPrice =
                    (data.OriginalPrice == data.DiscountedPrice)
                    || (
                        (data.DiscountedPrice.HasValue && !data.OriginalPrice.HasValue)
                        || (data.OriginalPrice.HasValue && !data.DiscountedPrice.HasValue)
                    )
                        ? null
                        : data.DiscountedPrice,
                PriceCustom = CheckIfCustomPrice(
                    data.Discount,
                    data.DiscountedPrice,
                    data.OriginalPrice,
                    data.PriceCustom
                ),
                Type = GetPriceType(data),
                BannerUrl = data.BannerUrl,
                ReportCount = data.ReportCount,
                isPrivate = data.IsPrivate,
                PriceFiled = GetPriceFileValue(data)
            };

        private readonly Expression<
            Func<Offer, OfferOneHubCardModel>
        > projectToOfferOneHubCardModel = data =>
            new OfferOneHubCardModel()
            {
                Categories = data.OfferCategories
                    .Select(
                        x =>
                            new CategoryModel
                            {
                                Title = x.Category != null ? x.Category.Title : String.Empty,
                                Id = x.CategoryId
                            }
                    )
                    .ToList(),
                Description =
                    data.Description.Length > 150
                        ? $"{data.Description.Substring(0, 140)}..."
                        : data.Description,
                DiscountFrom = data.DiscountFrom,
                DiscountTo = data.DiscountTo,
                Status = data.Status,
                Id = data.Id,
                City = data.OfferLocations
                    .Select(
                        ol =>
                            String.IsNullOrWhiteSpace(ol.Vicinity)
                                ? "Unnamed Location"
                                : ol.Vicinity
                    )
                    .FirstOrDefault(),
                Locations = data.OfferLocations
                    .Select(
                        ol =>
                            new OfferLocationModel
                            {
                                Vicinity = String.IsNullOrWhiteSpace(ol.Vicinity)
                                    ? "Unnamed Location"
                                    : ol.Vicinity,
                                DefaultAreaId = ol.DefaultAreaId,
                                DefaultAreaTitle = ol.DefaultArea.Title,
                            }
                    )
                    .ToList(),
                PriceFrom = data.PriceFrom,
                PriceTo = data.PriceTo,
                Tags = data.OfferTags
                    .Select(
                        x =>
                            new TagModel
                            {
                                Title = x.Tag != null ? x.Tag.Title : String.Empty,
                                Id = x.TagId
                            }
                    )
                    .ToList(),
                Title = FormatTitle(data.Title, data.CreatedOn, data.Company.NameEnglish),
                CompanyNameEnglish = data.Company.NameEnglish,
                CreatedBy = data.CreatedBy,
                CreatedOn = data.CreatedOn,
                UpdatedBy = data.UpdatedBy,
                UpdatedOn = data.UpdatedOn,
                ReviewedBy = data.ReviewedBy,
                ReviewedOn = data.ReviewedOn,
                DecisionBy = data.DecisionBy,
                DecisionOn = data.DecisionOn,
                MainImage = data.OfferDocuments
                    .Where(od => (int)od.Type == 3 && od.Cover)
                    .Select(od => od.DocumentId.ToString())
                    .FirstOrDefault(),
                OriginalPrice =
                    (data.OriginalPrice == data.DiscountedPrice)
                    || (
                        (data.DiscountedPrice.HasValue && !data.OriginalPrice.HasValue)
                        || (data.OriginalPrice.HasValue && !data.DiscountedPrice.HasValue)
                    )
                        ? null
                        : data.OriginalPrice,
                Discount =
                    (!data.Discount.HasValue && data.Discount.Value <= 0)
                        && (data.OriginalPrice == data.DiscountedPrice)
                    || (
                        (data.DiscountedPrice.HasValue && !data.OriginalPrice.HasValue)
                        || (data.OriginalPrice.HasValue && !data.DiscountedPrice.HasValue)
                    )
                        ? null
                        : data.Discount,
                DiscountedPrice =
                    (data.OriginalPrice == data.DiscountedPrice)
                    || (
                        (data.DiscountedPrice.HasValue && !data.OriginalPrice.HasValue)
                        || (data.OriginalPrice.HasValue && !data.DiscountedPrice.HasValue)
                    )
                        ? null
                        : data.DiscountedPrice,
                PriceCustom = CheckIfCustomPrice(
                    data.Discount,
                    data.DiscountedPrice,
                    data.OriginalPrice,
                    data.PriceCustom
                ),
                Type = GetPriceType(data),
                BannerUrl = data.BannerUrl,
                ReportCount = data.ReportCount,
                isPrivate = data.IsPrivate,
                PriceFiled = GetPriceFileValue(data)
            };

        private readonly Expression<Func<Offer, OfferModelMobile>> projectToOfferModelMobile =
            data =>
                new OfferModelMobile()
                {
                    AboutCompany = data.AboutCompany,
                    CategoryIds = data.OfferCategories.Select(x => x.CategoryId),
                    CollectionIds = data.OfferCollections.Select(x => x.CollectionId),
                    Description = data.Description,
                    DiscountFrom = data.DiscountFrom,
                    DiscountTo = data.DiscountTo,
                    Status = data.Status.ToString(),
                    Id = data.Id,
                    Locations = data.OfferLocations
                        .Select(
                            ol =>
                                new OfferLocationModel
                                {
                                    Id = ol.Id,
                                    Address = ol.Address,
                                    Country = ol.Address,
                                    Latitude = ol.Latitude,
                                    Longitude = ol.Longitude,
                                    Vicinity = ol.Vicinity,
                                    DefaultAreaId = ol.DefaultAreaId,
                                    DefaultAreaTitle = ol.DefaultArea.Title
                                }
                        )
                        .ToList(),
                    DefaultAreaId = data.OfferLocations.First().DefaultAreaId,
                    PriceCustom = data.PriceCustom,
                    PriceFrom = data.PriceFrom,
                    PriceTo = data.PriceTo,
                    PriceList = data.PriceList,
                    PromotionCode = data.PromotionCode,
                    TagIds = data.OfferTags.Select(x => x.TagId),
                    TermsAndCondition = data.TermsAndCondition,
                    Title = data.Title,
                    Brand = data.Brand,
                    ValidFrom = data.ValidFrom,
                    ValidUntil = data.ValidUntil,
                    Rating =
                        data.OfferRating.Count > 0
                            ? data.OfferRating
                                .Where(x => x.Status == OfferCommentStatus.Public.ToString())
                                .Average(x => x.Rating)
                            : 0,
                    RatingPercent =
                        data.OfferRating.Count > 0
                            ? data.OfferRating
                                .Where(x => x.Status == OfferCommentStatus.Public.ToString())
                                .Average(x => x.Rating) * 20
                            : 0,
                    Votes =
                        data.OfferRating.Count > 0
                            ? data.OfferRating
                                .Where(x => x.Status == OfferCommentStatus.Public.ToString())
                                .Count()
                            : 0,
                    WhatYouGet = data.WhatYouGet,
                    Images = data.OfferDocuments
                        .Where(od => (int)od.Type != 1)
                        .Select(od => od.DocumentId.ToString())
                        .ToList(),
                    Attachments = data.OfferDocuments
                        .Where(od => (int)od.Type == 1)
                        .Select(
                            od =>
                                new AttachmentModel
                                {
                                    Id = !string.IsNullOrEmpty(od.DocumentId.ToString())
                                        ? od.DocumentId.ToString()
                                        : string.Empty
                                }
                        )
                        .ToList(),
                    CreatedBy = data.CreatedBy,
                    CreatedOn = data.CreatedOn,
                    UpdatedBy = data.UpdatedBy,
                    UpdatedOn = data.UpdatedOn,
                    ReviewedBy = data.ReviewedBy,
                    ReviewedOn = data.ReviewedOn,
                    DecisionBy = data.DecisionBy,
                    DecisionOn = data.DecisionOn,
                    OriginalPrice = data.Discount == 0 ? null : data.OriginalPrice,
                    Discount = data.Discount == 0 ? null : data.Discount,
                    DiscountedPrice = data.DiscountedPrice,
                    //IsFavourite = data.UserFavouritesOffers.Where(ufo => ufo.OfferId == data.Id).FirstOrDefault()
                    //  ReportCount = data.ReportCount
                    BannerUrl = data.BannerUrl,
                    phoneNumber = data.InternationalNumber
                };

        private void PopulateEntityModel(Offer data, OfferModel model, string userId)
        {
            // Fields are encoded on frontend, we first need to decode them
            data.PriceList = model.PriceList;
            data.TermsAndCondition = model.TermsAndCondition;
            data.AboutCompany = model.AboutCompany;
            data.Description = model.Description;
            data.AnnouncementActive = model.AnnouncementActive;
            data.OfferCategories = model.Categories
                .Select(
                    category => new OfferCategory { OfferId = model.Id, CategoryId = category.Id }
                )
                .ToList();
            data.OfferCollections = model.Collections
                .Select(
                    collection =>
                        new OfferCollection { OfferId = model.Id, CollectionId = collection.Id }
                )
                .ToList();
            data.OfferTags = model.Tags
                .Select(tag => new OfferTag { OfferId = model.Id, TagId = tag.Id })
                .ToList();
            data.Description = model.Description;
            data.DiscountFrom = model.DiscountFrom;
            data.DiscountTo = model.DiscountTo;
            data.FlagIsLatest = model.FlagIsLatest;
            data.Id = model.Id;
            data.OfferLocations = model.Locations
                .Select(
                    oc =>
                        new OfferLocation
                        {
                            Address = oc.Address,
                            Country = oc.Country,
                            Latitude = oc.Latitude,
                            Longitude = oc.Longitude,
                            OfferId = model.Id,
                            Vicinity = oc.Vicinity,
                            DefaultAreaId = oc.DefaultAreaId
                        }
                )
                .ToList();
            data.PriceTo = model.PriceTo;
            data.PriceFrom = model.PriceFrom;
            data.PriceCustom = model.PriceCustom;
            //data.Status = model.Status.ToString();
            data.PromotionCode = model.PromotionCode;
            data.Title = model.Title;
            data.Brand = model.Brand;
            data.ValidFrom = model.ValidFrom;
            data.ValidUntil = model.ValidUntil;
            data.WhatYouGet = model.WhatYouGet;
            data.OfferDocuments = model.OfferDocuments
                .Select(
                    od =>
                        new OfferDocument
                        {
                            DocumentId = od.DocumentId,
                            OfferId = model.Id,
                            Type = od.Type.ToString() == "0" ? OfferDocumentType.Original : od.Type,
                            OriginalImageId =
                                od.OriginalImageId == Guid.Empty
                                    ? od.DocumentId
                                    : od.OriginalImageId,
                            X1 = od.X1,
                            X2 = od.X2,
                            Y1 = od.Y1,
                            Y2 = od.Y2,
                            cropX1 = od.cropX1,
                            cropX2 = od.cropX2,
                            cropY1 = od.cropY1,
                            cropY2 = od.cropY2,
                            Cover = od.Cover
                        }
                )
                .ToList();
            data.ReviewedBy = model.ReviewedBy;
            data.ReviewedOn = model.ReviewedOn;
            data.DecisionOn = model.DecisionOn;
            data.DecisionBy = model.DecisionBy;
            data.Comments = model.Comments
                .Select(
                    c =>
                        new Comment
                        {
                            CreatedBy = c.CreatedBy,
                            CreatedOn = c.CreatedOn,
                            OfferId = c.OfferId,
                            Text = c.Text
                        }
                )
                .ToList();
            data.OriginalPrice = model.OriginalPrice;
            data.DiscountedPrice = model.DiscountedPrice;
            data.Discount = model.Discount;
            data.BannerActive = model.BannerActive;
            data.AnnouncementActive = model.AnnouncementActive;
            data.CompanyId = model.CompanyId;
            data.CountryCode = model.PhoneNumber != null ? model.PhoneNumber.CountryCode : "";
            data.E164Number = model.PhoneNumber != null ? model.PhoneNumber.E164Number : "";
            data.InternationalNumber =
                model.PhoneNumber != null ? model.PhoneNumber.InternationalNumber : "";
            data.Number = model.PhoneNumber != null ? model.PhoneNumber.Number : "";
            data.BannerUrl = model.BannerUrl;
            data.IsPrivate = model.isPrivate;
        }

        public async Task<OfferModel> GetDraftOfferById(int id, string userId)
        {
            var context = ContextFactory();

            return await context.Offer
                .AsNoTracking()
                .Include(o => o.OfferLocations)
                .Where(o => o.Status == OfferStatus.Draft.ToString() && o.CreatedBy == userId)
                .Select(projectToOfferModel)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<OfferModel> GetSpecificOfferById(int id, string userId, List<Roles> roles)
        {
            var context = ContextFactory();
            var offer = new OfferModel();

            if (roles.Contains(Roles.Supplier) || roles.Contains(Roles.SupplierAdmin))
            {
                var companySupplier = context.CompanySuppliers
                    .Where(cs => cs.SupplierId == userId)
                    .FirstOrDefault();
                offer = await context.Offer
                    .AsNoTracking()
                    .Where(o => o.CompanyId == companySupplier.CompanyId)
                    .Select(projectToOfferModel)
                    .FirstOrDefaultAsync(x => x.Id == id);
            }
            else if (roles.Contains(Roles.Reviewer))
            {
                offer = await context.Offer
                    .AsNoTracking()
                    .Select(projectToOfferModel)
                    .Where(o => o.Status != OfferStatus.Draft.ToString() || o.ReviewedBy == userId)
                    .FirstOrDefaultAsync(x => x.Id == id);
            }
            else if (roles.Contains(Roles.AdnocCoordinator) || roles.Contains(Roles.Admin))
            {
                offer = await context.Offer
                    .AsNoTracking()
                    .Where(
                        o =>
                            o.Status != OfferStatus.Draft.ToString()
                            || o.Status != OfferStatus.Review.ToString()
                            || o.DecisionBy == userId
                    )
                    .Select(projectToOfferModel)
                    .FirstOrDefaultAsync(x => x.Id == id);
            }
            else
            {
                offer = await context.Offer
                    .AsNoTracking()
                    .Where(o => o.Status == OfferStatus.Approved.ToString())
                    .Select(projectToOfferModel)
                    .FirstOrDefaultAsync(x => x.Id == id);
            }

            offer.MembershipType = _membershipECardRepository.GetMembershipsForOffer(id).ToArray();
            return offer;
        }

        public async Task<OfferModel> GetReviewOfferById(int id)
        {
            var context = ContextFactory();

            return await context.Offer
                .AsNoTracking()
                .Include(o => o.OfferLocations)
                .Where(o => o.Status == OfferStatus.Review.ToString())
                .Select(projectToOfferModel)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<OfferModel> GetPendingOfferById(int id)
        {
            var context = ContextFactory();

            return await context.Offer
                .AsNoTracking()
                .Where(o => o.Status == OfferStatus.PendingApproval.ToString())
                .Select(projectToOfferModel)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<OfferModel> DeleteAsync(int id, string userId)
        {
            var context = ContextFactory();
            var offer = await context.Offer
                .AsNoTracking()
                .Select(projectToOfferModel)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (offer != null)
            {
                var userRole = await (
                    from ur in context.UserRoles
                    join r in context.Roles on ur.RoleId equals r.Id
                    where ur.UserId == userId
                    select r.Name
                ).FirstOrDefaultAsync();
                if (
                    userRole == Roles.Admin.ToString()
                    || userRole == Roles.AdnocCoordinator.ToString()
                )
                {
                    var data = new Offer();
                    data.Id = offer.Id;

                    var mails = context.MailStorage.Where(m => m.OfferId == offer.Id);
                    var notifications = context.UserNotification.Where(m => m.OfferId == offer.Id);
                    var documents = (
                        from d in context.Document
                        join od in context.OfferDocument on d.Id equals od.DocumentId
                        where od.OfferId == offer.Id
                        select d
                    );
                    context.MailStorage.RemoveRange(mails);
                    context.UserNotification.RemoveRange(notifications);
                    context.Document.RemoveRange(documents);

                    context.Offer.Remove(data);
                    await context.SaveChangesAsync();
                }
                else if (
                    (
                        userRole == Roles.SupplierAdmin.ToString()
                        || userRole == Roles.Supplier.ToString()
                    )
                    && offer.Status == OfferStatus.Draft.ToString()
                )
                {
                    var companySupplier = context.CompanySuppliers
                        .Where(cs => cs.SupplierId == userId)
                        .FirstOrDefault();

                    if (companySupplier.CompanyId == offer.CompanyId)
                    {
                        var data = new Offer();
                        data.Id = offer.Id;

                        var mails = context.MailStorage.Where(m => m.OfferId == offer.Id);
                        var notifications = context.UserNotification.Where(
                            m => m.OfferId == offer.Id
                        );
                        var documents = (
                            from d in context.Document
                            join od in context.OfferDocument on d.Id equals od.DocumentId
                            where od.OfferId == offer.Id
                            select d
                        );

                        context.MailStorage.RemoveRange(mails);
                        context.UserNotification.RemoveRange(notifications);
                        context.Document.RemoveRange(documents);

                        context.Offer.Remove(data);
                        await context.SaveChangesAsync();
                    }
                }
            }
            return offer;
        }

        /// <summary>
        /// We include company names for offers only if it is offer from old system
        /// Migration of DB was done on 04.02.2021. (Set in const MIGRATION_DATE)
        /// </summary>
        /// <param name="title"></param>
        /// <param name="offerCreationDate"></param>
        /// <param name="companyName"></param>
        /// <returns></returns>
        private static string FormatTitle(
            string title,
            DateTime offerCreationDate,
            string companyName
        )
        {
            return offerCreationDate < DateTime.Parse(MIGRATION_DATE) ? companyName : title;
        }

        public static string GetPriceType(Offer offer)
        {
            if (
                offer.PriceFrom.HasValue
                && offer.PriceTo.HasValue
                && offer.PriceFrom > 0
                && offer.PriceTo > 0
            )
            {
                return PriceTypes.Price.ToString();
            }
            else if (
                (
                    offer.DiscountFrom.HasValue
                    && offer.DiscountTo.HasValue
                    && offer.DiscountFrom > 0
                    && offer.DiscountTo > 0
                ) || (offer.Discount.HasValue && offer.Discount.Value > 0)
            )
            {
                return PriceTypes.Discount.ToString();
            }
            else if (
                !string.IsNullOrEmpty(
                    CheckIfCustomPrice(
                        offer.Discount,
                        offer.DiscountedPrice,
                        offer.OriginalPrice,
                        offer.PriceCustom
                    )
                )
            )
            {
                return PriceTypes.Other.ToString();
            }
            else if (
                (offer.OriginalPrice.HasValue || offer.DiscountedPrice.HasValue)
                && (offer.OriginalPrice > 0 || offer.DiscountedPrice > 0)
            )
            {
                return PriceTypes.DiscountPrice.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        public static string GetPriceFileValue(Offer offer)
        {
            if (
                offer.PriceFrom.HasValue
                && offer.PriceTo.HasValue
                && offer.PriceFrom > 0
                && offer.PriceTo > 0
            )
            { //AED 200 - AED 300
                return "AED "
                    + offer.PriceFrom.Value.ToString("F")
                    + " - AED "
                    + offer.PriceTo.Value.ToString("F");
            }
            else if (
                offer.DiscountFrom.HasValue
                && offer.DiscountTo.HasValue
                && offer.DiscountFrom > 0
                && offer.DiscountTo > 0
            )
            { //From 10% -15% discount
                return "From "
                    + offer.DiscountFrom.Value.ToString("F")
                    + "% - To "
                    + offer.DiscountTo.Value.ToString("F")
                    + "% Discount";
            }
            else if (offer.Discount.HasValue && offer.Discount.Value > 0)
            { //55% OFF
                return offer.Discount.Value.ToString("F") + "% OFF ";
            }
            else if (
                !string.IsNullOrEmpty(
                    CheckIfCustomPrice(
                        offer.Discount,
                        offer.DiscountedPrice,
                        offer.OriginalPrice,
                        offer.PriceCustom
                    )
                )
            )
            {
                return CheckIfCustomPrice(
                    offer.Discount,
                    offer.DiscountedPrice,
                    offer.OriginalPrice,
                    offer.PriceCustom
                );
            }
            else if (
                (offer.OriginalPrice.HasValue || offer.DiscountedPrice.HasValue)
                && (offer.OriginalPrice > 0 || offer.DiscountedPrice > 0)
            )
            { // From AED 3000 to 2000 AED
                return "From AED "
                    + offer.OriginalPrice.Value.ToString("F")
                    + " to AED "
                    + offer.DiscountedPrice.Value.ToString("F"); // precrtana linija?
            }
            else
            {
                return string.Empty;
            }
        }

        public static decimal GetRating(Offer offer)
        {
            offer.OfferRating = new List<OfferRating>();
            int numberOfCurrentRatings = offer.OfferRating.Count(or => or.OfferId == offer.Id);
            if (numberOfCurrentRatings == 0)
            {
                return 0;
            }
            else
            {
                return Math.Round(
                    offer.OfferRating
                        .Where(or => or.OfferId == offer.Id)
                        .Select(x => x.Rating)
                        .Sum() / offer.OfferRating.Count(or => or.OfferId == offer.Id),
                    2
                );
            }
        }

        public static decimal GetOfferRating(Offer offer)
        {
            int count = offer.OfferRating.Count(or => or.OfferId == offer.Id);
            decimal sum = offer.OfferRating
                .Where(or => or.OfferId == offer.Id)
                .Select(x => x.Rating)
                .Sum();

            return sum / count;
        }

        public async Task SetOfferAsFavourite(OfferFavoriteModel offerFavorite, string userId)
        {
            var context = ContextFactory();
            UserFavouritesOffer userFavouritesOffer =
                await context.UserFavouritesOffers.FirstOrDefaultAsync(
                    x => x.ApplicationUserId == userId && x.OfferId == offerFavorite.OfferId
                );

            if (userFavouritesOffer == null)
            {
                userFavouritesOffer = new UserFavouritesOffer
                {
                    ApplicationUserId = userId,
                    OfferId = offerFavorite.OfferId,
                    UpdatedOn = DateTime.UtcNow,
                    IsFavourite = offerFavorite.IsFavourite
                };
                context.UserFavouritesOffers.Add(userFavouritesOffer);
            }
            else
            {
                userFavouritesOffer.IsFavourite = offerFavorite.IsFavourite;
                userFavouritesOffer.UpdatedOn = DateTime.UtcNow;
                context.UserFavouritesOffers.Update(userFavouritesOffer);
            }

            await context.SaveChangesAsync();
        }

        public void CreateOrUpdateBanner(List<int> offerIds, IVisitor<IChangeable> auditVisitor)
        {
            var context = ContextFactory();
            var offersToUpdate = new List<Offer>();

            //Remove all offers from banner
            var offersCurrentlyInBanner = context.Offer.Where(x => x.BannerActive.Value).ToList();
            foreach (var offer in offersCurrentlyInBanner)
            {
                offer.BannerActive = false;
                offer.UpdatedOn = DateTime.UtcNow;
            }
            context.Offer.UpdateRange(offersCurrentlyInBanner);

            //Set new ones
            foreach (var offerId in offerIds)
            {
                var offer = context.Offer.Where(x => x.Id == offerId).FirstOrDefault();
                offer.BannerActive = true;
                offer.UpdatedOn = DateTime.UtcNow;
                offersToUpdate.Add(offer);
            }

            context.Offer.UpdateRange(offersToUpdate);
            context.SaveChangesAsync();
        }

        public List<int> GetBannerOffers()
        {
            var context = ContextFactory();
            IQueryable<int> offersID = context.Offer
                .Where(o => o.BannerActive.Value == true)
                .Select(x => x.Id);

            var list = offersID.ToList();
            return list;
        }

        public IQueryable<OfferModel> GetBannerOffers(QueryModel queryModel)
        {
            var context = ContextFactory();
            IQueryable<Offer> offers = context.Offer.Include(o => o.OfferLocations).AsNoTracking();
            IQueryable<OfferModel> offerModels = null;
            IQueryable<Offer> filteredOffers = null;

            offers = offers.Where(offer => offer.BannerActive.Value);

            filteredOffers = Filter(offers, queryModel);
            offerModels = filteredOffers.Select(projectToOfferCardModel);

            return Sort(queryModel.Sort, offerModels);
        }

        /// <summary>
        /// Generates QR Code for certain offer based on his ID with logo
        /// </summary>
        /// <param name="offerId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<DocumentFileModel> GenerateQRCodeWithLogo(int offerId, string userId)
        {
            var context = ContextFactory();
            string url = _configuration["BaseURL:Url"] + "offers/" + offerId;
            string name = "QRCodeImage" + offerId;
            return await myQRCodeGenerator.GenerateAndWriteQRCode(
                offerId,
                userId,
                context,
                url,
                _documentService,
                name
            );
        }

        public async Task<Guid> GetQRCodeForOffer(int offerId)
        {
            var context = ContextFactory();
            var qrCode = await (
                from od in context.OfferDocument
                join d in context.Document on od.DocumentId equals d.Id
                where od.OfferId == offerId && od.Type == OfferDocumentType.QRCode
                select d
            ).FirstOrDefaultAsync();

            return qrCode == null ? Guid.Empty : qrCode.Id;
        }

        public async Task<byte[]> GetQRCodeData(int offerId)
        {
            var context = ContextFactory();
            var qrCode = await (
                from od in context.OfferDocument
                join d in context.Document on od.DocumentId equals d.Id
                where od.OfferId == offerId && od.Type == OfferDocumentType.QRCode
                select d
            ).FirstOrDefaultAsync();
            return qrCode == null ? null : qrCode.Content;
        }

        public async Task DeactivateOffers(int companyId)
        {
            var context = ContextFactory();

            var offerIds = context.Offer
                .Where(o => o.CompanyId == companyId)
                .Select(x => x.Id)
                .ToList();
            var offers = context.Offer.Where(x => offerIds.Contains(x.Id)).ToList();
            offers.ForEach(o => o.Status = "Deactivated");

            context.Offer.UpdateRange(offers);
            await context.SaveChangesAsync();
        }

        public async Task HardOfCompanyDeleteOffers(int companyId)
        {
            var context = ContextFactory();

            var offerIds = context.Offer
                .Where(o => o.CompanyId == companyId)
                .Select(x => x.Id)
                .ToList();
            var offers = context.Offer.Where(x => offerIds.Contains(x.Id)).ToList();
            var mails = context.MailStorage
                .Where(m => offerIds.Contains(m.OfferId.GetValueOrDefault()))
                .ToList();
            var notifications = context.UserNotification
                .Where(m => offerIds.Contains(m.OfferId.GetValueOrDefault()))
                .ToList();

            context.MailStorage.RemoveRange(mails);
            context.UserNotification.RemoveRange(notifications);
            context.Offer.RemoveRange(offers);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<OfferModel>> CheckExpiredOffers(ILogger logger)
        {
            try
            {
                logger.LogInformation($"CheckExpiredOffers in repository: {DateTime.Now}");

                var context = ContextFactory();

                logger.LogInformation(
                    $"CheckExpiredOffers in repository -> after ContextFactory()"
                );

                var expiredOffers = context.Offer.Where(
                    o =>
                        o.ValidUntil.Value.Date < DateTime.UtcNow.Date
                        && o.Status != OfferStatus.Expired.ToString()
                );

                var offerToSendMails = context.Offer
                    .Where(
                        o =>
                            o.ValidUntil.Value.Date < DateTime.UtcNow.Date
                            && o.Status != OfferStatus.Expired.ToString()
                    )
                    .Select(o => new OfferModel { Id = o.Id, CompanyId = o.CompanyId })
                    .ToList();

                logger.LogInformation(
                    $"CheckExpiredOffers in repository -> after query, before changing of status in foreach"
                );

                foreach (var o in expiredOffers)
                {
                    o.Status = OfferStatus.Expired.ToString();
                    o.UpdatedOn = DateTime.UtcNow;
                }

                logger.LogInformation($"CheckExpiredOffers in repository -> after foreach");

                await context.SaveChangesAsync();

                logger.LogInformation(
                    $"CheckExpiredOffers in repository -> after context.SaveChangesAsync()"
                );

                return offerToSendMails;
            }
            catch (Exception e)
            {
                logger.LogInformation(
                    $"CheckExpiredOffers caused exception in repo: {e.ToString()}"
                );
            }

            return null;
        }

        public IQueryable<DefaultAreaModel> GetAllDefaultAreas()
        {
            var context = ContextFactory();

            return context.DefaultArea.Select(projectToDefaultAreaModel);
        }

        public DefaultAreaModel GetDefaultAreaById(int id)
        {
            var context = ContextFactory();

            var area = context.DefaultArea.Where(o => o.Id == id).FirstOrDefault();

            var areaModel = new DefaultAreaModel() { Id = area.Id, Title = area.Title };

            return areaModel;
        }

        public async Task<bool> PostDefaultArea(DefaultAreaModel model)
        {
            var context = ContextFactory();
            if (model.Id != 0)
            {
                // update
                var areaTitleExists = context.DefaultArea.Any(
                    o => o.Title == model.Title && o.Id != model.Id
                );
                if (areaTitleExists)
                {
                    // area with same title already exists
                    return false;
                }

                var area = context.DefaultArea.FirstOrDefault(o => o.Id == model.Id);
                area.Title = model.Title;
                context.DefaultArea.Update(area);
            }
            else
            {
                // create
                var areaTitleExists = context.DefaultArea.Any(o => o.Title == model.Title);
                if (areaTitleExists)
                {
                    // area with same title already exists
                    return false;
                }
                var defaultArea = new DefaultArea();
                defaultArea.Title = model.Title;
                context.DefaultArea.Add(defaultArea);
            }

            await context.SaveChangesAsync();

            return true;
        }

        public async Task DeleteDefaultArea(int id)
        {
            var context = ContextFactory();

            var area = context.DefaultArea.Where(o => o.Id == id).FirstOrDefault();

            context.DefaultArea.Remove(area);
            await context.SaveChangesAsync();
        }

        public async Task PutDefaultArea(DefaultAreaModel model)
        {
            var context = ContextFactory();

            var area = context.DefaultArea.Where(o => o.Id == model.Id).FirstOrDefault();
            var existWithSameTitle = context.DefaultArea
                .Where(o => o.Id != model.Id && o.Title == model.Title)
                .FirstOrDefault();
            if (existWithSameTitle != null)
                throw new Exception("exist");
            area.Title = model.Title;
            area.Id = model.Id;

            await context.SaveChangesAsync();
        }

        public async Task<OfferModelMobile> GetSpecificOfferByIdForMobile(int id, string userId)
        {
            var context = ContextFactory();

            var offer = await (
                from o in context.Offer
                where o.Id == id && o.Status == OfferStatus.Approved.ToString()
                select new OfferModelMobile
                {
                    AnnouncementActive = o.AnnouncementActive,
                    CompanyName = o.Company.NameEnglish,
                    AboutCompany = o.AboutCompany,
                    CompanyWebsite = o.Company.Website,
                    CompanyPhoneNumber = o.Company.Mobile,
                    CategoryIds = o.OfferCategories.Select(x => x.CategoryId),
                    CollectionIds = o.OfferCollections.Select(x => x.CollectionId),
                    Description = o.Description,
                    DiscountFrom = o.DiscountFrom,
                    DiscountTo = o.DiscountTo,
                    Status = o.Status.ToString(),
                    BannerActive = o.BannerActive,
                    Id = o.Id,
                    Locations = o.OfferLocations
                        .Select(
                            ol =>
                                new OfferLocationModel
                                {
                                    Id = ol.Id,
                                    Address = ol.Address,
                                    Country = ol.Address,
                                    Latitude = ol.Latitude,
                                    Longitude = ol.Longitude,
                                    Vicinity = ol.Vicinity,
                                    DefaultAreaId = ol.DefaultAreaId,
                                    DefaultAreaTitle = ol.DefaultArea.Title
                                }
                        )
                        .ToList(),
                    PriceCustom = o.PriceCustom,
                    PriceFrom = o.PriceFrom,
                    PriceTo = o.PriceTo,
                    PriceList = o.PriceList,
                    PromotionCode = o.PromotionCode,
                    TagIds = o.OfferTags.Select(x => x.TagId),
                    TermsAndCondition = o.TermsAndCondition,
                    Title = o.Title,
                    Brand = o.Brand,
                    ValidFrom = o.ValidFrom,
                    ValidUntil = o.ValidUntil,
                    Rating =
                        o.OfferRating.Count > 0
                            ? o.OfferRating
                                .Where(x => x.Status == OfferCommentStatus.Public.ToString())
                                .Average(x => x.Rating)
                            : 0,
                    RatingPercent =
                        o.OfferRating.Count > 0
                            ? o.OfferRating
                                .Where(x => x.Status == OfferCommentStatus.Public.ToString())
                                .Average(x => x.Rating) * 20
                            : 0,
                    Votes =
                        o.OfferRating.Count > 0
                            ? o.OfferRating
                                .Where(x => x.Status == OfferCommentStatus.Public.ToString())
                                .Count()
                            : 0,
                    WhatYouGet = o.WhatYouGet,
                    Images = o.OfferDocuments
                        .Where(od => od.Type != OfferDocumentType.Document)
                        .Select(od => od.DocumentId.ToString())
                        .ToList(),
                    Attachments = o.OfferDocuments
                        .Where(od => od.Type == OfferDocumentType.Document)
                        .Select(
                            od =>
                                new AttachmentModel
                                {
                                    Id = !string.IsNullOrEmpty(od.DocumentId.ToString())
                                        ? od.DocumentId.ToString()
                                        : string.Empty
                                }
                        )
                        .ToList(),
                    CreatedBy = o.CreatedBy,
                    CreatedOn = o.CreatedOn,
                    UpdatedBy = o.UpdatedBy,
                    UpdatedOn = o.UpdatedOn,
                    ReviewedBy = o.ReviewedBy,
                    ReviewedOn = o.ReviewedOn,
                    DecisionBy = o.DecisionBy,
                    DecisionOn = o.DecisionOn,
                    OriginalPrice = o.Discount == 0 ? null : o.OriginalPrice,
                    Discount = o.Discount == 0 ? null : o.Discount,
                    DiscountedPrice = o.DiscountedPrice,
                    IsFavourite = o.UserFavouritesOffers
                        .Where(ufo => ufo.OfferId == o.Id && ufo.ApplicationUserId == userId)
                        .FirstOrDefault()
                        .IsFavourite,
                    BannerUrl = o.BannerUrl,
                    phoneNumber = o.InternationalNumber
                }
            ).FirstOrDefaultAsync();

            return offer;
        }

        private readonly Expression<Func<DefaultArea, DefaultAreaModel>> projectToDefaultAreaModel =
            data =>
                new DefaultAreaModel()
                {
                    Id = data.Id,
                    Title = data.Title,
                    TitleArabic = data.TitleArabic
                };

        private string HandleStatusChange(List<Roles> roles, OfferModel model, Offer offer)
        {
            if (roles.Contains(Roles.Admin) || roles.Contains(Roles.AdnocCoordinator))
            {
                if (model.Id == 0)
                {
                    return model.Status;
                }
                else if (model.Status == OfferStatus.Draft.ToString())
                {
                    return OfferStatus.Approved.ToString();
                }
                else
                {
                    return (model.Status == OfferStatus.Approved.ToString())
                        ? OfferStatus.Draft.ToString()
                        : model.Status;
                }
            }
            else if (roles.Contains(Roles.SupplierAdmin) || roles.Contains(Roles.Supplier))
            {
                if (model.Id == 0)
                    return (
                        model.Status == OfferStatus.Draft.ToString()
                        || model.Status == OfferStatus.Review.ToString()
                    )
                        ? model.Status
                        : OfferStatus.Draft.ToString();

                if (model.Id != 0)
                    return (
                        offer.Status == OfferStatus.Draft.ToString()
                        && (
                            model.Status == OfferStatus.Draft.ToString()
                            || model.Status == OfferStatus.Review.ToString()
                        )
                    )
                        ? model.Status
                        : OfferStatus.Draft.ToString();
            }
            else if (roles.Contains(Roles.Reviewer))
            {
                return (
                    offer.Status == OfferStatus.Review.ToString()
                    && (
                        model.Status == OfferStatus.Draft.ToString()
                        || model.Status == OfferStatus.Approved.ToString()
                        || model.Status == OfferStatus.Review.ToString()
                    )
                )
                    ? model.Status
                    : OfferStatus.Review.ToString();
            }

            return OfferStatus.Draft.ToString();
        }

        public async Task<int> GetAssignedOffersCountForAdmin()
        {
            var context = ContextFactory();

            return await context.Offer
                .Where(oa => oa.Status == OfferStatus.PendingApproval.ToString())
                .CountAsync();
        }

        public async Task<int> GetAssignedOffersCountForReviewer()
        {
            var context = ContextFactory();

            return await context.Offer
                .Where(oa => oa.Status == OfferStatus.Review.ToString())
                .CountAsync();
        }

        public async Task<int> GetAllOffersCount(string userId, List<Roles> roles)
        {
            var context = ContextFactory();

            var count = 0;

            if (roles.Contains(Roles.SupplierAdmin) || roles.Contains(Roles.Supplier))
            {
                var userCompanyId = context.Company
                    .Where(x => x.CompanySuppliers.Any(cs => cs.SupplierId == userId))
                    .FirstOrDefault()
                    .Id;
                count = await context.Offer.Where(o => o.CompanyId == userCompanyId).CountAsync();
            }
            else if (roles.Contains(Roles.Reviewer))
            {
                count = await context.Offer
                    .Where(
                        o =>
                            o.Status != OfferStatus.Draft.ToString()
                            && o.Status != OfferStatus.Review.ToString()
                    )
                    .CountAsync();
            }
            else if (roles.Contains(Roles.AdnocCoordinator) || roles.Contains(Roles.Admin))
            {
                count = await context.Offer
                    .Where(
                        o =>
                            o.Status != OfferStatus.Draft.ToString()
                            && o.Status != OfferStatus.Review.ToString()
                            && o.Status != OfferStatus.PendingApproval.ToString()
                    )
                    .CountAsync();
            }
            else if (roles.Contains(Roles.Buyer))
            {
                count = await context.Offer
                    .Where(o => o.Status == OfferStatus.Approved.ToString())
                    .CountAsync();
            }

            return count;
        }

        public async Task<int> GetMyOffersCount(string userId, List<Roles> roles)
        {
            var context = ContextFactory();
            var count = 0;

            if (roles.Contains(Roles.Supplier) || roles.Contains(Roles.SupplierAdmin))
            {
                //In my tab Supplier or Supplier Admin sees only offers he created, regardless of status
                count = await context.Offer.Where(o => o.CreatedBy == userId).CountAsync();
            }
            else if (roles.Contains(Roles.Reviewer))
            {
                count = await context.Offer.Where(o => o.ReviewedBy == userId).CountAsync();
            }
            else if (roles.Contains(Roles.AdnocCoordinator) || roles.Contains(Roles.Admin))
            {
                count = await context.Offer.Where(o => o.DecisionBy == userId).CountAsync();
            }

            return count;
        }

        public async Task<int> CheckIfThereIsNewOffersToSendPushNotification()
        {
            var context = ContextFactory();

            return await context.Offer.CountAsync(
                o => o.CreatedOn > DateTime.UtcNow.AddDays(-7) && o.ValidFrom < DateTime.UtcNow
            );
        }

        public IEnumerable<OfferSupplierCategoryModel> GetOffersThatHaveEmptyAboutCompany()
        {
            var context = ContextFactory();
            return (
                from o in context.Offer
                join cmp in context.Company on o.CompanyId equals cmp.Id
                join oc in context.OfferCategory on o.Id equals oc.OfferId
                join c in context.Category on oc.CategoryId equals c.Id
                where o.AboutCompany == null || o.AboutCompany == string.Empty
                select new OfferSupplierCategoryModel
                {
                    Id = o.Id,
                    Title = o.Title,
                    AboutCompany = o.AboutCompany,
                    CompanyNameEnglish = cmp.NameEnglish,
                    CompanyNameArabic = cmp.NameArabic,
                    Categories = o.OfferCategories.Select(c => c.Category.Title)
                }
            );
        }

        public async Task ReturnToPending(int offerId)
        {
            var context = ContextFactory();

            var offer = await context.Offer.FirstOrDefaultAsync(x => x.Id == offerId);
            if (offer != null)
            {
                offer.Status = Declares.OfferStatus.PendingApproval.ToString();
                context.Offer.Update(offer);
                await context.SaveChangesAsync();
            }
        }

        public IEnumerable<OfferModel> GetCompanyOffers(int companyId)
        {
            var context = ContextFactory();

            return context.Offer
                .Where(o => o.CompanyId == companyId)
                .Select(projectToOfferCardModel)
                .AsEnumerable();
        }

        public async Task TransferOffers(TransferOffersModel transferOffersModel)
        {
            var context = ContextFactory();

            var offers = context.Offer.Where(
                o =>
                    transferOffersModel.Offers.Select(offer => offer.Id).Contains(o.Id)
                    && o.CompanyId != transferOffersModel.CompanyId
            );

            foreach (var offer in offers)
            {
                offer.CompanyId = transferOffersModel.CompanyId;
            }

            context.SaveChanges();
        }

        public async Task<OfferModel> DeltaMigration(
            OfferModel model,
            IVisitor<IChangeable> auditVisitor,
            string userId,
            List<Roles> roles
        )
        {
            var context = ContextFactory();

            var offer = context.Offer
                .Include(o => o.OfferCategories)
                .Include(o => o.OfferCollections)
                .Include(o => o.OfferRating)
                .Include(o => o.OfferTags)
                .Include(o => o.OfferLocations)
                .ThenInclude(ol => ol.DefaultArea)
                .Include(o => o.OfferRating)
                .Include(o => o.UserFavouritesOffers)
                .Include(o => o.OfferDocuments)
                .Include(o => o.Company)
                .ThenInclude(c => c.Logo)
                .Include(o => o.Comments)
                .FirstOrDefault(x => x.Id == model.Id);

            // Temporary solution for offer locations
            var offerLocations = await context.OfferLocation
                .Where(ol => ol.OfferId == model.Id)
                .ToListAsync();

            if (offer == null)
                offer = new Offer();

            var offerDocuments = new List<OfferDocumentModel>();

            if (model.Images != null && model.Images.Count > 0)
            {
                foreach (var imageModel in model.Images)
                {
                    offerDocuments.Add(
                        new OfferDocumentModel
                        {
                            DocumentId = new Guid(imageModel.Id),
                            OfferId = model.Id,
                            Type = imageModel.Type,
                            OriginalImageId = imageModel.OriginalImageId,
                            X1 = imageModel.CropCoordinates.X1,
                            X2 = imageModel.CropCoordinates.X2,
                            Y1 = imageModel.CropCoordinates.Y1,
                            Y2 = imageModel.CropCoordinates.Y2,
                            cropX1 = imageModel.CropNGXCoordinates.X1,
                            cropX2 = imageModel.CropNGXCoordinates.X2,
                            cropY1 = imageModel.CropNGXCoordinates.Y1,
                            cropY2 = imageModel.CropNGXCoordinates.Y2,
                            Cover = imageModel.Cover
                        }
                    );

                    //If Reviewer didn't change image just add already exisitng thumbnail and large image
                    if (model.Id != 0)
                    {
                        var existingImagesForThisOffer = _offerDocumentRepository
                            .GetOfferImages(new Guid(imageModel.Id))
                            .ToList();
                        foreach (var existingImage in existingImagesForThisOffer)
                        {
                            if (
                                existingImage.Type == OfferDocumentType.Large
                                || existingImage.Type == OfferDocumentType.Thumbnail
                            )
                            {
                                offerDocuments.Add(existingImage);
                            }
                        }
                    }
                }
            }

            if (model.Attachments != null && model.Attachments.Count > 0)
            {
                foreach (var attachmentModel in model.Attachments)
                {
                    offerDocuments.Add(
                        new OfferDocumentModel
                        {
                            DocumentId = new Guid(attachmentModel.Id),
                            OfferId = model.Id,
                            Type = OfferDocumentType.Document
                        }
                    );
                }
            }
            model.OfferDocuments = offerDocuments;

            if (model.Categories == null)
            {
                model.Categories = new List<CategoryModel>();
            }

            if (model.Collections == null)
            {
                model.Collections = new List<CollectionModel>();
            }

            if (model.Tags == null)
            {
                model.Tags = new List<TagModel>();
            }

            if (model.Locations == null)
            {
                model.Locations = new List<OfferLocationModel>();
            }
            if (model.Comments == null)
            {
                model.Comments = new List<CommentModel>();
            }
            if (model.OfferRating == null)
            {
                model.OfferRating = new List<OfferRatingModel>();
            }

            PopulateEntityModel(offer, model, userId);

            offer.Status = HandleStatusChange(roles, model, offer);
            // Migrated offers until we manually fix offers
            offer.Status = Declares.OfferStatus.Migrated.ToString();

            foreach (var offerDocument in offer.OfferDocuments)
            {
                offerDocument.Accept(auditVisitor);
                var document = await context.Document.FirstOrDefaultAsync(
                    d => d.Id == offerDocument.DocumentId
                );
                if (document == null)
                {
                    context.Document.Add(
                        new Document()
                        {
                            Id = offerDocument.DocumentId,
                            CreatedOn = DateTime.UtcNow,
                            UpdatedOn = DateTime.UtcNow,
                        }
                    );
                }
            }

            if (model.Id == 0)
            {
                offer.OfferRating = new List<OfferRating>();
                offer.UserFavouritesOffers = new List<UserFavouritesOffer>();

                offer.Accept(auditVisitor);
                context.Add(offer);
            }
            else
            {
                // Temporary solution for offer locations
                context.OfferLocation.RemoveRange(offerLocations);
                offer.UpdatedOn = DateTime.UtcNow;
                context.Update(offer);
            }

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                throw e;
            }

            return new OfferModel() { Id = offer.Id };
        }

        public async Task<OffersAboutToExpireModel> OffersAboutToExpire()
        {
            var context = ContextFactory();

            var offersAboutToExpireModel = new OffersAboutToExpireModel
            {
                ThreeWeeksBeforeExpiration = await context.Offer
                    .Where(o => o.ValidUntil.Value.Date == DateTime.UtcNow.AddDays(21).Date)
                    .Select(o => new OfferModel { Id = o.Id, CompanyId = o.CompanyId })
                    .ToListAsync(),
                WeekBeforeExpiration = await context.Offer
                    .Where(o => o.ValidUntil.Value.Date == DateTime.UtcNow.AddDays(7).Date)
                    .Select(o => new OfferModel { Id = o.Id, CompanyId = o.CompanyId })
                    .ToListAsync(),
                DayBeforeExpiration = await context.Offer
                    .Where(o => o.ValidUntil.Value.Date == DateTime.UtcNow.AddDays(1).Date)
                    .Select(o => new OfferModel { Id = o.Id, CompanyId = o.CompanyId })
                    .ToListAsync()
            };

            return offersAboutToExpireModel;
        }

        public async Task<PaginationListModel<OfferModel>> GetOffersForMembership(
            QueryModel queryModel,
            int membershipType
        )
        {
            var context = ContextFactory();
            var membershipOffer = context.Offer.Where(
                x => x.Status == OfferStatus.Approved.ToString()
            );
            //membership filter
            var filteredOffers = Filter(membershipOffer, queryModel);
            var offerModels = filteredOffers.Select(projectToOfferCardModel);

            var sortedOffers = Sort(queryModel.Sort, offerModels);
            return await sortedOffers.ToPagedListAsync(
                queryModel.PaginationParameters.PageNumber,
                queryModel.PaginationParameters.PageSize
            );
        }

        public IEnumerable<OfferModel> GetReportedOffers(QueryModel queryModel)
        {
            var context = ContextFactory();
            var retVal = context.Offer.Where(x => x.ReportCount > 0);

            retVal = Filter(retVal, queryModel);
            var retValModel = retVal.Select(projectToOfferCardModel);

            retValModel = Sort(queryModel.Sort, retValModel);
            return retValModel;
        }

        public async Task<IEnumerable<OfferModelMobile>> GetOfferSpecificData(string userId)
        {
            var context = ContextFactory();
            var specificData = await (
                from o in context.Offer
                where
                    (
                        !o.ValidUntil.HasValue
                        || (o.ValidUntil.HasValue && o.ValidUntil.Value >= DateTime.UtcNow)
                    )
                    && (!o.ValidFrom.HasValue || o.ValidFrom.Value <= DateTime.UtcNow)
                    && o.UserFavouritesOffers.Any(ufo => ufo.ApplicationUserId == userId)
                    && o.Status == OfferStatus.Approved.ToString()
                select new OfferModelMobile
                {
                    Id = o.Id,
                    IsFavourite = o.UserFavouritesOffers
                        .Where(ufo => ufo.OfferId == o.Id && ufo.ApplicationUserId == userId)
                        .FirstOrDefault()
                        .IsFavourite,
                    IsRated = o.OfferRating.Any(
                        x => x.OfferId == o.Id && x.ApplicationUserId == userId
                    ),
                }
            ).ToListAsync();

            return specificData;
        }

        public async Task RenewOffer(int offerId)
        {
            var context = ContextFactory();
            var offer = context.Offer.Where(x => x.Id == offerId).FirstOrDefault();
            if (offer != null)
            {
                offer.Status = OfferStatus.Draft.ToString();
                context.Offer.Update(offer);
                await context.SaveChangesAsync();
            }
        }

        private void SetSpecialAnnoucment(Offer offer)
        {
            if (offer.Status == OfferStatus.Draft.ToString())
            {
                offer.AnnouncementActive = false;
                offer.SpecialAnnouncement = null;
            }
        }
    }
}
