using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MMA.WebApi.DataAccess.Extensions;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.Categories;
using MMA.WebApi.Shared.Interfaces.Collection;
using MMA.WebApi.Shared.Interfaces.Companies;
using MMA.WebApi.Shared.Interfaces.Document;
using MMA.WebApi.Shared.Interfaces.Domain.Document;
using MMA.WebApi.Shared.Interfaces.Image;
using MMA.WebApi.Shared.Interfaces.MailStorage;
using MMA.WebApi.Shared.Interfaces.Offers;
using MMA.WebApi.Shared.Interfaces.RoadshowDocuments;
using MMA.WebApi.Shared.Interfaces.Roles;
using MMA.WebApi.Shared.Interfaces.Tag;
using MMA.WebApi.Shared.Models.Document;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.Image;
using MMA.WebApi.Shared.Models.Logger;
using MMA.WebApi.Shared.Models.Offer;
using MMA.WebApi.Shared.Models.Response;
using MMA.WebApi.Shared.Models.Roadshow;
using MMA.WebApi.Shared.Monads;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Document.DocumentDeclares;

namespace MMA.WebApi.Core.Services
{
    public class RoadshowOfferService : IRoadshowOfferService
    {
        private readonly IRoadshowOfferRepository _roadshowOfferRepository;
        private readonly IRoadshowDocumentRepository _roadshowDocumentRepository;
        private readonly IRoadshowOfferDocumentRepository _roadshowOfferDocumentRepository;
        private readonly IDocumentRepository _documentRepository;
        private readonly IImageUtilsService _imageUtilsService;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICollectionRepository _collectionRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IConfiguration _configuration;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDocumentService _documentService;
        private readonly IRoleService _roleService;
        private readonly ICompanyRepository _companyRepository;
        private readonly IRoadshowProposalService _roadshowProposalService;
        private readonly IMailStorageService _mailStorageServiceService;

        public RoadshowOfferService(
                IRoadshowOfferRepository roadshowOfferRepository, IRoadshowProposalService roadshowProposalService,
                IRoadshowOfferDocumentRepository roadshowOfferDocumentRepository, IRoadshowDocumentRepository roadshowDocumentRepository,
                IDocumentRepository documentRepository, IImageUtilsService imageUtilsService, ICompanyRepository companyRepository,
                ICategoryRepository categoryRepository, ICollectionRepository collectionRepository,
                ITagRepository tagRepository, IMailStorageService mailStorageService,
                IDocumentService documentService, UserManager<ApplicationUser> userManager, IConfiguration configuration,
                IRoleService roleService)
        {
            _roadshowOfferRepository = roadshowOfferRepository;
            _roadshowOfferDocumentRepository = roadshowOfferDocumentRepository;
            _roadshowDocumentRepository = roadshowDocumentRepository;
            _documentRepository = documentRepository;
            _imageUtilsService = imageUtilsService;
            _mailStorageServiceService = mailStorageService;
            _categoryRepository = categoryRepository;
            _collectionRepository = collectionRepository;
            _tagRepository = tagRepository;
            _configuration = configuration;
            _documentService = documentService;
            _userManager = userManager;
            _roleService = roleService;
            _companyRepository = companyRepository;
            _roadshowProposalService = roadshowProposalService;
        }

        public async Task<IEnumerable<RoadshowOfferModel>> GetRoadshowOffers()
        {
            var roadshows = await _roadshowOfferRepository.Get().ToListAsync();

            return roadshows;
        }

        public async Task<PaginationListModel<RoadshowOfferModel>> GetAllRoadshowOffers(QueryModel queryModel, string userId)
        {
            var roles = await _roleService.GetUserRoles(userId);
            var roadshowProposals = await _roadshowOfferRepository.GetAllRoadshowOffers(userId, roles, queryModel)
                                                                  .ToPagedListAsync(queryModel.PaginationParameters.PageNumber, queryModel.PaginationParameters.PageSize);

            return roadshowProposals;
        }

        public async Task<PaginationListModel<RoadshowOfferModel>> GetAllRoadshowOffersForMyCompany(QueryModel queryModel, string userId)
        {
            var roadshowProposals = await _roadshowOfferRepository.GetAllRoadshowOffersForMyCompany(queryModel, userId).ToPagedListAsync(queryModel.PaginationParameters.PageNumber, queryModel.PaginationParameters.PageSize);

            return roadshowProposals;
        }

