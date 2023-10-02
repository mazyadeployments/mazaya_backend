using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MMA.WebApi.DataAccess.Helpers;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Extensions;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.ApplicationUsers;
using MMA.WebApi.Shared.Interfaces.Companies;
using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Interfaces.MailStorage;
using MMA.WebApi.Shared.Interfaces.Offers;
using MMA.WebApi.Shared.Interfaces.RoadshowDocuments;
using MMA.WebApi.Shared.Interfaces.Roles;
using MMA.WebApi.Shared.Models.ApplicationUser;
using MMA.WebApi.Shared.Models.Attachment;
using MMA.WebApi.Shared.Models.Companies;
using MMA.WebApi.Shared.Models.DefaultLocations;
using MMA.WebApi.Shared.Models.Image;
using MMA.WebApi.Shared.Models.MailStorage;
using MMA.WebApi.Shared.Models.Roadshow;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Declares;
using static MMA.WebApi.Shared.Enums.Document.DocumentDeclares;

namespace MMA.WebApi.DataAccess.Repository.Offers
{
    public class RoadshowRepository : BaseRepository<RoadshowModel>, IRoadshowRepository
    {
        private readonly IRoadshowDocumentRepository _roadshowDocumentRepository;
        private readonly IMailStorageService _mailStorageServiceService;
        private readonly IRoleService _roleService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IApplicationUserService applicationUserService;
        private readonly ICompanyRepository _companyRepository;

        public RoadshowRepository(
            IRoadshowDocumentRepository roadshowDocumentRepository,
            Func<MMADbContext> contexFactory,
            IRoleService roleService,
            IMailStorageService mailStorageServiceService,
            UserManager<ApplicationUser> userManager,
            IApplicationUserService applicationUserService,
            ICompanyRepository companyRepository
        )
            : base(contexFactory)
        {
            _companyRepository = companyRepository;
            _roadshowDocumentRepository = roadshowDocumentRepository;
            _roleService = roleService;
            _mailStorageServiceService = mailStorageServiceService;
            _userManager = userManager;
            this.applicationUserService = applicationUserService;
        }

        public async Task<RoadshowModel> CreateRoadshowAsync(
            RoadshowModel model,
            IVisitor<IChangeable> auditVisitor,
            string userId,
            int companyId
        )
        {
            var context = ContextFactory();
            var roadshow = new Roadshow();
            ProcessImages(model);
            PopulateEntityModel(roadshow, model);

            foreach (var offerDocument in roadshow.Documents)
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
                }
            }

