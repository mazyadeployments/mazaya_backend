using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MMA.WebApi.Shared.Interfaces;
using MMA.WebApi.Shared.Interfaces.ApplicationUsers;
using MMA.WebApi.Shared.Interfaces.Domain.Document;
using MMA.WebApi.Shared.Interfaces.Helpers;
using MMA.WebApi.Shared.Interfaces.Membership;
using MMA.WebApi.Shared.Models.ApplicationUser;
using MMA.WebApi.Shared.Models.Document;
using MMA.WebApi.Shared.Models.Image;
using MMA.WebApi.Shared.Models.Membership;
using MMA.WebApi.Shared.Models.ServiceNowModels;
using Newtonsoft.Json;
using Passbook.Generator;
using Passbook.Generator.Fields;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Declares;
using static MMA.WebApi.Shared.Enums.Document.DocumentDeclares;

namespace MMA.WebApi.Core.Services
{
    public class MembershipECardService : IMembershipECardService
    {
        private readonly IMembershipECardMakerRepository _membershipECardMakerRepository;
        private readonly IMembershipECardRepository _membershipECardRepository;
        private readonly IServiceNow _serviceNow;
        private readonly IDocumentHelper _documentHelper;
        private readonly IDocumentService _documentService;
        private readonly IApplicationUserService _applicationUserService;
        private readonly IConfiguration _configuration;

        public MembershipECardService(
            IServiceNow serviceNow,
            IMembershipECardMakerRepository membershipECardMakerRepository,
            IDocumentService documentService,
            IDocumentHelper documentHelper,
            IApplicationUserService applicationUserService,
            IConfiguration configuration,
            IMembershipECardRepository membershipECardRepository
        )
        {
            _applicationUserService = applicationUserService;
            _documentHelper = documentHelper;
            _serviceNow = serviceNow;
            _documentService = documentService;
            _membershipECardMakerRepository = membershipECardMakerRepository;
            _configuration = configuration;
            _membershipECardRepository = membershipECardRepository;
        }

        public async Task AddMembershipCard(MembershipEcardModel membershipEcard, string userId)
        {
            await _membershipECardMakerRepository.AddMembershipCard(membershipEcard, userId);
        }

        public async Task<byte[]> CreatePdfForMembershipECard(int ecardId, string userId)
        {
            if (ecardId == 0)
                return await _membershipECardMakerRepository.CreatePdfForMazayaCard(userId);
            return await _membershipECardMakerRepository.CreatePdfForMembershipECard(ecardId);
        }

        public async Task<WalletCardType> GetWalletCardTypeByMembershipId(string membershipId)
        {
            if (membershipId == null)
                return WalletCardType.Basic;

            var membershipTypes = await _membershipECardRepository.GetMembershipTypes();

            if (membershipTypes != null && membershipTypes.ContainsKey(membershipId.ToString()))
            {
                var membershipTypeName = membershipTypes[membershipId];
                if (membershipTypeName.Equals("Family Entertainment"))
                    return WalletCardType.FamilyEntertainment;
                else if (membershipTypeName.Equals("Leisure And Family Entertainment"))
                    return WalletCardType.LeisureAndFamilyEntertainment;
                else if (membershipTypeName.Equals("Health And Leisure"))
                    return WalletCardType.HealtAndLeisure;
            }
            return WalletCardType.Basic;
        }

        public async Task<MembershipEcardModel> GetMembershipECardById(int ecardId)
        {
            return await _membershipECardMakerRepository.GetMembershipECardById(ecardId);
        }

        public async Task<FileContentResult> GenerateMembershipAppleWalletCard(
            int ecardId,
            string userId
        )
        {
            var card = await _membershipECardMakerRepository.GetMembershipECardById(ecardId);
            if (card == null)
            {
                throw new Exception(
                    "Something went wrong. Please contact support with this status code - Error#233"
                );
            }
            var membershipUser = new ApplicationUserModel
            {
                Id = card.OwnerId,
                FirstName = card.Name,
                LastName = card.Surname,
                ECardSequence = card.ECardSequence,
            };
            var membershipType = await GetWalletCardTypeByMembershipId(
                card.MembershipId.ToString()
            );

            var expireDate = card.ValidTo == null ? "--" : card.ValidTo.Date.ToShortDateString();
            return GenerateAppleWalletCard(
                membershipUser,
                membershipType,
                expireDate,
                card.PhotoUrl
            );
        }

