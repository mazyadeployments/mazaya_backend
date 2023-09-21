using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Constants;
using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.ApplicationUsers;
using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Interfaces.MailStorage;
using MMA.WebApi.Shared.Interfaces.Membership;
using MMA.WebApi.Shared.Models.ApplicationUser;
using MMA.WebApi.Shared.Models.ApplicationUserDocument;
using MMA.WebApi.Shared.Models.ApplicationUsers;
using MMA.WebApi.Shared.Models.Image;
using MMA.WebApi.Shared.Models.MailStorage;
using MMA.WebApi.Shared.Models.Users;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;
using static MMA.WebApi.Shared.Enums.Declares;
using static MMA.WebApi.Shared.Enums.Document.DocumentDeclares;

namespace MMA.WebApi.DataAccess.Users
{
    public class ApplicationUsersRepository
        : BaseRepository<ApplicationUserModel>,
            IApplicationUsersRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IMailStorageRepository _mailStorageRepository;
        private readonly IMembershipECardRepository _membershipECardRepository;

        public ApplicationUsersRepository(
            Func<MMADbContext> contextFactory,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            IMailStorageRepository mailStorageRepository,
            IMembershipECardRepository membershipECardRepository
        )
            : base(contextFactory)
        {
            _userManager = userManager;
            _configuration = configuration;
            _mailStorageRepository = mailStorageRepository;
            _membershipECardRepository = membershipECardRepository;
        }

        private static ApplicationUserModel MapToApplicationUserModel(
            ApplicationUser applicationUser,
            ApplicationUserDocument applicationUserDocument = null,
            UserDomain userDomain = null
        )
        {
            return new ApplicationUserModel()
            {
                AccessFailedCount = applicationUser.AccessFailedCount,
                Active = applicationUser.Active,
                ConcurrencyStamp = applicationUser.ConcurrencyStamp,
                CreatedBy = applicationUser.CreatedBy,
                CreatedOn = applicationUser.CreatedOn,
                Email = applicationUser.Email,
                EmailConfirmed = applicationUser.EmailConfirmed,
                FirstName = applicationUser.FirstName,
                Id = applicationUser.Id,
                LastName = applicationUser.LastName,
                LockoutEnabled = applicationUser.LockoutEnabled,
                LockoutEnd = applicationUser.LockoutEnd,
                NormalizedEmail = applicationUser.NormalizedEmail,
                NormalizedUserName = applicationUser.NormalizedUserName,
                PhoneNumber = applicationUser.PhoneNumber,
                PhoneNumberConfirmed = applicationUser.PhoneNumberConfirmed,
                SecurityStamp = applicationUser.SecurityStamp,
                Title = applicationUser.Title,
                TwoFactorEnabled = applicationUser.TwoFactorEnabled,
                UpdatedBy = applicationUser.UpdatedBy,
                UpdatedOn = applicationUser.UpdatedOn,
                UserName = applicationUser.UserName,
                UserType = new UserDomainModel
                {
                    Id = applicationUser.UserType,
                    DomainName = userDomain.DomainName
                },
                ECard = applicationUser.ECardSequence ?? "",
                Image = MapImageModel(applicationUserDocument),
                ImageUrls = MapImageUrlModel(applicationUserDocument),
                ImageSets = new List<ImageModel> { MapImageModel(applicationUserDocument) },
                ReceiveAnnouncement = applicationUser.ReceiveAnnouncement
            };
        }

        public async Task<IEnumerable<string>> GetUsersWithEmptyEcard()
        {
            var context = ContextFactory();
            return context.Users
                .Where(x => string.IsNullOrEmpty(x.ECardSequence))
                .Select(x => x.Id)
                .AsEnumerable();
        }

        private static ImageModel MapImageModel(ApplicationUserDocument document)
        {
            return document != null
                ? new ImageModel()
                {
                    Id = document.DocumentId.ToString(),
                    OriginalImageId = document.OriginalImageId,
                    CropCoordinates = new CropCoordinates()
                    {
                        X1 = document.cropX1,
                        X2 = document.cropX2,
                        Y1 = document.cropY1,
                        Y2 = document.cropY2,
                    },
                    CropNGXCoordinates = new CropCoordinates()
                    {
                        X1 = document.cropX1,
                        X2 = document.cropX2,
                        Y1 = document.cropY1,
                        Y2 = document.cropY2,
                    },
                    Type = document.Type
                }
                : new ImageModel();
        }

        private static ImageUrlsModel MapImageUrlModel(ApplicationUserDocument document)
        {
            return document != null
                ? new ImageUrlsModel()
                {
                    Original = document.DocumentId.ToString(),
                    Thumbnail = document.DocumentId.ToString(),
                    Large = document.DocumentId.ToString()
                }
                : new ImageUrlsModel();
        }