        public async Task<RoadshowOfferModel> CreateOrUpdate(RoadshowOfferModel model, string userId)
        {
            var userCompany = await _companyRepository.GetMyCompany(userId);

            if (model.Id == 0 && userCompany.Id == model.CompanyId)
            {
                var auditVisitor = new CreateAuditVisitor(userId, DateTime.UtcNow);

                // Decode fields
                model.RoadshowDetails = DecodeBase64String(model.RoadshowDetails);
                model.EquipmentItem = DecodeBase64String(model.EquipmentItem);

                var roadshowOffer = await _roadshowOfferRepository.CreateAsync(model, auditVisitor, userId);

                //Group images in the sets of 3 - Original, Large and Thumbnail
                GroupImages(model, roadshowOffer);

                foreach (var category in roadshowOffer.RoadshowOfferCategories)
                {
                    var offerCategory = await _categoryRepository.Get(category.Id);
                    category.Title = offerCategory.Title;
                }

                foreach (var collection in roadshowOffer.RoadshowOfferCollections)
                {
                    var offerCollection = await _collectionRepository.Get(collection.Id);
                    collection.Title = offerCollection.Title;
                }

                foreach (var tag in roadshowOffer.RoadshowOfferTags)
                {
                    var offerTag = await _tagRepository.Get(tag.Id);
                    tag.Title = offerTag.Title;
                }

                return roadshowOffer;
            }

            return null;
        }

        public async Task<Maybe<RoadshowOfferModel>> GetSpecificRoadshowOfferById(int id, string userId)
        {
            var roles = await _roleService.GetUserRoles(userId);
            var roadshowOffer = await _roadshowOfferRepository.GetSpecificRoadshowOfferById(id, userId, roles);

            if (roadshowOffer != null)
            {

                // Get roadshow offer locations
                roadshowOffer.Locations = _roadshowOfferRepository.GetRoadshowOfferLocation(roadshowOffer.Id);

                // Get average rating of all company offers and roadshow offers
                var companyRating = await _companyRepository.GetCompanyRating(roadshowOffer.CompanyId);
                roadshowOffer.AverageRatingOfAllCompanyOffers = Math.Round(companyRating.AverageRating, 2);
                roadshowOffer.NumberOfVotesOnCompanyOffers = companyRating.TotalRatings;

                // Check if offer is rated and favorite
                var isRatedAndFavorite = await _roadshowOfferRepository.CheckIfOfferIsFavoriteAndRated(roadshowOffer.Id, userId);
                roadshowOffer.IsAlreadyRated = isRatedAndFavorite.IsRated;
                roadshowOffer.IsFavourite = isRatedAndFavorite.IsFavorite;

                var offers = new List<RoadshowOfferModel>();
                offers.Add(roadshowOffer);

                ProcessImages(offers);

                List<ImageModel> onlyOriginalImages = new List<ImageModel>();

                foreach (var image in roadshowOffer.Images)
                {
                    if (image.Type == OfferDocumentType.Original)
                    {
                        onlyOriginalImages.Add(image);
                    }
                }

                roadshowOffer.Images = onlyOriginalImages;

                foreach (var document in roadshowOffer.OfferDocuments)
                {
                    var a = await _documentRepository.GetSingleAsync(x => x.Id == new Guid(document.DocumentId));
                    if (document.Document == null) document.Document = new DocumentFileModel();
                    document.Document.Name = a.Name;
                }
                roadshowOffer.Rating = Math.Round(roadshowOffer.Rating, 2);
                roadshowOffer.RatingPercent = Math.Round(roadshowOffer.RatingPercent, 2);

                return roadshowOffer;
            }
            else
            {
                return null;
            }
        }


        private static void ProcessImages(List<RoadshowOfferModel> offers)
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


