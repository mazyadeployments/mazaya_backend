using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MMA.WebApi.DataAccess.Models.Views;
using MMA.WebApi.Shared.Enums;
using System.Linq;

namespace MMA.WebApi.DataAccess.Models
{
    public class MMADbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public MMADbContext(DbContextOptions<MMADbContext> options)
            : base(options) { }

        public virtual DbSet<Document> Document { get; set; }
        public virtual DbSet<Company> Company { get; set; }
        public virtual DbSet<RefreshToken> RefreshToken { get; set; }
        public virtual DbSet<UserNotification> UserNotification { get; set; }
        public virtual DbSet<NotificationType> NotificationType { get; set; }
        public virtual DbSet<Collection> Collection { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Offer> Offer { get; set; }
        public virtual DbSet<OfferDocument> OfferDocument { get; set; }
        public virtual DbSet<OfferImages> OfferImages { get; set; }
        public virtual DbSet<OfferReport> OfferReport { get; set; }
        public virtual DbSet<Tag> Tag { get; set; }
        public virtual DbSet<OfferCategory> OfferCategory { get; set; }
        public virtual DbSet<OfferCollection> OfferCollection { get; set; }
        public virtual DbSet<OfferTag> OfferTag { get; set; }
        public virtual DbSet<CompanyLocation> CompanyLocation { get; set; }
        public virtual DbSet<OfferLocation> OfferLocation { get; set; }
        public virtual DbSet<Comment> Comment { get; set; }
        public virtual DbSet<DiscountInfo> DiscountInfo { get; set; }
        public virtual DbSet<CompanyPartner> CompanyPartner { get; set; }
        public virtual DbSet<CompanyActivity> CompanyActivity { get; set; }
        public virtual DbSet<CompanyCategory> CompanyCategory { get; set; }
        public virtual DbSet<CompanyDocument> CompanyDocument { get; set; }
        public virtual DbSet<CollectionDocument> CollectionDocument { get; set; }
        public virtual DbSet<CategoryDocument> CategoryDocument { get; set; }
        public virtual DbSet<ApplicationUserDocument> ApplicationUserDocument { get; set; }
        public virtual DbSet<UserFavouritesOffer> UserFavouritesOffers { get; set; }
        public virtual DbSet<OfferRating> OfferRating { get; set; }
        public virtual DbSet<CompanySuppliers> CompanySuppliers { get; set; }

        public virtual DbSet<MailStorage> MailStorage { get; set; }

        public virtual DbSet<EmailTemplate> EmailTemplate { get; set; }
        public virtual DbSet<EmailTemplateRoot> EmailTemplateRoot { get; set; }
        public virtual DbSet<MembershipECard> MembershipECards { get; set; }

        //Roadshow
        public virtual DbSet<Roadshow> Roadshow { get; set; }
        public virtual DbSet<RoadshowComment> RoadshowComment { get; set; }
        public virtual DbSet<RoadshowDocument> RoadshowDocument { get; set; }
        public virtual DbSet<RoadshowEvent> RoadshowEvent { get; set; }
        public virtual DbSet<RoadshowInvite> RoadshowInvite { get; set; }
        public virtual DbSet<RoadshowLocation> RoadshowLocation { get; set; }
        public virtual DbSet<RoadshowOffer> RoadshowOffer { get; set; }
        public virtual DbSet<RoadshowOfferCategory> RoadshowOfferCategory { get; set; }
        public virtual DbSet<RoadshowOfferCollection> RoadshowOfferCollection { get; set; }
        public virtual DbSet<RoadshowOfferTag> RoadshowOfferTag { get; set; }
        public virtual DbSet<RoadshowOfferDocument> RoadshowOfferDocument { get; set; }
        public virtual DbSet<RoadshowOfferProposalDocument> RoadshowOfferProposalDocument { get; set; }
        public virtual DbSet<RoadshowOfferRating> RoadshowOfferRating { get; set; }
        public virtual DbSet<RoadshowProposal> RoadshowProposal { get; set; }
        public virtual DbSet<RoadshowVoucher> RoadshowVoucher { get; set; }
        public virtual DbSet<DefaultLocation> DefaultLocation { get; set; }
        public virtual DbSet<DefaultArea> DefaultArea { get; set; }
        public virtual DbSet<RoadshowEventOffer> RoadshowEventOffer { get; set; }
        public virtual DbSet<UserFavouritesRoadshowOffer> UserFavouritesRoadshowOffer { get; set; }
        public virtual DbSet<AdnocTermsAndConditions> AdnocTermsAndConditions { get; set; }
        public virtual DbSet<AllowedEmailsForRegistration> AllowedEmailsForRegistration { get; set; }
        public virtual DbSet<UserInvitations> UserInvitations { get; set; }
        public virtual DbSet<UserFcmToken> UserFcmTokens { get; set; }
        public virtual DbSet<AcceptedDomain> AcceptedDomain { get; set; }
        public virtual DbSet<UserDomain> UserDomain { get; set; }
        public virtual DbSet<MobileCacheData> MobileCacheDatas { get; set; }
        public virtual DbSet<RedeemOffer> RedeemOffers { get; set; }
        public virtual DbSet<OfferReport> OfferReports { get; set; }

        //log analytic
        public virtual DbSet<LogOfferClick> LogOfferClick { get; set; }
        public virtual DbSet<LogOfferSearch> LogOfferSearch { get; set; }
        public virtual DbSet<LogKeywordSearch> LogKeywordSearch { get; set; }
        public virtual DbSet<LogBannerClick> LogBannerClicks { get; set; }

        //survey
        public virtual DbSet<Survey> Surveys { get; set; }
        public virtual DbSet<SurveyForUser> SurveyForUsers { get; set; }

        //membership
        public virtual DbSet<ServiceNowData> ServiceNowDatas { get; set; }
        public virtual DbSet<MembershipPictureData> MembershipPictureDatas { get; set; }
        public virtual DbSet<Membership> Memberships { get; set; }
        public virtual DbSet<OffersMemberships> OffersMemberships { get; set; }

        public virtual DbSet<OfferSuggestion> OfferSuggestions { get; set; }

        //announcement
        public virtual DbSet<Announcement> Announcement { get; set; }
        public virtual DbSet<AnnouncementAttachments> AnnouncementAttachments { get; set; }
        public virtual DbSet<AnnouncementSpecificBuyer> AnnouncementSpecificBuyers { get; set; }
        public virtual DbSet<AnnouncementSpecificSupplier> AnnouncementSpecificSuppliers { get; set; }

        //Logout Logic
        public virtual DbSet<ExpiredToken> ExpiredTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OfferImages>(e => e.ToView("OfferImages").HasNoKey());

            modelBuilder.Entity<OfferCategory>().HasKey(oc => new { oc.OfferId, oc.CategoryId });
            modelBuilder
                .Entity<CompanySuppliers>()
                .HasKey(cs => new { cs.CompanyId, cs.SupplierId });
            modelBuilder
                .Entity<OfferCollection>()
                .HasKey(oc => new { oc.OfferId, oc.CollectionId });
            modelBuilder.Entity<OfferTag>().HasKey(ot => new { ot.OfferId, ot.TagId });
            modelBuilder
                .Entity<RoadshowEventOffer>()
                .HasKey(eo => new { eo.RoadshowOfferId, eo.RoadshowEventId });

            modelBuilder
                .Entity<UserFavouritesOffer>()
                .HasKey(uo => new { uo.OfferId, uo.ApplicationUserId });
            modelBuilder
                .Entity<UserFavouritesRoadshowOffer>()
                .HasKey(uo => new { uo.RoadshowOfferId, uo.ApplicationUserId });
            modelBuilder
                .Entity<OfferRating>()
                .HasKey(uo => new { uo.OfferId, uo.ApplicationUserId });

            modelBuilder
                .Entity<CompanyCategory>()
                .HasKey(cc => new { cc.CompanyId, cc.CategoryId });
            //Roadshow
            modelBuilder
                .Entity<RoadshowOfferCategory>()
                .HasKey(rc => new { rc.RoadshowOfferId, rc.CategoryId });
            modelBuilder
                .Entity<RoadshowOfferCollection>()
                .HasKey(rc => new { rc.RoadshowOfferId, rc.CollectionId });
            modelBuilder
                .Entity<RoadshowOfferTag>()
                .HasKey(rc => new { rc.RoadshowOfferId, rc.TagId });
            modelBuilder
                .Entity<RoadshowOfferRating>()
                .HasKey(ror => new { ror.RoadshowOfferId, ror.ApplicationUserId });
            modelBuilder
                .Entity<UserFcmToken>()
                .HasKey(uft => new { uft.UserId, uft.FcmMessageToken });
            //modelBuilder.Entity<RoadshowLocation>().HasKey(rl => new { rl.DefaultLocationId, rl.RoadshowId });

            foreach (
                var property in modelBuilder.Model
                    .GetEntityTypes()
                    .SelectMany(t => t.GetProperties())
                    .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?))
            )
            {
                property.SetColumnType("decimal(28, 12)");
            }

            modelBuilder
                .Entity<Tag>()
                .HasData(
                    new Tag
                    {
                        Id = 1,
                        Title = "Special Offer",
                        IsEditable = true
                    },
                    new Tag
                    {
                        Id = 2,
                        Title = "Best Price",
                        IsEditable = true
                    },
                    new Tag
                    {
                        Id = 3,
                        Title = "Exclusive Offer",
                        IsEditable = true
                    },
                    new Tag
                    {
                        Id = 4,
                        Title = "Top Seller",
                        IsEditable = true
                    },
                    new Tag
                    {
                        Id = 5,
                        Title = "Featured",
                        IsEditable = true
                    },
                    new Tag
                    {
                        Id = 6,
                        Title = "Trending",
                        IsEditable = true
                    },
                    new Tag
                    {
                        Id = 7,
                        Title = "Ending Soon",
                        IsEditable = false
                    },
                    new Tag
                    {
                        Id = 8,
                        Title = "Upcoming",
                        IsEditable = false
                    },
                    new Tag
                    {
                        Id = 9,
                        Title = "Latest",
                        IsEditable = false
                    },
                    new Tag
                    {
                        Id = 10,
                        Title = "Best Rates",
                        IsEditable = true
                    },
                    new Tag
                    {
                        Id = 11,
                        Title = "Any Other Tag",
                        IsEditable = true
                    }
                );

            modelBuilder
                .Entity<UserDomain>()
                .HasData(
                    new UserDomain
                    {
                        Id = (int)Declares.UserType.ADNOCEmployee,
                        DomainName = Declares.UserType.ADNOCEmployee.ToString(),
                        KeyValue = "1971",
                        Domains = "@adnoc;",
                        SequencerName = "dbo.ADNOCEmployeeSequencer"
                    },
                    new UserDomain
                    {
                        Id = (int)Declares.UserType.ADNOCEmployeeFamilyMember,
                        DomainName = Declares.UserType.ADNOCEmployeeFamilyMember.ToString(),
                        KeyValue = "1971",
                        Domains = "",
                        SequencerName = "dbo.ADNOCEmployeeSequencer"
                    },
                    new UserDomain
                    {
                        Id = (int)Declares.UserType.ADPolice,
                        DomainName = Declares.UserType.ADPolice.ToString(),
                        KeyValue = "1957",
                        Domains = "",
                        SequencerName = "dbo.ADPoliceSequencer"
                    },
                    new UserDomain
                    {
                        Id = (int)Declares.UserType.RedCrescent,
                        DomainName = Declares.UserType.RedCrescent.ToString(),
                        KeyValue = "1983",
                        Domains = "",
                        SequencerName = "dbo.RedCrescentSequencer"
                    },
                    new UserDomain
                    {
                        Id = (int)Declares.UserType.AlumniRetirementMembers,
                        DomainName = Declares.UserType.AlumniRetirementMembers.ToString(),
                        KeyValue = "2018",
                        Domains = "",
                        SequencerName = "dbo.AlumniRetirementMembersSequencer"
                    },
                    new UserDomain
                    {
                        Id = (int)Declares.UserType.ADSchools,
                        DomainName = Declares.UserType.ADSchools.ToString(),
                        KeyValue = "1971",
                        Domains = "",
                        SequencerName = "dbo.DefaultSequencer"
                    },
                    new UserDomain
                    {
                        Id = (int)Declares.UserType.Other,
                        DomainName = Declares.UserType.Other.ToString(),
                        KeyValue = "1971",
                        Domains = "",
                        SequencerName = "dbo.DefaultSequencer"
                    }
                );
            modelBuilder
                .Entity<ApplicationUser>()
                .Property(c => c.ReceiveAnnouncement)
                .HasDefaultValue(true);

            //modelBuilder.ApplyConfiguration(new ApplicationRoleConfiguration());
            //modelBuilder.ApplyConfiguration(new ApplicationAdminUserConfiguration());
            //modelBuilder.ApplyConfiguration(new ApplicationAdminUserRolesConfiguration());
            //modelBuilder.ApplyConfiguration(new ApplicationUserConfiguration());


            base.OnModelCreating(modelBuilder);
        }
    }
}
