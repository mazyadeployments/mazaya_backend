using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.MazayaSubCategory;
using MMA.WebApi.Shared.Monads;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.MazayaSubCategory
{
    public interface IMazayaSubCategoryService
    {
        Task<IEnumerable<MazayaSubCategoryModel>> GetallMembeship();
        Task<IEnumerable<MazayaSubCategoryModel>> GetMazayasubCategories();
        Task<Maybe<MazayaSubCategoryModel>> GetMazayasubCategory(int id);
        Task<PaginationListModel<MazayaSubCategoryModel>> GetMazayasubCategoriesPage(QueryModel queryModel);
        Task<Maybe<MazayaSubCategoryModel>> CreateOrUpdateAsync(MazayaSubCategoryModel model, string userId);
        Task<Maybe<MazayaSubCategoryModel>> UpdateCountAsync(MazayaSubCategoryModel model, string userId);
        Task DeleteMazayasubCategory(int id);
    }
}