        private static void GroupImages(RoadshowOfferModel model, RoadshowOfferModel offer)
        {
            offer.ImageUrls = new List<ImageUrlsModel>();
            Guid coverIMG = Guid.Empty;

            foreach (var image in model.Images)
            {
                if (image.OriginalImageId == Guid.Empty)
                {
                    image.OriginalImageId = new Guid(image.Id);
                    //image.Cover = image.Cover;
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
                    if (imageModel.OriginalImageId == coverIMG && imageModel.Type == OfferDocumentType.Thumbnail)
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


        //Skip images that are unchanged during Review phase
        private async Task<bool> ShouldProcessImage(ImageModel image)
        {
            var imageDb = await _roadshowOfferDocumentRepository.GetByDocumentId(new Guid(image.Id));

            if (imageDb != null && imageDb.X1 == image.CropCoordinates.X1 &&
                imageDb.X2 == image.CropCoordinates.X2 &&
                imageDb.Y1 == image.CropCoordinates.Y1 &&
                imageDb.Y2 == image.CropCoordinates.Y2)
            {
                await _roadshowOfferDocumentRepository.UpdateRoadshowOfferImagesCover(new Guid(image.Id), image.Cover);
                return false;
            }
            else
            {
                //Delete old thumbanail and large image
                var imagesToDelete = _roadshowOfferDocumentRepository.GetRoadshowOfferImages(new Guid(image.Id)).Where(x => x.Type == OfferDocumentType.Thumbnail || x.Type == OfferDocumentType.Large);
                foreach (RoadshowOfferDocumentModel imageToDelete in imagesToDelete)
                {
                    await _roadshowOfferDocumentRepository.DeleteAsync(imageToDelete.Id);
                }

                return true;
            }
        }
        public async Task ShareOffer(OfferShareModel offerShareModel, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return;

            await _mailStorageServiceService.CreateShareMail(userId, offerShareModel.ShareTo, offerShareModel.Subject, offerShareModel.Message);
        }

        /// <summary>
        /// Checks if QR Code exists and based on that returns it or generates it
        /// </summary>
        /// <param name="offerId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<ImageModel> GetQRCodeForRoadshowOffer(int offerId, string userId)
        {
            var qrCodeID = await _roadshowOfferRepository.GetQRCodeForRoadshowOffer(offerId);
            DocumentFileModel qrFileModel = new DocumentFileModel();

            if (qrCodeID == Guid.Empty)
            {
                var qrCodeIMG = await _roadshowOfferRepository.GenerateQRCodeWithLogoForRoadshowOffer(offerId, userId);
                qrFileModel.Id = qrCodeID;
            }

            return new ImageModel()
            {
                Id = qrFileModel.Id.ToString(),
                OriginalImageId = qrFileModel.Id,
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

        public async Task SetRoadshowOfferAsFavourite(RoadshowOfferFavoriteModel roadshowOfferFavorite, string userId)
        {
            await _roadshowOfferRepository.SetRoadshowOfferAsFavourite(roadshowOfferFavorite, userId);
        }

        private void SendMail(OfferShareModel shareModel, string userMail)
        {
            string smtpHost = _configuration["Emails:MailHost"];
            string smtpPassword = _configuration["Emails:EmailPassword"];
            int smtpPort = Convert.ToInt32(_configuration["Emails:MailServerPort"]);
            bool EnableSsl = Convert.ToBoolean(_configuration["Emails:EnableSsl"]);
            bool UseDefaultCredentials = Convert.ToBoolean(_configuration["Emails:UseDefaultCredentials"]);
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

        public bool CanManageRoadshowOffers(List<Declares.Roles> roles, RoadshowOfferModel model, string userId)
        {
            // Buyer, Reviewer -> False

            // Kreira
            //1. Admin -> False
            //2. Suppliers && model.companyId == supplier.companyId && model.status == draft
            // Update

            return false;
        }

        public async Task<RoadshowOfferMobileModel> GetSpecificOfferByIdForMobile(int id, string userId)
        {
            return await _roadshowOfferRepository.GetSpecificOfferByIdForMobile(id, userId);
        }

        public async Task<ResponseDetailsModel> DeleteRSOffer(int id, string userId)
        {
            if (await _roadshowOfferRepository.Get().AnyAsync(x => x.Id == id && x.CreatedBy == userId))
            {
                return await _roadshowOfferRepository.DeleteRSOffer(id);
            }

            return new ResponseDetailsModel
            {
                Description = "You don't have permission to delete this roadshow offer.",
                Message = "Offer can't be deleted.",
                StatusCode = (int)HttpStatusCode.BadRequest
            };
        }

        private string DecodeBase64String(string encodedString)
        {
            encodedString ??= "";
            byte[] data = Convert.FromBase64String(encodedString);
            return Encoding.UTF8.GetString(data);
        }

        public async Task<byte[]> GetPdfQRCodeForOffer(int offerId, string userId)
        {
            var img = await _roadshowOfferRepository.GetQRCodeData(offerId);
            if (img == null)
            {
                var qrCodeIMG = await _roadshowOfferRepository.GenerateQRCodeWithLogoForRoadshowOffer(offerId, userId);
                img = qrCodeIMG.Content;
            }

            var imgWithBack = myQRCodeGenerator.createQRCodeWithBackgroun(img);
            var pdfArray = myQRCodeGenerator.CreatePdfsReturnArrayFromQRCode(imgWithBack);
            return pdfArray;
        }

    }
}