using MMA.WebApi.Shared.Interfaces.Categories;
using MMA.WebApi.Shared.Interfaces.Collection;
using MMA.WebApi.Shared.Interfaces.Offers;
using MMA.WebApi.Shared.Interfaces.Tag;
using MMA.WebApi.Shared.Models.Mobile;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MMA.WebApi.Core.Services
{
    public class MobileCacheDataService : IMobileCacheDataService
    {
        private readonly IMobileCacheDataRepository _mobileCacheDataRepository;
        private readonly ITagService _tagService;
        private readonly ICategoryService _categoryService;
        private readonly ICollectionService _collectionService;

        public MobileCacheDataService(IMobileCacheDataRepository mobileCacheDataRepository,
                                      ITagService tagService,
                                      ICategoryService categoryService,
                                      ICollectionService collectionService)
        {
            _mobileCacheDataRepository = mobileCacheDataRepository;
            _tagService = tagService;
            _categoryService = categoryService;
            _collectionService = collectionService;
        }

        public async Task<string> GetSynchronizationDataModel()
        {
            return await _mobileCacheDataRepository.GetSynchronizationDataModel();
        }

        public async Task SetMobileCacheData(SynchronizationDataModel model)
        {
            await _mobileCacheDataRepository.SetMobileCacheData(model);
        }

        public async Task GetTagsIds(SynchronizationDataModel synchronizationDataModel)
        {
            var tagIds = await _tagService.GetTagsMobile();
            synchronizationDataModel.TagIds = tagIds.Select(t => t.Id);
        }

        public async Task GetTags(SynchronizationDataModel synchronizationDataModel, DateTime lastUpdateOn)
        {
            var tags = await _tagService.GetTags();
            synchronizationDataModel.Tags = tags.Where(c => c.UpdatedOn >= lastUpdateOn).OrderBy(t => t.Id);
        }

        public async Task GetCollectionsIds(SynchronizationDataModel synchronizationDataModel)
        {
            var collectionsIds = await _collectionService.GetCollections();
            synchronizationDataModel.CollectionsIds = collectionsIds.Select(c => c.Id);
        }

        public async Task GetCollections(SynchronizationDataModel synchronizationDataModel, DateTime lastUpdateOn)
        {
            var collections = await _collectionService.GetCollections();
            synchronizationDataModel.Collections = collections.Where(c => c.UpdatedOn >= lastUpdateOn).OrderBy(c => c.Id);
        }

        public async Task GetCategoriesIds(SynchronizationDataModel synchronizationDataModel)
        {
            var categoriesIds = await _categoryService.GetCategories();
            synchronizationDataModel.CategoriesIds = categoriesIds.Select(c => c.Id);
        }

        public async Task GetCategories(SynchronizationDataModel synchronizationDataModel, DateTime lastUpdateOn)
        {
            var categories = await _categoryService.GetCategories();
            synchronizationDataModel.Categories = categories.Where(c => c.UpdatedOn >= lastUpdateOn).OrderBy(c => c.Title);
        }
    }
}
