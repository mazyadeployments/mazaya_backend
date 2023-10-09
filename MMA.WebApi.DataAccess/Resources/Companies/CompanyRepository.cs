using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Constants;
using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Extensions;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.Companies;
using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Models.ApplicationUser;
using MMA.WebApi.Shared.Models.ApplicationUserDocument;
using MMA.WebApi.Shared.Models.Categories;
using MMA.WebApi.Shared.Models.Category;
using MMA.WebApi.Shared.Models.Companies;
using MMA.WebApi.Shared.Models.Image;
using MMA.WebApi.Shared.Models.SupplierToApprove;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using static MMA.WebApi.Shared.Enums.Declares;
using static MMA.WebApi.Shared.Enums.Document.DocumentDeclares;

namespace MMA.WebApi.DataAccess.Companies
{
    public class CompanyRepository : BaseRepository<CompanyModel>, ICompanyRepository
    {
        private readonly UserManager<ApplicationUser> userManager;

        public CompanyRepository(
            Func<MMADbContext> contextFactory,
            UserManager<ApplicationUser> userManager
        )
            : base(contextFactory)
        {
            this.userManager = userManager;
        }

        public async Task DeleteFocalPointAsync(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            var context = ContextFactory();

            var userImage = context.ApplicationUserDocument.Where(x => x.ApplicationUserId == id);
            context.ApplicationUserDocument.RemoveRange(userImage);

            var notifications = context.UserNotification.Where(ui => ui.UserId == id);
            context.RemoveRange(notifications);

            var mailStorage = context.MailStorage.Where(ui => ui.UserId == id);
            context.RemoveRange(mailStorage);

            var companySupplier = context.CompanySuppliers
                .Where(x => x.SupplierId == id)
                .FirstOrDefault();
            context.CompanySuppliers.Remove(companySupplier);
            context.SaveChanges();

            if (user != null)
            {
                await userManager.DeleteAsync(user);
            }
            ;
        }

        public IQueryable<CompanyCardModel> GetAllCompaniesPage(QueryModel queryModel)
        {
            var context = ContextFactory();

            var company = context.Company
                .AsNoTracking()
                .Where(x => x.ApproveStatus == SupplierStatus.Approved.ToString());

            var filteredCompany = FilterCompany(company, queryModel);
            var companyCardModels = filteredCompany.Select(projectToCompanyCardModel);

            return Sort(queryModel.Sort, companyCardModels);
        }

        public IQueryable<CompanyCardModel> GetAllRejectedAndDeactivatedCompaniesPage(
            QueryModel queryModel
        )
        {
            var context = ContextFactory();

            var company = context.Company
                .AsNoTracking()
                .Where(
                    x =>
                        x.ApproveStatus == SupplierStatus.Rejected.ToString()
                        || x.ApproveStatus == SupplierStatus.Deactivated.ToString()
                );

            var filteredCompany = FilterCompany(company, queryModel);
            var companyCardModels = filteredCompany.Select(projectToCompanyCardModel);

            return Sort(queryModel.Sort, companyCardModels);
        }

        private static IQueryable<CompanyCardModel> Sort(
            SortModel sortModel,
            IQueryable<CompanyCardModel> companies
        )
        {
            if (sortModel.Type == "date")
            {
                if (sortModel.Order == Order.DESC)
                {
                    return companies.OrderByDescending(x => x.UpdatedOn);
                }
                else
                {
                    return companies.OrderBy(x => x.UpdatedOn);
                }
            }
            else
            {
                return companies.OrderByDescending(x => x.UpdatedOn);
            }
        }

        public async Task<ApplicationUserModel> FindFocalPointByIdAsync(string id)
        {
            var context = ContextFactory();

            ApplicationUser applicationUser = await userManager.FindByIdAsync(id);
            ApplicationUserModel applicationUserModel = new ApplicationUserModel();
            applicationUser.ApplicationUserDocuments = new List<ApplicationUserDocument>();
            applicationUserModel.Image = new ImageModel();
            var role = (
                from r in context.Roles
                join ur in context.UserRoles on r.Id equals ur.RoleId
                where ur.UserId == applicationUser.Id
                select r.Name
            ).FirstOrDefault();

            applicationUserModel.AccessFailedCount = applicationUser.AccessFailedCount;
            applicationUserModel.Active = applicationUser.Active;
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
            applicationUserModel.Role = role;
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

            return applicationUserModel;
        }

