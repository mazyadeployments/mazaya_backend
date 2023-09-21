using Microsoft.EntityFrameworkCore;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Interfaces.Announcement;
using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Models.Announcement;
using MMA.WebApi.Shared.Models.SupplierAnnouncement;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.WebApi.DataAccess.Repositories.Announcement
{
    public class AnnouncementRepository : BaseRepository<AnnouncementModel>, IAnnouncementRepository
    {
        public AnnouncementRepository(Func<MMADbContext> contexFactory)
            : base(contexFactory) { }

        public async Task CreateAnnouncementAsync(
            AnnouncementModel model,
            IVisitor<IChangeable> auditVisitor
        )
        {
            var context = ContextFactory();
            var entityModel = PopulateDbFromDomainModel(new Models.Announcement(), model);
            entityModel.Accept(auditVisitor);
            try
            {
                await context.Announcement.AddAsync(entityModel);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<AnnouncementModel> GetAnnouncementByStatus(
            Declares.AnnouncementStatus status
        )
        {
            var context = ContextFactory();

            return await context.Announcement
                .AsNoTracking()
                .Select(projectToAnnouncementModel)
                .FirstOrDefaultAsync(x => x.Status == status);
        }

        public async Task ChangeAnnouncementStatus(
            AnnouncementStatus status,
            int announcementId,
            int? countUsers,
            int? countSent,
            int? countFailed
        )
        {
            var context = ContextFactory();
            var announcement = await context.Announcement.FirstOrDefaultAsync(
                x => x.Id == announcementId
            );
            if (announcement != null)
            {
                announcement.Status = status;
                announcement.CountAllToSend =
                    countUsers != null ? (int)countUsers : announcement.CountAllToSend;
                announcement.CountSent =
                    countSent != null ? (int)countSent : announcement.CountSent;
                announcement.CountFailed =
                    countFailed != null ? (int)countFailed : announcement.CountFailed;

                context.Update(announcement);
                await context.SaveChangesAsync();
            }
        }

        public async Task<AnnouncementModel> GetSpecificAnnouncement(int announcementId)
        {
            var context = ContextFactory();
            return await context.Announcement
                .AsNoTracking()
                .Select(projectToAnnouncementModel)
                .FirstOrDefaultAsync(x => x.Id == announcementId);
        }

        public async Task UpdateCounts(int announcementId, int sentCount, int failedCount)
        {
            var context = ContextFactory();
            var announcement = await context.Announcement.FirstOrDefaultAsync(
                x => x.Id == announcementId
            );
            if (announcement != null)
            {
                announcement.CountSent = sentCount;
                announcement.CountFailed = failedCount;
                announcement.Status =
                    failedCount > 0 ? AnnouncementStatus.Failed : AnnouncementStatus.Success;
                context.Update(announcement);
                await context.SaveChangesAsync();
            }
        }

        protected virtual Models.Announcement PopulateDbFromDomainModel(
            Models.Announcement entityModel,
            AnnouncementModel data
        )
        {
            entityModel.Status = Shared.Enums.Declares.AnnouncementStatus.Process;
            entityModel.AllBuyers = data.AllBuyers;
            entityModel.AllSuppliers = data.AllSuppliers;
            entityModel.SpecificBuyers =
                data.CategoriesBuyer != null && data.CategoriesBuyer.Count > 0 ? true : false;
            entityModel.SpecificSuppliers =
                data.CategoriesSupplier != null && data.CategoriesSupplier.Count > 0 ? true : false;
            entityModel.CountAllToSend = 0;
            entityModel.CountFailed = 0;
            entityModel.CountSent = 0;
            entityModel.Message = data.AnnouncementText;
            entityModel.SpecificBuyersCollection =
                data.CategoriesBuyer != null && data.CategoriesBuyer.Count > 0
                    ? data.CategoriesBuyer
                        .Select(buyerTypeId =>
                        {
                            return new AnnouncementSpecificBuyer
                            {
                                AnnouncementId = entityModel.Id,
                                BuyerType = buyerTypeId
                            };
                        })
                        .ToList()
                    : null;
            entityModel.SpecificSuppliersCollection =
                data.CategoriesSupplier != null && data.CategoriesSupplier.Count > 0
                    ? data.CategoriesSupplier
                        .Select(supplierCategoryId =>
                        {
                            return new AnnouncementSpecificSupplier
                            {
                                AnnouncementId = entityModel.Id,
                                SupplierCategory = supplierCategoryId
                            };
                        })
                        .ToList()
                    : null;
            entityModel.Attachments =
                data.Attachments != null && data.Attachments.Length > 0
                    ? data.Attachments
                        .Select(attachments =>
                        {
                            return new AnnouncementAttachments
                            {
                                AnnouncementId = entityModel.Id,
                                DocumentId = Guid.Parse(attachments.Id)
                            };
                        })
                        .ToList()
                    : null;

            return entityModel;
        }

        private Expression<
            Func<Models.Announcement, AnnouncementModel>
        > projectToAnnouncementModel = data =>
            new AnnouncementModel()
            {
                Id = data.Id,
                Status = data.Status,
                AllBuyers = data.AllBuyers,
                AllSuppliers = data.AllSuppliers,
                CategoriesSupplier = data.SpecificSuppliers
                    ? data.SpecificSuppliersCollection.Select(x => x.SupplierCategory).ToList()
                    : null,
                CategoriesBuyer = data.SpecificBuyers
                    ? data.SpecificBuyersCollection.Select(x => x.BuyerType).ToList()
                    : null,
                AnnouncementText = data.Message,
                Attachments =
                    data.Attachments != null && data.Attachments.Count > 0
                        ? data.Attachments
                            .Select(
                                x =>
                                    new AnnouncementAttachmentModel { Id = x.DocumentId.ToString() }
                            )
                            .ToArray()
                        : null,
                CreatedBy = data.CreatedBy.ToString()
            };

        protected override IQueryable<AnnouncementModel> GetEntities()
        {
            throw new NotImplementedException();
        }
    }
}
