using Microsoft.EntityFrameworkCore;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Interfaces.MazayaPackageSubscriptions;
using MMA.WebApi.Shared.Models.MazayaPackagesubscriptionsModel;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMA.WebApi.Shared.Models.MazayaSubCategory;

namespace MMA.WebApi.DataAccess.Repositories.MazayaPackagesubscriptions
{
    public class MazayapackagesubscriptionsRepository : BaseRepository<MazayaPackageSubscriptionsModel>, IMazayaPackagesubscriptionsRepository
    {
        public MazayapackagesubscriptionsRepository(Func<MMADbContext> contexFactory) : base(contexFactory) { }

        public async Task<MazayaPackageSubscriptionsModel> CreateOrUpdateAsync(MazayaPackageSubscriptionsModel model, IVisitor<IChangeable> auditVisitor)
        {
            var context = ContextFactory();

            var mazayapackagesubscriptions = context.MazayaPackageSubscriptions.FirstOrDefault(x => x.Id == model.Id);

            if (model.Id == 0)
            {
                mazayapackagesubscriptions.Accept(auditVisitor);
                context.Add(mazayapackagesubscriptions);
            }
            else
            {
                mazayapackagesubscriptions.UpdatedOn = DateTime.UtcNow;
                context.Update(mazayapackagesubscriptions);
            }

            await context.SaveChangesAsync();

            return projectToMazayapackagesubscriptionsModel.Compile().Invoke(mazayapackagesubscriptions);
        }

        public async Task<MazayaPackageSubscriptionsModel> DeleteAsync(int id)
        {
            var context = ContextFactory();
            var mazayapackagesubscriptions = await context.MazayaPackageSubscriptions
                        .AsNoTracking()
                        .Select(projectToMazayapackagesubscriptionsModel)
                        .FirstOrDefaultAsync(x => x.Id == id);

            if (mazayapackagesubscriptions != null)
            {
                var data = new MMA.WebApi.DataAccess.Models.MazayaPackageSubscription();
                data.Id = mazayapackagesubscriptions.Id;

                context.Remove(data);
                context.SaveChanges();
            }
            return mazayapackagesubscriptions;
        }

        public async Task<MazayaPackageSubscriptionsModel> Get(int id)
        {
            var context = ContextFactory();

            return await context.MazayaPackageSubscriptions
                    .Include(x => x.MazayaSubCategory)
                    .AsNoTracking()
                    .Select(projectToMazayapackagesubscriptionsModel)
                    .FirstOrDefaultAsync(x => x.Id == id);
        }

        public IQueryable<MazayaPackageSubscriptionsModel> GetAllPackagesubscriptions()
        {
            var context = ContextFactory();

            var mazayapaymentgateway = context.MazayaPackageSubscriptions
                               .Include(x => x.MazayaSubCategory)
                               .Select(projectToMazayapackagesubscriptionsModel);
            return mazayapaymentgateway.OrderBy(c => c.Id);
        }

        public async Task<int> GetMazayaPackagesubscriptionsCount()
        {
            var context = ContextFactory();

            return await context.MazayaPackageSubscriptions.CountAsync();
        }

        public IEnumerable<MazayaPackageSubscriptionsModel> GetMazayaPackagesubscriptionsNumber()
        {
            var context = ContextFactory();

            var mazayapackagesubscriptions = context.MazayaPackageSubscriptions
                .AsNoTracking()
                .Select(projectToMazayapackagesubscriptionsModel);

            return mazayapackagesubscriptions.OrderBy(c => c.Id);
        }

        public IQueryable<MazayaPackageSubscriptionsModel> GetMazayaPackagesubscriptionsNumberPage(QueryModel queryModel)
        {
            var context = ContextFactory();

            var mazayapackagesubscriptions = context.MazayaPackageSubscriptions.AsNoTracking();
            var subscriptionsModels = mazayapackagesubscriptions.Select(projectToMazayapackagesubscriptionsModel);

            return Sort(queryModel.Sort, subscriptionsModels);
        }

        protected override IQueryable<MazayaPackageSubscriptionsModel> GetEntities()
        {
            var context = ContextFactory();

            return context.MazayaPackageSubscriptions
                .Select(projectToMazayapackagesubscriptionsModel);
        }

        private static IQueryable<MazayaPackageSubscriptionsModel> Sort(SortModel sortModel, IQueryable<MazayaPackageSubscriptionsModel> mazayaPackageSubscriptions)
        {
            // Currently sorting needs to be done alphabetically
            return mazayaPackageSubscriptions.OrderBy(c => c.Id);
        }

        private Expression<Func<MMA.WebApi.DataAccess.Models.MazayaPackageSubscription, MazayaPackageSubscriptionsModel>> projectToMazayapackagesubscriptionsModel = data =>
        new MazayaPackageSubscriptionsModel()
        {
            Id = data.Id,
            ApplicationUserId = data.ApplicationUserId,
            Amount = data.Amount,
            SubCategoryId = data.SubCategoryId,
            CreatedBy = data.CreatedBy,
            CreatedOn = data.CreatedOn,
            UpdatedBy = data.UpdatedBy,
            UpdatedOn = data.UpdatedOn,
            MazayaSubCategory = new MazayaSubCategoryModel
            {
                Id = data.MazayaSubCategory.Id,
                Name = data.MazayaSubCategory.Name,
                Amount = data.MazayaSubCategory.Amount,
                NoofChildren = data.MazayaSubCategory.NoofChildren,
                Description = data.MazayaSubCategory.Description,
                MazayaCategoryId = data.MazayaSubCategory.MazayaCategoryId,
                CreatedBy = data.MazayaSubCategory.CreatedBy,
                CreatedOn = data.MazayaSubCategory.CreatedOn,
                UpdatedBy = data.MazayaSubCategory.UpdatedBy,
                UpdatedOn = data.MazayaSubCategory.UpdatedOn
                //MazayaCategory = data.MazayaSubCategory.MazayaCategory,
            }

        };

        private Expression<Func<MMA.WebApi.DataAccess.Models.MazayaPackageSubscription, MazayaPackageSubscriptionsModel>> projectToMazayapackagesubscriptionsCardModel = data =>
           new MazayaPackageSubscriptionsModel()
           {
               Id = data.Id,
               ApplicationUserId = data.ApplicationUserId,
               Amount = data.Amount,
               SubCategoryId = data.SubCategoryId,
               UpdatedOn = data.UpdatedOn,
               UpdatedBy = data.UpdatedBy,
               CreatedBy = data.CreatedBy,
               CreatedOn = data.CreatedOn,
               MazayaSubCategory = new MazayaSubCategoryModel
               {
                   Id = data.MazayaSubCategory.Id,
                   Name = data.MazayaSubCategory.Name,
                   Amount = data.MazayaSubCategory.Amount,
                   NoofChildren = data.MazayaSubCategory.NoofChildren,
                   Description = data.MazayaSubCategory.Description,
                   MazayaCategoryId = data.MazayaSubCategory.MazayaCategoryId,
                   CreatedBy = data.MazayaSubCategory.CreatedBy,
                   CreatedOn = data.MazayaSubCategory.CreatedOn,
                   UpdatedBy = data.MazayaSubCategory.UpdatedBy,
                   UpdatedOn = data.MazayaSubCategory.UpdatedOn
                   //MazayaCategory = data.MazayaSubCategory.MazayaCategory,
               }
           };
    }
}
