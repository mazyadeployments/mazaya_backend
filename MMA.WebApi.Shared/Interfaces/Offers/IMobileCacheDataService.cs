using MMA.WebApi.Shared.Models.Mobile;
using System;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.Offers
{
    public interface IMobileCacheDataService
    {
        Task SetMobileCacheData(SynchronizationDataModel model);
        Task<string> GetSynchronizationDataModel();
        Task GetTagsIds(SynchronizationDataModel synchronizationDataModel);

        Task GetTags(SynchronizationDataModel synchronizationDataModel, DateTime lastUpdateOn);

        Task GetCollectionsIds(SynchronizationDataModel synchronizationDataModel);

        Task GetCollections(SynchronizationDataModel synchronizationDataModel, DateTime lastUpdateOn);

        Task GetCategoriesIds(SynchronizationDataModel synchronizationDataModel);

        Task GetCategories(SynchronizationDataModel synchronizationDataModel, DateTime lastUpdateOn);

    }
}
