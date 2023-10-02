using Microsoft.Extensions.Configuration;
using MMA.WebApi.DataAccess.Extensions;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.MazayaSubCategory;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.MazayaSubCategory;
using MMA.WebApi.Shared.Monads;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMA.WebApi.Core.Services
{
    public class MazayaSubcategoryService : IMazayaSubCategoryService
    {
        private readonly IMazayaSubcategoryRepository _mazayasubcategoryRepository;
        private readonly IConfiguration _configuration;

        public MazayaSubcategoryService(IMazayaSubcategoryRepository MazayasubCategoryRepository, IConfiguration configuration)
        {
            _mazayasubcategoryRepository = MazayasubCategoryRepository;
            _configuration = configuration;
        }


        public async Task<Maybe<MazayaSubCategoryModel>> CreateOrUpdateAsync(MazayaSubCategoryModel model, string userId)
        {
            var auditVisitor = new CreateAuditVisitor(userId, DateTime.UtcNow);
            var collection = await _mazayasubcategoryRepository.CreateOrUpdateAsync(model, auditVisitor);
            return collection;
        }

        public async Task DeleteMazayasubCategory(int id)
        {
            var categoryModel = await _mazayasubcategoryRepository.DeleteAsync(id);

        }

        public async Task<IEnumerable<MazayaSubCategoryModel>> GetallMembeship()
        {
            var query = _mazayasubcategoryRepository.Getallmembership();
            return await Task.FromResult(query.ToList());
        }

        public async Task<IEnumerable<MazayaSubCategoryModel>> GetMazayasubCategories()
        {
            var query = _mazayasubcategoryRepository.GetMazayasubCategoriesNumber();
            return await Task.FromResult(query.ToList());
        }

        public async Task<PaginationListModel<MazayaSubCategoryModel>> GetMazayasubCategoriesPage(QueryModel queryModel)
        {
            var categories = _mazayasubcategoryRepository.GetMazayasubCategoriesNumberPage(queryModel);
            return await categories.ToPagedListAsync(queryModel.PaginationParameters.PageNumber, queryModel.PaginationParameters.PageSize);
        }

        public async Task<Maybe<MazayaSubCategoryModel>> GetMazayasubCategory(int id)
        {
            return await _mazayasubcategoryRepository.Get(id);
        }
    }
}
