using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class m1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AcceptedDomain",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Domain = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcceptedDomain", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdnocTermsAndConditions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(nullable: true),
                    ContentArabic = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdnocTermsAndConditions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AllowedEmailsForRegistration",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 1000, nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 1000, nullable: true),
                    UserType = table.Column<int>(nullable: false),
                    InviteSent = table.Column<bool>(nullable: false),
                    InviteSentOn = table.Column<DateTime>(nullable: false),
                    InviteSentBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllowedEmailsForRegistration", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Announcement",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<int>(nullable: false),
                    AllBuyers = table.Column<bool>(nullable: false),
                    AllSuppliers = table.Column<bool>(nullable: false),
                    SpecificBuyers = table.Column<bool>(nullable: false),
                    SpecificSuppliers = table.Column<bool>(nullable: false),
                    CountAllToSend = table.Column<int>(nullable: false),
                    CountFailed = table.Column<int>(nullable: false),
                    CountSent = table.Column<int>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Announcement", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Collection",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    HomeVisible = table.Column<bool>(nullable: false),
                    ValidUntil = table.Column<DateTime>(nullable: true),
                    ValidFrom = table.Column<DateTime>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Collection", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DefaultArea",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(nullable: true),
                    TitleArabic = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultArea", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DefaultLocation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(nullable: true),
                    Longitude = table.Column<string>(nullable: true),
                    Latitude = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    Vicinity = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultLocation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DiscountInfo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Service = table.Column<string>(nullable: true),
                    DiscountOrPrice = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Document",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    MimeType = table.Column<string>(nullable: true),
                    StorageType = table.Column<string>(nullable: true),
                    Size = table.Column<long>(nullable: true),
                    Content = table.Column<byte[]>(nullable: true),
                    StoragePath = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true),
                    ParentId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Document", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Document_Document_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Document",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmailTemplate",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Subject = table.Column<string>(nullable: true),
                    Body = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    Notification = table.Column<string>(nullable: true),
                    NotificationTypeId = table.Column<int>(nullable: false),
                    Sms = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailTemplate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailTemplateRoot",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    MailTemplate = table.Column<string>(nullable: true),
                    MailBodyFooter = table.Column<string>(nullable: true),
                    MailApplicationLogin = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailTemplateRoot", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExpiredTokens",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: true),
                    Token = table.Column<string>(nullable: true),
                    ExpiredAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpiredTokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LogBannerClicks",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OfferId = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogBannerClicks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LogKeywordSearch",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Keyword = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogKeywordSearch", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LogOfferClick",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OfferId = table.Column<int>(nullable: false),
                    ClickCount = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogOfferClick", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LogOfferSearch",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OfferId = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogOfferSearch", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MazayaCategories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MazayaCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MazayaPaymentgateways",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Bankref = table.Column<string>(nullable: true),
                    Device = table.Column<string>(nullable: true),
                    Deviceid = table.Column<string>(nullable: true),
                    Cardname = table.Column<string>(nullable: true),
                    Cardtype = table.Column<string>(nullable: true),
                    Cardno = table.Column<string>(nullable: true),
                    PayDate = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(28, 12)", nullable: false),
                    Paystatus = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MazayaPaymentgateways", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MembershipPictureDatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DocumentIdHorizontalPicture = table.Column<Guid>(nullable: false),
                    DocumentIdHorizontalBackPicture = table.Column<Guid>(nullable: false),
                    DocumentIdVerticalPicture = table.Column<Guid>(nullable: false),
                    DocumentIdVerticalBackPicture = table.Column<Guid>(nullable: false),
                    MembershipType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MembershipPictureDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MobileCacheDatas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MobileCacheDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotificationType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RedeemOffers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OfferId = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RedeemOffers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RefreshToken",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(nullable: true),
                    Refreshtoken = table.Column<string>(nullable: true),
                    Revoked = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshToken", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceNowDatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    JsonData = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    Processed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceNowDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SurveyForUsers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Answer = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    SurveyId = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyForUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tag",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(maxLength: 100, nullable: true),
                    Description = table.Column<string>(nullable: true),
                    IsPrivate = table.Column<bool>(nullable: false),
                    IsEditable = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 1000, nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tag", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserDomain",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DomainName = table.Column<string>(nullable: true),
                    KeyValue = table.Column<string>(nullable: true),
                    Domains = table.Column<string>(nullable: true),
                    SequencerName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDomain", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AnnouncementSpecificBuyers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnnouncementId = table.Column<int>(nullable: false),
                    BuyerType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnouncementSpecificBuyers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnnouncementSpecificBuyers_Announcement_AnnouncementId",
                        column: x => x.AnnouncementId,
                        principalTable: "Announcement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnnouncementSpecificSuppliers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnnouncementId = table.Column<int>(nullable: false),
                    SupplierCategory = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnouncementSpecificSuppliers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnnouncementSpecificSuppliers_Announcement_AnnouncementId",
                        column: x => x.AnnouncementId,
                        principalTable: "Announcement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnnouncementAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnnouncementId = table.Column<int>(nullable: false),
                    DocumentId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnouncementAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnnouncementAttachments_Announcement_AnnouncementId",
                        column: x => x.AnnouncementId,
                        principalTable: "Announcement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnnouncementAttachments_Document_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Document",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CategoryDocument",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(nullable: false),
                    DocumentId = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 1000, nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 1000, nullable: true),
                    Type = table.Column<int>(nullable: false),
                    OriginalImageId = table.Column<Guid>(nullable: false),
                    X1 = table.Column<double>(nullable: false),
                    Y1 = table.Column<double>(nullable: false),
                    X2 = table.Column<double>(nullable: false),
                    Y2 = table.Column<double>(nullable: false),
                    cropX1 = table.Column<double>(nullable: false),
                    cropY1 = table.Column<double>(nullable: false),
                    cropX2 = table.Column<double>(nullable: false),
                    cropY2 = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryDocument", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoryDocument_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryDocument_Document_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Document",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CollectionDocument",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CollectionId = table.Column<int>(nullable: false),
                    DocumentId = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 1000, nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 1000, nullable: true),
                    Type = table.Column<int>(nullable: false),
                    OriginalImageId = table.Column<Guid>(nullable: false),
                    X1 = table.Column<double>(nullable: false),
                    Y1 = table.Column<double>(nullable: false),
                    X2 = table.Column<double>(nullable: false),
                    Y2 = table.Column<double>(nullable: false),
                    cropX1 = table.Column<double>(nullable: false),
                    cropY1 = table.Column<double>(nullable: false),
                    cropX2 = table.Column<double>(nullable: false),
                    cropY2 = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionDocument", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CollectionDocument_Collection_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "Collection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CollectionDocument_Document_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Document",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyDocument",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentId = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 1000, nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyDocument", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyDocument_Document_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Document",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MazayaSubcategories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(28, 12)", nullable: false),
                    NoofChildren = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    MazayaCategoryId = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MazayaSubcategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MazayaSubcategories_MazayaCategories_MazayaCategoryId",
                        column: x => x.MazayaCategoryId,
                        principalTable: "MazayaCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Memberships",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    NameEng = table.Column<string>(nullable: true),
                    NameAr = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    PictureDataId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Memberships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Memberships_MembershipPictureDatas_PictureDataId",
                        column: x => x.PictureDataId,
                        principalTable: "MembershipPictureDatas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Active = table.Column<bool>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true),
                    LastDataSynchronizationOn = table.Column<DateTime>(nullable: false),
                    UserType = table.Column<int>(nullable: false),
                    ECardSequence = table.Column<string>(nullable: true),
                    PlatformType = table.Column<string>(nullable: true),
                    ReceiveAnnouncement = table.Column<bool>(nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_UserDomain_UserType",
                        column: x => x.UserType,
                        principalTable: "UserDomain",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Company",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameEnglish = table.Column<string>(nullable: true),
                    NameArabic = table.Column<string>(nullable: true),
                    Website = table.Column<string>(nullable: true),
                    OfficialEmail = table.Column<string>(nullable: true),
                    CompanyDescription = table.Column<string>(nullable: true),
                    MobileCountryCode = table.Column<string>(nullable: true),
                    MobileE164Number = table.Column<string>(nullable: true),
                    Mobile = table.Column<string>(nullable: true),
                    MobileNumber = table.Column<string>(nullable: true),
                    LandCountryCode = table.Column<string>(nullable: true),
                    LandE164Number = table.Column<string>(nullable: true),
                    Land = table.Column<string>(nullable: true),
                    LandNumber = table.Column<string>(nullable: true),
                    FaxCountryCode = table.Column<string>(nullable: true),
                    FaxE164Number = table.Column<string>(nullable: true),
                    Fax = table.Column<string>(nullable: true),
                    FaxNumber = table.Column<string>(nullable: true),
                    LogoId = table.Column<int>(nullable: true),
                    LegalForm = table.Column<string>(nullable: true),
                    POBox = table.Column<string>(nullable: true),
                    Facebook = table.Column<string>(nullable: true),
                    Instagram = table.Column<string>(nullable: true),
                    Twitter = table.Column<string>(nullable: true),
                    IDforADCCI = table.Column<string>(nullable: true),
                    LicenseNo = table.Column<string>(nullable: true),
                    CompanyNationality = table.Column<string>(nullable: true),
                    EstablishDate = table.Column<DateTime>(nullable: false),
                    ExpiryDate = table.Column<DateTime>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true),
                    ApproveStatus = table.Column<string>(nullable: true),
                    ApprovedBy = table.Column<string>(nullable: true),
                    ApprovedOn = table.Column<DateTime>(nullable: false),
                    TradeLicenseExpDate = table.Column<DateTime>(nullable: false),
                    TradeLicenceId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Company_CompanyDocument_LogoId",
                        column: x => x.LogoId,
                        principalTable: "CompanyDocument",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Company_Document_TradeLicenceId",
                        column: x => x.TradeLicenceId,
                        principalTable: "Document",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MazayaPackageSubscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationUserId = table.Column<string>(nullable: true),
                    SubCategoryId = table.Column<int>(nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(28, 12)", nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MazayaPackageSubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MazayaPackageSubscriptions_MazayaSubcategories_SubCategoryId",
                        column: x => x.SubCategoryId,
                        principalTable: "MazayaSubcategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationUserDocument",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationUserId = table.Column<string>(nullable: true),
                    DocumentId = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 1000, nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 1000, nullable: true),
                    Type = table.Column<int>(nullable: false),
                    OriginalImageId = table.Column<Guid>(nullable: false),
                    X1 = table.Column<double>(nullable: false),
                    Y1 = table.Column<double>(nullable: false),
                    X2 = table.Column<double>(nullable: false),
                    Y2 = table.Column<double>(nullable: false),
                    cropX1 = table.Column<double>(nullable: false),
                    cropY1 = table.Column<double>(nullable: false),
                    cropX2 = table.Column<double>(nullable: false),
                    cropY2 = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserDocument", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationUserDocument_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApplicationUserDocument_Document_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Document",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MembershipECards",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerId = table.Column<string>(nullable: true),
                    MemberId = table.Column<string>(nullable: true),
                    MemberEmail = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Surname = table.Column<string>(nullable: true),
                    ECardSequence = table.Column<string>(nullable: true),
                    MembershipType = table.Column<int>(nullable: false),
                    IsMembershipCard = table.Column<bool>(nullable: false),
                    PhotoUrl = table.Column<string>(nullable: true),
                    isMember = table.Column<bool>(nullable: false),
                    MembershipId = table.Column<Guid>(nullable: true),
                    ValidTo = table.Column<DateTime>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true),
                    sys_updated_on = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MembershipECards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MembershipECards_Memberships_MembershipId",
                        column: x => x.MembershipId,
                        principalTable: "Memberships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MembershipECards_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OfferSuggestions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: true),
                    Text = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfferSuggestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OfferSuggestions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Surveys",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(nullable: true),
                    Start = table.Column<DateTime>(nullable: true),
                    End = table.Column<DateTime>(nullable: true),
                    AdminId = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    UsersId = table.Column<string>(nullable: true),
                    UserTypes = table.Column<string>(nullable: true),
                    UserRoles = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    Questions = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ForAllUsers = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true),
                    Opportunity = table.Column<int>(nullable: false),
                    IsCreateMail = table.Column<bool>(nullable: false),
                    IsQuickSurvey = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Surveys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Surveys_AspNetUsers_AdminId",
                        column: x => x.AdminId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserFcmTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    FcmMessageToken = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFcmTokens", x => new { x.UserId, x.FcmMessageToken });
                    table.ForeignKey(
                        name: "FK_UserFcmTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserInvitations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvitedUserEmail = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true),
                    UserType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInvitations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserInvitations_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserInvitations_UserDomain_UserType",
                        column: x => x.UserType,
                        principalTable: "UserDomain",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserNotification",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: true),
                    NotificationTypeId = table.Column<int>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    URL = table.Column<string>(nullable: true),
                    Acknowledged = table.Column<bool>(nullable: false),
                    AcknowledgedOn = table.Column<DateTime>(nullable: true),
                    OfferId = table.Column<int>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNotification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserNotification_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompanyActivity",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    CompanyId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyActivity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyActivity_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyCategory",
                columns: table => new
                {
                    CompanyId = table.Column<int>(nullable: false),
                    CategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyCategory", x => new { x.CompanyId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_CompanyCategory_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyCategory_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyLocation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Longitude = table.Column<string>(nullable: true),
                    Latitude = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    Vicinity = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    CompanyId = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyLocation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyLocation_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyPartner",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    CompanyId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyPartner", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyPartner_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanySuppliers",
                columns: table => new
                {
                    CompanyId = table.Column<int>(nullable: false),
                    SupplierId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanySuppliers", x => new { x.CompanyId, x.SupplierId });
                    table.ForeignKey(
                        name: "FK_CompanySuppliers_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanySuppliers_AspNetUsers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Offer",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(nullable: true),
                    Brand = table.Column<string>(nullable: true),
                    CompanyId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    PromotionCode = table.Column<string>(nullable: true),
                    PriceFrom = table.Column<decimal>(type: "decimal(28, 12)", nullable: true),
                    PriceTo = table.Column<decimal>(type: "decimal(28, 12)", nullable: true),
                    OriginalPrice = table.Column<decimal>(type: "decimal(28, 12)", nullable: true),
                    DiscountedPrice = table.Column<decimal>(type: "decimal(28, 12)", nullable: true),
                    DiscountFrom = table.Column<decimal>(type: "decimal(28, 12)", nullable: true),
                    DiscountTo = table.Column<decimal>(type: "decimal(28, 12)", nullable: true),
                    PriceCustom = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    Discount = table.Column<decimal>(type: "decimal(28, 12)", nullable: true),
                    ValidFrom = table.Column<DateTime>(nullable: true),
                    ValidUntil = table.Column<DateTime>(nullable: true),
                    WhatYouGet = table.Column<string>(nullable: true),
                    PriceList = table.Column<string>(nullable: true),
                    TermsAndCondition = table.Column<string>(nullable: true),
                    AboutCompany = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true),
                    FlagIsLatest = table.Column<bool>(nullable: false),
                    FlagIsWeekendOffer = table.Column<bool>(nullable: false),
                    ReviewedOn = table.Column<DateTime>(nullable: false),
                    ReviewedBy = table.Column<string>(nullable: true),
                    DecisionOn = table.Column<DateTime>(nullable: false),
                    DecisionBy = table.Column<string>(nullable: true),
                    BannerActive = table.Column<bool>(nullable: true),
                    BannerUrl = table.Column<string>(nullable: true),
                    AnnouncementActive = table.Column<bool>(nullable: true),
                    MembershipType = table.Column<int>(nullable: false),
                    CountryCode = table.Column<string>(nullable: true),
                    E164Number = table.Column<string>(nullable: true),
                    InternationalNumber = table.Column<string>(nullable: true),
                    Number = table.Column<string>(nullable: true),
                    ReportCount = table.Column<int>(nullable: false),
                    SpecialAnnouncement = table.Column<Guid>(nullable: true),
                    IsPrivate = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Offer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Offer_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Roadshow",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    CompanyId = table.Column<int>(nullable: true),
                    DateFrom = table.Column<DateTime>(nullable: true),
                    DateTo = table.Column<DateTime>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Activities = table.Column<string>(nullable: true),
                    InstructionBox = table.Column<string>(nullable: true),
                    FocalPointName = table.Column<string>(nullable: true),
                    FocalPointSurname = table.Column<string>(nullable: true),
                    FocalPointEmail = table.Column<string>(nullable: true),
                    CountryCode = table.Column<string>(nullable: true),
                    E164Number = table.Column<string>(nullable: true),
                    InternationalNumber = table.Column<string>(nullable: true),
                    Number = table.Column<string>(nullable: true),
                    EmiratesId = table.Column<Guid>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 1000, nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roadshow", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Roadshow_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Roadshow_Document_EmiratesId",
                        column: x => x.EmiratesId,
                        principalTable: "Document",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoadshowProposal",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<int>(nullable: false),
                    RoadshowDetails = table.Column<string>(nullable: true),
                    EquipmentItem = table.Column<string>(nullable: true),
                    Subject = table.Column<string>(nullable: true),
                    CompanyId = table.Column<int>(nullable: true),
                    TermsAndCondition = table.Column<string>(nullable: true),
                    TermsAndConditionChecked = table.Column<bool>(nullable: false),
                    OfferEffectiveDate = table.Column<DateTime>(nullable: false),
                    ExpiryDate = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Signature = table.Column<string>(nullable: true),
                    Manager = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadshowProposal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoadshowProposal_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Comment",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 1000, nullable: true),
                    OfferId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comment_Offer_OfferId",
                        column: x => x.OfferId,
                        principalTable: "Offer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MailStorage",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: true),
                    Subject = table.Column<string>(nullable: true),
                    Body = table.Column<string>(nullable: true),
                    OfferId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    StatusId = table.Column<int>(nullable: false),
                    StatusOn = table.Column<DateTime>(nullable: false),
                    StatusNote = table.Column<string>(nullable: true),
                    UserEmail = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true),
                    AnnouncementId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailStorage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MailStorage_Announcement_AnnouncementId",
                        column: x => x.AnnouncementId,
                        principalTable: "Announcement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MailStorage_Offer_OfferId",
                        column: x => x.OfferId,
                        principalTable: "Offer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MailStorage_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OfferCategory",
                columns: table => new
                {
                    OfferId = table.Column<int>(nullable: false),
                    CategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfferCategory", x => new { x.OfferId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_OfferCategory_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OfferCategory_Offer_OfferId",
                        column: x => x.OfferId,
                        principalTable: "Offer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OfferCollection",
                columns: table => new
                {
                    OfferId = table.Column<int>(nullable: false),
                    CollectionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfferCollection", x => new { x.OfferId, x.CollectionId });
                    table.ForeignKey(
                        name: "FK_OfferCollection_Collection_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "Collection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OfferCollection_Offer_OfferId",
                        column: x => x.OfferId,
                        principalTable: "Offer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OfferDocument",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OfferId = table.Column<int>(nullable: false),
                    DocumentId = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 1000, nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 1000, nullable: true),
                    Type = table.Column<int>(nullable: false),
                    OriginalImageId = table.Column<Guid>(nullable: false),
                    X1 = table.Column<double>(nullable: false),
                    Y1 = table.Column<double>(nullable: false),
                    X2 = table.Column<double>(nullable: false),
                    Y2 = table.Column<double>(nullable: false),
                    cropX1 = table.Column<double>(nullable: false),
                    cropY1 = table.Column<double>(nullable: false),
                    cropX2 = table.Column<double>(nullable: false),
                    cropY2 = table.Column<double>(nullable: false),
                    Cover = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfferDocument", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OfferDocument_Document_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Document",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OfferDocument_Offer_OfferId",
                        column: x => x.OfferId,
                        principalTable: "Offer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OfferLocation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Longitude = table.Column<string>(nullable: true),
                    Latitude = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    Vicinity = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    OfferId = table.Column<int>(nullable: false),
                    DefaultAreaId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfferLocation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OfferLocation_DefaultArea_DefaultAreaId",
                        column: x => x.DefaultAreaId,
                        principalTable: "DefaultArea",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OfferLocation_Offer_OfferId",
                        column: x => x.OfferId,
                        principalTable: "Offer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OfferRating",
                columns: table => new
                {
                    OfferId = table.Column<int>(nullable: false),
                    ApplicationUserId = table.Column<string>(nullable: false),
                    Rating = table.Column<decimal>(type: "decimal(28, 12)", nullable: false),
                    CommentText = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfferRating", x => new { x.OfferId, x.ApplicationUserId });
                    table.ForeignKey(
                        name: "FK_OfferRating_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OfferRating_Offer_OfferId",
                        column: x => x.OfferId,
                        principalTable: "Offer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OfferReport",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OfferId = table.Column<int>(nullable: false),
                    ReportType = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true),
                    isResolved = table.Column<bool>(nullable: false),
                    ResolvedOn = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfferReport", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OfferReport_Offer_OfferId",
                        column: x => x.OfferId,
                        principalTable: "Offer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OffersMemberships",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    OfferId = table.Column<int>(nullable: false),
                    MembershipId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OffersMemberships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OffersMemberships_Memberships_MembershipId",
                        column: x => x.MembershipId,
                        principalTable: "Memberships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OffersMemberships_Offer_OfferId",
                        column: x => x.OfferId,
                        principalTable: "Offer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OfferTag",
                columns: table => new
                {
                    OfferId = table.Column<int>(nullable: false),
                    TagId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfferTag", x => new { x.OfferId, x.TagId });
                    table.ForeignKey(
                        name: "FK_OfferTag_Offer_OfferId",
                        column: x => x.OfferId,
                        principalTable: "Offer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OfferTag_Tag_TagId",
                        column: x => x.TagId,
                        principalTable: "Tag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserFavouritesOffers",
                columns: table => new
                {
                    OfferId = table.Column<int>(nullable: false),
                    ApplicationUserId = table.Column<string>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsFavourite = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFavouritesOffers", x => new { x.OfferId, x.ApplicationUserId });
                    table.ForeignKey(
                        name: "FK_UserFavouritesOffers_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFavouritesOffers_Offer_OfferId",
                        column: x => x.OfferId,
                        principalTable: "Offer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoadshowComment",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 1000, nullable: true),
                    CreatedByName = table.Column<string>(nullable: true),
                    RoadshowId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadshowComment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoadshowComment_Roadshow_RoadshowId",
                        column: x => x.RoadshowId,
                        principalTable: "Roadshow",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoadshowDocument",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoadshowId = table.Column<int>(nullable: false),
                    DocumentId = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 1000, nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 1000, nullable: true),
                    Type = table.Column<int>(nullable: false),
                    OriginalImageId = table.Column<Guid>(nullable: false),
                    X1 = table.Column<double>(nullable: false),
                    Y1 = table.Column<double>(nullable: false),
                    X2 = table.Column<double>(nullable: false),
                    Y2 = table.Column<double>(nullable: false),
                    cropX1 = table.Column<double>(nullable: false),
                    cropY1 = table.Column<double>(nullable: false),
                    cropX2 = table.Column<double>(nullable: false),
                    cropY2 = table.Column<double>(nullable: false),
                    Cover = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadshowDocument", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoadshowDocument_Document_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Document",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoadshowDocument_Roadshow_RoadshowId",
                        column: x => x.RoadshowId,
                        principalTable: "Roadshow",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoadshowInvite",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true),
                    RoadshowId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadshowInvite", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoadshowInvite_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoadshowInvite_Roadshow_RoadshowId",
                        column: x => x.RoadshowId,
                        principalTable: "Roadshow",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoadshowLocation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DefaultLocationId = table.Column<int>(nullable: false),
                    RoadshowId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadshowLocation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoadshowLocation_DefaultLocation_DefaultLocationId",
                        column: x => x.DefaultLocationId,
                        principalTable: "DefaultLocation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoadshowLocation_Roadshow_RoadshowId",
                        column: x => x.RoadshowId,
                        principalTable: "Roadshow",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoadshowOffer",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(nullable: true),
                    RoadshowProposalId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    RoadshowDetails = table.Column<string>(nullable: true),
                    EquipmentItem = table.Column<string>(nullable: true),
                    PromotionCode = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadshowOffer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoadshowOffer_RoadshowProposal_RoadshowProposalId",
                        column: x => x.RoadshowProposalId,
                        principalTable: "RoadshowProposal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoadshowOfferProposalDocument",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoadshowOfferProposalId = table.Column<int>(nullable: false),
                    DocumentId = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 1000, nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 1000, nullable: true),
                    Type = table.Column<int>(nullable: false),
                    RoadshowProposalId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadshowOfferProposalDocument", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoadshowOfferProposalDocument_Document_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Document",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoadshowOfferProposalDocument_RoadshowProposal_RoadshowProposalId",
                        column: x => x.RoadshowProposalId,
                        principalTable: "RoadshowProposal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MailStorageDocument",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MailStorageId = table.Column<int>(nullable: false),
                    DocumentId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailStorageDocument", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MailStorageDocument_Document_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Document",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MailStorageDocument_MailStorage_MailStorageId",
                        column: x => x.MailStorageId,
                        principalTable: "MailStorage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoadshowEvent",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DefaultLocationId = table.Column<int>(nullable: true),
                    RoadshowInviteId = table.Column<int>(nullable: false),
                    DateFrom = table.Column<DateTime>(nullable: false),
                    DateTo = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadshowEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoadshowEvent_DefaultLocation_DefaultLocationId",
                        column: x => x.DefaultLocationId,
                        principalTable: "DefaultLocation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoadshowEvent_RoadshowInvite_RoadshowInviteId",
                        column: x => x.RoadshowInviteId,
                        principalTable: "RoadshowInvite",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoadshowOfferCategory",
                columns: table => new
                {
                    RoadshowOfferId = table.Column<int>(nullable: false),
                    CategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadshowOfferCategory", x => new { x.RoadshowOfferId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_RoadshowOfferCategory_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoadshowOfferCategory_RoadshowOffer_RoadshowOfferId",
                        column: x => x.RoadshowOfferId,
                        principalTable: "RoadshowOffer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoadshowOfferCollection",
                columns: table => new
                {
                    RoadshowOfferId = table.Column<int>(nullable: false),
                    CollectionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadshowOfferCollection", x => new { x.RoadshowOfferId, x.CollectionId });
                    table.ForeignKey(
                        name: "FK_RoadshowOfferCollection_Collection_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "Collection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoadshowOfferCollection_RoadshowOffer_RoadshowOfferId",
                        column: x => x.RoadshowOfferId,
                        principalTable: "RoadshowOffer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoadshowOfferDocument",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoadshowOfferId = table.Column<int>(nullable: false),
                    DocumentId = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 1000, nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 1000, nullable: true),
                    Type = table.Column<int>(nullable: false),
                    OriginalImageId = table.Column<Guid>(nullable: false),
                    X1 = table.Column<double>(nullable: false),
                    Y1 = table.Column<double>(nullable: false),
                    X2 = table.Column<double>(nullable: false),
                    Y2 = table.Column<double>(nullable: false),
                    cropX1 = table.Column<double>(nullable: false),
                    cropY1 = table.Column<double>(nullable: false),
                    cropX2 = table.Column<double>(nullable: false),
                    cropY2 = table.Column<double>(nullable: false),
                    Cover = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadshowOfferDocument", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoadshowOfferDocument_Document_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Document",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoadshowOfferDocument_RoadshowOffer_RoadshowOfferId",
                        column: x => x.RoadshowOfferId,
                        principalTable: "RoadshowOffer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoadshowOfferRating",
                columns: table => new
                {
                    RoadshowOfferId = table.Column<int>(nullable: false),
                    ApplicationUserId = table.Column<string>(nullable: false),
                    Rating = table.Column<decimal>(type: "decimal(28, 12)", nullable: false),
                    CommentText = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadshowOfferRating", x => new { x.RoadshowOfferId, x.ApplicationUserId });
                    table.ForeignKey(
                        name: "FK_RoadshowOfferRating_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoadshowOfferRating_RoadshowOffer_RoadshowOfferId",
                        column: x => x.RoadshowOfferId,
                        principalTable: "RoadshowOffer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoadshowOfferTag",
                columns: table => new
                {
                    RoadshowOfferId = table.Column<int>(nullable: false),
                    TagId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadshowOfferTag", x => new { x.RoadshowOfferId, x.TagId });
                    table.ForeignKey(
                        name: "FK_RoadshowOfferTag_RoadshowOffer_RoadshowOfferId",
                        column: x => x.RoadshowOfferId,
                        principalTable: "RoadshowOffer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoadshowOfferTag_Tag_TagId",
                        column: x => x.TagId,
                        principalTable: "Tag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoadshowVoucher",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoadshowOfferId = table.Column<int>(nullable: true),
                    RoadshowProposalId = table.Column<int>(nullable: true),
                    RoadshowId = table.Column<int>(nullable: true),
                    Quantity = table.Column<int>(nullable: false),
                    Validity = table.Column<DateTime>(nullable: false),
                    Details = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadshowVoucher", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoadshowVoucher_Roadshow_RoadshowId",
                        column: x => x.RoadshowId,
                        principalTable: "Roadshow",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoadshowVoucher_RoadshowOffer_RoadshowOfferId",
                        column: x => x.RoadshowOfferId,
                        principalTable: "RoadshowOffer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoadshowVoucher_RoadshowProposal_RoadshowProposalId",
                        column: x => x.RoadshowProposalId,
                        principalTable: "RoadshowProposal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserFavouritesRoadshowOffer",
                columns: table => new
                {
                    RoadshowOfferId = table.Column<int>(nullable: false),
                    ApplicationUserId = table.Column<string>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsFavourite = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFavouritesRoadshowOffer", x => new { x.RoadshowOfferId, x.ApplicationUserId });
                    table.ForeignKey(
                        name: "FK_UserFavouritesRoadshowOffer_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFavouritesRoadshowOffer_RoadshowOffer_RoadshowOfferId",
                        column: x => x.RoadshowOfferId,
                        principalTable: "RoadshowOffer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoadshowEventOffer",
                columns: table => new
                {
                    RoadshowEventId = table.Column<int>(nullable: false),
                    RoadshowOfferId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadshowEventOffer", x => new { x.RoadshowOfferId, x.RoadshowEventId });
                    table.ForeignKey(
                        name: "FK_RoadshowEventOffer_RoadshowEvent_RoadshowEventId",
                        column: x => x.RoadshowEventId,
                        principalTable: "RoadshowEvent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoadshowEventOffer_RoadshowOffer_RoadshowOfferId",
                        column: x => x.RoadshowOfferId,
                        principalTable: "RoadshowOffer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Tag",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "Description", "IsEditable", "IsPrivate", "Title", "UpdatedBy", "UpdatedOn" },
                values: new object[,]
                {
                    { 1, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, false, "Special Offer", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 11, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, false, "Any Other Tag", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 10, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, false, "Best Rates", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 8, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, false, "Upcoming", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 7, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, false, "Ending Soon", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 9, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, false, "Latest", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, false, "Featured", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, false, "Top Seller", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, false, "Exclusive Offer", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, false, "Best Price", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 6, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, false, "Trending", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "UserDomain",
                columns: new[] { "Id", "DomainName", "Domains", "KeyValue", "SequencerName" },
                values: new object[,]
                {
                    { 6, "ADSchools", "", "1971", "dbo.DefaultSequencer" },
                    { 1, "ADNOCEmployee", "@adnoc;", "1971", "dbo.ADNOCEmployeeSequencer" },
                    { 2, "ADNOCEmployeeFamilyMember", "", "1971", "dbo.ADNOCEmployeeSequencer" },
                    { 3, "ADPolice", "", "1957", "dbo.ADPoliceSequencer" },
                    { 4, "RedCrescent", "", "1983", "dbo.RedCrescentSequencer" },
                    { 5, "AlumniRetirementMembers", "", "2018", "dbo.AlumniRetirementMembersSequencer" },
                    { 7, "Other", "", "1971", "dbo.DefaultSequencer" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnnouncementAttachments_AnnouncementId",
                table: "AnnouncementAttachments",
                column: "AnnouncementId");

            migrationBuilder.CreateIndex(
                name: "IX_AnnouncementAttachments_DocumentId",
                table: "AnnouncementAttachments",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_AnnouncementSpecificBuyers_AnnouncementId",
                table: "AnnouncementSpecificBuyers",
                column: "AnnouncementId");

            migrationBuilder.CreateIndex(
                name: "IX_AnnouncementSpecificSuppliers_AnnouncementId",
                table: "AnnouncementSpecificSuppliers",
                column: "AnnouncementId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserDocument_ApplicationUserId",
                table: "ApplicationUserDocument",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserDocument_DocumentId",
                table: "ApplicationUserDocument",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UserType",
                table: "AspNetUsers",
                column: "UserType");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryDocument_CategoryId",
                table: "CategoryDocument",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryDocument_DocumentId",
                table: "CategoryDocument",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionDocument_CollectionId",
                table: "CollectionDocument",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionDocument_DocumentId",
                table: "CollectionDocument",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_OfferId",
                table: "Comment",
                column: "OfferId");

            migrationBuilder.CreateIndex(
                name: "IX_Company_LogoId",
                table: "Company",
                column: "LogoId");

            migrationBuilder.CreateIndex(
                name: "IX_Company_TradeLicenceId",
                table: "Company",
                column: "TradeLicenceId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyActivity_CompanyId",
                table: "CompanyActivity",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyCategory_CategoryId",
                table: "CompanyCategory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyDocument_DocumentId",
                table: "CompanyDocument",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyLocation_CompanyId",
                table: "CompanyLocation",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyPartner_CompanyId",
                table: "CompanyPartner",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanySuppliers_SupplierId",
                table: "CompanySuppliers",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_Document_ParentId",
                table: "Document",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_MailStorage_AnnouncementId",
                table: "MailStorage",
                column: "AnnouncementId");

            migrationBuilder.CreateIndex(
                name: "IX_MailStorage_OfferId",
                table: "MailStorage",
                column: "OfferId");

            migrationBuilder.CreateIndex(
                name: "IX_MailStorage_UserId",
                table: "MailStorage",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MailStorageDocument_DocumentId",
                table: "MailStorageDocument",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_MailStorageDocument_MailStorageId",
                table: "MailStorageDocument",
                column: "MailStorageId");

            migrationBuilder.CreateIndex(
                name: "IX_MazayaPackageSubscriptions_SubCategoryId",
                table: "MazayaPackageSubscriptions",
                column: "SubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_MazayaSubcategories_MazayaCategoryId",
                table: "MazayaSubcategories",
                column: "MazayaCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_MembershipECards_MembershipId",
                table: "MembershipECards",
                column: "MembershipId");

            migrationBuilder.CreateIndex(
                name: "IX_MembershipECards_OwnerId",
                table: "MembershipECards",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Memberships_PictureDataId",
                table: "Memberships",
                column: "PictureDataId");

            migrationBuilder.CreateIndex(
                name: "IX_Offer_CompanyId",
                table: "Offer",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferCategory_CategoryId",
                table: "OfferCategory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferCollection_CollectionId",
                table: "OfferCollection",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferDocument_DocumentId",
                table: "OfferDocument",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferDocument_OfferId",
                table: "OfferDocument",
                column: "OfferId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferLocation_DefaultAreaId",
                table: "OfferLocation",
                column: "DefaultAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferLocation_OfferId",
                table: "OfferLocation",
                column: "OfferId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferRating_ApplicationUserId",
                table: "OfferRating",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferReport_OfferId",
                table: "OfferReport",
                column: "OfferId");

            migrationBuilder.CreateIndex(
                name: "IX_OffersMemberships_MembershipId",
                table: "OffersMemberships",
                column: "MembershipId");

            migrationBuilder.CreateIndex(
                name: "IX_OffersMemberships_OfferId",
                table: "OffersMemberships",
                column: "OfferId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferSuggestions_UserId",
                table: "OfferSuggestions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferTag_TagId",
                table: "OfferTag",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Roadshow_CompanyId",
                table: "Roadshow",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Roadshow_EmiratesId",
                table: "Roadshow",
                column: "EmiratesId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowComment_RoadshowId",
                table: "RoadshowComment",
                column: "RoadshowId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowDocument_DocumentId",
                table: "RoadshowDocument",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowDocument_RoadshowId",
                table: "RoadshowDocument",
                column: "RoadshowId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowEvent_DefaultLocationId",
                table: "RoadshowEvent",
                column: "DefaultLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowEvent_RoadshowInviteId",
                table: "RoadshowEvent",
                column: "RoadshowInviteId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowEventOffer_RoadshowEventId",
                table: "RoadshowEventOffer",
                column: "RoadshowEventId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowInvite_CompanyId",
                table: "RoadshowInvite",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowInvite_RoadshowId",
                table: "RoadshowInvite",
                column: "RoadshowId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowLocation_DefaultLocationId",
                table: "RoadshowLocation",
                column: "DefaultLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowLocation_RoadshowId",
                table: "RoadshowLocation",
                column: "RoadshowId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowOffer_RoadshowProposalId",
                table: "RoadshowOffer",
                column: "RoadshowProposalId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowOfferCategory_CategoryId",
                table: "RoadshowOfferCategory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowOfferCollection_CollectionId",
                table: "RoadshowOfferCollection",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowOfferDocument_DocumentId",
                table: "RoadshowOfferDocument",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowOfferDocument_RoadshowOfferId",
                table: "RoadshowOfferDocument",
                column: "RoadshowOfferId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowOfferProposalDocument_DocumentId",
                table: "RoadshowOfferProposalDocument",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowOfferProposalDocument_RoadshowProposalId",
                table: "RoadshowOfferProposalDocument",
                column: "RoadshowProposalId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowOfferRating_ApplicationUserId",
                table: "RoadshowOfferRating",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowOfferTag_TagId",
                table: "RoadshowOfferTag",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowProposal_CompanyId",
                table: "RoadshowProposal",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowVoucher_RoadshowId",
                table: "RoadshowVoucher",
                column: "RoadshowId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowVoucher_RoadshowOfferId",
                table: "RoadshowVoucher",
                column: "RoadshowOfferId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowVoucher_RoadshowProposalId",
                table: "RoadshowVoucher",
                column: "RoadshowProposalId");

            migrationBuilder.CreateIndex(
                name: "IX_Surveys_AdminId",
                table: "Surveys",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFavouritesOffers_ApplicationUserId",
                table: "UserFavouritesOffers",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFavouritesRoadshowOffer_ApplicationUserId",
                table: "UserFavouritesRoadshowOffer",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInvitations_UserId",
                table: "UserInvitations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInvitations_UserType",
                table: "UserInvitations",
                column: "UserType");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotification_UserId",
                table: "UserNotification",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcceptedDomain");

            migrationBuilder.DropTable(
                name: "AdnocTermsAndConditions");

            migrationBuilder.DropTable(
                name: "AllowedEmailsForRegistration");

            migrationBuilder.DropTable(
                name: "AnnouncementAttachments");

            migrationBuilder.DropTable(
                name: "AnnouncementSpecificBuyers");

            migrationBuilder.DropTable(
                name: "AnnouncementSpecificSuppliers");

            migrationBuilder.DropTable(
                name: "ApplicationUserDocument");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CategoryDocument");

            migrationBuilder.DropTable(
                name: "CollectionDocument");

            migrationBuilder.DropTable(
                name: "Comment");

            migrationBuilder.DropTable(
                name: "CompanyActivity");

            migrationBuilder.DropTable(
                name: "CompanyCategory");

            migrationBuilder.DropTable(
                name: "CompanyLocation");

            migrationBuilder.DropTable(
                name: "CompanyPartner");

            migrationBuilder.DropTable(
                name: "CompanySuppliers");

            migrationBuilder.DropTable(
                name: "DiscountInfo");

            migrationBuilder.DropTable(
                name: "EmailTemplate");

            migrationBuilder.DropTable(
                name: "EmailTemplateRoot");

            migrationBuilder.DropTable(
                name: "ExpiredTokens");

            migrationBuilder.DropTable(
                name: "LogBannerClicks");

            migrationBuilder.DropTable(
                name: "LogKeywordSearch");

            migrationBuilder.DropTable(
                name: "LogOfferClick");

            migrationBuilder.DropTable(
                name: "LogOfferSearch");

            migrationBuilder.DropTable(
                name: "MailStorageDocument");

            migrationBuilder.DropTable(
                name: "MazayaPackageSubscriptions");

            migrationBuilder.DropTable(
                name: "MazayaPaymentgateways");

            migrationBuilder.DropTable(
                name: "MembershipECards");

            migrationBuilder.DropTable(
                name: "MobileCacheDatas");

            migrationBuilder.DropTable(
                name: "NotificationType");

            migrationBuilder.DropTable(
                name: "OfferCategory");

            migrationBuilder.DropTable(
                name: "OfferCollection");

            migrationBuilder.DropTable(
                name: "OfferDocument");

            migrationBuilder.DropTable(
                name: "OfferLocation");

            migrationBuilder.DropTable(
                name: "OfferRating");

            migrationBuilder.DropTable(
                name: "OfferReport");

            migrationBuilder.DropTable(
                name: "OffersMemberships");

            migrationBuilder.DropTable(
                name: "OfferSuggestions");

            migrationBuilder.DropTable(
                name: "OfferTag");

            migrationBuilder.DropTable(
                name: "RedeemOffers");

            migrationBuilder.DropTable(
                name: "RefreshToken");

            migrationBuilder.DropTable(
                name: "RoadshowComment");

            migrationBuilder.DropTable(
                name: "RoadshowDocument");

            migrationBuilder.DropTable(
                name: "RoadshowEventOffer");

            migrationBuilder.DropTable(
                name: "RoadshowLocation");

            migrationBuilder.DropTable(
                name: "RoadshowOfferCategory");

            migrationBuilder.DropTable(
                name: "RoadshowOfferCollection");

            migrationBuilder.DropTable(
                name: "RoadshowOfferDocument");

            migrationBuilder.DropTable(
                name: "RoadshowOfferProposalDocument");

            migrationBuilder.DropTable(
                name: "RoadshowOfferRating");

            migrationBuilder.DropTable(
                name: "RoadshowOfferTag");

            migrationBuilder.DropTable(
                name: "RoadshowVoucher");

            migrationBuilder.DropTable(
                name: "ServiceNowDatas");

            migrationBuilder.DropTable(
                name: "SurveyForUsers");

            migrationBuilder.DropTable(
                name: "Surveys");

            migrationBuilder.DropTable(
                name: "UserFavouritesOffers");

            migrationBuilder.DropTable(
                name: "UserFavouritesRoadshowOffer");

            migrationBuilder.DropTable(
                name: "UserFcmTokens");

            migrationBuilder.DropTable(
                name: "UserInvitations");

            migrationBuilder.DropTable(
                name: "UserNotification");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "MailStorage");

            migrationBuilder.DropTable(
                name: "MazayaSubcategories");

            migrationBuilder.DropTable(
                name: "DefaultArea");

            migrationBuilder.DropTable(
                name: "Memberships");

            migrationBuilder.DropTable(
                name: "RoadshowEvent");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "Collection");

            migrationBuilder.DropTable(
                name: "Tag");

            migrationBuilder.DropTable(
                name: "RoadshowOffer");

            migrationBuilder.DropTable(
                name: "Announcement");

            migrationBuilder.DropTable(
                name: "Offer");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "MazayaCategories");

            migrationBuilder.DropTable(
                name: "MembershipPictureDatas");

            migrationBuilder.DropTable(
                name: "DefaultLocation");

            migrationBuilder.DropTable(
                name: "RoadshowInvite");

            migrationBuilder.DropTable(
                name: "RoadshowProposal");

            migrationBuilder.DropTable(
                name: "UserDomain");

            migrationBuilder.DropTable(
                name: "Roadshow");

            migrationBuilder.DropTable(
                name: "Company");

            migrationBuilder.DropTable(
                name: "CompanyDocument");

            migrationBuilder.DropTable(
                name: "Document");
        }
    }
}
