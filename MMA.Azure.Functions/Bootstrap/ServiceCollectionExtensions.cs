using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MMA.Documents.Domain.Services;
using MMA.WebApi.Core.Services;
using MMA.WebApi.DataAccess.Bootstrap;
using MMA.WebApi.Shared.Interfaces;
using MMA.WebApi.Shared.Interfaces.Announcement;
using MMA.WebApi.Shared.Interfaces.ApplicationSettings;
using MMA.WebApi.Shared.Interfaces.ApplicationUsers;
using MMA.WebApi.Shared.Interfaces.Domain.Document;
using MMA.WebApi.Shared.Interfaces.ExpiredToken;
using MMA.WebApi.Shared.Interfaces.Image;
using MMA.WebApi.Shared.Interfaces.MailStorage;
using MMA.WebApi.Shared.Interfaces.Membership;
using MMA.WebApi.Shared.Interfaces.OfferDocuments;
using MMA.WebApi.Shared.Interfaces.Offers;
using MMA.WebApi.Shared.Interfaces.Survey;
using MMA.WebApi.Shared.Interfaces.Urls;
using MMA.WebApi.Shared.Interfaces.UserNotifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace MMA.Azure.Functions.Bootstrap
{
    public static class ServiceCollectionExtensions
    {
        public static void AddServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddTransient<IApplicationUserService, ApplicationUserService>();
            services.AddTransient<IOfferService, OfferService>();
            services.AddTransient<IExpiredTokenService, ExpiredTokenService>();
            services.AddTransient<IRoadshowService, RoadshowService>();
            services.AddTransient<IRoadshowLocationService, RoadshowLocationService>();
            services.AddTransient<IImageUtilsService, ImageUtilsService>();
            services.AddTransient<IDocumentUrlService, DocumentUrlService>();
            services.AddTransient<IOfferDocumentService, OfferDocumentService>();
            services.AddTransient<IDocumentService, DocumentService>();
            services.AddTransient<IMailStorageService, MailStorageService>();
            services.AddTransient<ISurveyService, SurveyService>();
            services.AddTransient<IMobileCacheDataService, MobileCacheDataService>();
            services.AddTransient<IUserNotificationService, UserNotificationService>();
            services.AddTransient<IServiceNow, ServiceNow>();
            services.AddTransient<IMembershipECardService, MembershipECardService>();
            services.AddTransient<IAnnouncementService, AnnouncementService>();
            services.AddHttpContextAccessor();
            services.AddDataAccess(config).WithMMADbContext(config.GetConnectionString("eMarketOffers"));
        }
    }
}
