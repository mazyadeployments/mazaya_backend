using IdentityServer4.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Interfaces.ApplicationUsers;
using MMA.WebApi.Shared.Interfaces.Domain.Document;
using MMA.WebApi.Shared.Interfaces.Membership;
using MMA.WebApi.Shared.Models.ApplicationUser;
using MMA.WebApi.Shared.Models.Document;
using MMA.WebApi.Shared.Models.Membership;
using MMA.WebApi.Shared.Models.ServiceNowModels;
using Newtonsoft.Json;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MMA.WebApi.DataAccess.Repositories.Membership
{
    public class MembershipECardMakerRepository : IMembershipECardMakerRepository
    {
        private readonly Func<MMADbContext> _contexFactory;
        private readonly IApplicationUserService _applicationUserService;
        private readonly IDocumentService _documentService;

        public MembershipECardMakerRepository(
            Func<MMADbContext> contexFactory,
            IApplicationUserService applicationUserService,
            IDocumentService documentService
        )
        {
            _applicationUserService = applicationUserService;
            _contexFactory = contexFactory;
            _documentService = documentService;
        }

        public async Task AddMembershipCard(MembershipEcardModel membershipEcard, string userId)
        {
            var context = _contexFactory();

            var membershipECard = PopulateDbFromDomainModel(new MembershipECard(), membershipEcard);
            membershipECard.CreatedBy = userId;

            await context.MembershipECards.AddAsync(membershipECard);
            await context.SaveChangesAsync();
        }

        private async Task<MembershipEcardModel> CreateOffersECard(string userId)
        {
            var context = _contexFactory();
            var membershipDataPicture = context.MembershipPictureDatas
                .Where(x => x.MembershipType == (int)Declares.OfferMembership.OffersAndDiscounts)
                .FirstOrDefault();
            var adnocType = context.UserDomain
                .Where(x => x.DomainName.Equals("ADNOCEmployee"))
                .Select(x => x.Id)
                .ToList();
            var user = _applicationUserService.GetUserForECard(userId);
            var flag = adnocType.Contains(user.UserType.Id);
            return new MembershipEcardModel()
            {
                Id = 0,
                OwnerId = user.Id,
                Name = user.FirstName,
                Surname = user.LastName,
                ECardSequence = user.ECard,
                MembershipType = ((int)Declares.OfferMembership.OffersAndDiscounts),
                IsMembershipCard = false,
                MembershipNameEng = "Offers And Discounts",
                MembershipNameAr = "العروض والخصومات",
                BackgroundPortraitUrl = membershipDataPicture.DocumentIdVerticalPicture.ToString(),
                BackgroundLandscapeUrl =
                    membershipDataPicture.DocumentIdHorizontalPicture.ToString(),
                backgroundLandscapeBackUrl =
                    membershipDataPicture.DocumentIdHorizontalBackPicture.ToString(),
                backgroundPortraitBackUrl =
                    membershipDataPicture.DocumentIdVerticalBackPicture.ToString(),
                supportArText = flag
                    ? "للاستفسار يرجى التواصل عبر 027071717 أو البريد\ngssupport@adnoc.ae الالكتروني"
                    : "للاستفسارات ، يرجى الاتصال على 027071717 ",
                supportEngText = flag
                    ? "For any enquiries, please contact 02 7071717\nor email gssupport@adnoc.ae"
                    : "For any enquiries, please contact 02 7071717"
            };
        }

        private MembershipECard PopulateNewDbFromDomainModel(
            MembershipECard entityModel,
            MembershipEcardModel data
        )
        {
            var context = _contexFactory();
            var membershipDataPicture = context.MembershipPictureDatas
                .Where(x => x.MembershipType == data.MembershipType)
                .FirstOrDefault();

            entityModel.Id = data.Id;
            entityModel.OwnerId = data.OwnerId;
            entityModel.MemberId = data.MemberId;
            entityModel.Name = data.Name;
            entityModel.Surname = data.Surname;
            entityModel.ECardSequence = data.ECardSequence;
            entityModel.MembershipType = data.MembershipType;
            entityModel.IsMembershipCard = data.IsMembershipCard;
            entityModel.PhotoUrl = data.PhotoUrl;
            entityModel.ValidTo = data.ValidTo;
            entityModel.Status = data.Status;
            entityModel.MembershipId = data.MembershipId;
            entityModel.CreatedOn = DateTime.UtcNow;
            entityModel.CreatedBy = data.OwnerId;
            entityModel.UpdatedBy = data.OwnerId;
            entityModel.UpdatedOn = entityModel.CreatedOn;
            return entityModel;
        }

        private MembershipECard PopulateDbFromDomainModel(
            MembershipECard entityModel,
            MembershipEcardModel data
        )
        {
            entityModel.Id = data.Id;
            entityModel.OwnerId = data.OwnerId;
            entityModel.MemberId = data.MemberId;
            entityModel.Name = data.Name;
            entityModel.Surname = data.Surname;
            entityModel.ECardSequence = data.ECardSequence;
            entityModel.MembershipType = data.MembershipType;
            entityModel.IsMembershipCard = data.IsMembershipCard;
            entityModel.PhotoUrl = data.PhotoUrl;
            entityModel.ValidTo = data.ValidTo;
            entityModel.Status = data.Status;
            entityModel.CreatedOn = DateTime.UtcNow;
            entityModel.UpdatedOn = DateTime.UtcNow;
            entityModel.MembershipId = data.MembershipId;
            entityModel.CreatedBy = data.OwnerId;
            entityModel.UpdatedBy = data.OwnerId;
            return entityModel;
        }

        public async Task AddMembershipCards(IEnumerable<MembershipECard> membershipEcards)
        {
            var context = _contexFactory();
            await context.MembershipECards.AddRangeAsync(membershipEcards.Where(x => x.Id == 0));
            await context.SaveChangesAsync();
        }

        public async Task UploadPicture(IEnumerable<Guid> filesId, int type)
        {
            var context = _contexFactory();
            var membershipPicture = new MembershipPictureData()
            {
                MembershipType = type,
                DocumentIdVerticalPicture = filesId.ToList()[0],
                DocumentIdHorizontalPicture = filesId.ToList()[1],
                DocumentIdVerticalBackPicture = filesId.ToList()[2],
                DocumentIdHorizontalBackPicture = filesId.ToList()[3]
            };
            context.MembershipPictureDatas.Add(membershipPicture);
            await context.SaveChangesAsync();
        }

        public async Task<DateTime> GetLastDateForServiceNow()
        {
            var context = _contexFactory();
            int count = context.ServiceNowDatas.Count();

            if (count < 1)
                return DateTime.Parse("1/1/2015");

            return context.ServiceNowDatas.Max(x => x.EndDate);
        }

        MembershipECard GetCardById(int ecardId)
        {
            var context = _contexFactory();

            var temp = context.MembershipECards
                .Where(x => x.Id == ecardId)
                .Include(x => x.Membership)
                .Include(x => x.Membership.PictureData)
                .FirstOrDefault();
            return temp;
        }

        public async Task<MembershipEcardModel> GetMembershipECardById(int ecardId)
        {
            var card = GetCardById(ecardId);
            if (card == null)
                throw new Exception(
                    "Something went wrong. Please contact support with this status code - Error#299"
                );
            return new MembershipEcardModel
            {
                Id = card.Id,
                ECardSequence = card.ECardSequence,
                IsMembershipCard = card.IsMembershipCard,
                MemberId = card.MemberId,
                OwnerId = card.OwnerId,
                Name = card.Name,
                Surname = card.Surname,
                MembershipType = card.MembershipType,
                PhotoUrl = card.PhotoUrl,
                ValidTo = card.ValidTo,
                Status = card.Status,
                MembershipId = card.MembershipId
            };
        }

        private Image setResolution(Image img, float resolutin)
        {
            Bitmap BitMap = new Bitmap(img);
            BitMap.SetResolution(resolutin, resolutin);
            return BitMap;
        }

        public async Task<byte[]> CreatePdfForMazayaCard(string userId)
        {
            var card = await CreateOffersECard(userId);

            Image imageFroFront = await LoadPictureForEcardBackGround(
                new Guid(card.BackgroundLandscapeUrl)
            );
            Image imageForBack = await LoadPictureForEcardBackGround(
                new Guid(card.backgroundLandscapeBackUrl)
            );
            Image imageBackground = Image.FromFile("images/MazayaCards/background-a4-2.jpg");

            imageFroFront = setResolution(imageFroFront, 78);
            imageForBack = setResolution(imageForBack, 78);

            imageFroFront = await createMazayaCard(imageFroFront, card);
            imageForBack = await createMazayaCardBack(imageForBack, card);

            Graphics gbackground = Graphics.FromImage(imageBackground);

            gbackground.DrawImage(
                imageFroFront,
                new Point((int)((imageBackground.Width / 6) * (1.1)), imageBackground.Height / 5)
            );
            gbackground.DrawImage(
                imageForBack,
                new Point(
                    (int)((imageBackground.Width / 6) * (1.1)),
                    (int)((imageBackground.Height / 5) * 2.8)
                )
            );

            imageBackground = setResolution(imageBackground, 78);

            imageBackground = setResolution(imageBackground, 72);
            return createPdf(imageBackground);
        }

        public async Task<byte[]> CreatePdfForMembershipECard(int ecardId)
        {
            var card = GetCardById(ecardId);
            if (card == null)
                return null;

            Image imageFroFront = await LoadPictureForEcardBackGround(
                card.Membership.PictureData.DocumentIdHorizontalPicture
            );
            Image imageForBack = await LoadPictureForEcardBackGround(
                card.Membership.PictureData.DocumentIdHorizontalBackPicture
            );
            Image imageBackground = Image.FromFile("images/background-a4.jpg");

            imageFroFront = setResolution(imageFroFront, 78);
            imageForBack = setResolution(imageForBack, 78);

            imageFroFront = await createFrontSide(imageFroFront, ecardId);

            Graphics gbackground = Graphics.FromImage(imageBackground);
            gbackground.DrawImage(
                imageFroFront,
                new Point((int)((imageBackground.Width / 6) * (1.1)), imageBackground.Height / 5)
            );
            gbackground.DrawImage(
                imageForBack,
                new Point(
                    (int)((imageBackground.Width / 6) * (1.1)),
                    (int)((imageBackground.Height / 5) * 2.8)
                )
            );

            imageBackground = setResolution(imageBackground, 78);

            imageBackground = setResolution(imageBackground, 72);
            return createPdf(imageBackground);
        }

        private async Task<Image> createMazayaCardBack(Image img, MembershipEcardModel card)
        {
            float height = img.Height / 66;
            float widht = img.Width / 105;
            PrivateFontCollection collection = new PrivateFontCollection();
            collection.AddFontFile(@"images/ADNOCSansRegular.ttf");
            Font font = new Font(collection.Families.First(), 19);
            SolidBrush brush = new SolidBrush(Color.White);
            Pen pen = new Pen(Color.White, 4);
            StringFormat drawFormat = new System.Drawing.StringFormat();

            Graphics g = Graphics.FromImage(img);
            g.DrawLine(
                pen,
                new Point((int)(widht * 8), (int)(height * 52.1)),
                new Point((int)(widht * 98.2), (int)(height * 52.1))
            );

            g.DrawString(
                card.supportEngText,
                font,
                brush,
                new Point((int)(widht * 7.8), (int)(height * 53.5)),
                drawFormat
            );
            drawFormat.Alignment = StringAlignment.Far;
            g.DrawString(
                card.supportArText,
                font,
                brush,
                new Point((int)(widht * 98), (int)(height * 53.5)),
                drawFormat
            );

            return img;
        }

        private async Task<Image> createMazayaCard(Image img, MembershipEcardModel card)
        {
            Graphics g = Graphics.FromImage(img);

            int top15 = img.Height / 17;
            int left3 = img.Height / 3;

            string artextMembershipName = card.MembershipNameAr;
            string engTextMembershipName = card.MembershipNameEng;
            string carNumber = card.ECardSequence;
            string fullName = (card.Name + " " + card.Surname).ToUpper();
            string expDate = "EXP: " + card.ValidTo.ToString("dd MMMM yyyy");

            Image resizedPicture = await LoadPictureForEcard(card.PhotoUrl);

            Font font = new Font("Arial", 35);
            SolidBrush brush = new SolidBrush(Color.White);
            StringFormat drawFormat = new System.Drawing.StringFormat();

            g.DrawString(
                artextMembershipName,
                font,
                brush,
                new Point((int)(left3 * 1.71), top15 * 10),
                drawFormat
            );
            g.DrawString(
                engTextMembershipName,
                font,
                brush,
                new Point((int)(left3 * 1.71), top15 * 11),
                drawFormat
            );

            font = new Font("Arial", 55);
            g.DrawString(
                carNumber,
                font,
                brush,
                new Point((int)(left3 * 1.7), top15 * 12),
                drawFormat
            );
            g.DrawString(
                fullName,
                font,
                brush,
                new Point((int)(left3 * 1.7), (int)(top15 * 13.5)),
                drawFormat
            );

            font = new Font("Arial", 20);
            g.DrawString(
                expDate,
                font,
                brush,
                new Point((int)(left3 * 1.72), (int)(top15 * 15.5)),
                drawFormat
            );
            g.DrawImage(resizedPicture, new Point((int)(left3 / 3), (int)(top15 * 10)));

            return img;
        }

        private async Task<Image> createFrontSide(Image img, int ecardId)
        {
            var card = GetCardById(ecardId);

            Graphics g = Graphics.FromImage(img);

            int top15 = img.Height / 17;
            int left3 = img.Height / 3;

            string artextMembershipName = card.Membership.NameAr;
            string engTextMembershipName = card.Membership.NameEng;
            string carNumber = card.ECardSequence;
            string fullName = (card.Name + " " + card.Surname).ToUpper();
            string expDate = "EXP: " + card.ValidTo.ToString("dd MMMM yyyy");

            Image resizedPicture = await LoadPictureForEcard(card.PhotoUrl);

            Font font = new Font("Arial", 35);
            SolidBrush brush = new SolidBrush(Color.White);
            StringFormat drawFormat = new System.Drawing.StringFormat();

            g.DrawString(
                artextMembershipName,
                font,
                brush,
                new Point((int)(left3 * 1.71), top15 * 10),
                drawFormat
            );
            g.DrawString(
                engTextMembershipName,
                font,
                brush,
                new Point((int)(left3 * 1.71), top15 * 11),
                drawFormat
            );

            font = new Font("Arial", 55);
            g.DrawString(
                carNumber,
                font,
                brush,
                new Point((int)(left3 * 1.7), top15 * 12),
                drawFormat
            );
            g.DrawString(
                fullName,
                font,
                brush,
                new Point((int)(left3 * 1.7), (int)(top15 * 13.5)),
                drawFormat
            );

            font = new Font("Arial", 20);
            g.DrawString(
                expDate,
                font,
                brush,
                new Point((int)(left3 * 1.72), (int)(top15 * 15.5)),
                drawFormat
            );
            g.DrawImage(resizedPicture, new Point((int)(left3 / 3), (int)(top15 * 10)));

            return img;
        }

        private byte[] createPdf(Image img)
        {
            ImageConverter converter = new ImageConverter();
            PdfSharp.Pdf.PdfDocument doc = new PdfSharp.Pdf.PdfDocument();
            PdfPage frontPage = doc.AddPage();

            frontPage.Width = 2480;
            frontPage.Height = 3508;

            XGraphics gfxFront = XGraphics.FromPdfPage(frontPage);
            byte[] byteArrayFrontPage = (byte[])converter.ConvertTo(img, typeof(byte[]));

            XImage imgFront = XImage.FromStream(new MemoryStream(byteArrayFrontPage));
            gfxFront.DrawImage(imgFront, 0, 0);

            var PdfDoc = new MemoryStream();
            doc.Save(PdfDoc, false);

            return PdfDoc.ToArray();
        }

        private async Task<Image> LoadPictureForEcardBackGround(Guid pictureGuid)
        {
            DocumentFileModel imageDocument = null;
            byte[] imageECard = null;

            Image picture = null;
            imageDocument = await _documentService.Download(pictureGuid);
            imageECard = imageDocument.Content;
            var ms = new MemoryStream(imageECard);
            picture = Image.FromStream(ms);
            return picture;
        }

        public async Task<Image> LoadPictureForEcard(string pictureGuid)
        {
            Image picture = null;
            Image resizedPicture = null;
            float aspectRatio = 1;
            if (string.IsNullOrEmpty(pictureGuid))
            {
                picture = Image.FromFile("images/user_def.jpg");
                aspectRatio = (float)picture.Width / picture.Height;
                resizedPicture = picture.GetThumbnailImage(
                    (int)(350 * aspectRatio),
                    350,
                    null,
                    IntPtr.Zero
                );

                return resizedPicture;
            }
            DocumentFileModel imageDocument = null;
            byte[] imageECard = null;
            try
            {
                imageDocument = await _documentService.Download(new Guid(pictureGuid));
                imageECard = imageDocument.Content;
                var ms = new MemoryStream(imageECard);
                picture = Image.FromStream(ms);
            }
            catch (Exception e)
            {
                picture = null;
            }
            if (picture == null)
            {
                picture = Image.FromFile("images/user_def.jpg");
            }
            aspectRatio = (float)picture.Width / picture.Height;

            resizedPicture = picture.GetThumbnailImage(
                (int)(350 * aspectRatio),
                350,
                null,
                IntPtr.Zero
            );

            //resizedPicture = picture.GetThumbnailImage(350, 350, null, IntPtr.Zero);

            return resizedPicture;
        }

        public async Task AddServiceNowData(string JsonData, DateTime startDate, DateTime endDate)
        {
            var data = new ServiceNowData()
            {
                StartDate = startDate,
                EndDate = endDate,
                JsonData = JsonData,
                Processed = false
            };
            var context = _contexFactory();
            await context.ServiceNowDatas.AddAsync(data);
            await context.SaveChangesAsync();
        }

        public async Task CreateECardForUser(
            IEnumerable<ServiceNowUserModel> UserDatas,
            string userId
        )
        {
            var ownerMail = UserDatas
                .Where(x => x.u_relationship.Equals("Employee"))
                .Select(x => x.u_email)
                .FirstOrDefault();
            var memberMail = UserDatas
                .Where(x => !string.IsNullOrEmpty(x.u_member_email_id))
                .Select(x => x.u_member_email_id)
                .ToList();

            if (string.IsNullOrEmpty(ownerMail))
                return;

            var membershipEcarsnumber = (await GetAllMembershipcardNumber()).ToHashSet();
            var userCard = GetCardsByEcardNumbers(UserDatas.Select(x => x.u_card_number).ToList());
            List<MembershipECard> ECardList = new List<MembershipECard>();

            var owner = await _applicationUserService.GetById(userId);
            List<Models.Membership> memberships = GetAllMemberships().ToList();

            var members = new List<MemberModel>();

            if (memberMail != null & memberMail.Count > 0)
            {
                members = GetMemberModels(memberMail).ToList();
            }

            foreach (var data in UserDatas)
            {
                if (!data.u_status.Equals("Active"))
                    continue;
                if (
                    !userCard.Select(x => x.ECardSequence).Contains(data.u_card_number)
                    && membershipEcarsnumber.Contains(data.u_card_number)
                )
                    continue;
                MembershipECard card = null;
                if (!userCard.Select(x => x.ECardSequence).Contains(data.u_card_number))
                    card = await createMembershipCard(data, owner, memberships);
                else
                    card = await updateMembershipCard(
                        data,
                        owner,
                        memberships,
                        userCard.FirstOrDefault(x => x.ECardSequence == data.u_card_number)
                    );

                if (data.u_data_type.ToLower().Contains("pioneer"))
                {
                    if (data.u_member_email_id.IsNullOrEmpty() && data.u_req_email.IsNullOrEmpty())
                    {
                        continue;
                    }
                    CreatePioneerCard(card, data, userId);
                    ECardList.Add(card);
                    continue;
                }
                if (
                    data.u_relationship
                        .ToLower()
                        .Equals(Declares.MembershipRelationship.Employee.ToString().ToLower())
                )
                {
                    card.isMember = false;
                }
                else
                {
                    card.isMember = true;
                    card.MemberEmail = data.u_member_email_id;
                    if (!string.IsNullOrEmpty(data.u_member_email_id) && members.Count > 0)
                    {
                        card.MemberId = members
                            .Where(x => x.mail == data.u_member_email_id)
                            .Select(x => x.Id)
                            .FirstOrDefault();
                    }
                }
                card.OwnerId = owner.Id;

                card.CreatedBy = card.OwnerId;
                ECardList.Add(card);
            }
            await AddMembershipCards(ECardList);
        }

        #region private

        private void CreatePioneerCard(
            MembershipECard card,
            ServiceNowUserModel data,
            string userId
        )
        {
            var context = _contexFactory();
            var member = context.Users.FirstOrDefault(
                x =>
                    (!data.u_member_email_id.IsNullOrEmpty() && x.Email == data.u_member_email_id)
                    || (!data.u_req_email.IsNullOrEmpty() && x.Email == data.u_req_email)
            );

            card.MemberId = member != null ? member.Id : null;
            card.MemberEmail = member != null ? member.Email : null;
            card.Name = data.u_applicant_first_name;
            card.Surname = data.u_dependent_last_name;
            card.Owner = null;
            card.OwnerId = null;
            card.isMember = true;
            card.CreatedBy = userId;
            card.CreatedOn = DateTime.UtcNow;
        }

        private HashSet<ServiceNowUserModel> DeserializedataToServiceNowUserModel(
            ServiceNowData serviceNowData
        )
        {
            IEnumerable<string> stringDatas = JsonConvert.DeserializeObject<IEnumerable<string>>(
                serviceNowData.JsonData
            );

            HashSet<ServiceNowUserModel> UsersData = new HashSet<ServiceNowUserModel>();
            foreach (var data in stringDatas)
            {
                ServiceNowUserModel userData = JsonConvert.DeserializeObject<ServiceNowUserModel>(
                    data
                );
                UsersData.Add(userData);
            }
            return UsersData;
        }

        private Dictionary<string, List<ServiceNowUserModel>> SortUserDataByFcr(
            HashSet<ServiceNowUserModel> UsersData
        )
        {
            Dictionary<string, List<ServiceNowUserModel>> FcrSortedUsersData =
                new Dictionary<string, List<ServiceNowUserModel>>();

            foreach (var user in UsersData)
            {
                if (!FcrSortedUsersData.ContainsKey(user.u_request_number_fcr))
                    FcrSortedUsersData[user.u_request_number_fcr] = new List<ServiceNowUserModel>();
                FcrSortedUsersData[user.u_request_number_fcr].Add(user);
            }
            return FcrSortedUsersData;
        }

        private async Task<IEnumerable<string>> GetAllMembershipcardNumber()
        {
            var context = _contexFactory();
            return await context.MembershipECards.Select(x => x.ECardSequence).ToListAsync();
        }

        private IEnumerable<MembershipECard> GetCardsByEcardNumbers(
            ICollection<string> eCardNumbers
        )
        {
            var context = _contexFactory();

            return context.MembershipECards
                .Where(x => eCardNumbers.Contains(x.ECardSequence))
                .ToList();
        }

        private async Task<string> uploadPicture(string base64Picture)
        {
            if (string.IsNullOrEmpty(base64Picture))
            {
                return string.Empty;
            }

            Image picture = null;

            var GuidId = Guid.NewGuid();
            byte[] imageBytes = Convert.FromBase64String(base64Picture);
            var fileType = checkType(base64Picture);
            if (fileType == "none")
                return string.Empty;

            if (fileType == "image/png")
            {
                Image pngImage = Image.FromStream(new MemoryStream(imageBytes), true);
                var jpgStream = new MemoryStream();
                pngImage.Save(jpgStream, ImageFormat.Jpeg);
                imageBytes = jpgStream.ToArray();
            }

            if (fileType == "application/pdf")
            {
                byte[] byteArray = System.Convert.FromBase64String(base64Picture);
                Spire.Pdf.PdfDocument pdf = new Spire.Pdf.PdfDocument();

                pdf.LoadFromBytes(byteArray);
                picture = pdf.SaveAsImage(0);
                fileType = "image/jpeg";
            }
            try
            {
                byte[] byteArray = System.Convert.FromBase64String(base64Picture);

                picture = Image.FromStream(new MemoryStream(byteArray));
            }
            catch
            {
                picture = null;
            }

            if (picture == null)
                return string.Empty;

            var data = await _documentService.Upload(
                imageBytes,
                GuidId,
                "ECardpicture",
                fileType,
                null
            );

            return data.Id.ToString();
        }

        private string checkType(string base64)
        {
            var signatures = new Dictionary<string, string>
            {
                ["JVBERi0"] = "application/pdf",
                ["iVBORw0KGgo"] = "image/png",
                ["/9j/"] = "image/jpg"
            };
            foreach (var s in signatures.Keys)
            {
                if (base64.IndexOf(s) == 0)
                {
                    return signatures[s];
                }
            }
            return "none";
        }

        private async Task<MembershipECard> updateMembershipCard(
            ServiceNowUserModel userHttpData,
            ApplicationUserModel applicationUserModel,
            IEnumerable<Models.Membership> membership,
            MembershipECard oldCard
        )
        {
            DateTime sysUpdate = DateTime.ParseExact(
                userHttpData.sys_updated_on,
                "dd-MM-yyyy HH:mm:ss",
                null
            );
            if (
                (sysUpdate != null && !oldCard.sys_updated_on.HasValue)
                || (
                    sysUpdate != null
                    && oldCard.sys_updated_on.HasValue
                    && sysUpdate > oldCard.sys_updated_on.Value
                )
            )
            {
                var context = _contexFactory();
                context.MembershipECards.Remove(oldCard);
                await context.SaveChangesAsync();
                return await createMembershipCard(userHttpData, applicationUserModel, membership);
            }
            return oldCard;
        }

        private async Task<MembershipECard> createMembershipCard(
            ServiceNowUserModel userHttpData,
            ApplicationUserModel applicationUserModel,
            IEnumerable<Models.Membership> membership
        )
        {
            CultureInfo provider = CultureInfo.InvariantCulture;

            var membershipId = membership
                .Where(x => x.NameEng.ToLower().Equals(userHttpData.u_plan.ToLower()))
                .FirstOrDefault()
                ?.Id;
            if (membershipId == null)
                membershipId = membership.FirstOrDefault().Id;
            var pictureId = await uploadPicture(userHttpData.u_photo);
            var newCard = new MembershipECard()
            {
                Name = userHttpData.u_applicant_first_name,
                Surname = userHttpData.u_dependent_last_name,
                IsMembershipCard = true,
                ECardSequence = userHttpData.u_card_number,
                MembershipId = membership
                    .Where(x => x.NameEng.ToLower().Equals(userHttpData.u_plan.ToLower()))
                    .FirstOrDefault()
                    .Id,
                PhotoUrl = !string.IsNullOrEmpty(pictureId) ? pictureId : null,
                ValidTo =
                    userHttpData.u_valid_to != ""
                        ? DateTime.ParseExact(userHttpData.u_valid_to, "dd-MM-yyyy", provider)
                        : DateTime.MaxValue,
                Status = userHttpData.u_status,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow,
                sys_updated_on = DateTime.ParseExact(
                    userHttpData.sys_updated_on,
                    "dd-MM-yyyy HH:mm:ss",
                    null
                )
            };
            return newCard;
        }

        private IEnumerable<MemberModel> GetMemberModels(IEnumerable<string> eMails)
        {
            var context = _contexFactory();
            var retVal = new HashSet<MemberModel>();
            retVal = context.Users
                .Where(x => eMails.Contains(x.Email))
                .Select(x => new MemberModel() { Id = x.Id, mail = x.Email })
                .ToHashSet();

            return retVal;
        }

        private IQueryable<Models.Membership> GetAllMemberships()
        {
            var context = _contexFactory();
            var temp = context.Memberships.Include(x => x.PictureData);
            var listemp = temp.ToList();
            return temp;
        }

        public async Task<DateTime> GetLastDateForServiceNowwithlog(ILogger log)
        {
            var context = _contexFactory();
            int count = await context.ServiceNowDatas.CountAsync();
            log.LogInformation("Count : " + count);

            if (count < 1)
                return DateTime.Parse("1/1/2015");

            return await context.ServiceNowDatas.MaxAsync(x => x.EndDate);
        }

        #endregion
    }
}
