using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Models.MazayaCategory;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.MazayaCategory
{
    public interface IMazayaCategoryRepository : IQueryableRepository<MazayaCategoryModel>
    {
        Task<MazayaCategoryModel> CreateOrUpdateAsync(MazayaCategoryModel model, IVisitor<IChangeable> auditVisitor);
        Task<MazayaCategoryModel> Get(int id);
        IEnumerable<MazayaCategoryModel> GetMazayaCategoriesNumber();
        IQueryable<MazayaCategoryModel> GetMazayaCategoriesNumberPage(QueryModel queryModel);
        Task<int> GetMazayaCategoriesCount();
        Task<MazayaCategoryModel> DeleteAsync(int id);
        IQueryable<MazayaCategoryModel> GetAllMazayaCategory();
    }
}
