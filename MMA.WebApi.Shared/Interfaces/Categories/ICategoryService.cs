using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Models.Category;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Monads;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.Categories
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryModel>> GetCategories();

        Task<Maybe<CategoryModel>> GetCategory(int id);
        Task<PaginationListModel<CategoryModel>> GetCategoriesPage(QueryModel queryModel);

        Task<Maybe<CategoryModel>> CreateOrUpdateAsync(CategoryModel model, string userId);

        Task DeleteCategory(int id);
    }
}
