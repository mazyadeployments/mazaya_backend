using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.Other;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.GenericData
{
    public interface ISearchablePaginationListAsync<T>
    {
        Task<PaginationListModel<T>> Search(SearchModel search);
    }
}
