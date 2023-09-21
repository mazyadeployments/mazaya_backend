using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MMA.WebApi.Core.Services;
using MMA.WebApi.DataAccess.Bootstrap;
using MMA.WebApi.DataAccess.Repositories.EmailTemplate;
using MMA.WebApi.Shared.Interfaces;
using MMA.WebApi.Shared.Interfaces.AdnocTermsAndConditions;
using MMA.WebApi.Shared.Interfaces.Analytics;
using MMA.WebApi.Shared.Interfaces.Announcement;
using MMA.WebApi.Shared.Interfaces.ApplicationSettings;
using MMA.WebApi.Shared.Interfaces.ApplicationUsers;
using MMA.WebApi.Shared.Interfaces.B2C;
using MMA.WebApi.Shared.Interfaces.Banner;
using MMA.WebApi.Shared.Interfaces.Categories;
using MMA.WebApi.Shared.Interfaces.Collection;
using MMA.WebApi.Shared.Interfaces.Companies;
using MMA.WebApi.Shared.Interfaces.Dashboard;
using MMA.WebApi.Shared.Interfaces.EmailTemplate;
using MMA.WebApi.Shared.Interfaces.ExpiredToken;
using MMA.WebApi.Shared.Interfaces.Image;
using MMA.WebApi.Shared.Interfaces.LogAnalytics;
using MMA.WebApi.Shared.Interfaces.MailStorage;
using MMA.WebApi.Shared.Interfaces.Membership;
using MMA.WebApi.Shared.Interfaces.OfferDocuments;
using MMA.WebApi.Shared.Interfaces.OfferLocations;
using MMA.WebApi.Shared.Interfaces.OfferRating;
using MMA.WebApi.Shared.Interfaces.OfferReports;
using MMA.WebApi.Shared.Interfaces.Offers;
using MMA.WebApi.Shared.Interfaces.OfferSuggestions;
using MMA.WebApi.Shared.Interfaces.ProposalPDFCreatorService;
using MMA.WebApi.Shared.Interfaces.RedeemOffer;
using MMA.WebApi.Shared.Interfaces.RefreshToken;
using MMA.WebApi.Shared.Interfaces.RoadshowDocuments;
using MMA.WebApi.Shared.Interfaces.RoadshowEvent;
using MMA.WebApi.Shared.Interfaces.RoadshowInvite;
using MMA.WebApi.Shared.Interfaces.Roles;
using MMA.WebApi.Shared.Interfaces.Survey;
using MMA.WebApi.Shared.Interfaces.Tag;
using MMA.WebApi.Shared.Interfaces.Urls;
using MMA.WebApi.Shared.Interfaces.UserNotifications;

namespace MMA.WebApi.Core.Bootstrap
{
    public static class ServiceCollectionExtensions
    {
        public static void AddServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddTransient<ICategoryService, CategoryService>();
            services.AddTransient<IBannerService, BannerService>();
            services.AddTransient<IApplicationUserService, ApplicationUserService>();
            services.AddTransient<IRefreshTokenService, RefreshTokenService>();
            services.AddTransient<IExpiredTokenService, ExpiredTokenService>();
            services.AddTransient<ICompanyService, CompanyService>();
            services.AddTransient<IAnalyticsService, AnalyticsService>();
            services.AddTransient<IUserNotificationService, UserNotificationService>();
            services.AddTransient<IOfferService, OfferService>();
            services.AddTransient<IDocumentUrlService, DocumentUrlService>();
            services.AddTransient<ICollectionService, CollectionService>();
            services.AddTransient<ITagService, TagService>();
            services.AddTransient<IImageUtilsService, ImageUtilsService>();
            services.AddTransient<ICompanyLocationsService, CompanyLocationsService>();
            services.AddTransient<IOfferLocationService, OfferLocationService>();
            services.AddTransient<IOfferDocumentService, OfferDocumentService>();
            services.AddTransient<IRoadshowService, RoadshowService>();
            services.AddTransient<IRoadshowProposalService, RoadshowProposalService>();
            services.AddTransient<IRoadshowOfferService, RoadshowOfferService>();
            services.AddTransient<IMailStorageService, MailStorageService>();
            services.AddTransient<IOfferRatingService, OfferRatingService>();
            services.AddTransient<IDashboardService, DashboardService>();
            services.AddTransient<IRoadshowDocumentService, RoadshowDocumentService>();
            services.AddTransient<IRoadshowOfferDocumentService, RoadshowOfferDocumentService>();
            services.AddTransient<IProposalPDFCreatorService, ProposalPDFCreatorService>();
            services.AddTransient<IRoadshowLocationService, RoadshowLocationService>();
            services.AddTransient<IB2CService, B2CService>();
            services.AddHttpContextAccessor();
            services.AddTransient<IRoadshowInviteService, RoadshowInviteService>();
            services.AddTransient<IRoadshowEventService, RoadshowEventService>();
            services.AddTransient<IRoleService, RoleService>();
            services.AddTransient<
                IAdnocTermsAndConditionsService,
                AdnocTermsAndConditionsService
            >();
            services.AddTransient<IEmailTemplateRootService, EmailTemplateRootService>();
            services.AddTransient<IEmailTemplateService, EmailTemplateService>();
            services.AddTransient<ISurveyService, SurveyService>();
            services.AddTransient<ISurveyForUserService, SurveyForUserService>();
            services.AddTransient<IOfferReportService, OfferReportService>();
            services.AddTransient<IMembershipECardService, MembershipECardService>();
            services.AddTransient<IRedeemOfferService, RedeemOfferService>();
            services.AddTransient<ILogAnalyticsService, LogAnalyticsService>();
            services.AddTransient<IMobileCacheDataService, MobileCacheDataService>();
            services.AddTransient<IServiceNow, ServiceNow>();
            services.AddTransient<UserService>();
            services.AddTransient<CustomerService>();
            services.AddTransient<SupplierService>();
            services.AddTransient<MicrosofGraphService>();
            services.AddTransient<RedeemOfferService>();
            services.AddTransient<IOfferSuggestionsService, OfferSuggestionsService>();
            services.AddSingleton<IApplicationSettings, ApplicationSettings>();
            services.AddTransient<IAnnouncementService, AnnouncementService>();

            services.AddDataAccess(config).WithMMADbContext(config.GetConnectionString("Database"));
        }
    }
}
