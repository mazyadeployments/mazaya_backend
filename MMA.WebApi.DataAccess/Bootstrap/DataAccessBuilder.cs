using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MMA.WebApi.DataAccess.Categories;
using MMA.WebApi.DataAccess.Collections;
using MMA.WebApi.DataAccess.Companies;
using MMA.WebApi.DataAccess.Country;
using MMA.WebApi.DataAccess.EmailTemplate;
using MMA.WebApi.DataAccess.Helpers;
using MMA.WebApi.DataAccess.MailStorage;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.DataAccess.RefreshTokens;
using MMA.WebApi.DataAccess.Repositories.AdnocTermsAndConditions;
using MMA.WebApi.DataAccess.Repositories.Announcement;
using MMA.WebApi.DataAccess.Repositories.Banners;
using MMA.WebApi.DataAccess.Repositories.Dashboard;
using MMA.WebApi.DataAccess.Repositories.ExpiredToken;
using MMA.WebApi.DataAccess.Repositories.Log;
using MMA.WebApi.DataAccess.Repositories.LogAnalytics;
using MMA.WebApi.DataAccess.Repositories.MazayaCategory;
using MMA.WebApi.DataAccess.Repositories.MazayaEcarddetails;
using MMA.WebApi.DataAccess.Repositories.MazayaEcardmain;
using MMA.WebApi.DataAccess.Repositories.MazayaPackagesubscriptions;
using MMA.WebApi.DataAccess.Repositories.MazayaPaymentgateway;
using MMA.WebApi.DataAccess.Repositories.Mazayasubcategory;
using MMA.WebApi.DataAccess.Repositories.Membership;
using MMA.WebApi.DataAccess.Repositories.OfferRatings;
using MMA.WebApi.DataAccess.Repositories.OfferReports;
using MMA.WebApi.DataAccess.Repositories.Offers;
using MMA.WebApi.DataAccess.Repositories.OfferSuggestions;
using MMA.WebApi.DataAccess.Repositories.RedeemOffer;
using MMA.WebApi.DataAccess.Repositories.Survey;
using MMA.WebApi.DataAccess.Repository.Offers;
using MMA.WebApi.DataAccess.Repository.RoadshowDocuments;
using MMA.WebApi.DataAccess.Repository.RoadshowEvents;
using MMA.WebApi.DataAccess.Repository.RoadshowInvites;
using MMA.WebApi.DataAccess.Tags;
using MMA.WebApi.DataAccess.UserNotifications;
using MMA.WebApi.DataAccess.Users;
using MMA.WebApi.Shared.Interfaces;
using MMA.WebApi.Shared.Interfaces.AdnocTermsAndConditions;
using MMA.WebApi.Shared.Interfaces.Announcement;
using MMA.WebApi.Shared.Interfaces.ApplicationUsers;
using MMA.WebApi.Shared.Interfaces.Banner;
using MMA.WebApi.Shared.Interfaces.Categories;
using MMA.WebApi.Shared.Interfaces.Collection;
using MMA.WebApi.Shared.Interfaces.Companies;
using MMA.WebApi.Shared.Interfaces.CompanyLocations;
using MMA.WebApi.Shared.Interfaces.Dashboard;
using MMA.WebApi.Shared.Interfaces.Document;
using MMA.WebApi.Shared.Interfaces.EmailTemplate;
using MMA.WebApi.Shared.Interfaces.ExpiredToken;
using MMA.WebApi.Shared.Interfaces.Logger;
using MMA.WebApi.Shared.Interfaces.MailStorage;
using MMA.WebApi.Shared.Interfaces.MazayaCategory;
using MMA.WebApi.Shared.Interfaces.MazayaEcardetails;
using MMA.WebApi.Shared.Interfaces.MazayaEcardmain;
using MMA.WebApi.Shared.Interfaces.MazayaPackageSubscriptions;
using MMA.WebApi.Shared.Interfaces.MazayaPaymentgateway;
using MMA.WebApi.Shared.Interfaces.MazayaSubCategory;
using MMA.WebApi.Shared.Interfaces.Membership;
using MMA.WebApi.Shared.Interfaces.OfferDocuments;
using MMA.WebApi.Shared.Interfaces.OfferLocations;
using MMA.WebApi.Shared.Interfaces.OfferRating;
using MMA.WebApi.Shared.Interfaces.OfferReports;
using MMA.WebApi.Shared.Interfaces.Offers;
using MMA.WebApi.Shared.Interfaces.OfferSuggestions;
using MMA.WebApi.Shared.Interfaces.RedeemOffer;
using MMA.WebApi.Shared.Interfaces.RefreshToken;
using MMA.WebApi.Shared.Interfaces.RoadshowDocuments;
using MMA.WebApi.Shared.Interfaces.RoadshowEvent;
using MMA.WebApi.Shared.Interfaces.RoadshowInvite;
using MMA.WebApi.Shared.Interfaces.Survey;
using MMA.WebApi.Shared.Interfaces.Tag;
using MMA.WebApi.Shared.Interfaces.UserNotifications;
using System;

