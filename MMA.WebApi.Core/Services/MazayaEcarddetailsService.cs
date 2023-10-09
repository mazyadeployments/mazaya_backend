using AutoMapper.Configuration;
using MMA.WebApi.DataAccess.Extensions;
using MMA.WebApi.DataAccess.Repositories.MazayaCategory;
using MMA.WebApi.DataAccess.Repositories.Mazayasubcategory;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.Document;
using MMA.WebApi.Shared.Interfaces.Domain.Document;
using MMA.WebApi.Shared.Interfaces.Image;
using MMA.WebApi.Shared.Interfaces.MazayaCategory;
using MMA.WebApi.Shared.Interfaces.MazayaEcardetails;
using MMA.WebApi.Shared.Interfaces.MazayaSubCategory;
using MMA.WebApi.Shared.Interfaces.Urls;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.MazayaEcarddetail;
using MMA.WebApi.Shared.Monads;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMA.WebApi.Core.Services
{
    public class MazayaEcarddetailsService : IMazayaEcarddetailsService
    {
        private readonly IMazayaEcarddetailsRepository _mazayaecarddetailsRepository;

        public MazayaEcarddetailsService(IMazayaEcarddetailsRepository MazayaEcarddetailsRepository)
        {
            _mazayaecarddetailsRepository = MazayaEcarddetailsRepository;
        }
        public async Task<Maybe<MazayaEcarddetailsModel>> CreateOrUpdateAsync(MazayaEcarddetailsModel model, string userId)
        {
            var auditVisitor = new CreateAuditVisitor(userId, DateTime.UtcNow);
            var collection = await _mazayaecarddetailsRepository.CreateOrUpdateAsync(model, auditVisitor);
            return collection;
        }

        public async Task DeleteMazayaEcarddetails(int id)
        {
            var mazayaecarddetailsModel = await _mazayaecarddetailsRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<MazayaEcarddetailsModel>> GetMazayaEcarddetails()
        {
            var query = _mazayaecarddetailsRepository.GetMazayaEcarddetailsNumber();
            return await Task.FromResult(query.ToList());
        }

        public async Task<Maybe<MazayaEcarddetailsModel>> GetMazayaEcarddetails(int id)
        {
            return await _mazayaecarddetailsRepository.Get(id);
        }

        public async Task<PaginationListModel<MazayaEcarddetailsModel>> GetMazayaEcarddetailsPage(QueryModel queryModel)
        {
            var mazayaecarddetails = _mazayaecarddetailsRepository.GetMazayaEcarddetailsNumberPage(queryModel);
            return await mazayaecarddetails.ToPagedListAsync(queryModel.PaginationParameters.PageNumber, queryModel.PaginationParameters.PageSize);

        }
    }
}
