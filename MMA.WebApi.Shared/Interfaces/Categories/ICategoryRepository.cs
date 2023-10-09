using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Models.Category;
using MMA.WebApi.Shared.Visitor;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.Categories
{
    public interface ICategoryRepository : IQueryableRepository<CategoryModel>
    {
        Task<CategoryModel> CreateOrUpdateAsync(CategoryModel model, IVisitor<IChangeable> auditVisitor);
        Task<CategoryModel> Get(int id);
        IEnumerable<CategoryModel> GetCategoriesWithOfferNumber();
        IQueryable<CategoryModel> GetCategoriesWithOfferNumberPage(QueryModel queryModel);
        Task<int> GetCategoriesCount();
        Task<CategoryModel> DeleteAsync(int id);
        IQueryable<CategoryModel> GetAllCategoty();
    }
}