namespace MMA.WebApi.DataAccess.Bootstrap
{
    public class DataAccessBuilder : IDataAccessBuilder
    {
        private readonly IServiceCollection _services;

        public DataAccessBuilder(IServiceCollection services)
        {
            _services = services;
        }

        public IDataAccessBuilder WithMMADbContext(string connectionString)
        {
            _services.AddTransient<IBannerRepository, BannerRepository>();
            _services.AddTransient<IApplicationUsersRepository, ApplicationUsersRepository>();
            _services.AddTransient<IRefreshTokenRepository, RefreshTokenRepository>();
            _services.AddSingleton<ImageHelper>();
            _services.AddTransient<ICompanyRepository, CompanyRepository>();
            _services.AddTransient<ICollectionRepository, CollectionRepository>();
            _services.AddTransient<ICategoryRepository, CategoryRepository>();
            _services.AddTransient<IDocumentRepository, DocumentRepository>();
            _services.AddTransient<ITagRepository, TagRepository>();
            _services.AddTransient<ICompanyLocationsRepository, CompanyLocationsRepository>();
            _services.AddTransient<IOfferLocationRepository, OfferLocationRepository>();
            _services.AddTransient<IOfferDocumentRepository, OfferDocumentRepository>();
            _services.AddTransient<IMailStorageRepository, MailStorageRepository>();
            _services.AddTransient<IEmailTemplateRepository, EmailTemplateRepository>();
            _services.AddTransient<IEmailTemplateRootRepository, EmailTemplateRootRepository>();
            _services.AddTransient<IUserNotificationRepository, UserNotificationRepository>();
            _services.AddTransient<IOfferRatingRepository, OfferRatingRepository>();
            _services.AddTransient<IDashboardRepository, DashboardRepository>();
            _services.AddTransient<IRoadshowRepository, RoadshowRepository>();
            _services.AddTransient<IRoadshowProposalRepository, RoadshowProposalRepository>();
            _services.AddTransient<IRoadshowOfferRepository, RoadshowOfferRepository>();
            _services.AddTransient<IRoadshowDocumentRepository, RoadshowDocumentRepository>();
            _services.AddTransient<IRoadshowOfferDocumentRepository, RoadshowOfferDocumentRepository>();
            _services.AddTransient<IRoadshowInviteRepository, RoadshowInviteRepository>();
            _services.AddTransient<IRoadshowEventRepository, RoadshowEventRepository>();
            _services.AddTransient<IRoadshowLocationRepository, RoadshowLocationRepository>();
            _services.AddTransient<IAdnocTermsAndConditionsRepository, AdnocTermsAndConditionsRepository>();
            _services.AddTransient<IOfferRepository, OfferRepository>();
            _services.AddTransient<IExpiredTokenRepository, ExpiredTokenRepository>();
            _services.AddTransient<ISurveyRepository, SurveyRepository>();
            _services.AddTransient<ISurveyForUserRepository, SurveyForUserRepository>();
            _services.AddTransient<IRedeemOfferRepository, RedeemOfferRepository>();
            _services.AddTransient<IOfferReportRepository, OfferReportRepository>();
            _services.AddTransient<IMembershipECardMakerRepository, MembershipECardMakerRepository>();
            _services.AddTransient<ILogOfferRepository, LogOfferRepository>();
            _services.AddTransient<IMobileCacheDataRepository, MobileCacheDataRepository>();
            _services.AddTransient<IMembershipECardRepository, MembershipECardRepository>();
            _services.AddTransient<ILogAnalyticsRepository, LogAnalyticsRepository>();
            _services.AddTransient<IOfferSuggestionsRepository, OfferSuggestionsRepository>();
            _services.AddTransient<IAnnouncementRepository, AnnouncementRepository>();

            _services.AddTransient<IMazayaCategoryRepository, MazayaCategoryRepository>();
            _services.AddTransient<IMazayaSubcategoryRepository, MazayasubcategoryRepository>();
            _services.AddTransient<IMazayaPaymentgatewayRepository, MazayaPaymentgatewayRepository>();
            _services.AddTransient<IMazayaPackagesubscriptionsRepository, MazayapackagesubscriptionsRepository>();
            _services.AddTransient<IMazayaEcarddetailsRepository, MazayaEcarddetailsRepository>();
            _services.AddTransient<IMazayaEcardmainRepository, MazayaEcardmainRepository>();

            _services.AddDbContext<MMADbContext>(
                builder =>
                {
                    builder.UseSqlServer(connectionString,
                        options =>
                        {
                            options.MigrationsAssembly("MMA.WebApi.DataAccess"); // this could be passed as a parameter as well
                        });
                }, ServiceLifetime.Transient);
            _services.AddTransient<Func<MMADbContext>>(p => () => p.GetService<MMADbContext>());
            return this;
        }
    }
}
