using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Models.MazayaPaymentgateway;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.MazayaPaymentgateway
{
    public interface IMazayaPaymentgatewayRepository
    {
        Task<MazayaPaymentgatewayModel> CreateOrUpdateAsync(MazayaPaymentgatewayModel model, IVisitor<IChangeable> auditVisitor);
        Task<MazayaPaymentgatewayModel> Get(int id);
        IEnumerable<MazayaPaymentgatewayModel> GetMazayaPaymentgatewayNumber();
        IQueryable<MazayaPaymentgatewayModel> GetMazayaPaymentgatewayNumberPage(QueryModel queryModel);
        Task<int> GetMazayaPaymentgatewayCount();
        Task<MazayaPaymentgatewayModel> DeleteAsync(int id);
        IQueryable<MazayaPaymentgatewayModel> GetAllPaymentgateway();
    }
}
