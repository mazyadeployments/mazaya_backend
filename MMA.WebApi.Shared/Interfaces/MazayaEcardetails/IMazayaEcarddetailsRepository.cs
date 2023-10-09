using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Models.MazayaCategory;
using MMA.WebApi.Shared.Models.MazayaEcarddetail;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.MazayaEcardetails
{
    public interface IMazayaEcarddetailsRepository
    {
        Task<MazayaEcarddetailsModel> CreateOrUpdateAsync(MazayaEcarddetailsModel model, IVisitor<IChangeable> auditVisitor);
        Task<MazayaEcarddetailsModel> Get(int id);
        IEnumerable<MazayaEcarddetailsModel> GetMazayaEcarddetailsNumber();
        IQueryable<MazayaEcarddetailsModel> GetMazayaEcarddetailsNumberPage(QueryModel queryModel);
        Task<int> GetMazayaEcarddetailsCount();
        Task<MazayaEcarddetailsModel> DeleteAsync(int id);
        IQueryable<MazayaEcarddetailsModel> GetAllEcarddetails();
    }
}
