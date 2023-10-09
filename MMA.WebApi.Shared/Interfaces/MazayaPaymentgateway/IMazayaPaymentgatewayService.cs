using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.MazayaPaymentgateway;
using MMA.WebApi.Shared.Monads;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.MazayaPaymentgateway
{
    public interface IMazayaPaymentgatewayService
    {
        Task<IEnumerable<MazayaPaymentgatewayModel>> GetMazayaPaymentgateways();

        Task<Maybe<MazayaPaymentgatewayModel>> GetMazayaPaymentgateway(int id);
        Task<PaginationListModel<MazayaPaymentgatewayModel>> GetMazayaPaymentgatewaypage(QueryModel queryModel);

        Task<Maybe<MazayaPaymentgatewayModel>> CreateOrUpdateAsync(MazayaPaymentgatewayModel model, string userId);

        Task DeleteMazayaPaymentgateway(int id);
    }
}