        public async Task<ICollection<ApplicationUserModel>> GetAllUsersFromList(
            ICollection<string> userIds
        )
        {
            var context = ContextFactory();
            var users = (
                from u in context.Users
                where userIds.Contains(u.Id)
                join ur in context.UserRoles on u.Id equals ur.UserId into g1
                from p in g1.DefaultIfEmpty()
                join r in context.Roles on p.RoleId equals r.Id into g2
                from p2 in g2.DefaultIfEmpty()
                join ud in context.UserDomain on u.UserType equals ud.Id into g3
                from p3 in g3.DefaultIfEmpty()
                select new ApplicationUserModel
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Active = u.Active,
                    Title = u.Title,
                    Email = u.Email,
                    Role = p2.Name,
                    FcmDevice = (
                        from fcm in context.UserFcmTokens
                        where u.Id == fcm.UserId
                        select fcm.FcmMessageToken
                    ).ToList(),
                    UserType = new UserDomainModel() { DomainName = p3.DomainName }
                }
            );
            return users.ToList();
        }

        public async Task<int> GetAllBuyers()
        {
            var context = ContextFactory();

            return await (
                from u in context.Users
                join ur in context.UserRoles on u.Id equals ur.UserId
                join r in context.Roles on ur.RoleId equals r.Id
                join ud in context.UserDomain on u.UserType equals ud.Id
                where
                    r.Name == Roles.Buyer.ToString() || r.Name == Roles.AdnocCoordinator.ToString()
                select new { buyer = u, userDomain = ud }
            ).CountAsync();
        }

        public async Task<ICollection<ApplicationUserModel>> GetAllSpecificSuppliersAsync(
            ICollection<int> categories
        )
        {
            var context = ContextFactory();
            var specificSuppliers = (
                from u in context.Users
                join ur in context.UserRoles on u.Id equals ur.UserId
                join r in context.Roles on ur.RoleId equals r.Id
                join cs in context.CompanySuppliers on u.Id equals cs.SupplierId
                join c in context.Company on cs.CompanyId equals c.Id
                join cc in context.CompanyCategory on c.Id equals cc.CompanyId
                where
                    r.Name == Roles.Supplier.ToString()
                    || r.Name == Roles.SupplierAdmin.ToString()
                        && categories.Contains(cc.CategoryId)
                select new ApplicationUserModel
                {
                    Id = u.Id,
                    Email = u.Email,
                    Role = r.Name
                }
            );
            return specificSuppliers.ToHashSet();
        }

        public async Task<ICollection<ApplicationUserModel>> GetAllSpecificBuyersAsync(
            ICollection<int> categories
        )
        {
            var context = ContextFactory();
            var specificBuyers = (
                from u in context.Users
                join ur in context.UserRoles on u.Id equals ur.UserId
                join r in context.Roles on ur.RoleId equals r.Id
                join ud in context.UserDomain on u.UserType equals ud.Id
                where
                    r.Name == Roles.Buyer.ToString()
                    && categories.Contains(ud.Id)
                    && u.ReceiveAnnouncement
                select new ApplicationUserModel
                {
                    Id = u.Id,
                    Email = u.Email,
                    Role = r.Name
                }
            );
            return specificBuyers.ToHashSet();
        }

        public async Task<ICollection<ApplicationUserModel>> GetAllBuyersAndSuppliersAsync()
        {
            var context = ContextFactory();
            var bayersAndSuppliers = (
                from u in context.Users
                join ur in context.UserRoles on u.Id equals ur.UserId
                join r in context.Roles on ur.RoleId equals r.Id
                where
                    r.Name == Roles.Buyer.ToString()
                    || r.Name == Roles.Supplier.ToString()
                    || r.Name == Roles.SupplierAdmin.ToString() && u.ReceiveAnnouncement
                select new ApplicationUserModel
                {
                    Id = u.Id,
                    Email = u.Email,
                    Role = r.Name
                }
            );
            return bayersAndSuppliers.ToHashSet();
        }

        public async Task<ICollection<ApplicationUserModel>> GetAllBuyersAsync()
        {
            var context = ContextFactory();
            var bayersAndSuppliers = (
                from u in context.Users
                join ur in context.UserRoles on u.Id equals ur.UserId
                join r in context.Roles on ur.RoleId equals r.Id
                where r.Name == Roles.Buyer.ToString() && u.ReceiveAnnouncement
                select new ApplicationUserModel
                {
                    Id = u.Id,
                    Email = u.Email,
                    Role = r.Name
                }
            );
            return bayersAndSuppliers.ToHashSet();
        }

        public async Task<ICollection<ApplicationUserModel>> GetAllSuppliersAsync()
        {
            var context = ContextFactory();
            var bayersAndSuppliers = (
                from u in context.Users
                join ur in context.UserRoles on u.Id equals ur.UserId
                join r in context.Roles on ur.RoleId equals r.Id
                where
                    r.Name == Roles.Supplier.ToString()
                    || r.Name == Roles.SupplierAdmin.ToString() && u.ReceiveAnnouncement
                select new ApplicationUserModel
                {
                    Id = u.Id,
                    Email = u.Email,
                    Role = r.Name
                }
            );
            return bayersAndSuppliers.ToHashSet();
        }

        public async Task<IEnumerable<ApplicationUserModel>> GetAllBuyersPage(QueryModel queryModel)
        {
            var context = ContextFactory();

            var buyers = (
                from u in context.Users
                join ur in context.UserRoles on u.Id equals ur.UserId
                join r in context.Roles on ur.RoleId equals r.Id
                join ud in context.UserDomain on u.UserType equals ud.Id
                where
                    r.Name == Roles.Buyer.ToString() || r.Name == Roles.AdnocCoordinator.ToString()
                select new { buyer = u, userDomain = ud }
            );

            var buyerModels = buyers.Select(
                b => MapToApplicationUserModel(b.buyer, null, b.userDomain)
            );

            var filteredApplicationUserModels = Filter(buyerModels, queryModel);

            return SortIEnumerable(queryModel.Sort, filteredApplicationUserModels);
        }

        public async Task<IEnumerable<ApplicationUserModel>> GetAllAdnocUsersPage(
            QueryModel queryModel
        )
        {
            var context = ContextFactory();

            var buyers = (
                from u in context.Users
                join ur in context.UserRoles on u.Id equals ur.UserId
                join r in context.Roles on ur.RoleId equals r.Id
                join ud in context.UserDomain on u.UserType equals ud.Id
                join aud in context.ApplicationUserDocument
                    on u.Id equals aud.ApplicationUserId
                    into aud
                from d in aud.DefaultIfEmpty()
                where
                    (
                        r.Name == Roles.Admin.ToString()
                        || r.Name == Roles.AdnocCoordinator.ToString()
                        || r.Name == Roles.Reviewer.ToString()
                    ) && (d == null || d.Type == OfferDocumentType.Large)
                select new
                {
                    buyer = u,
                    document = d,
                    userDomain = ud
                }
            );

            var test = buyers.Select(
                b => MapToApplicationUserModel(b.buyer, b.document, b.userDomain)
            );

            var filteredApplicationUserModels = Filter(test, queryModel);

            return SortIEnumerable(queryModel.Sort, filteredApplicationUserModels);
        }

        private static IEnumerable<ApplicationUserModel> Filter(
            IEnumerable<ApplicationUserModel> applicationUserModels,
            QueryModel queryModel
        )
        {
            var filteredApplicationUserModels = applicationUserModels.Where(
                applicationUserModel =>
                    (
                        applicationUserModel.FirstName != null
                        && applicationUserModel.FirstName
                            .ToLower()
                            .Contains(queryModel.Filter.Keyword.Trim().ToLower())
                    )
                    || (
                        applicationUserModel.LastName != null
                        && applicationUserModel.LastName
                            .ToLower()
                            .Contains(queryModel.Filter.Keyword.Trim().ToLower())
                    )
                    || (
                        applicationUserModel.ECard != null
                        && applicationUserModel.ECard
                            .ToLower()
                            .Contains(queryModel.Filter.Keyword.Trim().ToLower())
                    )
                    || (
                        applicationUserModel.Email != null
                        && applicationUserModel.Email
                            .ToLower()
                            .Contains(queryModel.Filter.Keyword.Trim().ToLower())
                    )
            );

            if (queryModel.Filter.Type?.Any() == true)
            {
                filteredApplicationUserModels = filteredApplicationUserModels.Where(
                    o => o.UserType != null
                );
                filteredApplicationUserModels = filteredApplicationUserModels.Where(
                    o =>
                        queryModel.Filter.Type.Any(
                            item => string.Equals(item, o.UserType.DomainName)
                        )
                );
            }

            if (queryModel.Filter.Status?.Any() == true)
            {
                if (
                    queryModel.Filter.Status.Contains("Active")
                    && queryModel.Filter.Status.Contains("Inactive")
                )
                {
                    return filteredApplicationUserModels;
                }

                if (queryModel.Filter.Status.Contains("Active"))
                {
                    return filteredApplicationUserModels.Where(o => o.Active == true).ToList();
                }

                if (queryModel.Filter.Status.Contains("Inactive"))
                {
                    return filteredApplicationUserModels.Where(o => o.Active == false).ToList();
                }
            }

            return filteredApplicationUserModels;
        }

        public async Task<bool> CheckIfUserExists(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task<ECardModel> GenerateECardForUser(string userId)
        {
            var context = ContextFactory();

            string sequence = string.Empty;
            var p = new SqlParameter("@Result", System.Data.SqlDbType.BigInt);
            p.Direction = System.Data.ParameterDirection.Output;

            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var userDomain = await context.UserDomain.FirstOrDefaultAsync(
                ud => ud.Id == user.UserType
            );

            //var sequencer = string.IsNullOrWhiteSpace(userDomain.SequencerName) ? _configuration["DefaultSequencer"] : userDomain.SequencerName;
            //context.Database.ExecuteSqlRaw("SET @Result = NEXT VALUE FOR " + sequencer, p);

            // Implementation changed from multiple sequencers for user types to one sequencer
            context.Database.ExecuteSqlRaw("SET @Result = NEXT VALUE FOR dbo.DefaultSequencer ", p);
            sequence = p.Value.ToString();

            string formattedSequence = FormatSequence(userDomain.KeyValue, sequence);

            if (!await context.Users.AnyAsync(x => x.ECardSequence == formattedSequence))
            {
                user.ECardSequence = formattedSequence;
                //string sequenceForDisplay = formattedSequence != null ? Regex.Replace(formattedSequence, @"^(....)(...)(...)(....)$", "$1 $2 $3 $4") : null;
                string sequenceForDisplay =
                    formattedSequence != null
                        ? Regex.Replace(formattedSequence, @"^(....)(.....)(....)$", "$1 $2 $3")
                        : null;
                context.Users.Update(user);
                await context.SaveChangesAsync();

                return new ECardModel()
                {
                    UserId = userId,
                    ECardSequence = formattedSequence,
                    FormattedECardSequence = sequenceForDisplay
                };
            }

            return new ECardModel();
        }

        public async Task<ECardModel> EditECardForUser(ECardModel model)
        {
            var context = ContextFactory();

            var user = await _userManager.FindByIdAsync(model.UserId);
            var userDomain = await context.UserDomain.FirstOrDefaultAsync(
                x => x.Id == model.UserDomainId
            );
            string formattedSequence = FormatSequence(userDomain.KeyValue, model.ECardSequence);

            if (!await context.Users.AnyAsync(x => x.ECardSequence == formattedSequence))
            {
                user.ECardSequence = formattedSequence;
                //string sequenceForDisplay = formattedSequence != null ? Regex.Replace(formattedSequence, @"^(....)(...)(...)(....)$", "$1 $2 $3 $4") : null;
                string sequenceForDisplay =
                    formattedSequence != null
                        ? Regex.Replace(formattedSequence, @"^(....)(.....)(....)$", "$1 $2 $3")
                        : null;
                await _userManager.UpdateAsync(user);

                return new ECardModel()
                {
                    UserId = user.Id,
                    ECardSequence = formattedSequence,
                    FormattedECardSequence = sequenceForDisplay
                };
            }

            return new ECardModel();
        }

        //private string FormatSequence(string prefix, string sequence)
        //{
        //    string formattedSequence = string.Empty;
        //    var sequenceSufix = formattedSequence.PadLeft(UserTypeSequenceConstats.SequenceLengthWithoutPrefix - sequence.Length, '0');
        //    formattedSequence = prefix + sequenceSufix + sequence;
        //    return formattedSequence;
        //}

        private string FormatSequence(string keyValue, string sequence)
        {
            string formattedSequence = string.Empty;
            var sequencePrefix = formattedSequence.PadLeft(
                UserTypeSequenceConstats.SequenceLength - sequence.Length,
                '0'
            );
            formattedSequence =
                DateTime.UtcNow.Year.ToString() + sequencePrefix + sequence + keyValue;
            return formattedSequence;
        }

        public async Task<ECardModel> GetECardForUser(string userId)
        {
            var context = ContextFactory();

            var eCardSequence = await context.Users
                .Where(u => u.Id == userId)
                .Select(u => u.ECardSequence)
                .FirstOrDefaultAsync();
            string sequenceForDisplay =
                eCardSequence != null
                    ? Regex.Replace(eCardSequence, @"^(....)(...)(...)(....)$", "$1 $2 $3 $4")
                    : null;

            return new ECardModel()
            {
                UserId = userId,
                ECardSequence = eCardSequence,
                FormattedECardSequence = sequenceForDisplay
            };
        }

        private Expression<
            Func<ApplicationUser, ApplicationUserModel>
        > projectToApplicationUserModel = applicationUser =>
            new ApplicationUserModel()
            {
                AccessFailedCount = applicationUser.AccessFailedCount,
                Active = applicationUser.Active,
                Id = applicationUser.Id,
                ConcurrencyStamp = applicationUser.ConcurrencyStamp,
                CreatedBy = applicationUser.CreatedBy,
                CreatedOn = applicationUser.CreatedOn,
                Email = applicationUser.Email,
                EmailConfirmed = applicationUser.EmailConfirmed,
                FirstName = applicationUser.FirstName,
                LastName = applicationUser.LastName,
                LockoutEnabled = applicationUser.LockoutEnabled,
                LockoutEnd = applicationUser.LockoutEnd,
                NormalizedEmail = applicationUser.NormalizedEmail,
                NormalizedUserName = applicationUser.NormalizedUserName,
                PhoneNumber = applicationUser.PhoneNumber,
                PhoneNumberConfirmed = applicationUser.PhoneNumberConfirmed,
                SecurityStamp = applicationUser.SecurityStamp,
                Title = applicationUser.Title,
                TwoFactorEnabled = applicationUser.TwoFactorEnabled,
                UpdatedBy = applicationUser.UpdatedBy,
                UpdatedOn = applicationUser.UpdatedOn,
                UserName = applicationUser.UserName,
                Image = applicationUser.ApplicationUserDocuments
                    .Where(cd => cd.ApplicationUserId == applicationUser.Id)
                    .Select(
                        od =>
                            new ImageModel
                            {
                                Id = !string.IsNullOrEmpty(od.DocumentId.ToString())
                                    ? od.DocumentId.ToString()
                                    : string.Empty,
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
                            }
                    )
                    .FirstOrDefault(),
                ImageUrls = new ImageUrlsModel
                {
                    Large = applicationUser.ApplicationUserDocuments
                        .Where(
                            x =>
                                x.ApplicationUserId == applicationUser.Id
                                && x.Type == OfferDocumentType.Large
                        )
                        .Select(x => x.DocumentId)
                        .FirstOrDefault()
                        .ToString(),
                    Original = applicationUser.ApplicationUserDocuments
                        .Where(
                            x =>
                                x.ApplicationUserId == applicationUser.Id
                                && x.Type == OfferDocumentType.Original
                        )
                        .Select(x => x.DocumentId)
                        .FirstOrDefault()
                        .ToString(),
                    Thumbnail = applicationUser.ApplicationUserDocuments
                        .Where(
                            x =>
                                x.ApplicationUserId == applicationUser.Id
                                && x.Type == OfferDocumentType.Thumbnail
                        )
                        .Select(x => x.DocumentId)
                        .FirstOrDefault()
                        .ToString()
                },
                ReceiveAnnouncement = applicationUser.ReceiveAnnouncement
            };

        private Expression<
            Func<ApplicationUser, ApplicationUserModel>
        > projectToApplicationUserModelForECard = applicationUser =>
            new ApplicationUserModel()
            {
                Id = applicationUser.Id,
                Email = applicationUser.Email,
                ECard = applicationUser.ECardSequence,
                FirstName = applicationUser.FirstName,
                LastName = applicationUser.LastName,
                UserName = applicationUser.UserName,
                UserType = new UserDomainModel { Id = applicationUser.UserType, },
                Image = applicationUser.ApplicationUserDocuments
                    .Where(cd => cd.ApplicationUserId == applicationUser.Id)
                    .Select(
                        od =>
                            new ImageModel
                            {
                                Id = !string.IsNullOrEmpty(od.DocumentId.ToString())
                                    ? od.DocumentId.ToString()
                                    : string.Empty,
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
                            }
                    )
                    .FirstOrDefault(),
                ImageUrls = new ImageUrlsModel
                {
                    Large = applicationUser.ApplicationUserDocuments
                        .Where(
                            x =>
                                x.ApplicationUserId == applicationUser.Id
                                && x.Type == OfferDocumentType.Large
                        )
                        .Select(x => x.DocumentId)
                        .FirstOrDefault()
                        .ToString(),
                    Original = applicationUser.ApplicationUserDocuments
                        .Where(
                            x =>
                                x.ApplicationUserId == applicationUser.Id
                                && x.Type == OfferDocumentType.Original
                        )
                        .Select(x => x.DocumentId)
                        .FirstOrDefault()
                        .ToString(),
                    Thumbnail = applicationUser.ApplicationUserDocuments
                        .Where(
                            x =>
                                x.ApplicationUserId == applicationUser.Id
                                && x.Type == OfferDocumentType.Thumbnail
                        )
                        .Select(x => x.DocumentId)
                        .FirstOrDefault()
                        .ToString()
                }
            };

        private static IQueryable<ApplicationUserModel> Sort(
            SortModel sortModel,
            IQueryable<ApplicationUserModel> applicationUserModel
        )
        {
            if (sortModel.Type == "date")
            {
                if (sortModel.Order == Order.DESC)
                {
                    return applicationUserModel.OrderByDescending(x => x.UpdatedOn);
                }
                else
                {
                    return applicationUserModel.OrderBy(x => x.UpdatedOn);
                }
            }
            else
            {
                return applicationUserModel.OrderByDescending(x => x.UpdatedOn);
            }
        }

        private static IEnumerable<ApplicationUserModel> SortIEnumerable(
            SortModel sortModel,
            IEnumerable<ApplicationUserModel> applicationUserModel
        )
        {
            if (sortModel.Type == "date")
            {
                if (sortModel.Order == Order.DESC)
                {
                    return applicationUserModel.OrderByDescending(x => x.UpdatedOn);
                }
                else
                {
                    return applicationUserModel.OrderBy(x => x.UpdatedOn);
                }
            }
            else
            {
                return applicationUserModel.OrderByDescending(x => x.UpdatedOn);
            }
        }

        public async Task<ApplicationUserModel> CreateOrUpdateUser(
            ApplicationUserModel model,
            IVisitor<IChangeable> auditVisitor
        )
        {
            var context = ContextFactory();

            using (
                var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled)
            )
            {
                var applicationUser = await _userManager.FindByIdAsync(model.Id);
                if (applicationUser == null)
                    applicationUser = new ApplicationUser();

                var userIMGs = context.ApplicationUserDocument.Where(
                    x => x.ApplicationUserId == model.Id
                );
                context.ApplicationUserDocument.RemoveRange(userIMGs);
                ProcessImages(model);
                PopulateEntityModel(applicationUser, model);

                foreach (var offerDocument in applicationUser.ApplicationUserDocuments)
                {
                    offerDocument.Accept(auditVisitor);
                    var document = await context.Document.FirstOrDefaultAsync(
                        d => d.Id == offerDocument.DocumentId
                    );
                    if (document == null)
                    {
                        context.Document.Add(
                            new Document()
                            {
                                Id = offerDocument.DocumentId,
                                CreatedOn = DateTime.UtcNow,
                                UpdatedOn = DateTime.UtcNow,
                            }
                        );

                        await context.SaveChangesAsync();
                    }
                }

                if (model.Id == "0")
                {
                    applicationUser.Accept(auditVisitor);
                    var resultCreate = await _userManager.CreateAsync(applicationUser);
                    await _membershipECardRepository.FindMembershipCardForUserAndUpdate(
                        new Shared.Models.ServiceNowModels.MemberModel()
                        {
                            Id = applicationUser.Id,
                            mail = applicationUser.Email
                        }
                    );
                    HandleErrorDuringCreate(resultCreate);
                    var resultRole = await SetRole(model, applicationUser);
                    HandleErrorDuringCreateRole(resultRole);
                }
                else
                {
                    applicationUser.Accept(auditVisitor);
                    var result = await _userManager.UpdateAsync(applicationUser);
                    applicationUser.UpdatedOn = DateTime.UtcNow;
                    HandleErrorDuringUpdate(result);
                }
                transactionScope.Complete();
            }

            await context.SaveChangesAsync();
            return model;
        }

        private void ProcessImages(ApplicationUserModel model)
        {
            var userDocuments = new List<ApplicationUserDocumentModel>();

            if (model.ImageSets != null && model.ImageSets.Count > 0)
            {
                foreach (var imageModel in model.ImageSets)
                {
                    userDocuments.Add(
                        new ApplicationUserDocumentModel
                        {
                            DocumentId = new Guid(imageModel.Id),
                            //ApplicationUserId = model.Id,
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
                        }
                    );
                }
            }

            model.ApplicationUserDocuments = userDocuments;
        }

        private static void SetVisitorForUserImage(
            IVisitor<IChangeable> auditVisitor,
            ApplicationUser applicationUser
        )
        {
            foreach (var applicationUserDocument in applicationUser.ApplicationUserDocuments)
            {
                applicationUserDocument.Accept(auditVisitor);
            }
        }

        private static void HandleErrorDuringUpdate(IdentityResult result)
        {
            StringBuilder sb = new StringBuilder();
            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (var error in result.Errors)
                    {
                        sb.Append(error.Description);
                    }
                }
                throw new Exception(sb.ToString());
            }
        }

        private static void HandleErrorDuringCreate(IdentityResult resultCreate)
        {
            StringBuilder sb = new StringBuilder();
            if (!resultCreate.Succeeded)
            {
                if (resultCreate.Errors != null)
                {
                    foreach (var error in resultCreate.Errors)
                    {
                        sb.Append(error.Description);
                    }
                }

                throw new Exception(sb.ToString());
            }
        }

        private static void HandleErrorDuringCreateRole(IdentityResult resultRole)
        {
            StringBuilder sb = new StringBuilder();
            if (!resultRole.Succeeded)
            {
                if (resultRole.Errors != null)
                {
                    foreach (var error in resultRole.Errors)
                    {
                        sb.Append(error.Description);
                    }
                }

                throw new Exception(sb.ToString());
            }
        }

        private async Task<IdentityResult> SetRole(
            ApplicationUserModel model,
            ApplicationUser applicationUser
        )
        {
            IdentityResult resultRole = new IdentityResult();

            if (model.Role == Roles.AdnocCoordinator.ToString())
            {
                resultRole = await _userManager.AddToRoleAsync(
                    applicationUser,
                    "ADNOC Coordinator"
                );
            }
            else
            {
                resultRole = await _userManager.AddToRoleAsync(applicationUser, model.Role);
            }

            return resultRole;
        }

        private static void SetUserImages(ApplicationUserModel model, MMADbContext context)
        {
            var applicationUserDocuments = new List<ApplicationUserDocumentModel>();

            if (model.ImageSets != null && model.ImageSets.Count > 0)
            {
                foreach (var imageModel in model.ImageSets)
                {
                    applicationUserDocuments.Add(
                        new ApplicationUserDocumentModel
                        {
                            DocumentId = new Guid(imageModel.Id),
                            ApplicationUserId = model.Id,
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
                        }
                    );
                }
            }

            var userIMGs = context.ApplicationUserDocument.Where(
                x => x.ApplicationUserId == model.Id
            );
            context.ApplicationUserDocument.RemoveRange(userIMGs);

            model.ApplicationUserDocuments = applicationUserDocuments;
        }

        private void PopulateEntityModel(ApplicationUser data, ApplicationUserModel model)
        {
            data.Title = model.Title;
            data.AccessFailedCount = model.AccessFailedCount;
            data.ConcurrencyStamp = model.ConcurrencyStamp;
            data.CreatedBy = model.CreatedBy;
            data.CreatedOn = model.CreatedOn;
            data.EmailConfirmed = model.EmailConfirmed;
            data.FirstName = model.FirstName;
            data.LastName = model.LastName;
            data.LockoutEnabled = model.LockoutEnabled;
            data.LockoutEnd = model.LockoutEnd;
            data.NormalizedEmail = model.NormalizedEmail;
            data.NormalizedUserName = model.NormalizedUserName;
            data.PhoneNumber = model.PhoneNumber;
            data.PhoneNumberConfirmed = model.PhoneNumberConfirmed;
            data.SecurityStamp = model.SecurityStamp;
            data.Title = model.Title;
            data.TwoFactorEnabled = model.TwoFactorEnabled;
            data.UpdatedBy = model.UpdatedBy;
            data.UpdatedOn = DateTime.UtcNow;
            data.Active = model.Active;
            // ID is 0 if user is new (Creation)
            if (model.Id == "0")
            {
                data.Email = model.Email;
                data.UserName = model.Email;
                data.NormalizedUserName = model.Email;
                data.NormalizedEmail = model.NormalizedEmail;
                data.Active = true;
            }
            data.ApplicationUserDocuments = model.ApplicationUserDocuments
                .Select(
                    od =>
                        new ApplicationUserDocument
                        {
                            DocumentId = od.DocumentId,
                            ApplicationUserId = model.Id,
                            Type = od.Type.ToString() == "0" ? OfferDocumentType.Original : od.Type,
                            OriginalImageId =
                                od.OriginalImageId == Guid.Empty
                                    ? od.DocumentId
                                    : od.OriginalImageId,
                            X1 = od.X1,
                            X2 = od.X2,
                            Y1 = od.Y1,
                            Y2 = od.Y2,
                            cropX1 = od.cropX1,
                            cropX2 = od.cropX2,
                            cropY1 = od.cropY1,
                            cropY2 = od.cropY2
                        }
                )
                .ToList();
            data.ReceiveAnnouncement = model.ReceiveAnnouncement;
        }

        public async Task<IEnumerable<ApplicationUserModel>> GetAllAsync()
        {
            List<ApplicationUser> applicationUsers = await _userManager.Users.ToListAsync();
            List<ApplicationUserModel> applicationUserModels = new List<ApplicationUserModel>();

            foreach (var applicationUser in applicationUsers)
            {
                ApplicationUserModel applicationUserModel = new ApplicationUserModel()
                {
                    AccessFailedCount = applicationUser.AccessFailedCount,
                    Active = applicationUser.Active,
                    ConcurrencyStamp = applicationUser.ConcurrencyStamp,
                    CreatedBy = applicationUser.CreatedBy,
                    CreatedOn = applicationUser.CreatedOn,
                    Email = applicationUser.Email,
                    EmailConfirmed = applicationUser.EmailConfirmed,
                    FirstName = applicationUser.FirstName,
                    Id = applicationUser.Id,
                    LastName = applicationUser.LastName,
                    LockoutEnabled = applicationUser.LockoutEnabled,
                    LockoutEnd = applicationUser.LockoutEnd,
                    NormalizedEmail = applicationUser.NormalizedEmail,
                    NormalizedUserName = applicationUser.NormalizedUserName,
                    PhoneNumber = applicationUser.PhoneNumber,
                    PhoneNumberConfirmed = applicationUser.PhoneNumberConfirmed,
                    SecurityStamp = applicationUser.SecurityStamp,
                    Title = applicationUser.Title,
                    TwoFactorEnabled = applicationUser.TwoFactorEnabled,
                    UpdatedBy = applicationUser.UpdatedBy,
                    UpdatedOn = applicationUser.UpdatedOn,
                    UserName = applicationUser.UserName,
                    LastDataSynchronizationOn = applicationUser.LastDataSynchronizationOn,
                    PlatformType = applicationUser.PlatformType,
                    ReceiveAnnouncement = applicationUser.ReceiveAnnouncement
                };

                applicationUserModels.Add(applicationUserModel);
            }

            return applicationUserModels;
        }

        public async Task<IEnumerable<UserCsvModel>> GetAllListUserAsync()
        {
            var context = ContextFactory();
            var users = (
                from u in context.Users
                join sc in context.CompanySuppliers on u.Id equals sc.SupplierId into g1
                from p in g1.DefaultIfEmpty()
                join c in context.Company on p.CompanyId equals c.Id into g2
                from p1 in g2.DefaultIfEmpty()
                join ur in context.UserRoles on u.Id equals ur.UserId into g3
                from p2 in g3.DefaultIfEmpty()
                join r in context.Roles on p2.RoleId equals r.Id into g4
                from p3 in g4.DefaultIfEmpty()
                select new UserCsvModel()
                {
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    CompanyName = p1.NameEnglish,
                    UserType = GetDisplayName<Declares.UserType>(u.UserType.ToString()),
                    RoleName = p3.NormalizedName
                }
            );

            return users;
        }

        public async Task<IEnumerable<UserCsvModel>> GetAllUserListAsync()
        {
            var context = ContextFactory();
            var users = (
                from u in context.Users
                join ur in context.UserRoles on u.Id equals ur.UserId
                join r in context.Roles on ur.RoleId equals r.Id
                join ud in context.UserDomain on u.UserType equals ud.Id
                where
                    r.Name == Roles.Buyer.ToString() || r.Name == Roles.AdnocCoordinator.ToString()
                select new UserCsvModel()
                {
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    UserType = ud.DomainName,
                }
            );

            return users;
        }

        public static string GetDisplayName<TEnum>(string value)
        {
            var enumValue = (TEnum)Enum.Parse(typeof(TEnum), value, true);
            var enumConstant = enumValue.GetType().GetField(enumValue.ToString());
            if (enumConstant == null)
            {
                return "";
            }

            var enumDisplayDescription = (DisplayAttribute[])
                enumConstant.GetCustomAttributes(typeof(DisplayAttribute), false);
            if (enumDisplayDescription.Length > 0)
            {
                return enumDisplayDescription[0].Name;
            }
            return enumValue.ToString();
        }

        //public async Task<IEnumerable<ApplicationUserModel>> GetAllPendingSuppliersAsync()
        //{
        //    //List<ApplicationUser> applicationUsers = await userManager.Users.ToListAsync();
        //    //var pendingSuppliers = applicationUsers.Where(x => x.ApproveStatus == ApproveStatus.PendingApproval);
        //    List<ApplicationUserModel> applicationUserModels = new List<ApplicationUserModel>();

        //    foreach (var applicationUser in pendingSuppliers)
        //    {
        //        ApplicationUserModel applicationUserModel = new ApplicationUserModel()
        //        {
        //            Email = applicationUser.Email,
        //            PhoneNumber = applicationUser.PhoneNumber,
        //            CompanyDescription = applicationUser.CompanyDescription,
        //            CompanyName = applicationUser.CompanyName,
        //            Id = applicationUser.Id
        //        };

        //        applicationUserModels.Add(applicationUserModel);
        //    }


        //    return applicationUserModels;
        //}
        public async Task DeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var context = ContextFactory();
            var userImage = context.ApplicationUserDocument.Where(x => x.ApplicationUserId == id);
            context.ApplicationUserDocument.RemoveRange(userImage);
            context.SaveChanges();

            if (user != null)
            {
                try
                {
                    await _userManager.DeleteAsync(user);
                }
                catch (Exception ex)
                {
                    var split = ex.InnerException.Message.Split('"')[5];
                    throw new Exception("Conflict key in DB table: " + split);
                }
            }
            ;
        }

        public async Task DeleteByUsername(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
                return;

            var context = ContextFactory();

            var userImage = context.ApplicationUserDocument.Where(
                x => x.ApplicationUserId == user.Id
            );
            var userNotif = context.UserNotification.Where(x => x.UserId == user.Id);
            var mailStorage = context.MailStorage.Where(x => x.UserId == user.Id);
            var cs = context.CompanySuppliers.Where(x => x.SupplierId == user.Id);

            context.ApplicationUserDocument.RemoveRange(userImage);
            context.UserNotification.RemoveRange(userNotif);
            context.MailStorage.RemoveRange(mailStorage);

            await context.SaveChangesAsync();

            await _userManager.DeleteAsync(user);
        }

        public async Task<ApplicationUserModel> FindByIdAsync(string id)
        {
            var context = ContextFactory();

            ApplicationUser applicationUser = await _userManager.FindByIdAsync(id);
            ApplicationUserModel applicationUserModel = new ApplicationUserModel();

            applicationUser.ApplicationUserDocuments = new List<ApplicationUserDocument>();
            applicationUserModel.Image = new ImageModel();
            var userDomain = await context.UserDomain.FirstOrDefaultAsync(
                x => x.Id == applicationUser.UserType
            );

            applicationUserModel.AccessFailedCount = applicationUser.AccessFailedCount;
            applicationUserModel.Active = applicationUser.Active;
            var eCard = applicationUser.ECardSequence ?? string.Empty;
            applicationUserModel.ECard = FormatECard(eCard);
            applicationUserModel.UserType = new UserDomainModel
            {
                Id = applicationUser.UserType,
                DomainName = userDomain.DomainName,
                KeyValue = userDomain.KeyValue,
                Domains = userDomain.Domains,
                SequencerName = userDomain.SequencerName
            };
            applicationUserModel.ConcurrencyStamp = applicationUser.ConcurrencyStamp;
            applicationUserModel.CreatedBy = applicationUser.CreatedBy;
            applicationUserModel.CreatedOn = applicationUser.CreatedOn;
            applicationUserModel.Email = applicationUser.Email;
            applicationUserModel.EmailConfirmed = applicationUser.EmailConfirmed;
            applicationUserModel.FirstName = applicationUser.FirstName;
            applicationUserModel.Id = applicationUser.Id;
            applicationUserModel.LastName = applicationUser.LastName;
            applicationUserModel.LockoutEnabled = applicationUser.LockoutEnabled;
            applicationUserModel.LockoutEnd = applicationUser.LockoutEnd;
            applicationUserModel.NormalizedEmail = applicationUser.NormalizedEmail;
            applicationUserModel.NormalizedUserName = applicationUser.NormalizedUserName;
            applicationUserModel.PhoneNumber = applicationUser.PhoneNumber;
            applicationUserModel.PhoneNumberConfirmed = applicationUser.PhoneNumberConfirmed;
            applicationUserModel.SecurityStamp = applicationUser.SecurityStamp;
            applicationUserModel.Title = applicationUser.Title;
            applicationUserModel.TwoFactorEnabled = applicationUser.TwoFactorEnabled;
            applicationUserModel.UpdatedBy = applicationUser.UpdatedBy;
            applicationUserModel.UpdatedOn = applicationUser.UpdatedOn;
            applicationUserModel.UserName = applicationUser.UserName;
            applicationUserModel.Role = (
                from u in context.Users
                join ur in context.UserRoles on u.Id equals ur.UserId
                join r in context.Roles on ur.RoleId equals r.Id
                where u.Id == applicationUser.Id
                select r.Name
            ).FirstOrDefault();
            applicationUserModel.Image = context.ApplicationUserDocument
                .Where(cd => cd.ApplicationUserId == applicationUser.Id)
                .Select(
                    od =>
                        new ImageModel
                        {
                            Id = !string.IsNullOrEmpty(od.DocumentId.ToString())
                                ? od.DocumentId.ToString()
                                : string.Empty,
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
                        }
                )
                .FirstOrDefault();
            applicationUserModel.InvitedFamilyMembers = context.UserInvitations
                .Where(x => x.UserId == applicationUser.Id)
                .Select(
                    x =>
                        new InvitedFamilyMembers
                        {
                            UserId = applicationUser.Id,
                            InvitedUserEmail = x.InvitedUserEmail,
                            UserType = x.UserType,
                            CreatedOn = x.CreatedOn
                        }
                )
                .ToList();
            var userInvitationByUser = context.UserInvitations.FirstOrDefault(
                ui =>
                    !applicationUser.Email.IsNullOrEmpty()
                        && ui.InvitedUserEmail == applicationUser.Email
                    || !applicationUser.PhoneNumber.IsNullOrEmpty()
                        && ui.InvitedUserEmail == applicationUser.PhoneNumber
            );
            applicationUserModel.IsInvited = userInvitationByUser != null;
            if (applicationUserModel.IsInvited)
            {
                var invitedBy = context.Users.FirstOrDefault(
                    x => x.Id == userInvitationByUser.UserId
                );
                if (invitedBy != null)
                {
                    applicationUserModel.InvitedBy = new InvitedByUser
                    {
                        ECard = invitedBy.ECardSequence,
                        Email = invitedBy.Email,
                        Name = invitedBy.FirstName + " " + invitedBy.LastName
                    };
                }
            }
            applicationUserModel.ReceiveAnnouncement = applicationUser.ReceiveAnnouncement;
            applicationUserModel.ECardSequence = applicationUser.ECardSequence;

            return applicationUserModel;
        }

        /// <summary>
        /// Remove prefix and sufix from eCard number
        /// </summary>
        /// <param name="eCard"></param>
        /// <returns></returns>
        private string FormatECard(string eCard)
        {
            eCard = string.Join(string.Empty, eCard.Skip(4));
            var reversedECard = new string(eCard.Reverse().ToArray());
            eCard = string.Join(string.Empty, reversedECard.Skip(4));

            return new string(eCard.Reverse().ToArray());
        }

        public async Task<Guid?> FindProfilePictureFromId(string userId)
        {
            var context = ContextFactory();
            var image = context.ApplicationUserDocument
                .Where(cd => cd.ApplicationUserId == userId)
                .Select(
                    od =>
                        new ImageModel
                        {
                            Id = !string.IsNullOrEmpty(od.DocumentId.ToString())
                                ? od.DocumentId.ToString()
                                : string.Empty,
                            Type = OfferDocumentType.Thumbnail
                        }
                )
                .FirstOrDefault();
            if (image != null)
            {
                var croppedImage = context.ApplicationUserDocument
                    .Where(
                        i =>
                            i.OriginalImageId == new Guid(image.Id.ToUpper())
                            && i.Type == OfferDocumentType.Thumbnail
                    )
                    .FirstOrDefault();
                return croppedImage?.DocumentId;
            }
            return null;
        }

        public IEnumerable<string> GetUserRoles(string username)
        {
            var context = ContextFactory();

            return (
                from u in context.Users
                join ur in context.UserRoles on u.Id equals ur.UserId
                join r in context.Roles on ur.RoleId equals r.Id
                where u.UserName == username
                select r.Name
            );
        }

        public IEnumerable<string> GetUserRolesId(string userId)
        {
            var context = ContextFactory();

            return (
                from u in context.Users
                join ur in context.UserRoles on u.Id equals ur.UserId
                join r in context.Roles on ur.RoleId equals r.Id
                where u.Id == userId
                select r.Name
            );
        }

        protected override IQueryable<ApplicationUserModel> GetEntities()
        {
            throw new NotImplementedException();
        }

        public async Task InsertNewAlumniUsers(
            List<AllowedEmailsForRegistrationModel> allowedEmails,
            ILogger logger
        )
        {
            logger.LogInformation($"CheckForNewAlumniUsers in repo -> start");
            var context = ContextFactory();

            var alumniDomain = await context.UserDomain.FirstOrDefaultAsync(
                x => x.DomainName == UserType.AlumniRetirementMembers.ToString()
            );
            var adminId = await (
                from ur in context.UserRoles
                join r in context.Roles on ur.RoleId equals r.Id
                where
                    r.Name == Roles.Admin.ToString() || r.Name == Roles.AdnocCoordinator.ToString()
                select ur.UserId
            ).FirstOrDefaultAsync();
            var invitationsList = context.UserInvitations.ToList();
            var newAlumni = new HashSet<UserInvitations>();
            logger.LogInformation($"CheckForNewAlumniUsers in repo -> before adding");

            foreach (var invitation in allowedEmails)
            {
                if (
                    invitationsList.Any(
                        ae =>
                            !ae.InvitedUserEmail.IsNullOrEmpty()
                            && ae.InvitedUserEmail.ToLower() == invitation.Email.ToLower()
                    )
                    || newAlumni.Any(
                        a =>
                            !a.InvitedUserEmail.IsNullOrEmpty()
                            && a.InvitedUserEmail.ToLower() == invitation.Email.ToLower()
                    )
                )
                    continue;

                var newInvitation = new UserInvitations
                {
                    InvitedUserEmail = invitation.Email,
                    UserId = adminId,
                    CreatedBy = adminId,
                    CreatedOn = invitation.CreatedOn,
                    UpdatedOn = invitation.UpdatedOn,
                    UpdatedBy = adminId,
                    UserType = alumniDomain.Id
                };
                newAlumni.Add(newInvitation);
            }
            try
            {
                context.UserInvitations.AddRange(newAlumni);
                logger.LogInformation($"CheckForNewAlumniUsers in repo -> after adding");
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError($"Repo error: -> " + e.ToString());
            }
            logger.LogInformation($"CheckForNewAlumniUsers in repo -> before SaveChangesAsync()");
        }

        public async Task ImportSupplierToAllowedEmailsForRegistration(string email)
        {
            var context = ContextFactory();

            if (
                !context.AllowedEmailsForRegistration
                    .Select(ae => ae.Email.ToLower() == email.ToLower())
                    .Any()
            )
            {
                context.AllowedEmailsForRegistration.Add(
                    new AllowedEmailsForRegistration
                    {
                        Email = email,
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow,
                        InviteSent = false,
                        // NOTE: This should be changed (After we get all correct UserTypes)
                        UserType = UserType.Other
                    }
                );
            }

            await context.SaveChangesAsync();
        }

        public async Task ImportUserFromMobileAPIToAllowedEmailsForRegistration(string email)
        {
            var context = ContextFactory();

            if (
                !context.AllowedEmailsForRegistration
                    .Select(ae => ae.Email.ToLower() == email.ToLower())
                    .Any()
            )
            {
                context.AllowedEmailsForRegistration.Add(
                    new AllowedEmailsForRegistration
                    {
                        Email = email,
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow,
                        InviteSent = false,
                        // NOTE: This should be changed (After we get all correct UserTypes)
                        UserType = UserType.Other
                    }
                );
            }

            await context.SaveChangesAsync();
        }

        public async Task<int> GetUserInRoleCount(string[] roles)
        {
            var context = ContextFactory();

            return await (
                from ur in context.UserRoles
                join r in context.Roles on ur.RoleId equals r.Id
                where roles.Contains(r.Name)
                select ur.UserId
            ).CountAsync();
        }

        public async Task<bool> CanUserSendInvitation(string userId)
        {
            var isCoordinatorRole = await IsAdnocCoordinator(userId);
            if (isCoordinatorRole)
                return false;

            var context = ContextFactory();
            var adnocFamilyDomain = await context.UserDomain.FirstOrDefaultAsync(
                x =>
                    x.DomainName == UserType.ADNOCEmployeeFamilyMember.ToString()
                    || x.DomainName == UserType.AlumniRetirementMembers.ToString()
            );
            bool isMaximumInvitiationExceeded = false;
            var userType = await context.Users
                .Where(x => x.Id == userId)
                .Select(x => x.UserType)
                .FirstOrDefaultAsync();

            bool isAdnocEmployee = userType == (int)Declares.UserType.ADNOCEmployee;

            if (await context.Users.AnyAsync(u => u.Id == userId))
            {
                int maxNumberOfUserInvitations = int.Parse(
                    _configuration["InviteIntegration:MaxNumberOfUserInvitations"]
                );
                isMaximumInvitiationExceeded =
                    await context.UserInvitations
                        .Where(ui => ui.UserId == userId && ui.UserType == adnocFamilyDomain.Id)
                        .GroupBy(ui => ui.UserId)
                        .Select(g => g.Count())
                        .FirstOrDefaultAsync() >= maxNumberOfUserInvitations;
            }

            return isMaximumInvitiationExceeded && isAdnocEmployee;
        }

        public async Task<bool> IsAdnocCoordinator(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var roles = await _userManager.GetRolesAsync(user);
            return roles.Contains(Declares.Roles.AdnocCoordinator.ToString());
        }

        public async Task SetUserInvitation(string userId, string invitedUserEmail)
        {
            var context = ContextFactory();
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var adnocFamilyMemberDomain = await context.UserDomain.FirstOrDefaultAsync(
                x => x.DomainName == UserType.ADNOCEmployeeFamilyMember.ToString()
            );

            if (user != null)
            {
                if (
                    user.UserName != invitedUserEmail
                    && !context.UserInvitations.Any(
                        ui => ui.UserId == userId && ui.InvitedUserEmail == invitedUserEmail
                    )
                )
                {
                    var invitation = new UserInvitations
                    {
                        UserId = userId,
                        InvitedUserEmail = invitedUserEmail,
                        CreatedBy = userId,
                        CreatedOn = DateTime.UtcNow,
                        UpdatedBy = userId,
                        UpdatedOn = DateTime.UtcNow,
                        UserType = adnocFamilyMemberDomain.Id
                    };

                    context.UserInvitations.Add(invitation);
                }
                else
                {
                    var invitation = context.UserInvitations.FirstOrDefault(
                        ui => ui.UserId == userId && ui.InvitedUserEmail == invitedUserEmail
                    );
                    invitation.UpdatedBy = userId;
                    invitation.UpdatedOn = DateTime.UtcNow;
                    context.UserInvitations.Update(invitation);
                }
            }

            await context.SaveChangesAsync();
        }

        public Task DeleteOfInvitedUser(string username)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DoesUserExist(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                user = await _userManager.FindByEmailAsync(username);
            return user == null ? true : false;
        }

        public async Task DeleteUserInvitation(string userId, string username)
        {
            var context = ContextFactory();
            //what needs to be done with userId
            IEnumerable<UserInvitations> invitations = context.UserInvitations.Where(
                ui => ui.InvitedUserEmail == username && ui.UserId == userId
            );

            foreach (var invitation in invitations)
            {
                context.UserInvitations.Remove(invitation);
            }

            await context.SaveChangesAsync();
        }

        public async Task DeleteInvitedUserByAdmin(string username)
        {
            var exist = await DoesUserExist(username);

            if (!exist)
                throw new Exception("User already registered.");

            var context = ContextFactory();

            IEnumerable<UserInvitations> invitations = context.UserInvitations.Where(
                ui => ui.InvitedUserEmail == username
            );

            foreach (var invitation in invitations)
            {
                context.UserInvitations.Remove(invitation);
            }

            await context.SaveChangesAsync();
        }

        public Task DeleteUser(string username)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<InvitedUserModel>> GetInvitedUsers(string userId)
        {
            var context = ContextFactory();
            List<InvitedUserModel> invitedUsers = new List<InvitedUserModel>();

            var adnocFamilyDomain = await context.UserDomain.FirstOrDefaultAsync(
                x => x.DomainName == UserType.ADNOCEmployeeFamilyMember.ToString()
            );

            var userInvitations = context.UserInvitations
                .Where(ui => ui.UserId == userId && ui.UserType == adnocFamilyDomain.Id)
                .ToList();

            foreach (var userInvitation in userInvitations)
            {
                var invitedUser = await context.Users.FirstOrDefaultAsync(
                    u => u.UserName == userInvitation.InvitedUserEmail
                );

                if (invitedUser != null)
                {
                    invitedUsers.Add(
                        new InvitedUserModel { IsRegistered = true, Email = invitedUser.Email }
                    );
                }
                else
                {
                    invitedUsers.Add(
                        new InvitedUserModel
                        {
                            Email = userInvitation.InvitedUserEmail,
                            IsInvited = true
                        }
                    );
                }
            }

            return invitedUsers;
        }

        public async Task<bool> DoesUserInvited(string invitedUserEmail)
        {
            var context = ContextFactory();

            var userInvitations = context.UserInvitations
                .Where(ui => ui.InvitedUserEmail == invitedUserEmail)
                .ToList();
            if (userInvitations.Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<IEnumerable<UserInvitationModel>> GetAllUserInvitations()
        {
            var context = ContextFactory();

            return await context.UserInvitations
                .Select(
                    ui =>
                        new UserInvitationModel
                        {
                            InvitedUserEmail = ui.InvitedUserEmail,
                            UserId = ui.UserId,
                            UserType = ui.UserDomain.DomainName
                        }
                )
                .ToListAsync();
        }

        public async Task<bool> CheckIfUserIsInvitedOrIsInAcceptedDomains(string username)
        {
            var context = ContextFactory();
            var invited = await context.UserInvitations.AnyAsync(
                ui => ui.InvitedUserEmail == username
            );
            var domain = GetDomainFromEmail(username);
            var accpetedDomains = await context.AcceptedDomain.AnyAsync(
                ad => ad.Domain.Equals(domain)
            );

            return invited || accpetedDomains;
        }

        public async Task<bool> CheckIfUserIsInvitedOrIsInUserDomains(string username)
        {
            var context = ContextFactory();
            if (await context.UserInvitations.AnyAsync(ui => ui.InvitedUserEmail == username))
                return true;

            // If user isn't invited we check if he belongs to specific domain
            var domain = GetDomainFromEmail(username);
            //var userDomain = await context.UserDomain.FirstOrDefaultAsync(ud => ud.Id == invitation.UserType);
            //var acceptedDomains = userDomain.Domains.Split(',').ToList();
            List<string> acceptedDomains = GetAllUserDomains();

            return acceptedDomains.Contains(domain);
        }

        private List<string> GetAllUserDomains()
        {
            var context = ContextFactory();
            var allAcceptedDomains = new List<string>();

            var userDomains = context.UserDomain.Select(x => x.Domains).ToList();

            foreach (var ud in userDomains)
            {
                var domains = ud.Split(',').ToList();
                allAcceptedDomains.AddRange(domains);
            }

            return allAcceptedDomains.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        }

        private string GetDomainFromEmail(string username)
        {
            var array = username.Split("@");

            if (array.Count() > 1)
                return "@" + array[1];

            return string.Empty;
        }

        public int GetInvitedUserType(string username)
        {
            var context = ContextFactory();
            return context.UserInvitations
                .Where(ui => ui.InvitedUserEmail == username)
                .Select(ui => ui.UserType)
                .FirstOrDefault();
        }

        public List<UserDomainModel> GetUserDomains()
        {
            var context = ContextFactory();
            return context.UserDomain
                .Select(
                    x =>
                        new UserDomainModel
                        {
                            Id = x.Id,
                            DomainName = x.DomainName,
                            Domains = x.Domains,
                            KeyValue = x.KeyValue,
                            SequencerName = x.SequencerName
                        }
                )
                .ToList();
        }

        public int GetUserDomain(string domain)
        {
            var context = ContextFactory();
            return context.UserDomain
                .Where(x => x.Domains.ToLower().Contains(domain.ToLower()))
                .Select(u => u.Id)
                .FirstOrDefault();
        }

        public async Task SetFcmMessagesToken(string userId, string token)
        {
            var context = ContextFactory();
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                // If token with that message exists with another user it is deleted from that user and added to the current user
                // If token with that message doesn't exists it is added to DB
                // If token with that message exists with current user nothing is done
                var userFcmToken = await context.UserFcmTokens.FirstOrDefaultAsync(
                    uft => uft.FcmMessageToken == token
                );
                if (userFcmToken != null && userFcmToken.UserId != userId)
                {
                    context.UserFcmTokens.Remove(userFcmToken);
                    context.UserFcmTokens.Add(
                        new UserFcmToken { UserId = userId, FcmMessageToken = token }
                    );
                }
                else if (userFcmToken == null)
                {
                    context.UserFcmTokens.Add(
                        new UserFcmToken { UserId = userId, FcmMessageToken = token }
                    );
                }
                await context.SaveChangesAsync();
            }
        }

        public async Task RemoveUserFcmMessagesToken(string userId, string token)
        {
            var context = ContextFactory();
            var fcmToken = await context.UserFcmTokens.FirstOrDefaultAsync(
                x => x.UserId == userId && x.FcmMessageToken == token
            );
            if (fcmToken != null)
            {
                context.UserFcmTokens.Remove(fcmToken);
                await context.SaveChangesAsync();
            }
        }

        public async Task<List<string>> GetFcmDevicesFromUsersIds(List<string> usersId)
        {
            var context = ContextFactory();
            return await context.UserFcmTokens
                .Where(uft => usersId.Contains(uft.UserId))
                .Select(uft => uft.FcmMessageToken)
                .ToListAsync();
        }

        public async Task<List<string>> GetUserFcmDevices(string userId)
        {
            var context = ContextFactory();

            return await context.UserFcmTokens
                .Where(uft => uft.UserId == userId)
                .Select(uft => uft.FcmMessageToken)
                .ToListAsync();
        }

        public async Task<List<string>> GetUserFcmDevicesInRole(Roles role)
        {
            var context = ContextFactory();

            return await (
                from ucf in context.UserFcmTokens
                join u in context.Users on ucf.UserId equals u.Id
                join ur in context.UserRoles on u.Id equals ur.UserId
                join r in context.Roles on ur.RoleId equals r.Id
                where r.Name == role.ToString()
                select ucf.FcmMessageToken
            ).ToListAsync();
        }

        public async Task<string> UpdateECardSequence(string username, string eCardSequence)
        {
            var context = ContextFactory();

            var user = await context.Users.Where(u => u.UserName == username).FirstOrDefaultAsync();

            if (user != null)
            {
                user.ECardSequence = eCardSequence;
                context.Users.Update(user);

                await context.SaveChangesAsync();

                return user.ECardSequence;
            }

            return null;
        }

        public async Task RemoveRolesFromUser(string userId, IEnumerable<string> roles)
        {
            var context = ContextFactory();

            foreach (var rl in roles)
            {
                var role = await context.Roles.FirstOrDefaultAsync(r => r.Name == rl);

                if (context.UserRoles.Any(r => r.RoleId == role.Id && r.UserId == userId))
                {
                    context.UserRoles.Remove(
                        new IdentityUserRole<string>() { UserId = userId, RoleId = role.Id }
                    );
                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task<int> GetCountOfUserRoles(string userId)
        {
            var context = ContextFactory();

            return await context.UserRoles.CountAsync(ur => ur.UserId == userId);
        }

        public IEnumerable<UserTypeCountModel> GetCountForAllTypeOfUsers()
        {
            var context = ContextFactory();

            return (
                from u in context.Users
                group u by u.UserType into g
                select new UserTypeCountModel { UserType = g.Key.ToString(), Count = g.Count() }
            );
        }

        public async Task InsertIntoUserInvitationTable(
            string userId,
            IEnumerable<InviteUsersModel> invitedEmails
        )
        {
            var context = ContextFactory();

            foreach (var mail in invitedEmails)
            {
                if (
                    !await context.UserInvitations.AnyAsync(
                        u => u.InvitedUserEmail == mail.InvitedUserEmail
                    )
                )
                {
                    context.UserInvitations.Add(
                        new UserInvitations
                        {
                            UserId = userId,
                            InvitedUserEmail = mail.InvitedUserEmail,
                            CreatedBy = userId,
                            CreatedOn = DateTime.UtcNow,
                            UpdatedBy = userId,
                            UpdatedOn = DateTime.UtcNow,
                            UserType = mail.UserTypeId
                        }
                    );

                    var emailData = _mailStorageRepository.CreateMailData(
                        mail,
                        Declares.MessageTemplateList.Adnoc_Employee_Invited_New_Family_Member
                    );
                    await _mailStorageRepository.CreateMail(emailData);
                }
            }
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserInvitationModel>> GetAllUserInvitationsPaginated(
            QueryModel queryModel
        )
        {
            var context = ContextFactory();

            var userInvitations =
                from ui in context.UserInvitations
                join u in context.Users on ui.InvitedUserEmail equals u.UserName into invited
                from invites in invited.DefaultIfEmpty()
                select new UserInvitationModel
                {
                    InvitedUserEmail = ui.InvitedUserEmail,
                    UserId = ui.UserId,
                    UserType = ui.UserDomain.DomainName,
                    InvitedOn = ui.CreatedOn,
                    IsRegistered = invites != null,
                    RegisteredOn = invites.CreatedOn,
                    LastLoggedInOn = invites.UpdatedOn
                };

            var filteredUserInvitations = FilterUserInvitations(userInvitations, queryModel);

            return SortUserInvitationIEnumerable(queryModel.Sort, filteredUserInvitations);
        }

        private static IEnumerable<UserInvitationModel> SortUserInvitationIEnumerable(
            SortModel sortModel,
            IEnumerable<UserInvitationModel> userInvitationModel
        )
        {
            if (sortModel.Type == "date")
            {
                if (sortModel.Order == Order.DESC)
                {
                    return userInvitationModel.OrderByDescending(x => x.InvitedOn);
                }
                else
                {
                    return userInvitationModel.OrderBy(x => x.InvitedOn);
                }
            }
            else
            {
                return userInvitationModel.OrderByDescending(x => x.InvitedOn);
            }
        }

        private static UserType GetCorrectUserType(UserType userType)
        {
            return userType switch
            {
                UserType.Undefined => UserType.Undefined,
                UserType.ADNOCEmployee => UserType.ADNOCEmployee,
                UserType.ADNOCEmployeeFamilyMember => UserType.ADNOCEmployeeFamilyMember,
                UserType.ADPolice => UserType.ADPolice,
                UserType.RedCrescent => UserType.RedCrescent,
                UserType.AlumniRetirementMembers => UserType.AlumniRetirementMembers,
                UserType.ADSchools => UserType.ADSchools,
                UserType.Other => UserType.Other,
                _ => UserType.Undefined,
            };
        }

        public async Task AddNewDomain(string domain)
        {
            var context = ContextFactory();

            if (!await context.AcceptedDomain.AnyAsync(x => x.Domain.Contains(domain)))
            {
                context.AcceptedDomain.Add(
                    new AcceptedDomain { Id = Guid.NewGuid().ToString(), Domain = domain }
                );

                await context.SaveChangesAsync();
            }
        }

        public async Task<string> GetUserType(string username)
        {
            var context = ContextFactory();

            var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == username);

            if (user == null)
                return "User doesn't exists.";

            return await context.UserDomain
                .Where(x => x.Id == user.UserType)
                .Select(x => x.DomainName)
                .FirstOrDefaultAsync();
        }

        public async Task ChangeUserType(string username, int newType)
        {
            var context = ContextFactory();

            var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == username);

            if (user == null || newType == 0)
                return;

            var userDomain = await context.UserDomain.FirstOrDefaultAsync(x => x.Id == newType);
            if (userDomain == null)
                return;

            if (user.UserType != userDomain.Id)
            {
                user.UserType = userDomain.Id;
                user.UpdatedOn = DateTime.UtcNow;
                context.Users.Update(user);
                await context.SaveChangesAsync();
            }
            var ecard = await GenerateECardForUser(user.Id);
            user.ECardSequence = ecard.ECardSequence;
            user.UpdatedOn = DateTime.UtcNow;
            context.Users.Update(user);
            await context.SaveChangesAsync();
        }

        public async Task<int> GetFcmTokenCount()
        {
            var context = ContextFactory();

            return await context.UserFcmTokens.CountAsync();
        }

        public async Task<int> GetFcmTokenCount(string role)
        {
            var context = ContextFactory();

            return await (
                from ucf in context.UserFcmTokens
                join u in context.Users on ucf.UserId equals u.Id
                join ur in context.UserRoles on u.Id equals ur.UserId
                join r in context.Roles on ur.RoleId equals r.Id
                where r.Name == role.ToString()
                select ucf.FcmMessageToken
            ).CountAsync();
        }

        public async Task<string> UpdateRedCrescentECardData(
            string username,
            string eCard,
            Declares.UserType type
        )
        {
            var context = ContextFactory();

            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
                return "User doesn't exist.";

            if (
                !await context.Users.AnyAsync(
                    x => x.ECardSequence == eCard && x.UserName != username
                )
            )
            {
                user.UserType = (int)type;
                user.ECardSequence = eCard;
                var result = await _userManager.UpdateAsync(user);

                return $"Successfully updated eCard data for user - {user.UserName} to eCard: [{user.ECardSequence}]";
            }

            return "There is already user with that eCard data";
        }

        public IEnumerable<string> GetAllAcceptedDomains()
        {
            var context = ContextFactory();

            return context.AcceptedDomain.Select(x => x.Domain).ToList();
        }

        public async Task<bool> CheckIfThereIsDuplicateECard()
        {
            var context = ContextFactory();

            return await context.Users
                .Where(x => x.ECardSequence != null)
                .GroupBy(x => x.ECardSequence)
                .AnyAsync(g => g.Count() > 1);
        }

        public List<ApplicationUserModel> UsersWithDuplicateECard(bool remove)
        {
            var context = ContextFactory();

            var DuplicatedECards = context.Users
                .GroupBy(x => x.ECardSequence)
                .Where(g => g.Count() > 1 && g.Key != null)
                .Select(g => g.Key)
                .ToList();

            var users = new List<ApplicationUserModel>();

            foreach (var eCard in DuplicatedECards)
            {
                var u = context.Users
                    .Where(x => x.ECardSequence == eCard)
                    .Select(projectToApplicationUserModel)
                    .ToList();

                users.AddRange(u);
            }

            if (remove)
            {
                foreach (var user in users)
                {
                    var u = context.Users.Where(x => x.Id == user.Id).FirstOrDefault();
                    u.ECardSequence = null;
                    context.Users.Update(u);
                }

                context.SaveChanges();
            }

            return users;
        }

        public async Task<int> UpdateInvalidUserTypes()
        {
            var context = ContextFactory();

            var users = context.Users.Where(u => u.UserType == 0 || (int)u.UserType > 7).ToList();

            foreach (var user in users)
            {
                user.UserType = (int)UserType.Other;
                user.ECardSequence = null;
                context.Users.Update(user);
            }

            await context.SaveChangesAsync();

            return users.Count;
        }

        public async Task<int> ClearInvalidECards()
        {
            var context = ContextFactory();

            var usersExceptRedCrescent = context.Users
                .Where(u => u.UserType != (int)UserType.RedCrescent)
                .ToList();

            foreach (var u in usersExceptRedCrescent)
            {
                u.ECardSequence = null;
                context.Users.Update(u);
            }

            await context.SaveChangesAsync();

            return usersExceptRedCrescent.Count;
        }

        public async Task AddNewUserDomain(UserDomainModel userDomainModel)
        {
            var context = ContextFactory();

            if (!await context.UserDomain.AnyAsync(x => x.DomainName == userDomainModel.DomainName))
            {
                var defaultSequencer = _configuration["DefaultSequencer"];
                var domain = new UserDomain
                {
                    DomainName = userDomainModel.DomainName,
                    Domains = userDomainModel.Domains,
                    KeyValue = userDomainModel.KeyValue,
                    SequencerName = defaultSequencer
                };

                context.UserDomain.Add(domain);
                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateUserDomain(UserDomainModel userDomainModel)
        {
            var context = ContextFactory();
            var userDomain = context.UserDomain.FirstOrDefault(x => x.Id == userDomainModel.Id);
            if (userDomain != null)
            {
                if (userDomain.KeyValue != userDomainModel.KeyValue)
                {
                    var usersWithNewDomain = await context.Users
                        .Where(x => x.UserType == userDomain.Id)
                        .ToListAsync();

                    foreach (var u in usersWithNewDomain)
                    {
                        if (u.ECardSequence != null)
                        {
                            u.ECardSequence = u.ECardSequence.Remove(
                                u.ECardSequence.Length - userDomain.KeyValue.Length
                            );
                            u.ECardSequence = u.ECardSequence + userDomainModel.KeyValue;
                            context.Users.Update(u);
                        }
                    }

                    await context.SaveChangesAsync();
                }

                userDomain.KeyValue = userDomainModel.KeyValue;
                userDomain.SequencerName = userDomainModel.SequencerName;
                userDomain.Domains = userDomainModel.Domains;
                userDomain.DomainName = userDomainModel.DomainName;
                context.UserDomain.Update(userDomain);
            }
            await context.SaveChangesAsync();
        }

        public async Task DeleteUserDomain(string domainName)
        {
            var context = ContextFactory();
            var userDomain = context.UserDomain.FirstOrDefault(x => x.DomainName == domainName);
            if (userDomain != null)
            {
                context.UserDomain.Remove(userDomain);
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteSelfInvitedUsers()
        {
            var context = ContextFactory();
            var selfInvited = await (
                from u in context.Users
                join ui in context.UserInvitations on u.Id equals ui.UserId
                where ui.InvitedUserEmail == u.UserName
                select ui
            ).ToListAsync();

            if (selfInvited != null)
            {
                context.UserInvitations.RemoveRange(selfInvited);
                await context.SaveChangesAsync();
            }
        }

        public async Task<List<UserDomainModel>> GetUserTypes()
        {
            var context = ContextFactory();

            return await context.UserDomain
                .Select(
                    x =>
                        new UserDomainModel
                        {
                            Id = x.Id,
                            DomainName = x.DomainName,
                            KeyValue = x.KeyValue,
                            Domains = x.Domains,
                            SequencerName = x.SequencerName
                        }
                )
                .ToListAsync();
        }

        public async Task<UserDomainModel> GetSpecificUserType(int id)
        {
            var context = ContextFactory();

            return await context.UserDomain
                .Where(x => x.Id == id)
                .Select(
                    x =>
                        new UserDomainModel
                        {
                            Id = x.Id,
                            DomainName = x.DomainName,
                            KeyValue = x.KeyValue,
                            Domains = x.Domains
                        }
                )
                .FirstOrDefaultAsync();
        }

        private IEnumerable<UserInvitationModel> FilterUserInvitations(
            IEnumerable<UserInvitationModel> userInvitations,
            QueryModel queryModel
        )
        {
            var filtered = userInvitations;
            if (queryModel.Filter.Keyword.Any())
            {
                filtered = filtered.Where(
                    ui => (ui.InvitedUserEmail ?? string.Empty).Contains(queryModel.Filter.Keyword)
                );
            }

            if (queryModel.Filter.Status.Any())
            {
                filtered = filtered.Where(ui => queryModel.Filter.Status.Contains(ui.UserType));
            }

            return filtered;
        }

        public ICollection<ApplicationUserModel> GetAllUsersForMail()
        {
            var context = ContextFactory();
            var users = (
                from u in context.Users
                join ur in context.UserRoles on u.Id equals ur.UserId into g1
                from p in g1.DefaultIfEmpty()
                join r in context.Roles on p.RoleId equals r.Id into g2
                from p2 in g2.DefaultIfEmpty()
                join ud in context.UserDomain on u.UserType equals ud.Id into g3
                from p3 in g3.DefaultIfEmpty()
                select new ApplicationUserModel
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Active = u.Active,
                    Title = u.Title,
                    Email = u.Email,
                    Role = p2.Name,
                    FcmDevice = (
                        from fcm in context.UserFcmTokens
                        where u.Id == fcm.UserId
                        select fcm.FcmMessageToken
                    ).ToList(),
                    UserType = new UserDomainModel() { DomainName = p3.DomainName }
                }
            );
            return users.ToList();
        }

        public string GetDomainNameFromEmail(string username)
        {
            var array = username.Split("@");

            if (array.Count() > 1)
                return "@" + array[1];

            return string.Empty;
        }

        public async Task<ICollection<ApplicationUserModel>> GetUsersByDomain(
            ICollection<string> domains
        )
        {
            var context = ContextFactory();

            var users = (
                from u in context.Users
                join ur in context.UserRoles on u.Id equals ur.UserId into g1
                from p in g1.DefaultIfEmpty()
                join r in context.Roles on p.RoleId equals r.Id into g2
                from p2 in g2.DefaultIfEmpty()
                join ud in context.UserDomain on u.UserType equals ud.Id into g3
                from p3 in g3.DefaultIfEmpty()
                where domains.Contains(p3.DomainName)
                select new ApplicationUserModel
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Active = u.Active,
                    Title = u.Title,
                    Email = u.Email,
                    FcmDevice = (
                        from fcm in context.UserFcmTokens
                        where u.Id == fcm.UserId
                        select fcm.FcmMessageToken
                    ).ToList(),
                    Role = p2.Name,
                    UserType = new UserDomainModel() { DomainName = p3.DomainName }
                }
            );

            return context.Users
                .Include(x => x.UserDomain)
                .Where(x => domains.Contains(x.UserDomain.Id.ToString()))
                .Select(
                    x =>
                        new ApplicationUserModel()
                        {
                            Id = x.Id,
                            FirstName = x.FirstName,
                            LastName = x.LastName,
                            Active = x.Active,
                            FcmDevice = (
                                from fcm in context.UserFcmTokens
                                where x.Id == fcm.UserId
                                select fcm.FcmMessageToken
                            ).ToList(),
                            Title = x.Title,
                            Email = x.Email,
                            Role = (
                                from role in context.Roles
                                join ur in context.UserRoles on role.Id equals ur.RoleId into g1
                                from p in g1.DefaultIfEmpty()
                                where p.UserId == x.Id
                                select role
                            )
                                .FirstOrDefault()
                                .Name
                        }
                )
                .ToHashSet();
        }

        public ICollection<ApplicationUserModel> GetAllUserByRoleForMail(
            ICollection<string> roleIds
        )
        {
            var rolesName = roleIds.Select(x => x.ToLower());
            var context = ContextFactory();
            var users = (
                from u in context.Users
                join ur in context.UserRoles on u.Id equals ur.UserId into g1
                from p in g1.DefaultIfEmpty()
                join r in context.Roles on p.RoleId equals r.Id into g2
                from p2 in g2.DefaultIfEmpty()
                join ud in context.UserDomain on u.UserType equals ud.Id into g3
                from p3 in g3.DefaultIfEmpty()
                where rolesName.Contains(p2.Name.ToLower())
                select new ApplicationUserModel
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Active = u.Active,
                    Title = u.Title,
                    Email = u.Email,
                    FcmDevice = (
                        from fcm in context.UserFcmTokens
                        where u.Id == fcm.UserId
                        select fcm.FcmMessageToken
                    ).ToList(),
                    Role = p2.Name,
                    UserType = new UserDomainModel() { DomainName = p3.DomainName }
                }
            );

            return users.ToHashSet();
        }

        public ICollection<string> GetAdminsForOfferReportNotifications()
        {
            var context = ContextFactory();
            var users = (
                from u in context.Users
                join ur in context.UserRoles on u.Id equals ur.UserId into g1
                from p in g1.DefaultIfEmpty()
                join r in context.Roles on p.RoleId equals r.Id into g2
                from p2 in g2.DefaultIfEmpty()
                where p2.Name == "SuperAdmin" || p2.Name == "Admin" || p2.Name == "AdnocCoordinator"
                select u.Id
            );

            return users.ToHashSet();
        }

        public async Task<IEnumerable<ApplicationUserModel>> GetUsersByEmailsForECard(
            ICollection<string> emails
        )
        {
            var context = ContextFactory();
            var t_emails = emails.Select(x => "t_" + x);

            var users = await context.Users
                .Where(x => emails.Contains(x.Email) || t_emails.Contains(x.Email))
                .Select(projectToApplicationUserModelForECard)
                .ToListAsync();
            return users;
        }

        public ApplicationUserModel GetUserForECard(string Id)
        {
            var context = ContextFactory();
            return context.Users
                .Where(x => x.Id == Id)
                .Select(projectToApplicationUserModelForECard)
                .FirstOrDefault();
        }

        public async Task CreateInvitationForUsersFromSpecificExcelFile(
            string userId,
            IEnumerable<InviteUsersModel> invitedEmails,
            bool flag = false
        )
        {
            var emailsList = new List<EmailDataModel>();
            var userInvationList = new List<UserInvitations>();

            foreach (var mail in invitedEmails)
            {
                userInvationList.Add(
                    new UserInvitations
                    {
                        UserId = userId,
                        InvitedUserEmail = mail.InvitedUserEmail,
                        CreatedBy = userId,
                        CreatedOn = DateTime.UtcNow,
                        UpdatedBy = userId,
                        UpdatedOn = DateTime.UtcNow,
                        UserType = mail.UserTypeId
                    }
                );

                emailsList.Add(
                    _mailStorageRepository.CreateMailData(
                        mail,
                        Declares.MessageTemplateList.Adnoc_Employee_Invited_New_Family_Member
                    )
                );
            }

            var builder = new DbContextOptionsBuilder<MMADbContext>();
            builder.UseSqlServer(
                _configuration.GetConnectionString("Database"),
                b => b.MigrationsAssembly("MMA.WebApi.DataAccess")
            );
            using (MMADbContext contextNew = new MMADbContext(builder.Options))
            {
                await contextNew.UserInvitations.AddRangeAsync(userInvationList);
                await contextNew.SaveChangesAsync();
            }

            userInvationList.Clear();
            userInvationList = null;

            // if (flag)
            // await _mailStorageRepository.CreateEmialInvitationForUsersFromSpecificExcelFile(emailsList, userId);


            emailsList.Clear();
            emailsList = null;

            GC.Collect();
        }

        public async Task<int> GetBuyersCount()
        {
            var context = ContextFactory();
            return await (
                from u in context.Users
                join ur in context.UserRoles on u.Id equals ur.UserId
                join r in context.Roles on ur.RoleId equals r.Id
                where u.Active && r.Name == Roles.Buyer.ToString()
                select new { buyer = u }
            ).CountAsync();
        }

        public async Task<int> GetSuppliersCount()
        {
            var context = ContextFactory();

            return await (
                from c in context.Company
                where c.ApproveStatus == SupplierStatus.Approved.ToString()
                select new { supplier = c }
            ).CountAsync();
        }

        public async Task<ApplicationUserModel> ChangeReceivedAnnouncementStatusForUser(
            bool status,
            string userId
        )
        {
            var context = ContextFactory();
            var user = context.Users
                .Include(x => x.ApplicationUserDocuments)
                .FirstOrDefault(x => x.Id == userId);
            if (user != null)
            {
                user.ReceiveAnnouncement = status;
                context.Users.Update(user);
                await context.SaveChangesAsync();
            }
            return projectToApplicationUserModel.Compile().Invoke(user);
        }
    }
}