        public async Task<ApplicationUserModel> CreateOrUpdateFocalPoint(
            ApplicationUserModel model,
            IVisitor<IChangeable> auditVisitor,
            string supplierAdminId
        )
        {
            StringBuilder sb = new StringBuilder();
            var resultCreate = new IdentityResult();
            var resultRole = new IdentityResult();
            var context = ContextFactory();

            //Focal point cannot be deactivated
            model.Active = true;

            using (
                var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled)
            )
            {
                var applicationUser = await userManager.FindByIdAsync(model.Id);

                if (applicationUser == null)
                    applicationUser = new ApplicationUser();

                var applicationUserDocuments = new List<ApplicationUserDocumentModel>();

                var userIMGs = context.ApplicationUserDocument.Where(
                    x => x.ApplicationUserId == model.Id
                );
                context.ApplicationUserDocument.RemoveRange(userIMGs);
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

                model.ApplicationUserDocuments = applicationUserDocuments;

                PopulateEntityModel(applicationUser, model);

                foreach (var applicationUserDocument in applicationUser.ApplicationUserDocuments)
                {
                    applicationUserDocument.Accept(auditVisitor);
                    if (
                        !await context.Document.AnyAsync(
                            d => d.Id == applicationUserDocument.DocumentId
                        )
                    )
                    {
                        context.Document.Add(
                            new Document()
                            {
                                Id = applicationUserDocument.DocumentId,
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
                    try
                    {
                        if (string.IsNullOrEmpty(applicationUser.Email))
                            applicationUser.UserName = applicationUser.PhoneNumber;
                        resultCreate = await userManager.CreateAsync(applicationUser);
                        resultRole = await userManager.AddToRoleAsync(applicationUser, model.Role);

                        var supplierAdminsCompanyId = 0;

                        if (model.CompanyId != 0)
                        {
                            supplierAdminsCompanyId = model.CompanyId;
                        }
                        else
                        {
                            var supplierAdminCompnay = context.CompanySuppliers
                                .AsNoTracking()
                                .Where(cs => cs.SupplierId == supplierAdminId)
                                .FirstOrDefault();
                            if (supplierAdminCompnay != null)
                                supplierAdminsCompanyId = supplierAdminCompnay.CompanyId;
                        }

                        context.CompanySuppliers.Add(
                            new CompanySuppliers
                            {
                                SupplierId = applicationUser.Id,
                                CompanyId = supplierAdminsCompanyId
                            }
                        );

                        context.SaveChanges();
                    }
                    catch (Exception ex)
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
                else
                {
                    try
                    {
                        await userManager.UpdateAsync(applicationUser);
                        await UpdateAspNetUser(
                            applicationUser,
                            new string[] { "Email", "PhoneNumber" }
                        );
                        applicationUser.UpdatedOn = DateTime.UtcNow;
                        var roles = await userManager.GetRolesAsync(applicationUser);
                        var role = await context.Roles.FirstOrDefaultAsync(
                            r => r.Name == roles.FirstOrDefault()
                        );
                        var trimmedRoleName = model.Role.Replace(" ", "");

                        if (roles.Count == 0)
                            await UpdateRoleSupplier(context, applicationUser, trimmedRoleName);

                        var contains = roles.Contains(trimmedRoleName);

                        if (roles.Count != 0 && !contains)
                        {
                            DeleteSupplierRole(context, applicationUser, role);
                            await UpdateRoleSupplier(context, applicationUser, trimmedRoleName);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
                transactionScope.Complete();
            }

            return model;
        }

        private async Task UpdateRoleSupplier(
            MMADbContext context,
            ApplicationUser applicationUser,
            string trimRoleName
        )
        {
            var roleForAdd = context.Roles.FirstOrDefault(x => x.Name == trimRoleName);
            context.UserRoles.Add(
                new IdentityUserRole<string> { RoleId = roleForAdd.Id, UserId = applicationUser.Id }
            );
            await context.SaveChangesAsync();
        }

        private void DeleteSupplierRole(
            MMADbContext context,
            ApplicationUser applicationUser,
            ApplicationRole role
        )
        {
            context.UserRoles.Remove(
                new IdentityUserRole<string>() { UserId = applicationUser.Id, RoleId = role.Id }
            );
        }

        public async Task UpdateAspNetUser(ApplicationUser user, string[] props)
        {
            var context = ContextFactory();
            var store = new UserStore<ApplicationUser>(context);
            Task<ApplicationUser> cUser = store.FindByIdAsync(user.Id);
            var oldUser = await cUser;

            foreach (var prop in props)
            {
                var pi = typeof(ApplicationUser).GetProperty(prop);
                var val = pi.GetValue(user);
                pi.SetValue(oldUser, val);
            }

            await store.UpdateAsync(oldUser);
            await context.SaveChangesAsync();
        }

        private void PopulateEntityModel(ApplicationUser data, ApplicationUserModel model)
        {
            data.Title = model.Title;
            data.AccessFailedCount = model.AccessFailedCount;
            data.Active = model.Active;
            data.ConcurrencyStamp = model.ConcurrencyStamp;
            data.CreatedBy = model.CreatedBy;
            data.CreatedOn = model.CreatedOn;
            data.Email = model.Email;
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
            data.UpdatedOn = model.UpdatedOn;
            data.UserName = model.Email;
            data.NormalizedUserName = model.Email;
            data.UserType = (int)Declares.UserType.Other;
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
        }

        public IQueryable<CompanyCardModel> GetAllSuppliersPage(QueryModel queryModel)
        {
            var context = ContextFactory();
            var suppliers = context.Company.Select(projectToCompanyCardModel);

            return SortCompanyModel(queryModel.Sort, suppliers);
        }

        public async Task<IQueryable<ApplicationUserModel>> GetMyFocalPointsPage(
            QueryModel queryModel,
            string supplierAdminId
        )
        {
            var context = ContextFactory();
            IQueryable<ApplicationUser> filteredFocalPoints = null;
            IQueryable<ApplicationUserModel> focalPointModels = null;
            var supplierAdminsCompanyId = context.CompanySuppliers
                .AsNoTracking()
                .Where(cs => cs.SupplierId == supplierAdminId)
                .FirstOrDefault()
                .CompanyId;
            //var focalPointIds = context.CompanySuppliers.AsNoTracking().Where(cs => cs.CompanyId == supplierAdminsCompanyId).Select(x => x.SupplierId).ToList();
            var focalPointIds = (
                from cs in context.CompanySuppliers
                join ur in context.UserRoles on cs.SupplierId equals ur.UserId
                join r in context.Roles on ur.RoleId equals r.Id
                where cs.CompanyId == supplierAdminsCompanyId // && (r.Name == Roles.Supplier.ToString())
                select cs.SupplierId
            ).ToList();

            var focalPoints = context.Users.Where(x => focalPointIds.Contains(x.Id));

            filteredFocalPoints = FilterFocalPoint(focalPoints, queryModel);
            focalPointModels = filteredFocalPoints.Select(projectToApplicationUserModel);

            return SortUserModel(queryModel.Sort, focalPointModels);
        }

        public async Task<IQueryable<ApplicationUserModel>> GetAllAdminSupliersAndSuppliersPage(
            QueryModel queryModel,
            int companyId
        )
        {
            var context = ContextFactory();
            IQueryable<ApplicationUser> filteredFocalPoints = null;
            IQueryable<ApplicationUserModel> focalPointModels = null;

            var focalPointIds = (
                from cs in context.CompanySuppliers
                join ur in context.UserRoles on cs.SupplierId equals ur.UserId
                join r in context.Roles on ur.RoleId equals r.Id
                where
                    cs.CompanyId == companyId
                    && (
                        r.Name == Roles.Supplier.ToString()
                        || r.Name == Roles.SupplierAdmin.ToString()
                    )
                select cs.SupplierId
            ).ToList();

            var focalPoints = context.Users.Where(x => focalPointIds.Contains(x.Id));

            filteredFocalPoints = FilterFocalPoint(focalPoints, queryModel);
            focalPointModels = filteredFocalPoints.Select(projectToApplicationUserModel);

            return SortUserModel(queryModel.Sort, focalPointModels);
        }

        private static IQueryable<ApplicationUser> FilterFocalPoint(
            IQueryable<ApplicationUser> focalPoints,
            QueryModel queryModel
        )
        {
            var filteredFocalPoints = focalPoints;
            //.Where(focalPoint => focalPoint.FirstName.ToLower().Contains(queryModel.Filter.Keyword.Trim().ToLower()) ||
            //       focalPoint.LastName.Contains(queryModel.Filter.Keyword.Trim().ToLower()) ||
            //       focalPoint.Email.Contains(queryModel.Filter.Keyword.Trim().ToLower()));

            return filteredFocalPoints;
        }

        private static IQueryable<Company> FilterCompany(
            IQueryable<Company> companies,
            QueryModel queryModel
        )
        {
            var filteredCompany = companies.Where(
                company =>
                    company.NameEnglish
                        .ToLower()
                        .Contains(queryModel.Filter.Keyword.Trim().ToLower())
                    || company.Mobile.Contains(queryModel.Filter.Keyword.Trim().ToLower())
                    || company.OfficialEmail.Contains(queryModel.Filter.Keyword.Trim().ToLower())
            );

            return filteredCompany;
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
            };

        private static IQueryable<ApplicationUserModel> SortUserModel(
            SortModel sortModel,
            IQueryable<ApplicationUserModel> suppliers
        )
        {
            if (sortModel.Type == "date")
            {
                if (sortModel.Order == Order.DESC)
                {
                    return suppliers.OrderByDescending(x => x.UpdatedOn);
                }
                else
                {
                    return suppliers.OrderBy(x => x.UpdatedOn);
                }
            }
            else
            {
                return suppliers.OrderByDescending(x => x.UpdatedOn);
            }
        }

        private static IQueryable<CompanyCardModel> SortCompanyModel(
            SortModel sortModel,
            IQueryable<CompanyCardModel> suppliers
        )
        {
            if (sortModel.Type == "date")
            {
                if (sortModel.Order == Order.DESC)
                {
                    return suppliers.OrderByDescending(x => x.UpdatedOn);
                }
                else
                {
                    return suppliers.OrderBy(x => x.UpdatedOn);
                }
            }
            else
            {
                return suppliers.OrderByDescending(x => x.UpdatedOn);
            }
        }

        public async Task<CompanyModel> CreateOrUpdateAsync(
            CompanyModel companyModel,
            IVisitor<IChangeable> auditVisitor,
            string userId
        )
        {
            var context = ContextFactory();
            var applicationUser = new ApplicationUser();
            applicationUser.Active = true;

            if (companyModel.Id == 0)
            {
                using (
                    var transactionScope = new TransactionScope(
                        TransactionScopeAsyncFlowOption.Enabled
                    )
                )
                {
                    var supplierAdminModel = companyModel.SupplierAdmins.FirstOrDefault();
                    supplierAdminModel.Active = true;
                    var applicationUserDocuments = new List<ApplicationUserDocumentModel>();

                    if (
                        supplierAdminModel.ImageSets != null
                        && supplierAdminModel.ImageSets.Count > 0
                    )
                    {
                        foreach (var imageModel in supplierAdminModel.ImageSets)
                        {
                            applicationUserDocuments.Add(
                                new ApplicationUserDocumentModel
                                {
                                    DocumentId = new Guid(imageModel.Id),
                                    ApplicationUserId = supplierAdminModel.Id,
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

                    supplierAdminModel.ApplicationUserDocuments = applicationUserDocuments;

                    PopulateEntityModel(applicationUser, supplierAdminModel);

                    foreach (
                        var applicationUserDocument in applicationUser.ApplicationUserDocuments
                    )
                    {
                        applicationUserDocument.Accept(auditVisitor);
                    }

                    applicationUser.Accept(auditVisitor);

                    //Insert user from the form, and give him SupplierAdmin role
                    StringBuilder sb = new StringBuilder();

                    var resultCreate = await userManager.CreateAsync(applicationUser);

                    if (resultCreate.Succeeded)
                    {
                        var resultRole = await userManager.AddToRoleAsync(
                            applicationUser,
                            "Supplier Admin"
                        );

                        if (resultRole.Succeeded)
                        {
                            transactionScope.Complete();
                        }
                        else
                        {
                            foreach (var error in resultCreate.Errors)
                            {
                                sb.Append(error.Description);
                            }
                            throw new Exception(sb.ToString());
                        }
                    }
                    else
                    {
                        foreach (var error in resultCreate.Errors)
                        {
                            sb.Append(error.Description);
                        }
                        throw new Exception(sb.ToString());
                    }
                }
            }

            //Insert Company
            var company = context.Company
                .Include(o => o.Logo)
                .Include(o => o.CompanyActivities)
                .Include(o => o.CompanyLocations)
                .Include(o => o.CompanyPartners)
                .Include(o => o.CompanySuppliers)
                .Include(o => o.CompanyCategories)
                .ThenInclude(cc => cc.Category)
                .FirstOrDefault(x => x.Id == companyModel.Id);

            if (company == null)
                company = new Company();

            var companyCategoriesModels = new List<CategoryModel>();

            if (companyModel.Categories != null && companyModel.Categories.Count() > 0)
            {
                foreach (var category in companyModel.Categories)
                {
                    if (category != null)
                    {
                        companyCategoriesModels.Add(
                            new CategoryModel { Id = category.Id, Title = category.Title }
                        );
                    }
                }
            }

            companyModel.Categories = companyCategoriesModels;

            var companyPartnerModels = new List<CompanyPartnerModel>();

            if (
                companyModel.CompanyPartnersList != null
                && companyModel.CompanyPartnersList.Count() > 0
            )
            {
                foreach (var partner in companyModel.CompanyPartnersList)
                {
                    if (!String.IsNullOrWhiteSpace(partner))
                    {
                        companyPartnerModels.Add(
                            new CompanyPartnerModel
                            {
                                Id = 0,
                                Name = partner,
                                CompanyId = companyModel.Id
                            }
                        );
                    }
                }
            }

            companyModel.CompanyPartners = companyPartnerModels;

            var companyActivityModels = new List<CompanyActivityModel>();

            if (
                companyModel.CompanyActivitiesList != null
                && companyModel.CompanyActivitiesList.Count() > 0
            )
            {
                foreach (var activity in companyModel.CompanyActivitiesList)
                {
                    if (!String.IsNullOrWhiteSpace(activity))
                    {
                        companyActivityModels.Add(
                            new CompanyActivityModel
                            {
                                Id = 0,
                                Name = activity,
                                CompanyId = companyModel.Id
                            }
                        );
                    }
                }
            }

            companyModel.CompanyActivities = companyActivityModels;

            if (companyModel.CompanyLocations == null)
            {
                companyModel.CompanyLocations = new List<CompanyLocationModel>();
            }

            if (companyModel.Suppliers == null)
            {
                companyModel.Suppliers = new List<ApplicationUserModel>();
            }
            if (companyModel.CompanyPartners == null)
            {
                companyModel.CompanyPartners = new List<CompanyPartnerModel>();
            }
            if (companyModel.CompanyActivities == null)
            {
                companyModel.CompanyActivities = new List<CompanyActivityModel>();
            }

            if (companyModel.Id == 0)
            {
                companyModel.Suppliers = companyModel.Suppliers
                    .Union(companyModel.SupplierAdmins)
                    .ToList();
            }

            PopulateEntityModel(company, companyModel, applicationUser);

            company.ApproveStatus = "Approved";
            company.ApprovedBy = userId;

            if (companyModel.Id == 0)
            {
                //company.Logo.Accept(auditVisitor);
                foreach (var companyLocation in company.CompanyLocations)
                {
                    companyLocation.Accept(auditVisitor);
                }
                company.Accept(auditVisitor);
                context.Add(company);
            }
            else
            {
                company.UpdatedBy = userId;
                company.UpdatedOn = DateTime.UtcNow;
                if (company.Logo != null)
                {
                    company.Logo.UpdatedBy = company.UpdatedBy;
                    company.Logo.UpdatedOn = company.UpdatedOn;
                }
                company.UpdatedOn = DateTime.UtcNow;
                foreach (var companyLocation in company.CompanyLocations)
                {
                    companyLocation.CreatedBy = company.CreatedBy;
                    companyLocation.CreatedOn = company.CreatedOn;
                    companyLocation.UpdatedBy = company.UpdatedBy;
                    companyLocation.UpdatedOn = company.UpdatedOn;
                }
                context.Update(company);
            }

            await context.SaveChangesAsync();

            company.CompanyCategories
                .ToList()
                .ForEach(c =>
                {
                    c.Category = context.Category.FirstOrDefault(x => x.Id == c.CategoryId);
                });
            company.TradeLicence = context.Document.FirstOrDefault(
                x => x.Id == company.TradeLicenceId
            );

            return projectToCompanyModel.Compile().Invoke(company);
        }

        public async Task<CompanyModel> RegisterCompanyAsync(
            CompanyModel companyModel,
            IVisitor<IChangeable> auditVisitor,
            string userId
        )
        {
            var context = ContextFactory();
            var company = new Company();

            PopulateCompanyRegistrationEntityModel(company, companyModel);
            company.Accept(auditVisitor);
            context.Add(company);

            await context.SaveChangesAsync();

            return projectToCompanyModelRegister.Compile().Invoke(company);
        }

        private Expression<Func<Company, CompanyModel>> projectToCompanyModel = data =>
            new CompanyModel()
            {
                Id = data.Id,
                CreatedBy = data.CreatedBy,
                CreatedOn = data.CreatedOn,
                UpdatedBy = data.UpdatedBy,
                UpdatedOn = data.UpdatedOn,
                ApproveStatus = data.ApproveStatus,
                NameEnglish = data.NameEnglish,
                CompanyDescription = data.CompanyDescription,
                Mobile = string.IsNullOrWhiteSpace(data.MobileE164Number)
                    ? null
                    : new PhoneNumberModel
                    {
                        CountryCode = data.MobileCountryCode,
                        E164Number = data.MobileE164Number,
                        InternationalNumber = data.Mobile,
                        Number = data.MobileNumber,
                    },
                OfficialEmail = data.OfficialEmail,
                CompanyNationality = data.CompanyNationality,
                EstablishDate = data.EstablishDate.SpecifyKind(DateTimeKind.Utc),
                ExpiryDate = data.ExpiryDate.SpecifyKind(DateTimeKind.Utc),
                Fax = string.IsNullOrWhiteSpace(data.FaxE164Number)
                    ? null
                    : new PhoneNumberModel
                    {
                        CountryCode = data.FaxCountryCode,
                        E164Number = data.FaxE164Number,
                        InternationalNumber = data.Fax,
                        Number = data.FaxNumber,
                    },
                IDforADCCI = data.IDforADCCI,
                Instagram = data.Instagram,
                Land = string.IsNullOrWhiteSpace(data.LandE164Number)
                    ? null
                    : new PhoneNumberModel
                    {
                        CountryCode = data.LandCountryCode,
                        E164Number = data.LandE164Number,
                        InternationalNumber = data.Land,
                        Number = data.LandNumber,
                    },
                LegalForm = data.LegalForm,
                LicenseNo = data.LicenseNo,
                Logo = data.Logo != null ? data.Logo.DocumentId.ToString() : string.Empty,
                NameArabic = data.NameArabic,
                POBox = data.POBox,
                Facebook = data.Facebook,
                Twitter = data.Twitter,
                Website = data.Website,
                CompanyActivities = data.CompanyActivities.Select(
                    a =>
                        new CompanyActivityModel
                        {
                            Id = a.Id,
                            Name = a.Name,
                            CompanyId = a.CompanyId
                        }
                ),
                CompanyActivitiesList = data.CompanyActivities.Select(a => a.Name),
                CompanyPartners = data.CompanyPartners.Select(
                    a =>
                        new CompanyPartnerModel
                        {
                            Id = a.Id,
                            Name = a.Name,
                            CompanyId = a.CompanyId
                        }
                ),
                CompanyPartnersList = data.CompanyPartners.Select(a => a.Name),
                CompanyLocations = data.CompanyLocations
                    .Select(
                        ol =>
                            new CompanyLocationModel
                            {
                                Address = ol.Address,
                                Country = ol.Country,
                                Id = ol.Id,
                                Latitude = ol.Latitude,
                                Longitude = ol.Longitude,
                                CompanyId = data.Id,
                                Vicinity = ol.Vicinity
                            }
                    )
                    .ToList(),
                Categories = data.CompanyCategories
                    .Where(c => c.CompanyId == data.Id)
                    .Select(
                        c => new CategoryModel() { Id = c.CategoryId, Title = c.Category.Title }
                    )
                    .ToList(),
                TradeLicenceExpiryDate = data.TradeLicenseExpDate,
                TradeLicence =
                    data.TradeLicence != null
                        ? new Shared.Models.Attachment.AttachmentModel()
                        {
                            Id = data.TradeLicence.Id.ToString(),
                            Name = data.TradeLicence.Name,
                            Type = data.TradeLicence.MimeType
                        }
                        : null
            };

        private Expression<Func<Company, CompanyModel>> projectToCompanyModelRegister = data =>
            new CompanyModel()
            {
                Id = data.Id,
                CreatedBy = data.CreatedBy,
                CreatedOn = data.CreatedOn,
                UpdatedBy = data.UpdatedBy,
                UpdatedOn = data.UpdatedOn,
                ApproveStatus = data.ApproveStatus,
                NameEnglish = data.NameEnglish,
                CompanyDescription = data.CompanyDescription,
                Mobile = new PhoneNumberModel
                {
                    CountryCode = data.MobileCountryCode,
                    E164Number = data.MobileE164Number,
                    InternationalNumber = data.Mobile,
                    Number = data.MobileNumber,
                },
                OfficialEmail = data.OfficialEmail,
            };

        private Expression<Func<Company, CompanyCardModel>> projectToCompanyCardModel = data =>
            new CompanyCardModel()
            {
                Id = data.Id,
                NameEnglish = data.NameEnglish,
                Status = data.ApproveStatus,
                Description = data.CompanyDescription,
                Mobile = data.Mobile,
                OfficialEmail = data.OfficialEmail,
                Logo = data.Logo != null ? data.Logo.DocumentId.ToString() : string.Empty,
                UpdatedBy = data.UpdatedBy,
                UpdatedOn = data.UpdatedOn,
                CreatedBy = data.CreatedBy,
                CreatedOn = data.CreatedOn,
                Categories = data.CompanyCategories
                    .Select(
                        x =>
                            new CategoryLiteModel
                            {
                                Title = x.Category != null ? x.Category.Title : String.Empty,
                                Id = x.CategoryId
                            }
                    )
                    .ToList(),
            };

        private void PopulateEntityModel(
            Company data,
            CompanyModel model,
            ApplicationUser supplierAdmin
        )
        {
            data.Id = model.Id;
            data.CompanyDescription = model.CompanyDescription;
            data.NameEnglish = model.NameEnglish;
            data.ApproveStatus = model.ApproveStatus;
            data.OfficialEmail = model.OfficialEmail;
            data.CompanyNationality = model.CompanyNationality;
            data.EstablishDate = model.EstablishDate;
            data.ExpiryDate = model.ExpiryDate;
            data.FaxCountryCode = model.Fax != null ? model.Fax.CountryCode : string.Empty;
            data.FaxE164Number = model.Fax != null ? model.Fax.E164Number : string.Empty;
            data.Fax = model.Fax != null ? model.Fax.InternationalNumber : string.Empty;
            data.FaxNumber = model.Fax != null ? model.Fax.Number : string.Empty;
            data.IDforADCCI = model.IDforADCCI;
            data.Instagram = model.Instagram;
            data.LandCountryCode = model.Land != null ? model.Land.CountryCode : string.Empty;
            data.LandE164Number = model.Land != null ? model.Land.E164Number : string.Empty;
            data.Land = model.Land != null ? model.Land.InternationalNumber : string.Empty;
            data.LandNumber = model.Land != null ? model.Land.Number : string.Empty;
            data.LegalForm = model.LegalForm;
            data.LicenseNo = model.LicenseNo;
            data.Logo = !String.IsNullOrWhiteSpace(model.Logo)
                ? new CompanyDocument { DocumentId = new Guid(model.Logo) }
                : null;
            data.NameArabic = model.NameArabic;
            data.POBox = model.POBox;
            data.Facebook = model.Facebook;
            data.MobileCountryCode = model.Mobile != null ? model.Mobile.CountryCode : string.Empty;
            data.MobileE164Number = model.Mobile != null ? model.Mobile.E164Number : string.Empty;
            data.Mobile = model.Mobile != null ? model.Mobile.InternationalNumber : string.Empty;
            data.MobileNumber = model.Mobile != null ? model.Mobile.Number : string.Empty;
            data.Twitter = model.Twitter;
            data.Website = model.Website;
            data.TradeLicenseExpDate = model.TradeLicenceExpiryDate;
            data.TradeLicenceId = new Guid(model.TradeLicence.Id);
            data.CompanyPartners = model.CompanyPartners
                .Select(
                    a =>
                        new CompanyPartner
                        {
                            Id = a.Id,
                            Name = a.Name,
                            CompanyId = a.CompanyId
                        }
                )
                .ToList();
            data.CompanyActivities = model.CompanyActivities
                .Select(
                    a =>
                        new CompanyActivity
                        {
                            Id = a.Id,
                            Name = a.Name,
                            CompanyId = a.CompanyId
                        }
                )
                .ToList();
            data.CompanyCategories = model.Categories
                .Select(a => new CompanyCategory { CompanyId = data.Id, CategoryId = a.Id })
                .ToList();
            data.CompanyLocations = model.CompanyLocations
                .Select(
                    oc =>
                        new CompanyLocation
                        {
                            Address = oc.Address,
                            Country = oc.Country,
                            Latitude = oc.Latitude,
                            Longitude = oc.Longitude,
                            CompanyId = model.Id,
                            Vicinity = oc.Vicinity
                        }
                )
                .ToList();
            if (model.Id == 0)
            {
                data.CompanySuppliers = model.Suppliers
                    .Select(
                        applicationUser =>
                            new CompanySuppliers
                            {
                                CompanyId = model.Id,
                                SupplierId = supplierAdmin.Id
                            }
                    )
                    .ToList();
            }
        }

        private void PopulateCompanyRegistrationEntityModel(Company data, CompanyModel model)
        {
            data.Id = model.Id;
            data.CompanyDescription = model.CompanyDescription;
            data.NameEnglish = model.NameEnglish;
            data.ApproveStatus = model.ApproveStatus;
            data.OfficialEmail = model.OfficialEmail;
            data.ExpiryDate = model.ExpiryDate.SpecifyKind(DateTimeKind.Utc);
            data.EstablishDate = model.EstablishDate.SpecifyKind(DateTimeKind.Utc);
            data.TradeLicenseExpDate = model.TradeLicenceExpiryDate;
            if (model.TradeLicence != null)
                data.TradeLicenceId = new Guid(model.TradeLicence.Id);
            else
                data.TradeLicenceId = null;
        }

        public async Task<CompanyModel> GetCompanyById(int id)
        {
            var context = ContextFactory();

            return await context.Company
                .Select(projectToCompanyModel)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<CompanyModel> GetMyCompany(string userId)
        {
            var context = ContextFactory();

            return await context.Company
                .Include(c => c.CompanyCategories)
                .ThenInclude(cc => cc.Category)
                .Where(x => x.CompanySuppliers.Any(cs => cs.SupplierId == userId))
                .Include(x => x.TradeLicence)
                .Select(projectToCompanyModel)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> ProcessCompany(
            CompanyToApproveModel companyToApprove,
            string userId
        )
        {
            var context = ContextFactory();
            var company = await context.Company.FirstOrDefaultAsync(
                x => x.Id == companyToApprove.CompanyId
            );
            var applicationUser = await userManager.FindByIdAsync(companyToApprove.SupplierId);

            if (company == null)
                return false;

            if (companyToApprove.Approved)
            {
                company.ApproveStatus = SupplierStatus.Approved.ToString();
                company.ApprovedBy = userId;
                company.ApprovedOn = DateTime.UtcNow;

                if (applicationUser != null)
                {
                    await userManager.AddToRolesAsync(
                        applicationUser,
                        new List<string> { "Supplier Admin" }
                    );
                }
                else
                {
                    applicationUser = new ApplicationUser()
                    {
                        Id = companyToApprove.SupplierId,
                        UserName = company.OfficialEmail ?? company.Mobile,
                        CreatedBy = userId,
                        UpdatedBy = userId,
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow,
                        UserType = (int)Declares.UserType.Other,
                    };

                    var result = await userManager.CreateAsync(applicationUser);

                    if (result.Succeeded)
                        await userManager.AddToRoleAsync(applicationUser, "Supplier Admin");
                }

                var companySuppliers = await context.CompanySuppliers.FirstOrDefaultAsync(
                    cs =>
                        cs.CompanyId == companyToApprove.CompanyId
                        && cs.SupplierId == companyToApprove.SupplierId
                );

                if (companySuppliers == null)
                {
                    companySuppliers = new CompanySuppliers
                    {
                        CompanyId = companyToApprove.CompanyId,
                        SupplierId = companyToApprove.SupplierId
                    };

                    context.CompanySuppliers.Add(companySuppliers);
                }

                await userManager.UpdateAsync(applicationUser);
                await context.SaveChangesAsync();

                return true;
            }
            else
            {
                company.ApproveStatus = SupplierStatus.Rejected.ToString();
                company.ApprovedBy = userId;
                company.ApprovedOn = DateTime.UtcNow;

                if (applicationUser != null)
                {
                    applicationUser.Active = false;
                    await userManager.UpdateAsync(applicationUser);
                }
                await context.SaveChangesAsync();

                return false;
            }
        }

        public async Task<IEnumerable<CompanyCardModel>> GetAllPendingCompaniesAsync(
            QueryModel queryModel
        )
        {
            var context = ContextFactory();
            var pendingCompanies = context.Company.Where(
                x => x.ApproveStatus == SupplierStatus.PendingApproval.ToString()
            );
            var filteredPendingCompanies = FilterCompany(pendingCompanies, queryModel);

            var pendingCompanyModels = filteredPendingCompanies.Select(projectToCompanyCardModel);
            return SortCompanyModel(queryModel.Sort, pendingCompanyModels);
        }

        public IQueryable<CompanyCardModel> GetAllMissingLicenseCompanies(QueryModel queryModel)
        {
            var context = ContextFactory();
            var missingLicenseCompanies = context.Company.Where(
                x => x.ApproveStatus == SupplierStatus.MissingTradeLicense.ToString()
            );

            var filteredMissingCompanies = FilterCompany(missingLicenseCompanies, queryModel);

            var missingCompanyModels = filteredMissingCompanies.Select(projectToCompanyCardModel);
            return SortCompanyModel(queryModel.Sort, missingCompanyModels);
        }

        protected override IQueryable<CompanyModel> GetEntities()
        {
            var context = ContextFactory();
            return from c in context.Company

                select new CompanyModel
                {
                    Id = c.Id,
                    NameEnglish = c.NameEnglish,
                    NameArabic = c.NameArabic,
                    Land = new PhoneNumberModel
                    {
                        CountryCode = c.LandCountryCode,
                        E164Number = c.LandE164Number,
                        InternationalNumber = c.Land,
                        Number = c.LandNumber,
                    },
                    Mobile = new PhoneNumberModel
                    {
                        CountryCode = c.MobileCountryCode,
                        E164Number = c.MobileE164Number,
                        InternationalNumber = c.Mobile,
                        Number = c.MobileNumber
                    },
                    Fax = new PhoneNumberModel
                    {
                        CountryCode = c.FaxCountryCode,
                        E164Number = c.FaxE164Number,
                        InternationalNumber = c.Fax,
                        Number = c.FaxNumber,
                    },
                    OfficialEmail = c.OfficialEmail,
                    Website = c.Website,
                };
        }

        public IQueryable<CompanyCardModel> GetAllCompaniesCard()
        {
            var context = ContextFactory();
            return from c in context.Company.Where(c => c.ApproveStatus == "Approved")

                select new CompanyCardModel
                {
                    Id = c.Id,
                    NameEnglish = c.NameEnglish,
                    CreatedBy = c.CreatedBy,
                    CreatedOn = c.CreatedOn
                };
        }

        public async Task<CompanyModel> DeleteAsync(int id)
        {
            var context = ContextFactory();
            var company = await context.Company.FirstOrDefaultAsync(x => x.Id == id);

            if (company != null)
            {
                company.ApproveStatus = SupplierStatus.Deactivated.ToString();
                context.Company.Update(company);

                await context.SaveChangesAsync();
            }

            return projectToCompanyModel.Compile().Invoke(company);
        }

        public async Task HardDelete(int id)
        {
            var context = ContextFactory();
            //using var transaction = context.Database.BeginTransaction();

            try
            {
                var company = await context.Company.FirstOrDefaultAsync(x => x.Id == id);

                if (company != null)
                {
                    // Remove Company Suppliers as (Users, Roles, Documents)
                    var companySuppliers = context.CompanySuppliers
                        .Where(x => x.CompanyId == id)
                        .ToList();
                    context.CompanySuppliers.RemoveRange(companySuppliers);
                    foreach (var cs in companySuppliers)
                    {
                        if (!await CheckIfCompanySupplierIsAdmin(cs.SupplierId))
                        {
                            var userDocuments = context.ApplicationUserDocument
                                .Where(ud => ud.ApplicationUserId == cs.SupplierId)
                                .ToList();
                            var userMails = context.MailStorage
                                .Where(m => m.UserId == cs.SupplierId)
                                .ToList();
                            var userNotifications = context.UserNotification
                                .Where(m => m.UserId == cs.SupplierId)
                                .ToList();

                            context.ApplicationUserDocument.RemoveRange(userDocuments);
                            context.MailStorage.RemoveRange(userMails);
                            context.UserNotification.RemoveRange(userNotifications);
                            await context.SaveChangesAsync();

                            var user = await userManager.FindByIdAsync(cs.SupplierId);
                            if (user != null)
                            {
                                await userManager.DeleteAsync(user);
                            }
                        }
                    }

                    // Remove CompanyDocuments, CompanyCategory, CompanyActivity, CompanyPartner
                    var companyActivities = context.CompanyActivity
                        .Where(ca => ca.CompanyId == id)
                        .ToList();
                    var companyCategorires = context.CompanyCategory
                        .Where(cc => cc.CompanyId == id)
                        .ToList();
                    var companyLocations = context.CompanyLocation
                        .Where(ca => ca.CompanyId == id)
                        .ToList();
                    var companyPartners = context.CompanyPartner
                        .Where(ca => ca.CompanyId == id)
                        .ToList();

                    context.CompanyActivity.RemoveRange(companyActivities);
                    context.CompanyCategory.RemoveRange(companyCategorires);
                    context.CompanyLocation.RemoveRange(companyLocations);
                    context.CompanyPartner.RemoveRange(companyPartners);
                    await context.SaveChangesAsync();

                    context.Company.Remove(company);

                    await context.SaveChangesAsync();
                    //transaction.Commit();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private async Task<bool> CheckIfCompanySupplierIsAdmin(string supplierId)
        {
            var context = ContextFactory();

            var roles = new List<string>()
            {
                Declares.Roles.Admin.ToString(),
                Declares.Roles.AdnocCoordinator.ToString()
            };
            var role = await (
                from ur in context.UserRoles
                join r in context.Roles on ur.RoleId equals r.Id
                where ur.UserId == supplierId
                select r.Name
            ).FirstOrDefaultAsync();

            return roles.Contains(role);
        }

        public IQueryable<CompanyModel> Get()
        {
            return GetEntities();
        }

        public async Task<IQueryable<CompanyModel>> GetCompanies()
        {
            var context = ContextFactory();
            return await Task.FromResult(
                from ait in context.Company
                select new CompanyModel()
                {
                    Id = ait.Id,
                    NameEnglish = ait.NameEnglish,
                    //SupplierId = ait.SupplierId
                }
            );
        }

        public async Task<IQueryable<CompanyModel>> GetCompaniesWithLocation()
        {
            var context = ContextFactory();
            return await Task.FromResult(
                from ait in context.Company
                select new CompanyModel()
                {
                    Id = ait.Id,
                    NameEnglish = ait.NameEnglish,
                    //SupplierId = ait.SupplierId
                }
            );
        }

        public IQueryable<CompanyCardModel> GetSuppliersForRoadshowInviteModal(
            QueryModel queryModel,
            int id
        )
        {
            var context = ContextFactory();
            var alreadyInvitedCompanies = context.RoadshowInvite
                .Where(roadshowInvite => roadshowInvite.RoadshowId == id)
                .Select(roadshowInvite => roadshowInvite.CompanyId)
                .ToList();
            IQueryable<Company> companiesToInvite = context.Company.Where(
                company =>
                    alreadyInvitedCompanies.All(id => id != company.Id)
                    && company.ApproveStatus == ApproveStatus.Approved.ToString()
            );

            var filteredCompanies = FilterSuppliersForRoadshowInviteModal(
                companiesToInvite,
                queryModel
            );
            var companyModels = filteredCompanies.Select(projectToCompanyCardModel);

            return Sort(queryModel.Sort, companyModels);
        }

        private static IQueryable<Company> FilterSuppliersForRoadshowInviteModal(
            IQueryable<Company> companies,
            QueryModel queryModel
        )
        {
            var filteredCompanies = companies;
            if (!String.IsNullOrWhiteSpace(queryModel.Filter.Keyword))
            {
                filteredCompanies = companies.Where(
                    company =>
                        company.NameEnglish
                            .ToLower()
                            .Contains(queryModel.Filter.Keyword.Trim().ToLower())
                );
            }

            if (queryModel.Filter.Categories?.Any() == true)
            {
                filteredCompanies = filteredCompanies.Where(
                    company =>
                        company.CompanyCategories.Any(
                            oc => queryModel.Filter.Categories.Contains(oc.CategoryId)
                        )
                );
            }

            return filteredCompanies;
        }

        public IQueryable<CompanyCardModel> GetAllPendingCompaniesPage(QueryModel queryModel)
        {
            var context = ContextFactory();

            var company = context.Company
                .AsNoTracking()
                .Where(x => x.ApproveStatus == SupplierStatus.PendingApproval.ToString());

            var filteredCompany = FilterCompany(company, queryModel);
            var companyCardModels = filteredCompany.Select(projectToCompanyCardModel);

            return Sort(queryModel.Sort, companyCardModels);
        }

        public async Task<bool> CheckIfCompanyWithSameNameExits(string companyName)
        {
            var context = ContextFactory();

            return await context.Company.AnyAsync(
                c => c.NameEnglish == companyName || c.NameArabic == companyName
            );
        }

        public async Task<bool> CheckIfUserExistsAsCompanySupplier(string userName)
        {
            var context = ContextFactory();

            var user = await context.Users.FirstOrDefaultAsync(u => u.UserName == userName);

            return await context.CompanySuppliers.AnyAsync(cs => cs.SupplierId == user.Id);
        }

        public async Task<int> GetMyFocalPointsCount(string userId)
        {
            var context = ContextFactory();

            var companySupplier = await context.CompanySuppliers.FirstOrDefaultAsync(
                cs => cs.SupplierId == userId
            );

            return await (
                from cs in context.CompanySuppliers
                where cs.CompanyId == companySupplier.CompanyId
                group cs by cs.CompanyId into g
                select g.Count()
            ).FirstOrDefaultAsync();
        }

        public async Task<int> GetCompaniesInStatusCount(string status)
        {
            var context = ContextFactory();

            return await context.Company.Where(x => x.ApproveStatus == status).CountAsync();
        }

        /// <summary>
        /// Returns average rating and number of ratings for company based on user reviews of offers and roadshow offers
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public async Task<CompanyRatingModel> GetCompanyRating(int companyId)
        {
            var context = ContextFactory();

            // Get rating percent of all offers for that company
            var companyOfferRating = await (
                from o in context.Offer
                join or in context.OfferRating on o.Id equals or.OfferId
                where o.CompanyId == companyId && or.Status == OfferCommentStatus.Public.ToString()
                group or by o.CompanyId into g
                select new CompanyRatingModel
                {
                    AverageRating = g.Average(r => r.Rating),
                    TotalRatings = g.Count()
                }
            ).FirstOrDefaultAsync();

            // Get rating percent of all roadshow offers for that company
            var companyRoadshowOfferRating = await (
                from o in context.RoadshowOffer
                join or in context.RoadshowOfferRating on o.Id equals or.RoadshowOfferId
                join rp in context.RoadshowProposal on o.RoadshowProposalId equals rp.Id
                where
                    rp.CompanyId.GetValueOrDefault() == companyId
                    && or.Status == OfferCommentStatus.Public.ToString()
                group or by rp.CompanyId.GetValueOrDefault() into g
                select new CompanyRatingModel
                {
                    AverageRating = g.Average(r => r.Rating),
                    TotalRatings = g.Count()
                }
            ).FirstOrDefaultAsync();

            return GetCompanyRating(companyOfferRating, companyRoadshowOfferRating);
        }

        /// <summary>
        /// Calculets average rating for company based on all the ratings of offers and roadshow offer
        /// </summary>
        /// <param name="companyOfferRating"></param>
        /// <param name="companyRoadshowOfferRating"></param>
        /// <returns></returns>
        private CompanyRatingModel GetCompanyRating(
            CompanyRatingModel companyOfferRating,
            CompanyRatingModel companyRoadshowOfferRating
        )
        {
            if (companyOfferRating == null && companyRoadshowOfferRating == null)
            {
                return new CompanyRatingModel { TotalRatings = 0, AverageRating = 0 };
            }
            else if (companyOfferRating == null)
            {
                return companyRoadshowOfferRating;
            }
            else if (companyRoadshowOfferRating == null)
            {
                return companyOfferRating;
            }

            return new CompanyRatingModel
            {
                AverageRating =
                    (
                        companyOfferRating.AverageRating * companyOfferRating.TotalRatings
                        + companyRoadshowOfferRating.AverageRating
                            * companyRoadshowOfferRating.TotalRatings
                    ) / (companyOfferRating.TotalRatings + companyRoadshowOfferRating.TotalRatings),
                TotalRatings =
                    companyOfferRating.TotalRatings + companyRoadshowOfferRating.TotalRatings
            };
        }

        public async Task<int> GetCompanyIdForCompanyName(string companyName)
        {
            var context = ContextFactory();

            var c = await context.Company
                .Where(c => c.NameEnglish == companyName)
                .FirstOrDefaultAsync();

            return c == null ? 0 : c.Id;
        }

        public async Task<bool> CheckIfCompanyIsAttachedToSysAdmin(string companyName)
        {
            var context = ContextFactory();
            var company = await context.Company.FirstOrDefaultAsync(
                c => c.NameEnglish == companyName
            );
            var companySuppliersCount = await context.CompanySuppliers
                .Where(cs => cs.CompanyId == company.Id)
                .CountAsync();

            if (companySuppliersCount > 1)
                return false;

            var supplierAdminRole = (
                from cs in context.CompanySuppliers
                join ur in context.UserRoles on cs.SupplierId equals ur.UserId
                join r in context.Roles on ur.RoleId equals r.Id
                where cs.CompanyId == company.Id
                select r.Name
            ).FirstOrDefault();

            if (
                supplierAdminRole == Roles.Admin.ToString()
                || supplierAdminRole == Roles.AdnocCoordinator.ToString()
            )
                return true;

            return false;
        }

        public async Task AttachSupplierToExistingCompany(string companyName, string supplierId)
        {
            var context = ContextFactory();
            var company = await context.Company.FirstOrDefaultAsync(
                c => c.NameEnglish == companyName
            );
            var supplier = await userManager.FindByIdAsync(supplierId);
            await userManager.AddToRoleAsync(supplier, "Supplier Admin");

            var companySupplier = await context.CompanySuppliers.FirstOrDefaultAsync(
                cs => cs.CompanyId == company.Id
            );
            context.CompanySuppliers.Remove(companySupplier);

            context.CompanySuppliers.Add(
                new CompanySuppliers { CompanyId = company.Id, SupplierId = supplierId }
            );

            await context.SaveChangesAsync();
        }

        public async Task AttachSupplierToExistingCompanyAsFocalPoint(
            string companyName,
            string supplierId
        )
        {
            var context = ContextFactory();
            var company = await context.Company.FirstOrDefaultAsync(
                c => c.NameEnglish == companyName
            );
            var supplier = await userManager.FindByIdAsync(supplierId);
            await userManager.AddToRoleAsync(supplier, "Supplier");

            context.CompanySuppliers.Add(
                new CompanySuppliers { CompanyId = company.Id, SupplierId = supplierId }
            );

            await context.SaveChangesAsync();
        }

        public async Task<string> GetSupplierAdminForCompany(int companyId)
        {
            var context = ContextFactory();

            return await (
                from cs in context.CompanySuppliers
                join ur in context.UserRoles on cs.SupplierId equals ur.UserId
                join u in context.Users on cs.SupplierId equals u.Id
                join r in context.Roles on ur.RoleId equals r.Id
                where r.Name == Declares.Roles.SupplierAdmin.ToString() && cs.CompanyId == companyId
                orderby u.CreatedOn descending
                select cs.SupplierId
            ).FirstOrDefaultAsync();
        }

        public async Task<ApplicationUserModel> GetSupplierAdminModelForCompany(int companyId)
        {
            var context = ContextFactory();

            return await (
                from cs in context.CompanySuppliers
                join ur in context.UserRoles on cs.SupplierId equals ur.UserId
                join u in context.Users on cs.SupplierId equals u.Id
                join r in context.Roles on ur.RoleId equals r.Id
                where r.Name == Declares.Roles.SupplierAdmin.ToString() && cs.CompanyId == companyId
                orderby u.CreatedOn descending
                select new ApplicationUserModel { Id = u.Id, Email = u.Email }
            ).FirstOrDefaultAsync();
        }

        public async Task<string> GetSupplierAdminUsernameForCompany(int companyId)
        {
            var context = ContextFactory();

            return await (
                from cs in context.CompanySuppliers
                join u in context.Users on cs.SupplierId equals u.Id
                join ur in context.UserRoles on cs.SupplierId equals ur.UserId
                join r in context.Roles on ur.RoleId equals r.Id
                where r.Name == Declares.Roles.SupplierAdmin.ToString() && cs.CompanyId == companyId
                select u.UserName
            ).FirstOrDefaultAsync();
        }

        public async Task<int> GetFocalPointsCount(int companyId)
        {
            var context = ContextFactory();

            return (
                from cs in context.CompanySuppliers
                where cs.CompanyId == companyId
                select cs.SupplierId
            ).Count();
        }

        public async Task<List<string>> GetFocalPointIds(int companyId)
        {
            var context = ContextFactory();

            return (
                from cs in context.CompanySuppliers
                where cs.CompanyId == companyId
                select cs.SupplierId
            ).ToList();
        }

        public async Task<IEnumerable<CompanyExcelModel>> GetCompaniesExcelReport(
            DateTime startDate,
            DateTime endDate
        )
        {
            var context = ContextFactory();

            var temp = context.Company
                .Include(x => x.Offers)
                .Include(x => x.CompanySuppliers)
                .Include(x => x.CompanySuppliers)
                .Where(
                    company =>
                        company.CreatedOn.Date >= startDate.Date
                        && company.CreatedOn.Date <= endDate.Date
                )
                .Select(
                    company =>
                        new CompanyExcelModel
                        {
                            Name = company.NameEnglish,
                            LandNumber = company.LandNumber,
                            MobileNumber = company.Mobile,
                            OfficialEmail = company.OfficialEmail,
                            Id = company.Id,
                            CategoryId = company.CompanyCategories.FirstOrDefault().CategoryId,
                            OfferNumber = company.Offers.Count(),
                            ApprovedOfferNumber = company.Offers
                                .Where(
                                    o =>
                                        o.Status
                                        == Enum.GetName(typeof(OfferStatus), OfferStatus.Approved)
                                )
                                .Count(),
                            DraftOfferNumber = company.Offers
                                .Where(
                                    o =>
                                        o.Status
                                        == Enum.GetName(typeof(OfferStatus), OfferStatus.Draft)
                                )
                                .Count(),
                            ExpiredOfferNumber = company.Offers
                                .Where(
                                    o =>
                                        o.Status
                                        == Enum.GetName(typeof(OfferStatus), OfferStatus.Expired)
                                )
                                .Count(),
                            FocalPointMails = company.CompanySuppliers
                                .Select(x => x.Supplier.Email)
                                .ToArray()
                        }
                )
                .ToList();

            return temp;
        }

        public async Task UpdateTeradeLicence(CompanyRegistrationModel data, string userId)
        {
            var context = ContextFactory();
            var company = context.CompanySuppliers
                .Include(x => x.Company)
                .Where(x => x.SupplierId == userId)
                .Select(x => x.Company)
                .FirstOrDefault();
            company.TradeLicenceId = new Guid(data.TradeLicence.Id);
            company.ApproveStatus = SupplierStatus.PendingApproval.ToString();
            company.TradeLicenseExpDate = data.TradeLicenceExpiryDate;

            context.Update(company);
            await context.SaveChangesAsync();

            return;
        }

        public async Task AddFocalPointToCompany(int companyId, string userId)
        {
            var context = ContextFactory();
            context.CompanySuppliers.Add(
                new CompanySuppliers() { CompanyId = companyId, SupplierId = userId }
            );
            await context.SaveChangesAsync();
        }
    }
}