        public async Task<IEnumerable<MembershipEcardModel>> GetMemberCard(string userId)
        {
            return await _membershipECardRepository.GetMemberCard(userId);
        }

        public IEnumerable<MembershipModel> GetMembershipsForUser(string userId, bool isBuyer)
        {
            return _membershipECardRepository.GetMembershipsForUser(userId, isBuyer);
        }

        public async Task<IEnumerable<MembershipModel>> GetMembershipTypes()
        {
            return await _membershipECardRepository.GetAllMembershipsModel();
        }

        public async Task<IEnumerable<MembershipEcardModel>> GetOwnerCards(string userId)
        {
            return await _membershipECardRepository.GetOwnerCards(userId);
        }

        public async Task<object> UploadForTestuat(IFormFileCollection files)
        {
            var picture = files[0];
            byte[] bytes = _documentHelper.GetBytes(picture);
            var result = await _documentService.Upload(
                bytes,
                Guid.NewGuid(),
                picture.Name,
                picture.ContentType,
                null
            );
            var imageList = new ImageModel[]
            {
                new ImageModel
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = OfferDocumentType.Original,
                    OriginalImageId = result.Id,
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
                    },
                    Cover = false
                }
            };
            var json = JsonConvert.SerializeObject(
                new ImageBackgroundUploadModel
                {
                    CroppedImages = imageList,
                    ImageForType = ImageForType.Collection
                }
            );
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var url = _configuration["AzureFunctions:Url"];
            url += "/ImportImageInBackground";
            using var client = new HttpClient();
            HttpResponseMessage response = null;
            try
            {
                response = await client.PostAsync(url, data);
            }
            catch (Exception ex)
            {
                return ex;
            }
            var env = _configuration["AzureFunctions:Environment"];

