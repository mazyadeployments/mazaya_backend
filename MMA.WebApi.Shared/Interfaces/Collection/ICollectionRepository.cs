using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Models.Collection;
using MMA.WebApi.Shared.Visitor;
using System.Linq;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.Collection
{
    public interface ICollectionRepository : IQueryableRepository<CollectionModel>
    {
        Task<CollectionModel> CreateOrUpdateAsync(CollectionModel model, IVisitor<IChangeable> auditVisitor);
        Task<CollectionModel> Get(int id);
        Task<int> GetCollectionsCount();
        Task<CollectionModel> DeleteAsync(int id);
        IQueryable<CollectionModel> GetCollectionsPage(QueryModel queryModel);
    }
}
