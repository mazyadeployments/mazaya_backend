using Microsoft.EntityFrameworkCore;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Interfaces.MazayaSubCategory;
using MMA.WebApi.Shared.Models.MazayaSubCategory;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMA.WebApi.Shared.Models.Membership;
using MMA.WebApi.Shared.Models.MazayaCategory;

namespace MMA.WebApi.DataAccess.Repositories.Mazayasubcategory
{
    public class MazayasubcategoryRepository : BaseRepository<MazayaSubCategoryModel>, IMazayaSubcategoryRepository
    {
        public MazayasubcategoryRepository(Func<MMADbContext> contexFactory) : base(contexFactory) { }

        public async Task<MazayaSubCategoryModel> CreateOrUpdateAsync(MazayaSubCategoryModel model, IVisitor<IChangeable> auditVisitor)
        {
            var context = ContextFactory();

            var mazayasubcategory = context.MazayaSubcategories.FirstOrDefault(x => x.Id == model.Id);

            if (model.Id == 0)
            {
                mazayasubcategory.Accept(auditVisitor);
                context.Add(mazayasubcategory);
            }
            else
            {
                mazayasubcategory.UpdatedOn = DateTime.UtcNow;
                context.Update(mazayasubcategory);
            }

            await context.SaveChangesAsync();

            return projectToMazayasubCategoryModel.Compile().Invoke(mazayasubcategory);
        }

        public async Task<MazayaSubCategoryModel> DeleteAsync(int id)
        {
            var context = ContextFactory();
            var mazayasubcategory = await context.MazayaSubcategories
                        .AsNoTracking()
                        .Select(projectToMazayasubCategoryModel)
                        .FirstOrDefaultAsync(x => x.Id == id);

            if (mazayasubcategory != null)
            {
                var data = new MMA.WebApi.DataAccess.Models.MazayaSubcategory();
                data.Id = mazayasubcategory.Id;

                context.Remove(data);
                context.SaveChanges();
            }
            return mazayasubcategory;
        }

        public async Task<MazayaSubCategoryModel> Get(int id)
        {
            var context = ContextFactory();

            var str = await context.MazayaSubcategories
                    .Include(x => x.MazayaCategory)
                    .AsNoTracking()
                    .Select(projectToMazayasubCategoryModel)
                    .FirstOrDefaultAsync(x => x.Id == id);
            return str;
        }

        public IQueryable<MazayaSubCategoryModel> GetAllMazayasubCategory()
        {
            var context = ContextFactory();

            var mazayasubcategories = context.MazayaSubcategories
                               .Include(x => x.MazayaCategory)
                               .Select(projectToMazayasubCategoryModel);
            return mazayasubcategories.OrderBy(c => c.Id);
        }

        public async Task<int> GetMazayasubCategoriesCount()
        {
            var context = ContextFactory();

            return await context.MazayaSubcategories.CountAsync();
        }

        public IEnumerable<MazayaSubCategoryModel> GetMazayasubCategoriesNumber()
        {
            var context = ContextFactory();

            var mazayasubcategories = context.MazayaSubcategories
                .AsNoTracking()
                .Select(projectToMazayasubCategoryModel);

            return mazayasubcategories.OrderBy(c => c.Id);
        }

        public IQueryable<MazayaSubCategoryModel> GetMazayasubCategoriesNumberPage(QueryModel queryModel)
        {
            var context = ContextFactory();

            var mazayasubcategories = context.MazayaSubcategories.AsNoTracking();
            var categoryModels = mazayasubcategories.Select(projectToMazayasubCategoryModel);

            return Sort(queryModel.Sort, categoryModels);
        }


        private static IQueryable<MazayaSubCategoryModel> Sort(SortModel sortModel, IQueryable<MazayaSubCategoryModel> mazayasubcategories)
        {
            // Currently sorting needs to be done alphabetically
            return mazayasubcategories.OrderBy(c => c.Id);
        }
        protected override IQueryable<MazayaSubCategoryModel> GetEntities()
        {
            var context = ContextFactory();

            return context.MazayaSubcategories
                .Select(projectToMazayasubCategoryModel);
        }

        public IEnumerable<MazayaSubCategoryModel> Getallmembership()
        {
            var context = ContextFactory();

            var mazayamembership = context.MazayaSubcategories
                .AsNoTracking()
                .Select(projectToMazayasubCategoryModel);

            return mazayamembership.OrderBy(c => c.Id);
        }

        private Expression<Func<MMA.WebApi.DataAccess.Models.MazayaSubcategory, MazayaSubCategoryModel>> projectToMazayasubCategoryModel = data =>
        new MazayaSubCategoryModel()
        {
            Id = data.Id,
            Name = data.Name,
            Amount = data.Amount,
            NoofChildren = data.NoofChildren,
            NoofAdult= data.NoofAdult,
            totalcount= data.totalcount,
            optiontype= data.optiontype,
            vat = data.vat,
            MazayaCategoryId = data.MazayaCategoryId,
           // MazayaCategory = data.MazayaCategory,
            Description = data.Description,
            CreatedBy = data.CreatedBy,
            CreatedOn = data.CreatedOn,
            UpdatedBy = data.UpdatedBy,
            UpdatedOn = data.UpdatedOn,
            currency= data.currency,
            MazayaCategory = new MazayaCategoryModel()
            {
                Id = data.MazayaCategory.Id,
                Name = data.MazayaCategory.Name,
                CreatedBy = data.MazayaCategory.CreatedBy,
                CreatedOn = data.MazayaCategory.CreatedOn,
                UpdatedBy = data.MazayaCategory.UpdatedBy,
                UpdatedOn = data.MazayaCategory.UpdatedOn,
                description = data.MazayaCategory.description,
                facilities = data.MazayaCategory.facilities
            }
        };

        private Expression<Func<MMA.WebApi.DataAccess.Models.MazayaSubcategory, MazayaSubCategoryModel>> projectToMazayacategoryCardModel = data =>
           new MazayaSubCategoryModel()
           {
               Id = data.Id,
               Name = data.Name,
               Amount = data.Amount,
               NoofChildren = data.NoofChildren,
               NoofAdult = data.NoofAdult,
               totalcount = data.totalcount,
               optiontype = data.optiontype,
               vat = data.vat,
               Description = data.Description,
               currency= data.currency,
               MazayaCategoryId = data.MazayaCategoryId,
               MazayaCategory = new MazayaCategoryModel()
               {
                   Id = data.MazayaCategory.Id,
                   Name = data.MazayaCategory.Name,
                   CreatedBy = data.MazayaCategory.CreatedBy,
                   CreatedOn = data.MazayaCategory.CreatedOn,
                   UpdatedBy = data.MazayaCategory.UpdatedBy,
                   UpdatedOn = data.MazayaCategory.UpdatedOn,
                   description = data.MazayaCategory.description,
                   facilities = data.MazayaCategory.facilities
               },
               UpdatedOn = data.UpdatedOn,
               UpdatedBy = data.UpdatedBy,
               CreatedBy = data.CreatedBy,
               CreatedOn = data.CreatedOn
           };
    }
}
