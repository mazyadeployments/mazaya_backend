using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.MazayaCategory;
using MMA.WebApi.Shared.Monads;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.MazayaCategory
{
    public interface IMazayaCategoryService
    {
        Task<IEnumerable<MazayaCategoryModel>> GetMazayaCategories();

        Task<Maybe<MazayaCategoryModel>> GetMazayaCategory(int id);
        Task<PaginationListModel<MazayaCategoryModel>> GetMazayaCategoriesPage(QueryModel queryModel);

        Task<Maybe<MazayaCategoryModel>> CreateOrUpdateAsync(MazayaCategoryModel model, string userId);

        Task DeleteMazayaCategory(int id);
    }
}
