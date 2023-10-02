using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Models.MazayaSubCategory;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.MazayaSubCategory
{
    public interface IMazayaSubcategoryRepository
    {
        Task<MazayaSubCategoryModel> CreateOrUpdateAsync(MazayaSubCategoryModel model, IVisitor<IChangeable> auditVisitor);
        Task<MazayaSubCategoryModel> Get(int id);
        IEnumerable<MazayaSubCategoryModel> Getallmembership();
        IEnumerable<MazayaSubCategoryModel> GetMazayasubCategoriesNumber();
        IQueryable<MazayaSubCategoryModel> GetMazayasubCategoriesNumberPage(QueryModel queryModel);
        Task<int> GetMazayasubCategoriesCount();
        Task<MazayaSubCategoryModel> DeleteAsync(int id);
        IQueryable<MazayaSubCategoryModel> GetAllMazayasubCategory();
    }
}
