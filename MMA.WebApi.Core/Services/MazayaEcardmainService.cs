using AutoMapper.Configuration;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.Document;
using MMA.WebApi.Shared.Interfaces.Domain.Document;
using MMA.WebApi.Shared.Interfaces.Image;
using MMA.WebApi.Shared.Interfaces.MazayaCategory;
using MMA.WebApi.Shared.Interfaces.MazayaEcardmain;
using MMA.WebApi.Shared.Interfaces.Urls;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.MazayaEcardmain;
using MMA.WebApi.Shared.Monads;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMA.WebApi.Core.Services
{
    public class MazayaEcardmainService : IMazayaEcardmainService
    {
        private readonly IMazayaEcardmainRepository _mazayaEcardmainRepository;
        public MazayaEcardmainService(IMazayaEcardmainRepository MazayaEcardmainRepository)
        {
            _mazayaEcardmainRepository = MazayaEcardmainRepository;
        }

        public async Task<Maybe<MazayaEcardmainModel>> CreateOrUpdateAsync(MazayaEcardmainModel model, string userId)
        {
            var auditVisitor = new CreateAuditVisitor(userId, DateTime.UtcNow);
            var collection = await _mazayaEcardmainRepository.CreateOrUpdateAsync(model, auditVisitor);
            return collection;
        }

        public async Task DeleteMazayaEcardmain(int id)
        {
            var ecardmainModel = await _mazayaEcardmainRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<MazayaEcardmainModel>> GetallMazayaEcardmain()
        {
            var query = _mazayaEcardmainRepository.GetAllEcardmain();
            return await Task.FromResult(query.ToList());
        }

        public async Task<Maybe<MazayaEcardmainModel>> GetMazayaEcardmain(int id)
        {
            return await _mazayaEcardmainRepository.Get(id);
        }

        public Task<PaginationListModel<MazayaEcardmainModel>> GetMazayaEcardmainPage(QueryModel queryModel)
        {
            throw new NotImplementedException();
        }
    }
}
