using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Models.MazayaEcarddetail;
using MMA.WebApi.Shared.Models.MazayaEcardmain;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.MazayaEcardmain
{
    public interface IMazayaEcardmainRepository
    {
        Task<MazayaEcardmainModel> CreateOrUpdateAsync(MazayaEcardmainModel model, IVisitor<IChangeable> auditVisitor);
        Task<MazayaEcardmainModel> Get(int id);
        IEnumerable<MazayaEcardmainModel> GetMazayaEcardmainNumber();
        IQueryable<MazayaEcardmainModel> GetMazayaEcardmainNumberPage(QueryModel queryModel);
        Task<int> GetMazayaEcardmainCount();
        Task<MazayaEcardmainModel> DeleteAsync(int id);
        IQueryable<MazayaEcardmainModel> GetAllEcardmain();
    }
}
