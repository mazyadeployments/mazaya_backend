using Microsoft.Extensions.Configuration;
using MMA.WebApi.DataAccess.Extensions;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.MazayaPackageSubscriptions;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.MazayaPackagesubscriptionsModel;
using MMA.WebApi.Shared.Monads;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMA.WebApi.Core.Services
{
    public class MazayaPackagesubscriptionsService : IMazayaPackagesubscriptionsService
    {
        private readonly IMazayaPackagesubscriptionsRepository _mazayapackagesubscriptionsRepository;
        private readonly IConfiguration _configuration;

        public MazayaPackagesubscriptionsService(IMazayaPackagesubscriptionsRepository MazayapackagesubscriptionsRepository, IConfiguration configuration)
        {
            _mazayapackagesubscriptionsRepository = MazayapackagesubscriptionsRepository;
            _configuration = configuration;
        }

        public async Task<Maybe<MazayaPackageSubscriptionsModel>> CreateOrUpdateAsync(MazayaPackageSubscriptionsModel model, string userId)
        {
            var auditVisitor = new CreateAuditVisitor(userId, DateTime.UtcNow);
            var collection = await _mazayapackagesubscriptionsRepository.CreateOrUpdateAsync(model, auditVisitor);
            return collection;
        }

        public async Task DeleteMazayaPackagesubscriptions(int id)
        {
            var categoryModel = await _mazayapackagesubscriptionsRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<MazayaPackageSubscriptionsModel>> GetMazayaPackagesubscriptions()
        {
            var query = _mazayapackagesubscriptionsRepository.GetMazayaPackagesubscriptionsNumber();
            return await Task.FromResult(query.ToList());
        }

        public async Task<Maybe<MazayaPackageSubscriptionsModel>> GetMazayaPackagesubscription(int id)
        {
            return await _mazayapackagesubscriptionsRepository.Get(id);
        }

        public async Task<PaginationListModel<MazayaPackageSubscriptionsModel>> GetMazayaPackagesubscriptionspage(QueryModel queryModel)
        {
            var paymentgateway = _mazayapackagesubscriptionsRepository.GetMazayaPackagesubscriptionsNumberPage(queryModel);
            return await paymentgateway.ToPagedListAsync(queryModel.PaginationParameters.PageNumber, queryModel.PaginationParameters.PageSize);
        }
    }
}
