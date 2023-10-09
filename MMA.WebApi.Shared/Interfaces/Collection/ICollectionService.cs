using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Models.Collection;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.Image;
using MMA.WebApi.Shared.Monads;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.Collection
{
    public interface ICollectionService
    {
        Task<IEnumerable<CollectionModel>> GetCollections();
        Task<Maybe<CollectionModel>> GetCollection(int id);
        Task<Maybe<CollectionModel>> CreateCollection(CollectionModel model, string userId);
        Task CreateImages(List<ImageModel> croppedImages);
        Task<PaginationListModel<CollectionModel>> GetCollectionsPage(QueryModel queryModel);
        Task DeleteCollection(int id);
    }
}