            roadshow.CompanyId = companyId;
            roadshow.Accept(auditVisitor);
            context.Add(roadshow);
            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
            return projectToRoadshowModel.Compile().Invoke(roadshow);
        }

        public async Task<RoadshowStatus> GetRoadshowStatusById(int id)
        {
            var context = ContextFactory();
            var roadshow = context.Roadshow.FirstOrDefault(x => x.Id == id);
            if (roadshow == null)
                throw new ArgumentNullException();

            return roadshow.Status;
        }

        public async Task<RoadshowModel> UpdateRoadshowAsync(
            RoadshowModel model,
            IVisitor<IChangeable> auditVisitor,
            string userId
        )
        {
            var context = ContextFactory();
            var roadshow = context.Roadshow
                .Include(roadshow => roadshow.Documents)
                .Include(roadshow => roadshow.RoadshowVouchers)
                .Include(roadshow => roadshow.Locations)
                .ThenInclude(location => location.DefaultLocation)
                .Include(roadshow => roadshow.RoadshowComments)
                .FirstOrDefault(x => x.Id == model.Id);
            if (roadshow == null)
            {
                roadshow = new Roadshow();
            }
            var currentRoadshowStatus = roadshow.Status;
            var newRoadshowStatus = model.Status;

            var newLocations = model.Locations.Select(x => x.Id).ToList();
            if (
                currentRoadshowStatus == RoadshowStatus.Submitted
                && newRoadshowStatus != RoadshowStatus.Draft
            )
            {
                var roadShowExistOnLocation = context.Roadshow
                    .Include(roadshow => roadshow.Locations)
                    .Where(
                        x =>
                            x.DateFrom.Value.Date <= model.DateTo.Value.Date
                            && x.DateTo.Value.Date >= model.DateFrom.Value.Date
                            && x.Locations.Any(
                                location => newLocations.Contains(location.DefaultLocationId)
                            )
                    )
                    .Any();
                if (roadShowExistOnLocation)
                    throw new Exception("Date and location is already booked.");
            }

            ProcessImages(model);
            ProcessComments(roadshow, model, userId, context);
            PopulateEntityModel(roadshow, model);

            if (roadshow.EmiratesId != null)
                roadshow.EmiratesIdDocument = await context.Document.FirstOrDefaultAsync(
                    e => e.Id == roadshow.EmiratesId
                );

            foreach (var offerDocument in roadshow.Documents)
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
                }
            }

            roadshow.Accept(auditVisitor);
            context.Update(roadshow);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw e;
            }

            var retVal = context.Roadshow
                .Include(roadshow => roadshow.Documents)
                .Include(roadshow => roadshow.RoadshowVouchers)
                .Include(roadshow => roadshow.Locations)
                .ThenInclude(location => location.DefaultLocation)
                .Include(roadshow => roadshow.RoadshowComments)
                .FirstOrDefault(x => x.Id == model.Id);

            return projectToRoadshowModel.Compile().Invoke(retVal);
        }

        private async Task SendEmailsToSupplierAdmins(
            string roadshowTitle,
            List<SupplierAdminEmailRecipient> supplierAdminEmailRecipients
        )
        {
            foreach (var supplierAdminEmailRecipient in supplierAdminEmailRecipients)
            {
                var roles = await _roleService.GetUserRoles(
                    supplierAdminEmailRecipient.SupplierAdmin.Id
                );
                if (!roles.Contains(Roles.Admin) && !roles.Contains(Roles.AdnocCoordinator))
                {
                    var emailData = new EmailDataModel()
                    {
                        RoadshowName = roadshowTitle,
                        User = supplierAdminEmailRecipient.SupplierAdmin,
                        CompanyName = supplierAdminEmailRecipient.SupplierAdmin.CompanyName,
                        //RoadshowLocation = loc,
                        MailTemplateId = Declares
                            .MessageTemplateList
                            .Roadshow_Published_Notify_SupplierAdminOrSupplier
                    };

                    await _mailStorageServiceService.CreateMail(emailData);
                }
            }
        }

        private async Task<List<SupplierAdminEmailRecipient>> PublishRoadshow(
            RoadshowModel model,
            Roadshow roadshow
        )
        {
            var supplierAdminEmailRecipients = new List<SupplierAdminEmailRecipient>();

            if (model.Status == RoadshowStatus.Published)
            {
                foreach (var roadshowInvite in roadshow.RoadshowInvites)
                {
                    var supplierAdminModel =
                        await _companyRepository.GetSupplierAdminModelForCompany(
                            roadshowInvite.Company.Id
                        ); //await GetSupplierAdmin(roadshowInvite.Company.CreatedBy);
                    bool isProcessedInTime =
                        roadshowInvite.Status == RoadshowInviteStatus.Approved
                        || roadshowInvite.Status == RoadshowInviteStatus.Rejected;

                    if (!isProcessedInTime)
                    {
                        roadshowInvite.Status = RoadshowInviteStatus.Expired;
                        supplierAdminEmailRecipients.Add(
                            new SupplierAdminEmailRecipient
                            {
                                SupplierAdmin = supplierAdminModel,
                                OffersForRoadshowAccepted = false
                            }
                        );
                    }
                    else
                    {
                        supplierAdminEmailRecipients.Add(
                            new SupplierAdminEmailRecipient
                            {
                                SupplierAdmin = supplierAdminModel,
                                OffersForRoadshowAccepted = true
                            }
                        );
                    }
                }
            }

            return supplierAdminEmailRecipients;
        }

        private async Task<ApplicationUserModel> GetSupplierAdmin(string supplierAdminId)
        {
            var supplierAdmin = await _userManager.FindByIdAsync(supplierAdminId);
            if (supplierAdmin != null)
            {
                ApplicationUserModel supplierAdminModel = new ApplicationUserModel
                {
                    Id = supplierAdmin.Id,
                    Email = supplierAdmin.Email
                };

                return supplierAdminModel;
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        public async Task<RoadshowModel> EditRoadshowForCalendar(
            string userId,
            List<Roles> roles,
            RoadshowModel model
        )
        {
            var context = ContextFactory();
            // add role check
            var roadshow = context.Roadshow.FirstOrDefault(r => r.Id == model.Id);
            if (roadshow != null)
            {
                roadshow.DateFrom = model.DateFrom;
                roadshow.DateTo = model.DateTo;
                roadshow.UpdatedOn = DateTime.UtcNow;
                roadshow.UpdatedBy = userId;
                context.Update(roadshow);

                await context.SaveChangesAsync();
                return projectToRoadshowModel.Compile().Invoke(roadshow);
            }

            return null;
        }

        private void ProcessLocations(RoadshowModel model, Roadshow roadshow)
        {
            roadshow.Locations = new List<RoadshowLocation>();

            foreach (var locationModel in model.Locations)
            {
                var location = GetDefaultLocationByTitle(locationModel.Title);

                roadshow.Locations.Add(
                    new RoadshowLocation
                    {
                        DefaultLocationId = location.Id,
                        RoadshowId = roadshow.Id
                    }
                );
            }
        }

        private void ProcessImages(RoadshowModel model)
        {
            var roadshowDocuments = new List<RoadshowDocumentModel>();

            if (model.ImageSets != null && model.ImageSets.Count > 0)
            {
                foreach (var imageModel in model.ImageSets)
                {
                    roadshowDocuments.Add(
                        new RoadshowDocumentModel
                        {
                            DocumentId = new Guid(imageModel.Id),
                            RoadshowId = model.Id,
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
                }
            }

            model.Documents = roadshowDocuments;
        }

        public DefaultLocation GetDefaultLocationByTitle(string title)
        {
            var context = ContextFactory();

            return context.DefaultLocation.AsNoTracking().FirstOrDefault(x => x.Title == title);
        }

        public async Task<RoadshowModel> GetRoadshow(int id)
        {
            var context = ContextFactory();

            var retval = await context.Roadshow
                .AsNoTracking()
                .Select(projectToRoadshowModel)
                .FirstOrDefaultAsync(x => x.Id == id);

            return retval;
        }

        public IQueryable<Shared.Models.Roadshow.RoadshowModel> Get()
        {
            var context = ContextFactory();

            return context.Roadshow.Select(projectToRoadshowModel);
        }

        protected override IQueryable<RoadshowModel> GetEntities()
        {
            var context = ContextFactory();

            return context.Roadshow.Select(projectToRoadshowModel);
        }

        public IQueryable<DefaultLocationModel> GetAllDefaultLocations()
        {
            var context = ContextFactory();

            return context.DefaultLocation.Select(projectToDefaultLocationModel);
        }

        public async Task<DefaultLocationModel> GetDefaultLocation(int id)
        {
            var context = ContextFactory();

            return await context.DefaultLocation
                .AsNoTracking()
                .Select(projectToDefaultLocationModel)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        private readonly Expression<
            Func<DefaultLocation, DefaultLocationModel>
        > projectToDefaultLocationModel = data =>
            new DefaultLocationModel()
            {
                Id = data.Id,
                Title = data.Title,
                Vicinity = data.Vicinity,
                Country = data.Country
            };

        public IQueryable<RoadshowModel> GetAllRoadshows(
            string userId,
            List<Roles> roles,
            QueryModel queryModel
        )
        {
            var context = ContextFactory();
            IQueryable<Roadshow> roadshows = context.Roadshow.AsNoTracking();

            if (roles.Contains(Roles.AdnocCoordinator) || roles.Contains(Roles.Admin))
            {
                roadshows = (from r in roadshows where r.Status != RoadshowStatus.Draft select r);
            }
            else if (roles.Contains(Roles.SupplierAdmin) || roles.Contains(Roles.Supplier))
            {
                roadshows = (
                    from r in roadshows
                    join cs in context.CompanySuppliers on r.CompanyId equals cs.CompanyId
                    where cs.SupplierId == userId
                    select r
                );
            }
            else if (roles.Contains(Roles.RoadshowFocalPoint))
            {
                roadshows = (
                    from r in roadshows
                    where r.Status == RoadshowStatus.Confirmed
                    select r
                );
            }

            var filteredRoadshow = Filter(roadshows, queryModel);
            var roadshowModels = filteredRoadshow.Select(projectToRoadshowCardModel);

            return Sort(queryModel.Sort, roadshowModels);
        }

        public List<RoadshowEventCalendarCard> GetAllEventsForCalendar(
            string userId,
            List<Roles> roles,
            QueryModel queryModel
        )
        {
            var context = ContextFactory();

            if (roles.Contains(Roles.AdnocCoordinator) || roles.Contains(Roles.Admin))
            {
                var roadshowEvents = (
                    from r in context.Roadshow
                    where
                        r.DateFrom < queryModel.Filter.DateTo
                        && r.DateTo > queryModel.Filter.DateFrom
                        && r.Status != RoadshowStatus.Draft
                        && r.Status != RoadshowStatus.Submitted
                        && r.Status != RoadshowStatus.Expired
                    select new RoadshowEventCalendarCard
                    {
                        Id = r.Id,
                        RoadshowId = r.Id,
                        RoadshowName = r.Title,
                        Status = r.Status,
                        DateFrom = r.DateFrom,
                        DateTo = r.DateTo,
                        Description = r.Description,
                        RoadshowLocations = r.Locations
                            .Select(
                                dl =>
                                    new DefaultLocationModel
                                    {
                                        Id = dl.DefaultLocation.Id,
                                        Latitude = dl.DefaultLocation.Latitude,
                                        Longitude = dl.DefaultLocation.Longitude,
                                        Address = dl.DefaultLocation.Address,
                                        Vicinity = dl.DefaultLocation.Vicinity,
                                        Country = dl.DefaultLocation.Country,
                                        Title = dl.DefaultLocation.Title
                                    }
                            )
                            .ToList(),
                        FocalPoint = null,
                        CompanyId = r.CompanyId,
                        CompanyName = r.Company.NameEnglish
                    }
                ).ToList();

                return FilterRoadshowEventsForCalendar(roadshowEvents, queryModel);
            }
            else if (roles.Contains(Roles.RoadshowFocalPoint))
            {
                var roadshowEvents = (
                    from r in context.Roadshow
                    where
                        r.DateFrom < queryModel.Filter.DateTo
                        && r.DateTo > queryModel.Filter.DateFrom
                        && r.Status == RoadshowStatus.Confirmed
                    select new RoadshowEventCalendarCard
                    {
                        Id = r.Id,
                        RoadshowId = r.Id,
                        RoadshowName = r.Title,
                        Status = r.Status,
                        DateFrom = r.DateFrom,
                        DateTo = r.DateTo,
                        Description = r.Description,
                        RoadshowLocations = r.Locations
                            .Select(
                                dl =>
                                    new DefaultLocationModel
                                    {
                                        Id = dl.DefaultLocation.Id,
                                        Latitude = dl.DefaultLocation.Latitude,
                                        Longitude = dl.DefaultLocation.Longitude,
                                        Address = dl.DefaultLocation.Address,
                                        Vicinity = dl.DefaultLocation.Vicinity,
                                        Country = dl.DefaultLocation.Country,
                                        Title = dl.DefaultLocation.Title
                                    }
                            )
                            .ToList(),
                        FocalPoint = null,
                        CompanyId = r.CompanyId,
                        CompanyName = r.Company.NameEnglish
                    }
                ).ToList();

                return FilterRoadshowEventsForCalendar(roadshowEvents, queryModel);
            }

            return new List<RoadshowEventCalendarCard>();
        }

        public IQueryable<RoadshowModel> GetAllRoadshowsForCalendar(
            string userId,
            List<Roles> roles,
            CalendarQueryModel queryModel
        )
        {
            var context = ContextFactory();
            var dateFrom = new DateTime(
                (int)queryModel.YearFrom,
                (int)queryModel.MonthFrom,
                (int)queryModel.DayFrom
            );
            var dateTo = new DateTime(
                (int)queryModel.YearTo,
                (int)queryModel.MonthTo,
                (int)queryModel.DayTo
            );
            IQueryable<Roadshow> roadshows = context.Roadshow
                .AsNoTracking()
                .Where(
                    r =>
                        (r.DateFrom >= dateFrom && r.DateFrom <= dateTo)
                        || (r.DateTo >= dateFrom && r.DateTo <= dateTo)
                );

            var roadshowModels = roadshows.Select(projectToRoadshowCardModel);

            return roadshowModels;
        }

        private static List<RoadshowEventCalendarCard> FilterRoadshowEventsForCalendar(
            List<RoadshowEventCalendarCard> roadshowEvents,
            QueryModel queryModel
        )
        {
            string s = "";
            var filteredRoadshowEvents = roadshowEvents.DistinctBy(r => r.RoadshowId, s);
            filteredRoadshowEvents = filteredRoadshowEvents
                .Where(
                    roadshowEvent =>
                        roadshowEvent.Status != RoadshowStatus.Draft
                        || roadshowEvent.Status != RoadshowStatus.Submitted
                )
                .ToList();

            if (queryModel.Filter.Companies.Count() > 0)
            {
                filteredRoadshowEvents = filteredRoadshowEvents
                    .Where(
                        roadshowEvent =>
                            queryModel.Filter.Companies.Contains((int)roadshowEvent.CompanyId)
                    )
                    .ToList();
            }

            if (queryModel.Filter.Roadshows.Count() > 0)
            {
                filteredRoadshowEvents = filteredRoadshowEvents
                    .Where(
                        roadshowEvent =>
                            queryModel.Filter.Roadshows.Contains(roadshowEvent.RoadshowId)
                    )
                    .ToList();
            }

            if (queryModel.Filter.Locations.Count() > 0)
            {
                var locations = GetLocationIdsFromObject(queryModel.Filter.Locations);

                filteredRoadshowEvents = filteredRoadshowEvents
                    .Where(
                        roadshowEvent =>
                            roadshowEvent.RoadshowLocations.Any(x => locations.Contains(x.Id))
                    )
                    .ToList();
            }

            if (queryModel.Filter.StatusEnum.Count() > 0)
            {
                var filteredStatuses = Enum.GetValues(typeof(RoadshowStatus))
                    .Cast<RoadshowStatus>()
                    .Where(rs => queryModel.Filter.Status.Contains(rs.ToString()))
                    .ToList();

                filteredRoadshowEvents = filteredRoadshowEvents
                    .Where(
                        roadshowEvent => queryModel.Filter.StatusEnum.Contains(roadshowEvent.Status)
                    )
                    .ToList();
            }

            return filteredRoadshowEvents.ToList();
        }

        private static IQueryable<Roadshow> Filter(
            IQueryable<Roadshow> roadshows,
            QueryModel queryModel
        )
        {
            var filteredRoadshows = roadshows.Where(
                roadshow =>
                    roadshow.Title.ToLower().Contains(queryModel.Filter.Keyword.Trim().ToLower())
                    || roadshow.Locations
                        .Where(
                            x =>
                                x.DefaultLocation.Title.Contains(
                                    queryModel.Filter.Keyword.Trim().ToLower()
                                )
                        )
                        .Any()
            );

            if (queryModel.Filter.Status?.Any() == true)
            {
                var filteredStatuses = Enum.GetValues(typeof(RoadshowStatus))
                    .Cast<RoadshowStatus>()
                    .Where(rs => queryModel.Filter.Status.Contains(rs.ToString()))
                    .ToList();

                filteredRoadshows = filteredRoadshows.Where(
                    r => filteredStatuses.Contains(r.Status)
                );
            }

            return filteredRoadshows;
        }

        private static IQueryable<RoadshowModel> Sort(
            SortModel sortModel,
            IQueryable<RoadshowModel> roadshows
        )
        {
            if (sortModel.Type == "date")
            {
                if (sortModel.Order == Order.DESC)
                {
                    return roadshows.OrderByDescending(x => x.CreatedOn);
                }
                else
                {
                    return roadshows.OrderBy(x => x.CreatedOn);
                }
            }
            else
            {
                return roadshows.OrderByDescending(x => x.CreatedOn);
            }
        }

        public async Task<RoadshowModel> DeleteAsync(int id)
        {
            var context = ContextFactory();
            var roadshow = await context.Roadshow
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (roadshow != null)
            {
                var locations = await context.RoadshowLocation
                    .Where(x => x.RoadshowId == roadshow.Id)
                    .ToListAsync();
                var vouchers = await context.RoadshowVoucher
                    .Where(x => x.RoadshowId == roadshow.Id)
                    .ToListAsync();
                var roadshowDocuments = await context.RoadshowDocument
                    .Where(x => x.RoadshowId == roadshow.Id)
                    .ToListAsync();

                context.RoadshowLocation.RemoveRange(locations);
                context.RoadshowVoucher.RemoveRange(vouchers);
                context.RoadshowDocument.RemoveRange(roadshowDocuments);
                context.Roadshow.Remove(roadshow);
                context.SaveChanges();
            }
            return projectToRoadshowModel.Compile().Invoke(roadshow);
        }

        private static List<int> GetLocationIdsFromObject(
            List<DefaultLocationModel> defaultLocations
        )
        {
            var defaultLocationIds = new List<int>();
            defaultLocations.ForEach(x =>
            {
                defaultLocationIds.Add(x.Id);
            });

            return defaultLocationIds;
        }

        private Expression<Func<Roadshow, RoadshowModel>> projectToRoadshowModel = data =>
            new RoadshowModel()
            {
                Id = data.Id,
                Title = data.Title,
                DateFrom = data.DateFrom,
                DateTo = data.DateTo,
                CreatedOn = data.CreatedOn,
                CreatedBy = data.CreatedBy,
                UpdatedOn = data.UpdatedOn.SpecifyKind(DateTimeKind.Utc),
                UpdatedBy = data.UpdatedBy,
                Locations =
                    data.Locations != null
                        ? data.Locations
                            .Select(
                                dl =>
                                    new DefaultLocationModel
                                    {
                                        Id = dl.DefaultLocationId,
                                        Address = dl.DefaultLocation.Address,
                                        Title = dl.DefaultLocation.Title,
                                        Country = dl.DefaultLocation.Country,
                                        Latitude = dl.DefaultLocation.Latitude,
                                        Longitude = dl.DefaultLocation.Longitude,
                                        Vicinity = dl.DefaultLocation.Vicinity
                                    }
                            )
                            .ToList()
                        : new List<DefaultLocationModel>(),
                // Documents
                Documents = data.Documents
                    .Select(
                        dr =>
                            new RoadshowDocumentModel
                            {
                                Id = dr.Id,
                                RoadshowId = dr.RoadshowId,
                                DocumentId = !string.IsNullOrEmpty(dr.DocumentId.ToString())
                                    ? dr.DocumentId
                                    : Guid.Empty,
                                CreatedOn = dr.CreatedOn.SpecifyKind(DateTimeKind.Utc),
                                CreatedBy = dr.CreatedBy,
                                UpdatedOn = dr.UpdatedOn.SpecifyKind(DateTimeKind.Utc),
                                UpdatedBy = dr.UpdatedBy,
                                Type = dr.Type,
                                OriginalImageId = dr.DocumentId,
                                X1 = 0, //dr.X1,
                                X2 = 0,
                                Y1 = 0,
                                Y2 = 0,
                                cropX1 = 0,
                                cropX2 = 0,
                                cropY1 = 0,
                                cropY2 = 0,
                                Cover = dr.Cover
                            }
                    )
                    .ToList(),
                ImageSets = data.Documents
                    .Where(x => x.RoadshowId == data.Id)
                    .Select(
                        im =>
                            new ImageModel
                            {
                                Id = im.DocumentId.ToString(),
                                Type = im.Type,
                                OriginalImageId = im.OriginalImageId,
                                CropCoordinates = new CropCoordinates
                                {
                                    X1 = im.X1,
                                    X2 = im.X2,
                                    Y1 = im.Y1,
                                    Y2 = im.Y2
                                },
                                CropNGXCoordinates = new CropCoordinates
                                {
                                    X1 = im.cropX1,
                                    X2 = im.cropX2,
                                    Y1 = im.cropY1,
                                    Y2 = im.cropY2
                                },
                                Cover = im.Cover
                            }
                    )
                    .ToList(),
                Image = data.Documents
                    .Where(cd => cd.RoadshowId == data.Id)
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
                Description = data.Description,
                Activities = data.Activities,
                Status = data.Status,
                RoadshowVouchers = data.RoadshowVouchers
                    .Select(
                        vouchers =>
                            new RoadshowVoucherModel
                            {
                                Id = vouchers.Id,
                                Quantity = vouchers.Quantity,
                                Validity = vouchers.Validity,
                                Details = vouchers.Details
                            }
                    )
                    .ToList(),
                InstructionBox = data.InstructionBox,
                FocalPointEmail = data.FocalPointEmail,
                FocalPointSurname = data.FocalPointSurname,
                FocalPointName = data.FocalPointName,
                PhoneNumber = new PhoneNumberModel
                {
                    CountryCode = data.CountryCode,
                    InternationalNumber = data.InternationalNumber,
                    Number = data.Number,
                    E164Number = data.E164Number
                },
                EmiratesId =
                    data.EmiratesIdDocument == null
                        ? null
                        : new AttachmentModel
                        {
                            Id = data.EmiratesIdDocument.Id.ToString(),
                            Name = data.EmiratesIdDocument.Name,
                            Type = data.EmiratesIdDocument.MimeType
                        },
                Comments = data.RoadshowComments
                    .Select(
                        x =>
                            new RoadshowCommentModel
                            {
                                Id = x.Id,
                                CreatedBy = x.CreatedBy,
                                CreatedOn = x.CreatedOn,
                                Text = x.Text,
                                RoadshowId = x.RoadshowId,
                                CreatedByName = x.CreatedByName
                            }
                    )
                    .ToList(),
                SupplierName = data.Company != null ? data.Company.NameEnglish : null
            };

        private void ProcessComments(
            Roadshow data,
            RoadshowModel model,
            string userId,
            MMADbContext context
        )
        {
            data.RoadshowComments = model.Comments
                .Select(x =>
                {
                    if (x.Id == 0)
                    {
                        return new RoadshowComment
                        {
                            CreatedBy = userId,
                            CreatedByName = context.Users
                                .FirstOrDefault(u => userId == u.Id)
                                ?.UserName,
                            CreatedOn = DateTime.UtcNow,
                            Text = x.Text,
                            RoadshowId = model.Id
                        };
                    }
                    else
                    {
                        return new RoadshowComment
                        {
                            CreatedBy = x.CreatedBy,
                            CreatedOn = (DateTime)x.CreatedOn,
                            CreatedByName = x.CreatedByName,
                            Id = x.Id,
                            Text = x.Text
                        };
                    }
                })
                .ToList();
        }

        private void PopulateEntityModel(Roadshow data, RoadshowModel model)
        {
            data.Status = model.Status;
            data.Documents = model.Documents
                .Select(
                    od =>
                        new RoadshowDocument
                        {
                            DocumentId = od.DocumentId,
                            RoadshowId = model.Id,
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
                            cropY2 = od.cropY2,
                            Cover = od.Cover
                        }
                )
                .ToList();
            data.Id = model.Id;
            data.Title = model.Title;
            data.Description = model.Description;
            data.Activities = model.Activities;
            data.RoadshowVouchers = model.RoadshowVouchers
                .Select(
                    vouchers =>
                        new RoadshowVoucher
                        {
                            RoadshowId = data.Id,
                            Quantity = vouchers.Quantity,
                            Validity = vouchers.Validity,
                            Details = vouchers.Details
                        }
                )
                .ToList();
            data.DateFrom = model.DateFrom;
            data.DateTo = model.DateTo;
            data.InstructionBox = model.InstructionBox;
            data.Locations =
                data.Status != RoadshowStatus.Draft
                    ? model.Locations
                        ?.Select(
                            location =>
                                new RoadshowLocation
                                {
                                    DefaultLocationId = location.Id,
                                    RoadshowId = data.Id
                                }
                        )
                        .ToList()
                    : new List<RoadshowLocation>();
            data.FocalPointName = model.FocalPointName;
            data.FocalPointSurname = model.FocalPointSurname;
            data.FocalPointEmail = model.FocalPointEmail;
            if (model.PhoneNumber != null)
            {
                data.Number = model.PhoneNumber.Number;
                data.E164Number = model.PhoneNumber.E164Number;
                data.InternationalNumber = model.PhoneNumber.InternationalNumber;
                data.CountryCode = model.PhoneNumber.CountryCode;
            }

            if (model.EmiratesId != null)
                data.EmiratesId = Guid.Parse(model.EmiratesId.Id);
        }

        private readonly Expression<Func<Roadshow, RoadshowModel>> projectToRoadshowCardModel =
            data =>
                new RoadshowModel()
                {
                    Id = data.Id,
                    Status = data.Status,
                    Description = data.Description,
                    Title = data.Title,
                    DateFrom = data.DateFrom,
                    DateTo = data.DateTo,
                    CreatedOn = data.CreatedOn,
                    Locations = data.Locations
                        .Select(x => new DefaultLocationModel { Title = x.DefaultLocation.Title })
                        .ToList()
                };

        public int MapOfferCountForRoadshow(int roadshowID)
        {
            var context = ContextFactory();

            return (
                from r in context.Roadshow
                join ri in context.RoadshowInvite on r.Id equals ri.RoadshowId
                join re in context.RoadshowEvent on ri.Id equals re.RoadshowInviteId
                join reo in context.RoadshowEventOffer on re.Id equals reo.RoadshowEventId
                join ro in context.RoadshowOffer on reo.RoadshowOfferId equals ro.Id
                where r.Id == roadshowID
                select ro
            ).Count();
        }

        public async Task<List<RoadshowEmailModel>> DoBackgroundJobAsync(ILogger logger)
        {
            var currentDate = DateTime.UtcNow.Date;
            var context = ContextFactory();

            var allExpiredRoadshows = context.Roadshow
                .Where(
                    r =>
                        r.Status == RoadshowStatus.Published
                        && r.DateTo.HasValue
                        && r.DateTo.Value.Date.AddDays(1) <= currentDate
                )
                .Select(r => r);

            var roadshowsThatStartToday = context.Roadshow
                .Where(
                    r =>
                        r.Status == RoadshowStatus.Confirmed
                        && r.DateTo.HasValue
                        && r.DateFrom.Value.Date == currentDate
                )
                .Select(r => r);

            logger.LogInformation("allExpiredRoadshows count -> " + allExpiredRoadshows.Count());
            logger.LogInformation(
                "roadshowsThatStartToday count -> " + roadshowsThatStartToday.Count()
            );

            var usersToNotify = new List<RoadshowEmailModel>();

            foreach (var r in allExpiredRoadshows)
            {
                r.Status = RoadshowStatus.Expired;
                r.UpdatedOn = DateTime.UtcNow;
                var coordinators = await _userManager.GetUsersInRoleAsync("ADNOC Coordinator");
                var s = coordinators
                    .Select(
                        c =>
                            new RoadshowEmailModel
                            {
                                UserId = c.Id,
                                EmailStatus =
                                    MessageTemplateList.Roadshow_Expired_Notify_SupplierAdminOrSupplier,
                                RoadshowId = r.Id,
                                RoadshowTitle = r.Title
                            }
                    )
                    .ToList();
                usersToNotify.AddRange(s);
            }

            foreach (var r in roadshowsThatStartToday)
            {
                r.Status = RoadshowStatus.Published;
            }

            await context.SaveChangesAsync();

            return usersToNotify;
        }

        public async Task<Tuple<string, List<string>>> UnpublishRoadshow(int roadshowId)
        {
            var context = ContextFactory();

            var roadshow = context.Roadshow.FirstOrDefault(r => r.Id == roadshowId);

            roadshow.Status = RoadshowStatus.Submitted;
            context.Roadshow.Update(roadshow);

            (from ri in context.RoadshowInvite where ri.RoadshowId == roadshowId select ri)
                .ToList()
                .ForEach(ri => ri.Status = RoadshowInviteStatus.Invited);

            await context.SaveChangesAsync();

            var userToNotify = (
                from ri in context.RoadshowInvite
                join cs in context.CompanySuppliers on ri.CompanyId equals cs.CompanyId
                where ri.RoadshowId == roadshowId
                select cs.SupplierId
            ).ToList();

            return Tuple.Create(roadshow.Title, userToNotify);
        }

        public async Task<int> GetRoadshowCount(
            string userId,
            List<Shared.Enums.Declares.Roles> roles
        )
        {
            var context = ContextFactory();

            IQueryable<Roadshow> roadshows = context.Roadshow.AsNoTracking();

            if (roles.Contains(Roles.AdnocCoordinator) || roles.Contains(Roles.Admin))
            {
                roadshows = (from r in roadshows where r.Status != RoadshowStatus.Draft select r);
            }
            else if (roles.Contains(Roles.SupplierAdmin) || roles.Contains(Roles.Supplier))
            {
                roadshows = (
                    from r in roadshows
                    join cs in context.CompanySuppliers on r.CompanyId equals cs.CompanyId
                    where cs.SupplierId == userId
                    select r
                );
            }
            else if (roles.Contains(Roles.RoadshowFocalPoint))
            {
                roadshows = (
                    from r in roadshows
                    where r.Status == RoadshowStatus.Confirmed
                    select r
                );
            }

            return roadshows.Count();
        }

        public IQueryable<RoadshowModel> GetConfirmedRoadshows()
        {
            var context = ContextFactory();

            return context.Roadshow
                .Where(x => x.Status == RoadshowStatus.Confirmed)
                .Select(projectToRoadshowModel);
        }

        public async Task<RoadshowOfferModel> GetSpecificRoadshowOfferById(int id)
        {
            var context = ContextFactory();
            return await context.Roadshow
                .AsNoTracking()
                .Select(projectToRoadshowOfferModel)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        private Expression<Func<Roadshow, RoadshowOfferModel>> projectToRoadshowOfferModel = data =>
            new RoadshowOfferModel
            {
                Id = data.Id,
                CompanyId = data.Company != null ? data.Company.Id : 0,
                CompanyAddress =
                    data.Company != null
                        ? data.Company.CompanyLocations.Select(c => c.Address).FirstOrDefault()
                        : String.Empty,
                CompanyWebsite = data.Company != null ? data.Company.Website : String.Empty,
                CompanyNameArabic = data.Company != null ? data.Company.NameArabic : String.Empty,
                CompanyNameEnglish = data.Company != null ? data.Company.NameEnglish : String.Empty,
                CompanyLogo =
                    data.Company != null ? data.Company.Logo.DocumentId.ToString() : String.Empty,
                CompanyPOBox = data.Company != null ? data.Company.POBox : String.Empty,
                CompanyPhoneNumber = data.Company != null ? data.Company.Mobile : String.Empty,
                Title = data.Title,
                Description = data.Description,
                RoadshowDetails = data.Activities,
                Status = data.Status,
                CreatedBy = data.CreatedBy,
                CreatedOn = data.CreatedOn,
                UpdatedBy = data.UpdatedBy,
                UpdatedOn = data.UpdatedOn,
                DateTo = (DateTime)data.DateTo,
                MainImage = data.Documents
                    .Where(od => od.Cover)
                    .Select(od => od.DocumentId.ToString())
                    .FirstOrDefault(),
                Images = data.Documents
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
                ImageUrls = data.Documents
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
                Locations = data.Locations
                    .Select(
                        x =>
                            new RoadshowOfferLocationModel()
                            {
                                Id = x.DefaultLocation.Id,
                                Title = x.DefaultLocation.Title,
                                Latitude = x.DefaultLocation.Latitude,
                                Longitude = x.DefaultLocation.Longitude,
                                Vicinity = x.DefaultLocation.Vicinity,
                                Address = x.DefaultLocation.Address,
                                Country = x.DefaultLocation.Country
                            }
                    )
                    .ToList()
            };
    }
}
