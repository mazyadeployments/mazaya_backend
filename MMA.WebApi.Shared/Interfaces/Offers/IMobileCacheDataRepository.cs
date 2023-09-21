using MMA.WebApi.Shared.Models.Mobile;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.Offers
{
    public interface IMobileCacheDataRepository
    {
        Task SetMobileCacheData(SynchronizationDataModel model);
        Task<string> GetSynchronizationDataModel();
    }
}