            return env + "***" + url + "***" + response;
        }

        public async Task UploadPicture(IFormFileCollection files, int type)
        {
            var fileVerical = files[0];
            var fileHorizontal = files[1];
            var fileVerticalBack = files[2];
            var fileHorizontalback = files[3];
            var idVerical = Guid.NewGuid();
            var idHorizontal = Guid.NewGuid();
            var idVericalBack = Guid.NewGuid();
            var idHorizontalBack = Guid.NewGuid();
            DocumentFileModel resultVertical = null;
            DocumentFileModel resultHorizontal = null;
            DocumentFileModel resultVerticalBack = null;
            DocumentFileModel resultHorizontalBack = null;

            if (fileHorizontal != null)
            {
                byte[] bytesHorizontal = _documentHelper.GetBytes(fileHorizontal);
                resultHorizontal = await _documentService.Upload(
                    bytesHorizontal,
                    idHorizontal,
                    fileHorizontal.Name,
                    fileHorizontal.ContentType,
                    null
                );
            }
            if (fileVerical != null)
            {
                byte[] bytesVertical = _documentHelper.GetBytes(fileVerical);
                resultVertical = await _documentService.Upload(
                    bytesVertical,
                    idVerical,
                    fileVerical.Name,
                    fileVerical.ContentType,
                    null
                );
            }

            if (fileHorizontalback != null)
            {
                byte[] bytesHorizontalBack = _documentHelper.GetBytes(fileHorizontalback);
                resultHorizontalBack = await _documentService.Upload(
                    bytesHorizontalBack,
                    idHorizontalBack,
                    fileHorizontalback.Name,
                    fileHorizontalback.ContentType,
                    null
                );
            }
            if (fileVerticalBack != null)
            {
                byte[] bytesVerticalBack = _documentHelper.GetBytes(fileVerticalBack);
                resultVerticalBack = await _documentService.Upload(
                    bytesVerticalBack,
                    idVericalBack,
                    fileVerticalBack.Name,
                    fileVerticalBack.ContentType,
                    null
                );
            }

            await _membershipECardMakerRepository.UploadPicture(
                new Guid[]
                {
                    resultVertical.Id,
                    resultHorizontal.Id,
                    resultVerticalBack.Id,
                    resultHorizontalBack.Id
                },
                type
            );
        }

        public async Task CreateECardForUser(string userId)
        {
            var user = await _applicationUserService.GetById(userId);
            var email = user.Email;

            var text = email.Substring(0, 2);

            if (text.Equals("t_"))
                email = email.Substring(2);

            var UserData = await _serviceNow.GetDataByMail(email);
            if (UserData == null)
                return;

            await _membershipECardMakerRepository.CreateECardForUser(UserData, userId);
        }

        public async Task CreateECardsForUser(
            IEnumerable<ServiceNowUserModel> UserDatas,
            string userId
        )
        {
            await _membershipECardMakerRepository.CreateECardForUser(UserDatas, userId);
        }

        public async Task CheckMembershipECardsValidTo()
        {
            await _membershipECardRepository.DeleteExpiredMembershipECards(DateTime.UtcNow);
        }

        public FileContentResult GenerateAppleWalletCard(
            ApplicationUserModel user,
            WalletCardType walletCardType,
            string expireDate = "",
            string userPhotoUrl = ""
        )
        {
            PassGenerator generator = new PassGenerator();
            PassGeneratorRequest request = new PassGeneratorRequest();
            try
            {
                PopulateAppleWalletCardGlobalFields(request, user);

                SignAppleWalletCardWithCertificates(request);

                FillAppleWalletCardWithImages(request);

                if (walletCardType != WalletCardType.Basic && string.IsNullOrEmpty(userPhotoUrl))
                {
                    throw new Exception(
                        "Something went wrong. Please contact support with this status code - Error#255"
                    );
                }
                SetSpecificPropertiesByCardType(
                    request,
                    walletCardType,
                    userPhotoUrl,
                    user,
                    expireDate
                );

                byte[] generatedPass = generator.Generate(request);
                FileContentResult result = new FileContentResult(
                    generatedPass,
                    "application/vnd.apple.pkpass"
                )
                {
                    FileDownloadName = "mazaya.pkpass",
                };
                return result;
            }
            catch
            {
                throw new Exception(
                    "Something went wrong. Please contact support with this status code - Error#277"
                );
            }
        }

        private void PopulateAppleWalletCardGlobalFields(
            PassGeneratorRequest request,
            ApplicationUserModel user
        )
        {
            request.PassTypeIdentifier = _configuration["AppleWallet:PassTypeIdentifier"];
            request.TeamIdentifier = _configuration["AppleWallet:TeamIdentifier"];
            request.Description = "Description";
            request.OrganizationName = _configuration["AppleWallet:OrganizationName"];
            request.LogoText = "";
            request.LabelColor = "rgb(255, 255, 255)";
            request.ForegroundColor = "rgb(255, 255, 255)";
            request.Style = PassStyle.Generic;
            request.AddSecondaryField(
                new StandardField("card-no", "Card No.", $"{user.ECardSequence}")
            );

            string arabicInfo = "02701717 \u200Eللاستفسارات، يرجى الاتصال على الرقم التالي";
            request.AddAuxiliaryField(
                new StandardField(
                    "informations",
                    "For any enquiries, please contact 027071717",
                    $"{arabicInfo}"
                )
            );
            request.TransitType = TransitType.PKTransitTypeAir;
            request.SetBarcode(BarcodeType.PKBarcodeFormatQR, "https://mazayaoffers.ae/", "UTF-8");
        }

        private void SignAppleWalletCardWithCertificates(PassGeneratorRequest request)
        {
            var certificatePassword = _configuration["AppleWallet:CertificateAccess"];
            X509KeyStorageFlags flags =
                X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable;
            request.PassbookCertificate = new X509Certificate2(
                System.IO.File.ReadAllBytes(
                    Path.GetFullPath(@".\Resources\Wallets\AppleWallet\passMazaya.p12")
                ),
                certificatePassword,
                flags
            );
            request.AppleWWDRCACertificate = new X509Certificate2(
                System.IO.File.ReadAllBytes(
                    Path.GetFullPath(@".\Resources\Wallets\AppleWallet\wwdr.pem")
                )
            );
        }

        private void FillAppleWalletCardWithImages(PassGeneratorRequest request)
        {
            request.Images.Add(
                PassbookImage.Icon,
                System.IO.File.ReadAllBytes(
                    Path.GetFullPath(@".\Resources\Wallets\AppleWallet\icon.png")
                )
            );
            request.Images.Add(
                PassbookImage.Icon2X,
                System.IO.File.ReadAllBytes(
                    Path.GetFullPath(@".\Resources\Wallets\AppleWallet\icon@2x.png")
                )
            );
        }

        private void SetSpecificPropertiesByCardType(
            PassGeneratorRequest request,
            WalletCardType walletCardType,
            string userPhotoUrl,
            ApplicationUserModel user,
            string expireDate
        )
        {
            var headerName = "Name";
            if (walletCardType == WalletCardType.Basic)
            {
                request.SerialNumber = user.Id.ToString();
                headerName = "Offers And Discounts";
                SetBasicSpecificPropertiesForAppleWalletCard(request);
                request.AddPrimaryField(
                    new StandardField("name", headerName, $"{user.FirstName} {user.LastName}")
                );
            }
            else if (
                walletCardType == WalletCardType.FamilyEntertainment
                || walletCardType == WalletCardType.LeisureAndFamilyEntertainment
                || walletCardType == WalletCardType.HealtAndLeisure
            )
            {
                request.SerialNumber = user.ECardSequence;
                SetMembershipSpecificPropertiesForAppleWalletCard(request, walletCardType, user);
                request.AddSecondaryField(
                    new StandardField("expire-date", "Expiry Date", $"{expireDate}")
                );
                AddToAppleWalletCardThumbnailImages(request, userPhotoUrl);
            }
        }

        private void AddToAppleWalletCardThumbnailImages(
            PassGeneratorRequest request,
            string userPhotoUrl
        )
        {
            try
            {
                Image resizedImage = _membershipECardMakerRepository
                    .LoadPictureForEcard(userPhotoUrl)
                    .Result;
                var th = (Image)(new Bitmap(resizedImage, new Size(388, 429)));
                var thx2 = (Image)(new Bitmap(resizedImage, new Size(388, 429)));
                var thumbnail = new byte[0];
                var thumbnailx2 = new byte[0];
                using (var _memorystream = new MemoryStream())
                {
                    th.Save(_memorystream, System.Drawing.Imaging.ImageFormat.Jpeg);
                    thumbnail = _memorystream.ToArray();
                    thx2.Save(_memorystream, System.Drawing.Imaging.ImageFormat.Jpeg);
                    thumbnailx2 = _memorystream.ToArray();
                }
                request.Images.Add(PassbookImage.Thumbnail, thumbnail);
                request.Images.Add(PassbookImage.Thumbnail2X, thumbnailx2);
            }
            catch
            {
                throw new Exception(
                    "Something went wrong. Please contact support with this status code - Error#266"
                );
            }
        }

        private void SetBasicSpecificPropertiesForAppleWalletCard(PassGeneratorRequest request)
        {
            request.BackgroundColor = "rgb(10, 47, 106)";
            request.Images.Add(
                PassbookImage.Logo,
                System.IO.File.ReadAllBytes(
                    Path.GetFullPath(@".\Resources\Wallets\AppleWallet\Logo.png")
                )
            );
            request.Images.Add(
                PassbookImage.Logo2X,
                System.IO.File.ReadAllBytes(
                    Path.GetFullPath(@".\Resources\Wallets\AppleWallet\Logo@2x.png")
                )
            );
        }

        private void SetMembershipSpecificPropertiesForAppleWalletCard(
            PassGeneratorRequest request,
            WalletCardType walletCardType,
            ApplicationUserModel user
        )
        {
            var headerName = "Name";
            if (walletCardType == WalletCardType.FamilyEntertainment)
            {
                request.BackgroundColor = "rgb(230, 171, 47)";
                headerName = "Family Entertainment";
            }
            else if (walletCardType == WalletCardType.LeisureAndFamilyEntertainment)
            {
                request.BackgroundColor = "rgb(154, 154, 156)";
                headerName = "Leisure And Family Entertainment";
            }
            else if (walletCardType == WalletCardType.HealtAndLeisure)
            {
                request.BackgroundColor = "rgb(47, 160, 215)";
                headerName = "Health And Leisure";
            }
            request.AddPrimaryField(
                new StandardField("name", headerName, $"{user.FirstName} {user.LastName}")
            );

            request.Images.Add(
                PassbookImage.Logo,
                System.IO.File.ReadAllBytes(
                    Path.GetFullPath(@".\Resources\Wallets\AppleWallet\Logo++.png")
                )
            );
            request.Images.Add(
                PassbookImage.Logo2X,
                System.IO.File.ReadAllBytes(
                    Path.GetFullPath(@".\Resources\Wallets\AppleWallet\Logo++@2x.png")
                )
            );
        }
    }
}
