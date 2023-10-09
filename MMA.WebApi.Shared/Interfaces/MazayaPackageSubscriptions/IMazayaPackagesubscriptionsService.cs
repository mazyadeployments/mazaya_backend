using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.MazayaPackagesubscriptionsModel;
using MMA.WebApi.Shared.Monads;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.MazayaPackageSubscriptions
{
    public interface IMazayaPackagesubscriptionsService
    {
        Task<IEnumerable<MazayaPackageSubscriptionsModel>> GetMazayaPackagesubscriptions();

        Task<Maybe<MazayaPackageSubscriptionsModel>> GetMazayaPackagesubscription(int id);
        Task<PaginationListModel<MazayaPackageSubscriptionsModel>> GetMazayaPackagesubscriptionspage(QueryModel queryModel);

        Task<Maybe<MazayaPackageSubscriptionsModel>> CreateOrUpdateAsync(MazayaPackageSubscriptionsModel model, string userId);

        Task DeleteMazayaPackagesubscriptions(int id);
    }
}
