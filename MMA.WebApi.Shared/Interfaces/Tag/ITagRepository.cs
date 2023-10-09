using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Models.Tag;
using MMA.WebApi.Shared.Visitor;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.Tag
{
    public interface ITagRepository : IQueryableRepository<TagModel>
    {
        Task<TagModel> CreateAsync(TagModel model, IVisitor<IChangeable> auditVisitor);
        Task<TagModel> DeleteAsync(int id);
        IQueryable<TagModel> GetTagsPage(QueryModel queryModel);
        Task<int> GetTagsCount();
        Task<TagModel> Get(int id);
        Task<IEnumerable<TagModel>> GetTags();
    }
}
