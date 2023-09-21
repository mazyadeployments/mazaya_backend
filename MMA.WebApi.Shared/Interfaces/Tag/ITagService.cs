using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.Tag;
using MMA.WebApi.Shared.Monads;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.Tag
{

    public interface ITagService
    {
        Task<IEnumerable<TagModel>> GetTags();
        Task<PaginationListModel<TagModel>> GetTagsPage(QueryModel queryModel);
        Task<IEnumerable<TagModel>> GetTagsMobile();
        Task<Maybe<TagModel>> CreateTag(TagModel model, string userId);
        Task DeleteTag(int id);
        Task<Maybe<TagModel>> GetTag(int id);
    }
}
