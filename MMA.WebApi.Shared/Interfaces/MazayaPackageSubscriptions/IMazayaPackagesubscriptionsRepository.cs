using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Models.MazayaPackagesubscriptionsModel;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.MazayaPackageSubscriptions
{
    public interface IMazayaPackagesubscriptionsRepository
    {
        Task<MazayaPackageSubscriptionsModel> CreateOrUpdateAsync(MazayaPackageSubscriptionsModel model, IVisitor<IChangeable> auditVisitor);
        Task<MazayaPackageSubscriptionsModel> Get(int id);
        IEnumerable<MazayaPackageSubscriptionsModel> GetMazayaPackagesubscriptionsNumber();
        IQueryable<MazayaPackageSubscriptionsModel> GetMazayaPackagesubscriptionsNumberPage(QueryModel queryModel);
        Task<int> GetMazayaPackagesubscriptionsCount();
        Task<MazayaPackageSubscriptionsModel> DeleteAsync(int id);
        IQueryable<MazayaPackageSubscriptionsModel> GetAllPackagesubscriptions();
    }
}
