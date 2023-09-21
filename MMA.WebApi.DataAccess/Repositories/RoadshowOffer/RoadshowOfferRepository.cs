using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MMA.WebApi.DataAccess.Extensions;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.Domain.Document;
using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Interfaces.Offers;
using MMA.WebApi.Shared.Interfaces.RoadshowDocuments;
using MMA.WebApi.Shared.Models.Attachment;
using MMA.WebApi.Shared.Models.DefaultLocations;
using MMA.WebApi.Shared.Models.Document;
using MMA.WebApi.Shared.Models.Image;
using MMA.WebApi.Shared.Models.Ratings;
using MMA.WebApi.Shared.Models.Response;
using MMA.WebApi.Shared.Models.Roadshow;
using MMA.WebApi.Shared.Visitor;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Declares;
using static MMA.WebApi.Shared.Enums.Document.DocumentDeclares;

namespace MMA.WebApi.DataAccess.Repository.Offers
{
    public class RoadshowOfferRepository
        : BaseRepository<RoadshowOfferModel>,
            IRoadshowOfferRepository
    {
        private readonly IRoadshowDocumentRepository _roadshowDocumentRepository;
        private readonly IConfiguration _configuration;
        private readonly IDocumentService _documentService;

        public RoadshowOfferRepository(
            IRoadshowDocumentRepository roadshowDocumentRepository,
            Func<MMADbContext> contexFactory,
            IConfiguration configuration,
            IDocumentService documentService
        )
            : base(contexFactory)
        {
            _roadshowDocumentRepository = roadshowDocumentRepository;
            _configuration = configuration;
            _documentService = documentService;
        }

        public async Task<RoadshowOfferModel> CreateAsync(
            RoadshowOfferModel model,
            IVisitor<IChangeable> auditVisitor,
            string userId
        )
        {
            var context = ContextFactory();

            var roadshowOffer = context.RoadshowOffer
                .Include(ro => ro.RoadshowProposal)
                .ThenInclude(rp => rp.Company)
                .ThenInclude(c => c.Logo)
                .Include(ro => ro.RoadshowProposal)
                .ThenInclude(rp => rp.Company)
                .ThenInclude(c => c.CompanyLocations)
                .Include(ro => ro.RoadshowOfferCategories)
                .ThenInclude(roc => roc.Category)
                .Include(ro => ro.RoadshowOfferCollections)
                .ThenInclude(roc => roc.Collection)
                .Include(ro => ro.OfferDocuments)
                .ThenInclude(rod => rod.Document)
                .Include(ro => ro.OfferRating)
                .Include(ro => ro.RoadshowOfferTags)
                .Include(ro => ro.RoadshowVouchers)
                .FirstOrDefault(x => x.Id == model.Id);

            if (roadshowOffer == null)
                roadshowOffer = new RoadshowOffer();

            var roadshowOfferDocuments = new List<RoadshowOfferDocumentModel>();

            if (model.Images != null && model.Images.Count > 0)
            {
                foreach (var imageModel in model.Images)
                {
                    roadshowOfferDocuments.Add(
                        new RoadshowOfferDocumentModel
                        {
                            DocumentId = imageModel.Id,
                            RoadshowOfferId = model.Id,
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
                        var existingImagesForThisOffer = _roadshowDocumentRepository
                            .GetRoadshowOfferImages(new Guid(imageModel.Id))
                            .ToList();
                        foreach (var existingImage in existingImagesForThisOffer)
                        {
                            if (
                                existingImage.Type == OfferDocumentType.Large
                                || existingImage.Type == OfferDocumentType.Thumbnail
                            )
                            {
                                roadshowOfferDocuments.Add(existingImage);
                            }
                        }
                    }
                }
            }

            if (model.OfferAttachments != null && model.OfferAttachments.Count > 0)
            {
                foreach (var attachmentModel in model.OfferAttachments)
                {
                    roadshowOfferDocuments.Add(
                        new RoadshowOfferDocumentModel
                        {
                            DocumentId = attachmentModel.Id,
                            RoadshowOfferId = model.Id,
                            Type = OfferDocumentType.Document
                        }
                    );
                }
            }

            if (model.Video != null)
            {
                roadshowOfferDocuments.Add(
                    new RoadshowOfferDocumentModel
                    {
                        DocumentId = model.Video.Id,
                        RoadshowOfferId = model.Id,
                        Type = OfferDocumentType.Video
                    }
                );
            }

            model.OfferDocuments = roadshowOfferDocuments;
            if (model.RoadshowOfferCategories == null)
            {
                model.RoadshowOfferCategories = new List<RoadshowOfferCategoryModel>();
            }

            if (model.RoadshowOfferCollections == null)
            {
                model.RoadshowOfferCollections = new List<RoadshowOfferCollectionModel>();
            }

            if (model.RoadshowOfferTags == null)
            {
                model.RoadshowOfferTags = new List<RoadshowOfferTagModel>();
            }

            if (model.RoadshowOfferRatings == null)
            {
                model.RoadshowOfferRatings = new List<RoadshowOfferRatingModel>();
            }

            PopulateEntityModel(roadshowOffer, model);

            roadshowOffer.RoadshowProposal = context.RoadshowProposal
                .Where(p => p.Id == roadshowOffer.RoadshowProposalId)
                .FirstOrDefault();

            foreach (var roadshowOfferDocument in roadshowOffer.OfferDocuments)
            {
                roadshowOfferDocument.Accept(auditVisitor);

                if (!await context.Document.AnyAsync(d => d.Id == roadshowOfferDocument.DocumentId))
                    context.Document.Add(
                        new Document
                        {
                            Id = roadshowOfferDocument.DocumentId,
                            CreatedOn = DateTime.UtcNow,
                            UpdatedOn = DateTime.UtcNow
                        }
                    );
            }

            if (model.Id == 0)
            {
                roadshowOffer.CreatedOn = DateTime.UtcNow;
                roadshowOffer.CreatedBy = userId;
                roadshowOffer.UpdatedOn = DateTime.UtcNow;
                roadshowOffer.UpdatedBy = userId;
                roadshowOffer.Accept(auditVisitor);
                context.Add(roadshowOffer);
            }
            else
            {
                roadshowOffer.UpdatedBy = userId;
                roadshowOffer.UpdatedOn = DateTime.UtcNow;
                context.Update(roadshowOffer);
            }

            await context.SaveChangesAsync();

            return projectToRoadshowOfferModel.Compile().Invoke(roadshowOffer);
        }

        public async Task<RoadshowOfferModel> GetSpecificRoadshowOfferById(
            int id,
            string userId,
            List<Roles> roles
        )
        {
            var context = ContextFactory();
            var roadshowOffer = new RoadshowOfferModel();

            if (roles.Contains(Roles.Supplier) || roles.Contains(Roles.SupplierAdmin))
            {
                var companySupplier = context.CompanySuppliers
                    .Where(cs => cs.SupplierId == userId)
                    .FirstOrDefault();

                roadshowOffer = await context.RoadshowOffer
                    .Include(roadshowOffer => roadshowOffer.RoadshowOfferCategories)
                    .Include(roadshowOffer => roadshowOffer.RoadshowOfferCollections)
                    .Include(roadshowOffer => roadshowOffer.OfferDocuments)
                    .ThenInclude(od => od.Document)
                    .Include(roadshowOffer => roadshowOffer.RoadshowOfferTags)
                    .Include(roadshowOffer => roadshowOffer.OfferRating)
                    .Include(roadshowOffer => roadshowOffer.RoadshowProposal)
                    .ThenInclude(roadshowProposal => roadshowProposal.Company)
                    .Include(roadshowOffer => roadshowOffer.RoadshowEventOffers)
                    .ThenInclude(roadshowEventOffer => roadshowEventOffer.RoadshowEvent)
                    .ThenInclude(roadshowEvent => roadshowEvent.DefaultLocation)
                    .AsNoTracking()
                    .Where(
                        ro =>
                            ro.RoadshowProposal.CompanyId.GetValueOrDefault()
                            == companySupplier.CompanyId
                    )
                    .Select(projectToRoadshowOfferModel)
                    .FirstOrDefaultAsync(x => x.Id == id);
            }
            else if (roles.Contains(Roles.AdnocCoordinator) || roles.Contains(Roles.Admin))
            {
                roadshowOffer = await context.RoadshowOffer
                    .Include(roadshowOffer => roadshowOffer.RoadshowOfferCategories)
                    .Include(roadshowOffer => roadshowOffer.RoadshowOfferCollections)
                    .Include(roadshowOffer => roadshowOffer.OfferDocuments)
                    .ThenInclude(od => od.Document)
                    .Include(roadshowOffer => roadshowOffer.RoadshowOfferTags)
                    .Include(roadshowOffer => roadshowOffer.OfferRating)
                    .Include(roadshowOffer => roadshowOffer.RoadshowProposal)
                    .ThenInclude(roadshowProposal => roadshowProposal.Company)
                    .AsNoTracking()
                    //.Where(ro => ro.CreatedBy == userId)
                    .Select(projectToRoadshowOfferModel)
                    .FirstOrDefaultAsync(x => x.Id == id);
            }
            else if (roles.Contains(Roles.Reviewer))
            {
                roadshowOffer = await context.RoadshowOffer
                    .Include(roadshowOffer => roadshowOffer.RoadshowOfferCategories)
                    .Include(roadshowOffer => roadshowOffer.RoadshowOfferCollections)
                    .Include(roadshowOffer => roadshowOffer.OfferDocuments)
                    .ThenInclude(od => od.Document)
                    .Include(roadshowOffer => roadshowOffer.RoadshowOfferTags)
                    .Include(roadshowOffer => roadshowOffer.OfferRating)
                    .Include(roadshowOffer => roadshowOffer.RoadshowProposal)
                    .ThenInclude(roadshowProposal => roadshowProposal.Company)
                    .AsNoTracking()
                    .Where(ro => ro.Status == RoadshowOfferStatus.Review)
                    .Select(projectToRoadshowOfferModel)
                    .FirstOrDefaultAsync(x => x.Id == id);
            }
            else if (roles.Contains(Roles.Buyer))
            {
                roadshowOffer = await context.RoadshowOffer
                    .Include(roadshowOffer => roadshowOffer.RoadshowOfferCategories)
                    .Include(roadshowOffer => roadshowOffer.RoadshowOfferCollections)
                    .Include(roadshowOffer => roadshowOffer.OfferDocuments)
                    .ThenInclude(od => od.Document)
                    .Include(roadshowOffer => roadshowOffer.RoadshowOfferTags)
                    .Include(roadshowOffer => roadshowOffer.OfferRating)
                    .Include(roadshowOffer => roadshowOffer.RoadshowProposal)
                    .ThenInclude(roadshowProposal => roadshowProposal.Company)
                    .AsNoTracking()
                    .Where(ro => ro.Status == RoadshowOfferStatus.Approved)
                    .Select(projectToRoadshowOfferModel)
                    .FirstOrDefaultAsync(x => x.Id == id);
            }

            return roadshowOffer;
        }

        public IQueryable<Shared.Models.Roadshow.RoadshowOfferModel> Get()
        {
            var context = ContextFactory();

            return context.RoadshowOffer.Select(projectToRoadshowOfferModel);
        }

        protected override IQueryable<RoadshowOfferModel> GetEntities()
        {
            var context = ContextFactory();

            return context.RoadshowOffer.Select(projectToRoadshowOfferModel);
        }

        public IQueryable<RoadshowOfferModel> GetAllRoadshowOffers(
            string userId,
            List<Roles> roles,
            QueryModel queryModel
        )
        {
            var context = ContextFactory();
            IQueryable<RoadshowOffer> roadshowOffers = context.RoadshowOffer
                .Include(roadshowOffers => roadshowOffers.RoadshowProposal)
                .Include(roadshowOffers => roadshowOffers.OfferDocuments)
                .Include(roadshowOffers => roadshowOffers.RoadshowOfferTags)
                .Include(roadshowOffers => roadshowOffers.RoadshowOfferCategories)
                .AsNoTracking();

            // TODO: CHECK WHAT ROLE CAN SEE WHAT LIST OF OFFERS
            if (roles.Contains(Roles.AdnocCoordinator) || roles.Contains(Roles.Admin))
            {
                // Admin can see all roadshow proposals
                //roadshowProposals = roadshowProposals;
            }
            else if (roles.Contains(Roles.SupplierAdmin) || roles.Contains(Roles.Supplier))
            {
                roadshowOffers = (
                    from ro in roadshowOffers
                    join rp in context.RoadshowProposal on ro.RoadshowProposalId equals rp.Id
                    join cs in context.CompanySuppliers on rp.Company.Id equals cs.CompanyId
                    where cs.SupplierId == userId
                    select ro
                );
            }
            else if (roles.Contains(Roles.Reviewer) || roles.Contains(Roles.Admin))
            {
                roadshowOffers = (
                    from ro in roadshowOffers
                    where ro.Status == RoadshowOfferStatus.Review
                    select ro
                );
            }
            else if (roles.Contains(Roles.Buyer))
            {
                roadshowOffers = (
                    from ro in roadshowOffers
                    where ro.Status == RoadshowOfferStatus.Approved
                    select ro
                );
            }

            var filteredroadshowOffers = Filter(roadshowOffers, queryModel);

            var roadshowOfferModels = filteredroadshowOffers.Select(
                projectToRoadshowOfferCardModel
            );

            //return roadshowOfferModels;
            return Sort(queryModel.Sort, roadshowOfferModels);
        }

        public IQueryable<RoadshowOfferModel> GetAllRoadshowOffersForMyCompany(
            QueryModel queryModel,
            string userId
        )
        {
            var context = ContextFactory();
            var companyId = context.Company
                .Where(x => x.CompanySuppliers.Any(cs => cs.SupplierId == userId))
                .Select(c => c.Id)
                .FirstOrDefault();
            IQueryable<RoadshowOffer> roadshowInvites = context.RoadshowEventOffer
                .AsNoTracking()
                .Include(re => re.RoadshowOffer)
                .ThenInclude(roadshowOffers => roadshowOffers.RoadshowProposal)
                .Include(re => re.RoadshowOffer)
                .ThenInclude(roadshowOffers => roadshowOffers.OfferDocuments)
                .Include(re => re.RoadshowOffer)
                .ThenInclude(roadshowOffers => roadshowOffers.RoadshowOfferTags)
                .Select(re => re.RoadshowOffer)
                .Where(
                    o =>
                        o.RoadshowProposal.Status == RoadshowProposalStatus.Active
                        && o.RoadshowProposal.Company.Id == companyId
                );

            var filteredRoadshowInvites = Filter(roadshowInvites, queryModel);
            var roadshowOfferModels = filteredRoadshowInvites.Select(
                projectToRoadshowOfferCardModel
            );

            return Sort(queryModel.Sort, roadshowOfferModels);
        }

        private static IQueryable<RoadshowOffer> Filter(
            IQueryable<RoadshowOffer> roadshowOffers,
            QueryModel queryModel
        )
        {
            var filteredRoadshowOffers = roadshowOffers.Where(
                roadshowOffer =>
                    roadshowOffer.Title
                        .ToLower()
                        .Contains(queryModel.Filter.Keyword.Trim().ToLower())
                    || roadshowOffer.Description
                        .ToLower()
                        .Contains(queryModel.Filter.Keyword.Trim().ToLower())
            );

            if (queryModel.Filter.Status?.Any() == true)
            {
                var filteredStatuses = Enum.GetValues(typeof(RoadshowOfferStatus))
                    .Cast<RoadshowOfferStatus>()
                    .Where(rs => queryModel.Filter.Status.Contains(rs.ToString()))
                    .ToList();

                filteredRoadshowOffers = filteredRoadshowOffers.Where(
                    r => filteredStatuses.Contains(r.Status)
                );
            }

            if (queryModel.Filter.Tags?.Any() == true)
            {
                filteredRoadshowOffers = filteredRoadshowOffers.Where(
                    ro =>
                        ro.RoadshowOfferTags.Any(rot => queryModel.Filter.Tags.Contains(rot.TagId))
                );
            }

            if (queryModel.Filter.Categories?.Any() == true)
            {
                filteredRoadshowOffers = filteredRoadshowOffers.Where(
                    ro =>
                        ro.RoadshowOfferCategories.Any(
                            roc => queryModel.Filter.Categories.Contains(roc.CategoryId)
                        )
                );
            }

            return filteredRoadshowOffers;
        }

        private static IQueryable<RoadshowOfferModel> Sort(
            SortModel sortModel,
            IQueryable<RoadshowOfferModel> roadshowOffers
        )
        {
            if (sortModel.Type == "date")
            {
                if (sortModel.Order == Order.DESC)
                {
                    return roadshowOffers.OrderByDescending(x => x.UpdatedOn);
                }
                else
                {
                    return roadshowOffers.OrderBy(x => x.UpdatedOn);
                }
            }
            else
            {
                return roadshowOffers.OrderByDescending(x => x.UpdatedOn);
            }
        }

        /// <summary>
        /// Generates QR Code for certain offer based on his ID
        /// </summary>
        /// <param name="offerId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DocumentFileModel GenerateQRCode(int roadshowOfferId, string userId)
        {
            var qrCodeText =
                _configuration.GetValue<string>("BaseURL:Url")
                + "roadshows/offers/"
                + roadshowOfferId;
            var context = ContextFactory();

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(
                qrCodeText,
                QRCodeGenerator.ECCLevel.Q
            );
            QRCode code = new QRCode(qrCodeData);
            Bitmap qrCodeImg = code.GetGraphic(10);

            ImageConverter converter = new ImageConverter();
            byte[] imgData = (byte[])converter.ConvertTo(qrCodeImg, typeof(byte[]));
            var qrCodeID = Guid.NewGuid();

            context.Document.Add(
                new Document()
                {
                    Id = qrCodeID,
                    //TODO: Check if each user can create QR Code for offer or that can only be done with special permissions?
                    CreatedBy = userId,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedBy = userId,
                    UpdatedOn = DateTime.UtcNow,
                    MimeType = "image/png",
                    Name = "RoadshowOffer-" + roadshowOfferId + "-QRCode",
                    StoragePath = "documents/" + qrCodeID.ToString().ToLower(),
                    StorageType = "azureblobstorage",
                    Content = imgData,
                    Size = imgData.Length,
                    ParentId = null
                }
            );

            context.RoadshowOfferDocument.Add(
                new RoadshowOfferDocument()
                {
                    DocumentId = qrCodeID,
                    OriginalImageId = qrCodeID,
                    RoadshowOfferId = roadshowOfferId,
                    //TODO: Check if each user can create QR Code for offer or that can only be done with special permissions?
                    CreatedBy = userId,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedBy = userId,
                    UpdatedOn = DateTime.UtcNow,
                    Type = OfferDocumentType.QRCode,
                    X1 = 0,
                    X2 = 0,
                    Y1 = 0,
                    Y2 = 0,
                    cropX1 = 0,
                    cropX2 = 0,
                    cropY1 = 0,
                    cropY2 = 0,
                }
            );

            context.SaveChangesAsync();

            return new DocumentFileModel
            {
                Id = qrCodeID,
                MimeType = "image/png",
                Content = imgData,
                Size = imgData.Length,
            };
        }

        public async Task SetRoadshowOfferAsFavourite(
            RoadshowOfferFavoriteModel roadshowOfferFavorite,
            string userId
        )
        {
            var context = ContextFactory();
            UserFavouritesRoadshowOffer userFavouritesRoadshowOffer =
                await context.UserFavouritesRoadshowOffer.FirstOrDefaultAsync(
                    x =>
                        x.ApplicationUserId == userId
                        && x.RoadshowOfferId == roadshowOfferFavorite.RoadshowOfferId
                );

            if (userFavouritesRoadshowOffer == null)
            {
                userFavouritesRoadshowOffer = new UserFavouritesRoadshowOffer
                {
                    ApplicationUserId = userId,
                    RoadshowOfferId = roadshowOfferFavorite.RoadshowOfferId,
                    UpdatedOn = DateTime.UtcNow,
                    IsFavourite = roadshowOfferFavorite.IsFavourite
                };
                context.UserFavouritesRoadshowOffer.Add(userFavouritesRoadshowOffer);
            }
            else
            {
                userFavouritesRoadshowOffer.IsFavourite = roadshowOfferFavorite.IsFavourite;
                userFavouritesRoadshowOffer.UpdatedOn = DateTime.UtcNow;
                context.UserFavouritesRoadshowOffer.Update(userFavouritesRoadshowOffer);
            }

            await context.SaveChangesAsync();
        }

        public async Task<Guid> GetQRCodeForRoadshowOffer(int roadshowOfferId)
        {
            var context = ContextFactory();
            var qrCode = await (
                from od in context.RoadshowOfferDocument
                join d in context.Document on od.DocumentId equals d.Id
                where od.RoadshowOfferId == roadshowOfferId && od.Type == OfferDocumentType.QRCode
                select d
            ).FirstOrDefaultAsync();

            return qrCode == null ? Guid.Empty : qrCode.Id;
        }

        public async Task<DocumentFileModel> GenerateQRCodeWithLogoForRoadshowOffer(
            int roadshowOfferId,
            string userId
        )
        {
            var context = ContextFactory();
            var qrCodeText = _configuration["BaseURL:Url"] + "roadshows/offers/" + roadshowOfferId;
            string name = "RoadshowQRCodeImage" + roadshowOfferId;

            return await myQRCodeGenerator.GenerateAndWriteQRCode(
                roadshowOfferId,
                userId,
                context,
                qrCodeText,
                _documentService,
                name
            );
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
                        .Where(od => od.Type == OfferDocumentType.Original && od.Cover)
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

        private void PopulateEntityModel(RoadshowOffer data, RoadshowOfferModel model)
        {
            data.RoadshowOfferCategories = model.RoadshowOfferCategories
                .Select(
                    category =>
                        new RoadshowOfferCategory
                        {
                            RoadshowOfferId = model.Id,
                            CategoryId = category.Id
                        }
                )
                .ToList();

            data.RoadshowOfferCollections = model.RoadshowOfferCollections
                .Select(
                    collection =>
                        new RoadshowOfferCollection
                        {
                            RoadshowOfferId = model.Id,
                            CollectionId = collection.Id
                        }
                )
                .ToList();

            data.RoadshowOfferTags = model.RoadshowOfferTags
                .Select(tag => new RoadshowOfferTag { RoadshowOfferId = model.Id, TagId = tag.Id })
                .ToList();

            data.RoadshowVouchers = model.RoadshowVouchers
                .Select(
                    voucher =>
                        new RoadshowVoucher
                        {
                            RoadshowOfferId = model.Id,
                            RoadshowProposalId = model.RoadshowProposalId,
                            Quantity = voucher.Quantity,
                            Details = voucher.Details,
                            Validity = voucher.Validity
                        }
                )
                .ToList();

            data.OfferDocuments = model.OfferDocuments
                .Select(
                    od =>
                        new RoadshowOfferDocument
                        {
                            DocumentId = new Guid(od.DocumentId),
                            RoadshowOfferId = model.Id,
                            Type = od.Type.ToString() == "0" ? OfferDocumentType.Original : od.Type,
                            OriginalImageId =
                                od.OriginalImageId == Guid.Empty
                                    ? new Guid(od.DocumentId)
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

            data.Title = model.Title;
            data.RoadshowProposalId = model.RoadshowProposalId;
            data.RoadshowDetails = model.RoadshowDetails;
            data.Description = model.Description;
            data.EquipmentItem = model.EquipmentItem;
            data.PromotionCode = model.PromotionCode;
            data.Status = RoadshowOfferStatus.Approved;
        }

        public async Task<RoadshowOfferMobileModel> GetSpecificOfferByIdForMobile(
            int id,
            string userId
        )
        {
            var context = ContextFactory();

            var roadshowOffer = await (
                from ro in context.RoadshowOffer
                join reo in context.RoadshowEventOffer on ro.Id equals reo.RoadshowOfferId
                join re in context.RoadshowEvent on reo.RoadshowEventId equals re.Id
                join dl in context.DefaultLocation on re.DefaultLocationId equals dl.Id
                where ro.Id == id
                select new RoadshowOfferMobileModel
                {
                    Id = ro.Id,
                    CompanyId =
                        ro.RoadshowProposal.Company != null ? ro.RoadshowProposal.Company.Id : 0,
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
                    //RoadshowTitle = r.Title,
                    RoadshowEventDateFrom = re.DateFrom,
                    RoadshowEventDateTo = re.DateTo,
                    MainImage = ro.OfferDocuments
                        .Where(od => od.Type == OfferDocumentType.Original && od.Cover)
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
                                    Title = oc.Category != null ? oc.Category.Title : String.Empty
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
                                        oc.Collection != null ? oc.Collection.Title : String.Empty
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
                                    Name = od.Document != null ? od.Document.Name : String.Empty,
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
                                    Type = od.Document != null ? od.Document.MimeType : String.Empty
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
                            ufo => ufo.RoadshowOfferId == ro.Id && ufo.ApplicationUserId == userId
                        )
                        .FirstOrDefault()
                        .IsFavourite,
                    Locations = ro.RoadshowEventOffers.Select(
                        re =>
                            new DefaultLocationMobileModel()
                            {
                                Id = dl.Id,
                                Title = dl.Title,
                                Longitude = Double.Parse(dl.Longitude),
                                Latitude = Double.Parse(dl.Latitude),
                                Address = dl.Address,
                                Vicinity = dl.Vicinity,
                                Country = dl.Country,
                            }
                    )
                }
            ).FirstOrDefaultAsync();

            return roadshowOffer;
        }

        /// <summary>
        /// Checks if roadshow offer is rated and added to favorites
        /// </summary>
        /// <param name="roadshowOfferId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IsOfferFavoriteAndRatedModel> CheckIfOfferIsFavoriteAndRated(
            int roadshowOfferId,
            string userId
        )
        {
            var isOfferFavoriteAndRated = new IsOfferFavoriteAndRatedModel();
            var context = ContextFactory();

            // Checks if offer is already rated by this user
            var offerRating = await (
                from o in context.RoadshowOfferRating
                where o.RoadshowOfferId == roadshowOfferId && o.ApplicationUserId == userId
                select o
            ).FirstOrDefaultAsync();
            isOfferFavoriteAndRated.IsRated = offerRating != null;

            // Checks if offer is favourites for this user
            var offerFavourite = await (
                from f in context.UserFavouritesRoadshowOffer
                where f.RoadshowOfferId == roadshowOfferId && f.ApplicationUserId == userId
                select f
            ).FirstOrDefaultAsync();
            isOfferFavoriteAndRated.IsFavorite =
                offerFavourite != null && offerFavourite.IsFavourite;

            return isOfferFavoriteAndRated;
        }

        /// <summary>
        /// Get all locations for roadshow offer
        /// </summary>
        /// <param name="roadshowOfferId"></param>
        /// <returns></returns>
        public IEnumerable<RoadshowOfferLocationModel> GetRoadshowOfferLocation(int roadshowOfferId)
        {
            var context = ContextFactory();

            return (
                from ro in context.RoadshowOffer
                join reo in context.RoadshowEventOffer on ro.Id equals reo.RoadshowOfferId
                join re in context.RoadshowEvent on reo.RoadshowEventId equals re.Id
                join ri in context.RoadshowInvite on re.RoadshowInviteId equals ri.Id
                join r in context.Roadshow on ri.RoadshowId equals r.Id
                join dl in context.DefaultLocation on re.DefaultLocationId equals dl.Id
                where ro.Id == roadshowOfferId
                select dl
            )
                .Distinct()
                .Select(defaultLocationToRoadshowOfferLocationModel);
        }

        public async Task<ResponseDetailsModel> DeleteRSOffer(int id)
        {
            var context = ContextFactory();

            var isAttachedToEvent = await (
                from ro in context.RoadshowOffer
                join reo in context.RoadshowEventOffer on ro.Id equals reo.RoadshowOfferId
                where reo.RoadshowOfferId == id
                select ro
            ).AnyAsync();

            if (isAttachedToEvent)
                return new ResponseDetailsModel
                {
                    Description = "This offer is attached to event, therefore it can't be deleted.",
                    Message = "Offer can't be deleted.",
                    StatusCode = (int)HttpStatusCode.BadRequest
                };

            var offer = await context.RoadshowOffer.FirstOrDefaultAsync(x => x.Id == id);

            if (offer != null)
            {
                var vouchers = await context.RoadshowVoucher
                    .Where(x => x.RoadshowOfferId == offer.Id)
                    .ToListAsync();

                context.RoadshowVoucher.RemoveRange(vouchers);
                context.RoadshowOffer.Remove(offer);
                await context.SaveChangesAsync();
                return new ResponseDetailsModel
                {
                    Description = "Roadshow offer successfully deleted",
                    Message = "Offer deleted.",
                    StatusCode = (int)HttpStatusCode.OK
                };
            }

            return new ResponseDetailsModel
            {
                Description = "Roadshow offer with that ID doesn't exists in database.",
                Message = "Offer can't be deleted.",
                StatusCode = (int)HttpStatusCode.BadRequest
            };
        }

        public async Task<byte[]> GetQRCodeData(int offerId)
        {
            var context = ContextFactory();
            var qrCode = await (
                from od in context.RoadshowOfferDocument
                join d in context.Document on od.DocumentId equals d.Id
                where od.RoadshowOfferId == offerId && od.Type == OfferDocumentType.QRCode
                select d
            ).FirstOrDefaultAsync();
            return qrCode == null ? null : qrCode.Content;
        }

        private readonly Expression<
            Func<RoadshowOffer, RoadshowOfferModel>
        > projectToRoadshowOfferCardModel = data =>
            new RoadshowOfferModel()
            {
                Id = data.Id,
                RoadshowProposalSubject = data.RoadshowProposal.Subject,
                Category = data.RoadshowOfferCategories
                    .Select(c => c.Category.Title)
                    .FirstOrDefault(),
                RoadshowProposalId = data.RoadshowProposalId,
                RoadshowProposalTitle = data.RoadshowProposal.Title,
                MainImage = data.OfferDocuments
                    .Where(d => d.Type == OfferDocumentType.Thumbnail && d.Cover)
                    .Select(d => d.DocumentId.ToString())
                    .FirstOrDefault(),
                Tag = data.RoadshowOfferTags.Select(t => t.Tag.Title).FirstOrDefault(),
                Title = data.Title,
                Description = data.Description,
                //Status = data.Status,
                CreatedBy = data.CreatedBy,
                CreatedOn = data.CreatedOn,
                UpdatedOn = data.UpdatedOn,
                UpdatedBy = data.UpdatedBy
            };

        private readonly Expression<
            Func<DefaultLocation, RoadshowOfferLocationModel>
        > defaultLocationToRoadshowOfferLocationModel = data =>
            new RoadshowOfferLocationModel()
            {
                Id = data.Id,
                Title = data.Title,
                Longitude = data.Longitude,
                Latitude = data.Latitude,
                Address = data.Address,
                Vicinity = data.Vicinity,
                Country = data.Country
            };
    }
}
