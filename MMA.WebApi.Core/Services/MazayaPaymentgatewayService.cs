using Microsoft.Extensions.Configuration;
using MMA.WebApi.DataAccess.Extensions;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.MazayaPaymentgateway;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.MazayaPaymentgateway;
using MMA.WebApi.Shared.Monads;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMA.WebApi.Core.Services
{
    public class MazayaPaymentgatewayService : IMazayaPaymentgatewayService
    {
        private readonly IMazayaPaymentgatewayRepository _mazayapaymentgatewayRepository;
        private readonly IConfiguration _configuration;

        public MazayaPaymentgatewayService(IMazayaPaymentgatewayRepository MazayapaymentgatewayRepository, IConfiguration configuration)
        {
            _mazayapaymentgatewayRepository = MazayapaymentgatewayRepository;
            _configuration = configuration;
        }

        public async Task<Maybe<MazayaPaymentgatewayModel>> CreateOrUpdateAsync(MazayaPaymentgatewayModel model, string userId)
        {
            var auditVisitor = new CreateAuditVisitor(userId, DateTime.UtcNow);
            var collection = await _mazayapaymentgatewayRepository.CreateOrUpdateAsync(model, auditVisitor);
            return collection;
        }

        public async Task DeleteMazayaPaymentgateway(int id)
        {
            var categoryModel = await _mazayapaymentgatewayRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<MazayaPaymentgatewayModel>> GetMazayaPaymentgateways()
        {
            var query = _mazayapaymentgatewayRepository.GetMazayaPaymentgatewayNumber();
            return await Task.FromResult(query.ToList());
        }

        public async Task<Maybe<MazayaPaymentgatewayModel>> GetMazayaPaymentgateway(int id)
        {
            return await _mazayapaymentgatewayRepository.Get(id);
        }

        public async Task<PaginationListModel<MazayaPaymentgatewayModel>> GetMazayaPaymentgatewaypage(QueryModel queryModel)
        {
            var paymentgateway = _mazayapaymentgatewayRepository.GetMazayaPaymentgatewayNumberPage(queryModel);
            return await paymentgateway.ToPagedListAsync(queryModel.PaginationParameters.PageNumber, queryModel.PaginationParameters.PageSize);
        }
    }
}
