using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MMA.WebApi.DataAccess.Helpers;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.Offers;
using MMA.WebApi.Shared.Models.Attachment;
using MMA.WebApi.Shared.Models.DefaultLocations;
using MMA.WebApi.Shared.Models.Document;
using MMA.WebApi.Shared.Models.Image;
using MMA.WebApi.Shared.Models.Roadshow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Declares;
using static MMA.WebApi.Shared.Enums.Document.DocumentDeclares;

namespace MMA.WebApi.DataAccess.Repository.Offers
{
    public class RoadshowLocationRepository
        : BaseRepository<RoadshowLocationModel>,
            IRoadshowLocationRepository
    {
        private readonly IConfiguration _configuration;

        public RoadshowLocationRepository(
            Func<MMADbContext> contexFactory,
            IConfiguration configuration
        )
            : base(contexFactory)
        {
            _configuration = configuration;
        }

        public DefaultLocation GetDefaultLocationByTitle(string title)
        {
            var context = ContextFactory();

            return context.DefaultLocation.AsNoTracking().FirstOrDefault(x => x.Title == title);
        }

        public IQueryable<RoadshowLocationModel> Get()
        {
            var context = ContextFactory();

            var roadshowLocation = (
                from l in context.DefaultLocation
                select new RoadshowLocationModel
                {
                    DefaultLocationId = l.Id,
                    Address = l.Address,
                    Country = l.Country,
                    Vicinity = l.Vicinity,
                    Title = l.Title,
                    RoadshowOffers = new List<RoadshowOfferCardModel>()
                }
            ).ToList();

            //roadshowLocation.ForEach(rl =>
            //{
            //    var roadshows = (from r in context.Roadshow join rloc in context.RoadshowLocation on r.Id equals rloc.RoadshowId join dl in context.DefaultLocation on rloc.DefaultLocationId equals dl.Id where rloc.DefaultLocationId == rl.DefaultLocationId select r).ToList();

            //    roadshows.ForEach(roadshow =>
            //    {
            //        var r = roadshowToRoadshowCardModel.Compile().Invoke(roadshow);
            //        rl.Roadshows.Add(r);
            //    });
            //});

            return roadshowLocation.AsQueryable();
        }

        protected override IQueryable<RoadshowLocationModel> GetEntities()
        {
            var context = ContextFactory();

            var roadshowLocation = (
                from l in context.DefaultLocation
                select new RoadshowLocationModel
                {
                    DefaultLocationId = l.Id,
                    Address = l.Address,
                    Country = l.Country,
                    Vicinity = l.Vicinity,
                    Title = l.Title,
                    RoadshowOffers = new List<RoadshowOfferCardModel>()
                }
            ).ToList();

            //roadshowLocation.ForEach(rl =>
            //{
            //    var roadshows = (from r in context.Roadshow join rloc in context.RoadshowLocation on r.Id equals rloc.RoadshowId join dl in context.DefaultLocation on rloc.DefaultLocationId equals dl.Id where rloc.DefaultLocationId == rl.DefaultLocationId select r).ToList();

            //    roadshows.ForEach(roadshow =>
            //    {
            //        var r = roadshowToRoadshowCardModel.Compile().Invoke(roadshow);
            //        rl.Roadshows.Add(r);
            //    });
            //});

            return roadshowLocation.AsQueryable();
        }

        public IQueryable<RoadshowLocationModel> GetAllRoadshowOffersForAllLocations()
        {
            var context = ContextFactory();

            var roadshowLocation = (
                from l in context.DefaultLocation
                select new RoadshowLocationModel
                {
                    DefaultLocationId = l.Id,
                    Address = l.Address,
                    Country = l.Country,
                    Vicinity = l.Vicinity,
                    Title = l.Title,
                    RoadshowOffers = new List<RoadshowOfferCardModel>()
                }
            ).ToList();

            var locations = new List<RoadshowLocationModel>();

            roadshowLocation.ForEach(rl =>
            {
                var roadshowOffers = (
                    from ro in context.RoadshowOffer
                    join reo in context.RoadshowEventOffer on ro.Id equals reo.RoadshowOfferId
                    join re in context.RoadshowEvent on reo.RoadshowEventId equals re.Id
                    //join rloc in context.RoadshowLocation on re.RoadshowLocationId equals rloc.Id
                    join dl in context.DefaultLocation on re.DefaultLocationId equals dl.Id
                    join ri in context.RoadshowInvite on re.RoadshowInviteId equals ri.Id
                    join r in context.Roadshow on ri.RoadshowId equals r.Id
                    where re.DefaultLocationId == rl.DefaultLocationId
                    select new RoadshowOfferCardModel
                    {
                        Id = ro.Id,
                        RoadshowProposalId = ro.RoadshowProposalId,
                        MainImage = ro.OfferDocuments
                            .Where(od => od.Type == OfferDocumentType.Thumbnail && od.Cover)
                            .Select(d => d.DocumentId.ToString())
                            .FirstOrDefault(),
                        Tag = ro.RoadshowOfferTags.Select(t => t.Tag.Title).FirstOrDefault(),
                        Category = ro.RoadshowOfferCategories
                            .Select(c => c.Category.Title)
                            .FirstOrDefault(),
                        Title = ro.Title,
                        Description = ro.Description,
                        //Status = ro.Status,
                        RoadshowId = r.Id,
                        RoadshowTitle = r.Title,
                        DateFrom = re.DateFrom,
                        DateTo = re.DateTo,
                    }
                ).Take(4);

                rl.RoadshowOffers.AddRange(roadshowOffers);

                rl.OffersCount = (
                    from ro in context.RoadshowOffer
                    join reo in context.RoadshowEventOffer on ro.Id equals reo.RoadshowOfferId
                    join re in context.RoadshowEvent on reo.RoadshowEventId equals re.Id
                    //join rloc in context.RoadshowLocation on re.RoadshowLocationId equals rloc.Id
                    join dl in context.DefaultLocation on re.DefaultLocationId equals dl.Id
                    join ri in context.RoadshowInvite on re.RoadshowInviteId equals ri.Id
                    join r in context.Roadshow on ri.RoadshowId equals r.Id
                    where re.DefaultLocationId == rl.DefaultLocationId
                    select ro
                ).Count();

                if (rl.RoadshowOffers.Count > 0)
                {
                    locations.Add(rl);
                }
            });

            return locations.AsQueryable();
        }

        public IQueryable<RoadshowLocationModel> GetRoadshowOffersForSpecificDates(
            QueryModel queryModel,
            string userId
        )
        {
            var context = ContextFactory();

            var roadshowLocation = (
                from l in context.DefaultLocation
                select new RoadshowLocationModel
                {
                    DefaultLocationId = l.Id,
                    Address = l.Address,
                    Country = l.Country,
                    Vicinity = l.Vicinity,
                    Title = l.Title,
                    RoadshowOffers = new List<RoadshowOfferCardModel>()
                }
            ).ToList();

            var locations = new List<RoadshowLocationModel>();

            roadshowLocation.ForEach(rl =>
            {
                var roadshowOffers = (
                    from ro in context.Roadshow
                    where
                        ro.Locations.Any(x => x.DefaultLocationId == rl.DefaultLocationId)
                        && ro.Status == RoadshowStatus.Published
                        && (
                            (
                                ro.DateFrom < queryModel.Filter.DateTo
                                && ro.DateTo > queryModel.Filter.DateFrom
                            )
                            || (
                                ro.DateFrom < queryModel.Filter.DateFrom
                                && ro.DateTo > queryModel.Filter.DateTo
                            )
                        )
                    select new RoadshowOfferCardModel
                    {
                        Id = ro.Id,
                        MainImage = ro.Documents
                            .Where(od => od.Cover)
                            .Select(d => d.DocumentId.ToString())
                            .FirstOrDefault(),
                        Title = ro.Title,
                        Description = ro.Description,
                        Status = ro.Status,
                        RoadshowId = ro.Id,
                        RoadshowTitle = ro.Title,
                        DateFrom = (DateTime)ro.DateFrom,
                        DateTo = (DateTime)ro.DateTo,
                    }
                ).Take(4);

                rl.RoadshowOffers.AddRange(roadshowOffers);

                locations.Add(rl);
            });

            return Filter(locations.AsQueryable(), queryModel);
        }

        public IQueryable<RoadshowLocationModel> GetAllRoadshowOffersForAllLocationsMobile(
            DateTime lastUpdatedOn,
            string userId
        )
        {
            var context = ContextFactory();

            var roadshowLocation = (
                from l in context.DefaultLocation
                select new RoadshowLocationModel
                {
                    DefaultLocationId = l.Id,
                    Address = l.Address,
                    Country = l.Country,
                    Vicinity = l.Vicinity,
                    Title = l.Title,
                    RoadshowOffers = new List<RoadshowOfferCardModel>()
                }
            ).ToList();

            var locations = new List<RoadshowLocationModel>();

            roadshowLocation.ForEach(rl =>
            {
                var roadshowOffers = (
                    from ro in context.RoadshowOffer
                    join reo in context.RoadshowEventOffer on ro.Id equals reo.RoadshowOfferId
                    join re in context.RoadshowEvent on reo.RoadshowEventId equals re.Id
                    //join rloc in context.RoadshowLocation on re.RoadshowLocationId equals rloc.Id
                    join dl in context.DefaultLocation on re.DefaultLocationId equals dl.Id
                    join ri in context.RoadshowInvite on re.RoadshowInviteId equals ri.Id
                    join r in context.Roadshow on ri.RoadshowId equals r.Id
                    where
                        re.DefaultLocationId == rl.DefaultLocationId
                        && re.DateTo >= DateTime.UtcNow
                        && (
                            ro.UpdatedOn >= lastUpdatedOn
                            || ro.UserFavouritesRoadshowOffers.Any(
                                ufo =>
                                    ufo.ApplicationUserId == userId && ufo.UpdatedOn > lastUpdatedOn
                            )
                        )
                    select new RoadshowOfferMobileModel
                    {
                        Id = ro.Id,
                        CompanyId =
                            ro.RoadshowProposal.Company != null
                                ? ro.RoadshowProposal.Company.Id
                                : 0,
                        CompanyAddress =
                            ro.RoadshowProposal.Company != null
                                ? ro.RoadshowProposal.Company.CompanyLocations
                                    .Select(c => c.Address)
                                    .FirstOrDefault()
                                : String.Empty,
                        CompanyWebsite =
                            ro.RoadshowProposal.Company != null
                                ? ro.RoadshowProposal.Company.Website
                                : String.Empty,
                        CompanyNameArabic =
                            ro.RoadshowProposal.Company != null
                                ? ro.RoadshowProposal.Company.NameArabic
                                : String.Empty,
                        CompanyNameEnglish =
                            ro.RoadshowProposal.Company != null
                                ? ro.RoadshowProposal.Company.NameEnglish
                                : String.Empty,
                        CompanyLogo =
                            ro.RoadshowProposal.Company != null
                                ? ro.RoadshowProposal.Company.Logo.DocumentId.ToString()
                                : String.Empty,
                        CompanyPOBox =
                            ro.RoadshowProposal.Company != null
                                ? ro.RoadshowProposal.Company.POBox
                                : String.Empty,
                        CompanyPhoneNumber =
                            ro.RoadshowProposal.Company != null
                                ? ro.RoadshowProposal.Company.Mobile
                                : String.Empty,
                        Title = ro.Title,
                        RoadshowProposalId = ro.RoadshowProposalId,
                        Description = ro.Description,
                        RoadshowDetails = ro.RoadshowDetails,
                        EquipmentItem = ro.EquipmentItem,
                        PromotionCode = ro.PromotionCode,
                        //Status = ro.Status,
                        CreatedBy = ro.CreatedBy,
                        CreatedOn = ro.CreatedOn,
                        UpdatedBy = ro.UpdatedBy,
                        UpdatedOn = ro.UpdatedOn,
                        RoadshowTitle = r.Title,
                        RoadshowEventDateFrom = re.DateFrom,
                        RoadshowEventDateTo = re.DateTo,
                        MainImage = ro.OfferDocuments
                            .Where(od => od.Type == OfferDocumentType.Thumbnail && od.Cover)
                            .Select(od => od.DocumentId.ToString())
                            .FirstOrDefault(),
                        Tag = ro.RoadshowOfferTags
                            .Select(ot => ot.Tag != null ? ot.Tag.Title : String.Empty)
                            .FirstOrDefault(),
                        RoadshowOfferCategories = ro.RoadshowOfferCategories
                            .Select(
                                oc =>
                                    new RoadshowOfferCategoryModel
                                    {
                                        Id = oc.CategoryId,
                                        Title =
                                            oc.Category != null ? oc.Category.Title : String.Empty
                                    }
                            )
                            .ToList(),
                        RoadshowOfferCollections = ro.RoadshowOfferCollections
                            .Select(
                                oc =>
                                    new RoadshowOfferCollectionModel
                                    {
                                        Id = oc.CollectionId,
                                        Title =
                                            oc.Collection != null
                                                ? oc.Collection.Title
                                                : String.Empty
                                    }
                            )
                            .ToList(),
                        RoadshowOfferTags = ro.RoadshowOfferTags
                            .Select(
                                ot =>
                                    new RoadshowOfferTagModel
                                    {
                                        Id = ot.TagId,
                                        Title = ot.Tag != null ? ot.Tag.Title : String.Empty
                                    }
                            )
                            .ToList(),
                        OfferAttachments = ro.OfferDocuments
                            .Where(od => (od.Type == OfferDocumentType.Document))
                            .Select(
                                od =>
                                    new AttachmentModel
                                    {
                                        Id = od.DocumentId.ToString(),
                                        Name =
                                            od.Document != null ? od.Document.Name : String.Empty,
                                        Type = od.Type.ToString()
                                    }
                            )
                            .ToList(),
                        Video = ro.OfferDocuments
                            .Where(od => (od.Type == OfferDocumentType.Video))
                            .Select(
                                od =>
                                    new VideoModel
                                    {
                                        Id = od.DocumentId.ToString(),
                                        Type =
                                            od.Document != null
                                                ? od.Document.MimeType
                                                : String.Empty
                                    }
                            )
                            .FirstOrDefault(),
                        RoadshowOfferRatings = ro.OfferRating
                            .Where(or => or.Status == OfferCommentStatus.Public.ToString())
                            .Select(
                                or =>
                                    new RoadshowOfferRatingModel
                                    {
                                        RoadshowOfferId = or.RoadshowOfferId,
                                        ApplicationUserId = or.ApplicationUserId,
                                        Rating = or.Rating,
                                        CommentText = or.CommentText,
                                        BuyerFirstName = or.ApplicationUser.FirstName,
                                        BuyerLastName = or.ApplicationUser.LastName,
                                        OfferTitle = or.RoadshowOffer.Title,
                                        CreatedBy = or.CreatedBy,
                                        CreatedOn = or.CreatedOn,
                                        Status = or.Status
                                    }
                            )
                            .ToList(),
                        Rating =
                            ro.OfferRating.Count > 0
                                ? ro.OfferRating
                                    .Where(x => x.Status == OfferCommentStatus.Public.ToString())
                                    .Average(x => x.Rating)
                                : 0,
                        RatingPercent =
                            ro.OfferRating.Count > 0
                                ? ro.OfferRating
                                    .Where(x => x.Status == OfferCommentStatus.Public.ToString())
                                    .Average(x => x.Rating) * 20
                                : 0,
                        RoadshowVouchers = ro.RoadshowVouchers
                            .Select(
                                ov =>
                                    new RoadshowVoucherModel
                                    {
                                        Id = ov.Id,
                                        Details = ov.Details,
                                        Quantity = ov.Quantity,
                                        Validity = ov.Validity
                                    }
                            )
                            .ToList(),
                        Images = ro.OfferDocuments
                            .Where(
                                od =>
                                    (
                                        od.Type != OfferDocumentType.Document
                                        && od.Type != OfferDocumentType.Video
                                    )
                            )
                            .Select(
                                od =>
                                    new ImageModel
                                    {
                                        Id = od.DocumentId.ToString(),
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
                        ImageUrls = ro.OfferDocuments
                            .Where(
                                od =>
                                    (
                                        od.Type != OfferDocumentType.Document
                                        && od.Type != OfferDocumentType.Video
                                    )
                            )
                            .Select(
                                od =>
                                    new ImageUrlsModel
                                    {
                                        Original = od.DocumentId.ToString(),
                                        Thumbnail = od.DocumentId.ToString(),
                                        Large = od.DocumentId.ToString(),
                                    }
                            )
                            .ToList(),
                        IsFavourite = ro.UserFavouritesRoadshowOffers
                            .Where(
                                ufo =>
                                    ufo.RoadshowOfferId == ro.Id && ufo.ApplicationUserId == userId
                            )
                            .FirstOrDefault()
                            .IsFavourite,
                        Locations = ro.RoadshowEventOffers.Select(
                            re =>
                                new DefaultLocationMobileModel()
                                {
                                    Id = re.RoadshowEvent.DefaultLocationId.Value,
                                    Longitude = Double.Parse(
                                        re.RoadshowEvent.DefaultLocation.Longitude
                                    ),
                                    Latitude = Double.Parse(
                                        re.RoadshowEvent.DefaultLocation.Latitude
                                    ),
                                    Address = re.RoadshowEvent.DefaultLocation.Address,
                                    Vicinity = re.RoadshowEvent.DefaultLocation.Vicinity,
                                    Country = re.RoadshowEvent.DefaultLocation.Country,
                                }
                        )
                    }
                );

                rl.RoadshowOffersMobile.AddRange(roadshowOffers);

                rl.OffersCount = roadshowOffers.Count();

                if (rl.RoadshowOffersMobile.Count > 0)
                {
                    locations.Add(rl);
                }
            });

            return locations.AsQueryable();
        }

        public async Task<IQueryable<int>> GetValidRoadshowOffersIds()
        {
            var context = ContextFactory();

            var roadshowLocation = (
                from l in context.DefaultLocation
                select new RoadshowLocationModel
                {
                    DefaultLocationId = l.Id,
                    Address = l.Address,
                    Country = l.Country,
                    Vicinity = l.Vicinity,
                    Title = l.Title,
                    RoadshowOffers = new List<RoadshowOfferCardModel>()
                }
            ).ToList();

            var roadshowOfferIds = new List<int>();

            roadshowLocation.ForEach(rl =>
            {
                var roadshowOffers = (
                    from ro in context.Roadshow
                    where
                        ro.Status == RoadshowStatus.Published
                        && ro.Locations.Any(x => rl.DefaultLocationId == x.DefaultLocationId)
                        && ro.DateTo >= DateTime.UtcNow
                    select ro.Id
                );

                if (roadshowOffers.Count() > 0)
                {
                    roadshowOfferIds.AddRange(roadshowOffers);
                }
            });

            return roadshowOfferIds.AsQueryable();
        }

        public IQueryable<RoadshowLocationModel> GetRoadshowOffersForSpecificDatesMobile(
            DateTime minDate,
            DateTime maxDate
        )
        {
            var context = ContextFactory();

            var roadshowLocation = (
                from l in context.DefaultLocation
                select new RoadshowLocationModel
                {
                    DefaultLocationId = l.Id,
                    Address = l.Address,
                    Country = l.Country,
                    Vicinity = l.Vicinity,
                    Title = l.Title,
                    RoadshowOffers = new List<RoadshowOfferCardModel>()
                }
            ).ToList();

            var locations = new List<RoadshowLocationModel>();

            roadshowLocation.ForEach(rl =>
            {
                var roadshowOffers = (
                    from ro in context.RoadshowOffer
                    join reo in context.RoadshowEventOffer on ro.Id equals reo.RoadshowOfferId
                    join re in context.RoadshowEvent on reo.RoadshowEventId equals re.Id
                    //join rloc in context.RoadshowLocation on re.RoadshowLocationId equals rloc.Id
                    join dl in context.DefaultLocation on re.DefaultLocationId equals dl.Id
                    join ri in context.RoadshowInvite on re.RoadshowInviteId equals ri.Id
                    join r in context.Roadshow on ri.RoadshowId equals r.Id
                    where
                        (
                            (re.DateFrom >= minDate && re.DateTo <= maxDate)
                            || (re.DateFrom <= maxDate && re.DateTo >= maxDate)
                            || (
                                re.DateTo <= maxDate
                                && re.DateFrom <= minDate
                                && re.DateFrom >= minDate
                                && re.DateTo >= minDate
                            )
                        )
                        && dl.Id == rl.DefaultLocationId
                    select new RoadshowOfferMobileModel
                    {
                        Id = ro.Id,
                        CompanyId =
                            ro.RoadshowProposal.Company != null
                                ? ro.RoadshowProposal.Company.Id
                                : 0,
                        CompanyAddress =
                            ro.RoadshowProposal.Company != null
                                ? ro.RoadshowProposal.Company.CompanyLocations
                                    .Select(c => c.Address)
                                    .FirstOrDefault()
                                : String.Empty,
                        CompanyWebsite =
                            ro.RoadshowProposal.Company != null
                                ? ro.RoadshowProposal.Company.Website
                                : String.Empty,
                        CompanyNameArabic =
                            ro.RoadshowProposal.Company != null
                                ? ro.RoadshowProposal.Company.NameArabic
                                : String.Empty,
                        CompanyNameEnglish =
                            ro.RoadshowProposal.Company != null
                                ? ro.RoadshowProposal.Company.NameEnglish
                                : String.Empty,
                        CompanyLogo =
                            ro.RoadshowProposal.Company != null
                                ? ro.RoadshowProposal.Company.Logo.DocumentId.ToString()
                                : String.Empty,
                        CompanyPOBox =
                            ro.RoadshowProposal.Company != null
                                ? ro.RoadshowProposal.Company.POBox
                                : String.Empty,
                        CompanyPhoneNumber =
                            ro.RoadshowProposal.Company != null
                                ? ro.RoadshowProposal.Company.Mobile
                                : String.Empty,
                        Title = ro.Title,
                        RoadshowProposalId = ro.RoadshowProposalId,
                        Description = ro.Description,
                        RoadshowDetails = ro.RoadshowDetails,
                        EquipmentItem = ro.EquipmentItem,
                        PromotionCode = ro.PromotionCode,
                        //Status = ro.Status,
                        CreatedBy = ro.CreatedBy,
                        CreatedOn = ro.CreatedOn,
                        UpdatedBy = ro.UpdatedBy,
                        UpdatedOn = ro.UpdatedOn,
                        RoadshowEventDateFrom = re.DateFrom,
                        RoadshowEventDateTo = re.DateTo,
                        MainImage = ro.OfferDocuments
                            .Where(od => od.Type == OfferDocumentType.Thumbnail && od.Cover)
                            .Select(od => od.DocumentId.ToString())
                            .FirstOrDefault(),
                        Tag = ro.RoadshowOfferTags
                            .Select(ot => ot.Tag != null ? ot.Tag.Title : String.Empty)
                            .FirstOrDefault(),
                        RoadshowOfferCategories = ro.RoadshowOfferCategories
                            .Select(
                                oc =>
                                    new RoadshowOfferCategoryModel
                                    {
                                        Id = oc.CategoryId,
                                        Title =
                                            oc.Category != null ? oc.Category.Title : String.Empty
                                    }
                            )
                            .ToList(),
                        RoadshowOfferCollections = ro.RoadshowOfferCollections
                            .Select(
                                oc =>
                                    new RoadshowOfferCollectionModel
                                    {
                                        Id = oc.CollectionId,
                                        Title =
                                            oc.Collection != null
                                                ? oc.Collection.Title
                                                : String.Empty
                                    }
                            )
                            .ToList(),
                        RoadshowOfferTags = ro.RoadshowOfferTags
                            .Select(
                                ot =>
                                    new RoadshowOfferTagModel
                                    {
                                        Id = ot.TagId,
                                        Title = ot.Tag != null ? ot.Tag.Title : String.Empty
                                    }
                            )
                            .ToList(),
                        OfferAttachments = ro.OfferDocuments
                            .Where(od => (od.Type == OfferDocumentType.Document))
                            .Select(
                                od =>
                                    new AttachmentModel
                                    {
                                        Id = od.DocumentId.ToString(),
                                        Name =
                                            od.Document != null ? od.Document.Name : String.Empty,
                                        Type = od.Type.ToString()
                                    }
                            )
                            .ToList(),
                        Video = ro.OfferDocuments
                            .Where(od => (od.Type == OfferDocumentType.Video))
                            .Select(
                                od =>
                                    new VideoModel
                                    {
                                        Id = od.DocumentId.ToString(),
                                        Type =
                                            od.Document != null
                                                ? od.Document.MimeType
                                                : String.Empty
                                    }
                            )
                            .FirstOrDefault(),
                        RoadshowOfferRatings = ro.OfferRating
                            .Where(or => or.Status == OfferCommentStatus.Public.ToString())
                            .Select(
                                or =>
                                    new RoadshowOfferRatingModel
                                    {
                                        RoadshowOfferId = or.RoadshowOfferId,
                                        ApplicationUserId = or.ApplicationUserId,
                                        Rating = or.Rating,
                                        CommentText = or.CommentText,
                                        BuyerFirstName = or.ApplicationUser.FirstName,
                                        BuyerLastName = or.ApplicationUser.LastName,
                                        OfferTitle = or.RoadshowOffer.Title,
                                        CreatedBy = or.CreatedBy,
                                        CreatedOn = or.CreatedOn,
                                        Status = or.Status
                                    }
                            )
                            .ToList(),
                        Rating =
                            ro.OfferRating.Count > 0
                                ? ro.OfferRating
                                    .Where(x => x.Status == OfferCommentStatus.Public.ToString())
                                    .Average(x => x.Rating)
                                : 0,
                        RatingPercent =
                            ro.OfferRating.Count > 0
                                ? ro.OfferRating
                                    .Where(x => x.Status == OfferCommentStatus.Public.ToString())
                                    .Average(x => x.Rating) * 20
                                : 0,
                        RoadshowVouchers = ro.RoadshowVouchers
                            .Select(
                                ov =>
                                    new RoadshowVoucherModel
                                    {
                                        Id = ov.Id,
                                        Details = ov.Details,
                                        Quantity = ov.Quantity,
                                        Validity = ov.Validity
                                    }
                            )
                            .ToList(),
                        Images = ro.OfferDocuments
                            .Where(
                                od =>
                                    (
                                        od.Type != OfferDocumentType.Document
                                        && od.Type != OfferDocumentType.Video
                                    )
                            )
                            .Select(
                                od =>
                                    new ImageModel
                                    {
                                        Id = od.DocumentId.ToString(),
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
                        ImageUrls = ro.OfferDocuments
                            .Where(
                                od =>
                                    (
                                        od.Type != OfferDocumentType.Document
                                        && od.Type != OfferDocumentType.Video
                                    )
                            )
                            .Select(
                                od =>
                                    new ImageUrlsModel
                                    {
                                        Original = od.DocumentId.ToString(),
                                        Thumbnail = od.DocumentId.ToString(),
                                        Large = od.DocumentId.ToString(),
                                    }
                            )
                            .ToList(),
                        Locations = ro.RoadshowEventOffers.Select(
                            re =>
                                new DefaultLocationMobileModel()
                                {
                                    Id = re.RoadshowEvent.DefaultLocationId.Value,
                                    Longitude = Double.Parse(
                                        re.RoadshowEvent.DefaultLocation.Longitude
                                    ),
                                    Latitude = Double.Parse(
                                        re.RoadshowEvent.DefaultLocation.Latitude
                                    ),
                                    Address = re.RoadshowEvent.DefaultLocation.Address,
                                    Vicinity = re.RoadshowEvent.DefaultLocation.Vicinity,
                                    Country = re.RoadshowEvent.DefaultLocation.Country,
                                }
                        )
                    }
                );

                rl.RoadshowOffersMobile.AddRange(roadshowOffers);

                rl.OffersCount = roadshowOffers.Count();

                if (rl.RoadshowOffersMobile.Count > 0)
                {
                    locations.Add(rl);
                }
            });

            return locations.AsQueryable();
        }

        private Expression<Func<RoadshowOffer, RoadshowOfferModel>> projectToRoadshowOfferModel =
            data =>
                new RoadshowOfferModel
                {
                    Id = data.Id,
                    CompanyId =
                        data.RoadshowProposal.Company != null
                            ? data.RoadshowProposal.Company.Id
                            : 0,
                    CompanyAddress =
                        data.RoadshowProposal.Company != null
                            ? data.RoadshowProposal.Company.CompanyLocations
                                .Select(c => c.Address)
                                .FirstOrDefault()
                            : String.Empty,
                    CompanyWebsite =
                        data.RoadshowProposal.Company != null
                            ? data.RoadshowProposal.Company.Website
                            : String.Empty,
                    CompanyNameArabic =
                        data.RoadshowProposal.Company != null
                            ? data.RoadshowProposal.Company.NameArabic
                            : String.Empty,
                    CompanyNameEnglish =
                        data.RoadshowProposal.Company != null
                            ? data.RoadshowProposal.Company.NameEnglish
                            : String.Empty,
                    CompanyLogo =
                        data.RoadshowProposal.Company != null
                            ? data.RoadshowProposal.Company.Logo.DocumentId.ToString()
                            : String.Empty,
                    CompanyPOBox =
                        data.RoadshowProposal.Company != null
                            ? data.RoadshowProposal.Company.POBox
                            : String.Empty,
                    CompanyPhoneNumber =
                        data.RoadshowProposal.Company != null
                            ? data.RoadshowProposal.Company.Mobile
                            : String.Empty,
                    Title = data.Title,
                    RoadshowProposalId = data.RoadshowProposalId,
                    Description = data.Description,
                    RoadshowDetails = data.RoadshowDetails,
                    EquipmentItem = data.EquipmentItem,
                    PromotionCode = data.PromotionCode,
                    //Status = data.Status,
                    CreatedBy = data.CreatedBy,
                    CreatedOn = data.CreatedOn,
                    UpdatedBy = data.UpdatedBy,
                    UpdatedOn = data.UpdatedOn,
                    MainImage = data.OfferDocuments
                        .Where(od => od.Type == OfferDocumentType.Thumbnail && od.Cover)
                        .Select(od => od.DocumentId.ToString())
                        .FirstOrDefault(),
                    Tag = data.RoadshowOfferTags
                        .Select(ot => ot.Tag != null ? ot.Tag.Title : String.Empty)
                        .FirstOrDefault(),
                    RoadshowOfferCategories = data.RoadshowOfferCategories
                        .Select(
                            oc =>
                                new RoadshowOfferCategoryModel
                                {
                                    Id = oc.CategoryId,
                                    Title = oc.Category != null ? oc.Category.Title : String.Empty
                                }
                        )
                        .ToList(),
                    RoadshowOfferCollections = data.RoadshowOfferCollections
                        .Select(
                            oc =>
                                new RoadshowOfferCollectionModel
                                {
                                    Id = oc.CollectionId,
                                    Title =
                                        oc.Collection != null ? oc.Collection.Title : String.Empty
                                }
                        )
                        .ToList(),
                    RoadshowOfferTags = data.RoadshowOfferTags
                        .Select(
                            ot =>
                                new RoadshowOfferTagModel
                                {
                                    Id = ot.TagId,
                                    Title = ot.Tag != null ? ot.Tag.Title : String.Empty
                                }
                        )
                        .ToList(),
                    OfferAttachments = data.OfferDocuments
                        .Where(od => (od.Type == OfferDocumentType.Document))
                        .Select(
                            od =>
                                new AttachmentModel
                                {
                                    Id = od.DocumentId.ToString(),
                                    Name = od.Document != null ? od.Document.Name : String.Empty,
                                    Type = od.Type.ToString()
                                }
                        )
                        .ToList(),
                    Video = data.OfferDocuments
                        .Where(od => (od.Type == OfferDocumentType.Video))
                        .Select(
                            od =>
                                new VideoModel
                                {
                                    Id = od.DocumentId.ToString(),
                                    Type = od.Document != null ? od.Document.MimeType : String.Empty
                                }
                        )
                        .FirstOrDefault(),
                    RoadshowOfferRatings = data.OfferRating
                        .Where(or => or.Status == OfferCommentStatus.Public.ToString())
                        .Select(
                            or =>
                                new RoadshowOfferRatingModel
                                {
                                    RoadshowOfferId = or.RoadshowOfferId,
                                    ApplicationUserId = or.ApplicationUserId,
                                    Rating = or.Rating,
                                    CommentText = or.CommentText,
                                    BuyerFirstName = or.ApplicationUser.FirstName,
                                    BuyerLastName = or.ApplicationUser.LastName,
                                    OfferTitle = or.RoadshowOffer.Title,
                                    CreatedBy = or.CreatedBy,
                                    CreatedOn = or.CreatedOn,
                                    Status = or.Status
                                }
                        )
                        .ToList(),
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
                    RoadshowVouchers = data.RoadshowVouchers
                        .Select(
                            ov =>
                                new RoadshowVoucherModel
                                {
                                    Id = ov.Id,
                                    Details = ov.Details,
                                    Quantity = ov.Quantity,
                                    Validity = ov.Validity
                                }
                        )
                        .ToList(),
                    Images = data.OfferDocuments
                        .Where(
                            od =>
                                (
                                    od.Type != OfferDocumentType.Document
                                    && od.Type != OfferDocumentType.Video
                                )
                        )
                        .Select(
                            od =>
                                new ImageModel
                                {
                                    Id = od.DocumentId.ToString(),
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
                    ImageUrls = data.OfferDocuments
                        .Where(
                            od =>
                                (
                                    od.Type != OfferDocumentType.Document
                                    && od.Type != OfferDocumentType.Video
                                )
                        )
                        .Select(
                            od =>
                                new ImageUrlsModel
                                {
                                    Original = od.DocumentId.ToString(),
                                    Thumbnail = od.DocumentId.ToString(),
                                    Large = od.DocumentId.ToString(),
                                }
                        )
                        .ToList(),
                };

        public IQueryable<RoadshowOfferCardModel> GetRoadshowOffersForSpecificLocation(
            int locationId,
            string userId
        )
        {
            var context = ContextFactory();

            var roadshowLocationOffers = (
                from ro in context.Roadshow
                where
                    ro.Locations.Any(x => x.DefaultLocationId == locationId)
                    && ro.Status == RoadshowStatus.Published
                select new RoadshowOfferCardModel
                {
                    Id = ro.Id,
                    MainImage = ro.Documents
                        .Where(od => od.Cover)
                        .Select(d => d.DocumentId.ToString())
                        .FirstOrDefault(),
                    Title = ro.Title,
                    Description = ro.Description,
                    Status = ro.Status,
                    RoadshowId = ro.Id,
                    RoadshowTitle = ro.Title,
                    DateFrom = (DateTime)ro.DateFrom,
                    DateTo = (DateTime)ro.DateTo,
                }
            );

            return roadshowLocationOffers;
        }

        public IQueryable<DefaultLocationModel> GetAllDefaultLocations()
        {
            var context = ContextFactory();

            return context.DefaultLocation.Select(projectToDefaultLocationModel);
        }

        public async Task<DefaultLocationModel> GetDefaultLocationById(int locationId)
        {
            var context = ContextFactory();

            return await context.DefaultLocation
                .Where(l => l.Id == locationId)
                .Select(projectToDefaultLocationModel)
                .FirstOrDefaultAsync();
        }

        public async Task<List<DefaultLocationModel>> GetDefaultLocations()
        {
            var context = ContextFactory();

            return await context.DefaultLocation
                .Select(defaultLocationToDefaultLocationModel)
                .ToListAsync();
        }

        public async Task<DefaultLocationModel> GetDefaultLocation(int id)
        {
            var context = ContextFactory();

            return await context.DefaultLocation
                .AsNoTracking()
                .Select(projectToDefaultLocationModel)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task UpdateDefaultLocations(
            List<DefaultLocationModel> defaultLocations,
            string userId
        )
        {
            var context = ContextFactory();

            var userRole = (
                from u in context.Users
                join ur in context.UserRoles on u.Id equals ur.UserId
                join r in context.Roles on ur.RoleId equals r.Id
                where u.Id == userId
                select r.Name
            ).FirstOrDefault();

            if (userRole == Roles.Admin.ToString() || userRole == Roles.AdnocCoordinator.ToString())
            {
                var currentDefaultLocations = context.DefaultLocation.ToList();

                List<DefaultLocation> updatedDefaultLocations = new List<DefaultLocation>();

                foreach (var dl in defaultLocations)
                {
                    var udl = context.DefaultLocation.Where(l => l.Id == dl.Id).FirstOrDefault();
                    if (udl != null)
                        updatedDefaultLocations.Add(udl);
                }

                var defaultLocationToBeDeleted = currentDefaultLocations
                    .Except(updatedDefaultLocations, new DefaultLocationEqualityHelper())
                    .ToList();

                foreach (var l in defaultLocationToBeDeleted)
                {
                    var roadshowLocation = await context.RoadshowLocation
                        .Where(rl => rl.DefaultLocationId == l.Id)
                        .FirstOrDefaultAsync();
                    if (roadshowLocation == null)
                    {
                        context.DefaultLocation.Remove(l);
                        await context.SaveChangesAsync();
                    }
                }

                foreach (var x in defaultLocations)
                {
                    if (x.Id != 0)
                    {
                        var updateDefaultLocation = context.DefaultLocation.FirstOrDefault(
                            dl => dl.Id == x.Id
                        );
                        updateDefaultLocation.Title = x.Title;
                        updateDefaultLocation.Address = x.Address;
                        updateDefaultLocation.Vicinity = x.Vicinity;

                        context.DefaultLocation.Update(updateDefaultLocation);
                    }
                    else
                    {
                        context.DefaultLocation.Add(
                            new DefaultLocation()
                            {
                                Address = x.Address,
                                Vicinity = x.Vicinity,
                                Country = x.Country,
                                Longitude = x.Longitude,
                                Latitude = x.Latitude,
                                Title = x.Title
                            }
                        );
                    }
                    await context.SaveChangesAsync();
                }
            }
        }

        private readonly Expression<
            Func<DefaultLocation, DefaultLocationModel>
        > projectToDefaultLocationModel = data =>
            new DefaultLocationModel() { Id = data.Id, Title = data.Title, };

        private readonly Expression<
            Func<DefaultLocation, DefaultLocationModel>
        > defaultLocationToDefaultLocationModel = data =>
            new DefaultLocationModel()
            {
                Id = data.Id,
                Title = data.Title,
                Latitude = data.Latitude,
                Longitude = data.Longitude,
                Vicinity = data.Vicinity,
                Address = data.Address,
                Country = data.Country
            };

        private static IQueryable<RoadshowLocationModel> Filter(
            IQueryable<RoadshowLocationModel> roadshowLocations,
            QueryModel queryModel
        )
        {
            var filteredRoadshows = roadshowLocations;
            if (queryModel.Filter.Locations.Any() == true)
            {
                var locations = queryModel.Filter.Locations.Select(x => x.Id);
                filteredRoadshows = roadshowLocations.Where(
                    roadshowLocation => locations.Contains(roadshowLocation.DefaultLocationId)
                );
            }

            if (queryModel.Filter.Keyword.Any())
            {
                var keyword = queryModel.Filter.Keyword;
                filteredRoadshows = filteredRoadshows.Where(
                    x =>
                        x.Address.Contains(keyword)
                        || x.Country.Contains(keyword)
                        || x.Title.Contains(keyword)
                );
            }

            return filteredRoadshows;
        }

        private static IQueryable<RoadshowModel> Sort(
            SortModel sortModel,
            IQueryable<RoadshowModel> roadshows
        )
        {
            if (sortModel.Type == "date")
            {
                if (sortModel.Order == Order.DESC)
                {
                    return roadshows.OrderByDescending(x => x.DateFrom);
                }
                else
                {
                    return roadshows.OrderBy(x => x.DateFrom);
                }
            }
            else
            {
                return roadshows.OrderByDescending(x => x.DateFrom);
            }
        }

        private static List<int> GetLocationIdsFromObject(
            List<DefaultLocationModel> defaultLocations
        )
        {
            var defaultLocationIds = new List<int>();
            defaultLocations.ForEach(x =>
            {
                defaultLocationIds.Add(x.Id);
            });

            return defaultLocationIds;
        }

        public async Task<IEnumerable<RoadshowOfferMobileModel>> GetAllRoadshowOffersMobile(
            DateTime lastUpdatedOn,
            string userId
        )
        {
            var context = ContextFactory();

            var roadshowOffers = (
                from ro in context.Roadshow
                where
                    ro.Status == RoadshowStatus.Published
                    && ro.DateTo >= DateTime.UtcNow
                    && ro.UpdatedOn >= lastUpdatedOn
                select new RoadshowOfferMobileModel
                {
                    Id = ro.Id,
                    CompanyId = ro.Company != null ? ro.Company.Id : 0,
                    CompanyAddress =
                        ro.Company != null
                            ? ro.Company.CompanyLocations.Select(c => c.Address).FirstOrDefault()
                            : String.Empty,
                    CompanyWebsite = ro.Company != null ? ro.Company.Website : String.Empty,
                    CompanyNameArabic = ro.Company != null ? ro.Company.NameArabic : String.Empty,
                    CompanyNameEnglish = ro.Company != null ? ro.Company.NameEnglish : String.Empty,
                    CompanyLogo =
                        ro.Company != null ? ro.Company.Logo.DocumentId.ToString() : String.Empty,
                    CompanyPOBox = ro.Company != null ? ro.Company.POBox : String.Empty,
                    CompanyPhoneNumber = ro.Company != null ? ro.Company.Land : String.Empty,
                    Title = ro.Title,
                    Description = ro.Description,
                    RoadshowDetails = ro.Activities,
                    Status = ro.Status,
                    CreatedBy = ro.CreatedBy,
                    CreatedOn = ro.CreatedOn,
                    UpdatedBy = ro.UpdatedBy,
                    UpdatedOn = ro.UpdatedOn,
                    RoadshowTitle = ro.Title,
                    RoadshowEventDateFrom = (DateTime)ro.DateFrom,
                    RoadshowEventDateTo = (DateTime)ro.DateTo,
                    MainImage = ro.Documents
                        .Where(od => od.Cover)
                        .Select(od => od.DocumentId.ToString())
                        .FirstOrDefault(),
                    Images = ro.Documents
                        .Where(
                            od =>
                                (
                                    od.Type != OfferDocumentType.Document
                                    && od.Type != OfferDocumentType.Video
                                )
                        )
                        .Select(
                            od =>
                                new ImageModel
                                {
                                    Id = od.DocumentId.ToString(),
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
                    ImageUrls = ro.Documents
                        .Where(
                            od =>
                                (
                                    od.Type != OfferDocumentType.Document
                                    && od.Type != OfferDocumentType.Video
                                )
                        )
                        .Select(
                            od =>
                                new ImageUrlsModel
                                {
                                    Original = od.DocumentId.ToString(),
                                    Thumbnail = od.DocumentId.ToString(),
                                    Large = od.DocumentId.ToString(),
                                }
                        )
                        .ToList(),
                    Locations = ro.Locations
                        .Select(
                            x =>
                                new DefaultLocationMobileModel
                                {
                                    Title = x.DefaultLocation.Title,
                                    Vicinity = x.DefaultLocation.Vicinity,
                                    Address = x.DefaultLocation.Address,
                                    Latitude = Double.Parse(x.DefaultLocation.Latitude),
                                    Longitude = Double.Parse(x.DefaultLocation.Longitude),
                                    Id = x.DefaultLocation.Id,
                                    Country = x.DefaultLocation.Country
                                }
                        )
                        .ToList()
                }
            ).ToList();
            List<RoadshowOfferMobileModel> r = roadshowOffers;

            return roadshowOffers;
        }

        public async Task<IEnumerable<RoadshowOfferMobileModel>> GetAllRoadshowOffersMobile()
        {
            var context = ContextFactory();

            var roadshowOffers = (
                from ro in context.Roadshow
                where ro.Status == RoadshowStatus.Published
                select new RoadshowOfferMobileModel
                {
                    Id = ro.Id,
                    CompanyId = ro.CompanyId != null ? (int)ro.CompanyId : 0,
                    CompanyAddress =
                        ro.Company != null
                            ? ro.Company.CompanyLocations.Select(c => c.Address).FirstOrDefault()
                            : String.Empty,
                    CompanyWebsite = ro.Company != null ? ro.Company.Website : String.Empty,
                    CompanyNameArabic = ro.Company != null ? ro.Company.NameArabic : String.Empty,
                    CompanyNameEnglish = ro.Company != null ? ro.Company.NameEnglish : String.Empty,
                    CompanyLogo =
                        ro.Company != null ? ro.Company.Logo.DocumentId.ToString() : String.Empty,
                    CompanyPOBox = ro.Company != null ? ro.Company.POBox : String.Empty,
                    CompanyPhoneNumber = ro.Company != null ? ro.Company.Land : String.Empty,
                    Title = ro.Title,
                    Description = ro.Description,
                    RoadshowDetails = ro.Activities,
                    Status = ro.Status,
                    CreatedBy = ro.CreatedBy,
                    CreatedOn = ro.CreatedOn,
                    UpdatedBy = ro.UpdatedBy,
                    UpdatedOn = ro.UpdatedOn,
                    RoadshowTitle = ro.Title,
                    RoadshowEventDateFrom = (DateTime)ro.DateFrom,
                    RoadshowEventDateTo = (DateTime)ro.DateTo,
                    MainImage = ro.Documents
                        .Where(od => od.Cover)
                        .Select(od => od.DocumentId.ToString())
                        .FirstOrDefault(),
                    OfferAttachments = ro.Documents
                        .Where(od => (od.Type == OfferDocumentType.Document))
                        .Select(
                            od =>
                                new AttachmentModel
                                {
                                    Id = od.DocumentId.ToString(),
                                    Name = od.Document != null ? od.Document.Name : String.Empty,
                                    Type = od.Document != null ? od.Document.MimeType : string.Empty
                                }
                        )
                        .ToList(),
                    Images = ro.Documents
                        .Where(
                            od =>
                                (
                                    od.Type != OfferDocumentType.Document
                                    && od.Type != OfferDocumentType.Video
                                )
                        )
                        .Select(
                            od =>
                                new ImageModel
                                {
                                    Id = od.DocumentId.ToString(),
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
                    ImageUrls = ro.Documents
                        .Where(
                            od =>
                                (
                                    od.Type != OfferDocumentType.Document
                                    && od.Type != OfferDocumentType.Video
                                )
                        )
                        .Select(
                            od =>
                                new ImageUrlsModel
                                {
                                    Original = od.DocumentId.ToString(),
                                    Thumbnail = od.DocumentId.ToString(),
                                    Large = od.DocumentId.ToString(),
                                }
                        )
                        .ToList(),
                    Locations = ro.Locations
                        .Select(
                            x =>
                                new DefaultLocationMobileModel
                                {
                                    Title = x.DefaultLocation.Title,
                                    Vicinity = x.DefaultLocation.Vicinity,
                                    Address = x.DefaultLocation.Address,
                                    Latitude = Double.Parse(x.DefaultLocation.Latitude),
                                    Longitude = Double.Parse(x.DefaultLocation.Longitude),
                                    Id = x.DefaultLocation.Id,
                                    Country = x.DefaultLocation.Country
                                }
                        )
                        .ToList()
                }
            ).ToList();

            return roadshowOffers;
        }
    }
}
