using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.MazayaCategory;
using MMA.WebApi.Shared.Models.MazayaEcardmain;
using MMA.WebApi.Shared.Monads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.MazayaEcardmain
{
    public interface IMazayaEcardmainService
    {
        Task<IEnumerable<MazayaEcardmainModel>> GetallMazayaEcardmain();
        Task<Maybe<MazayaEcardmainModel>> GetMazayaEcardmain(int id);
        Task<PaginationListModel<MazayaEcardmainModel>> GetMazayaEcardmainPage(QueryModel queryModel);
        Task<Maybe<MazayaEcardmainModel>> CreateOrUpdateAsync(MazayaEcardmainModel model, string userId);
        Task DeleteMazayaEcardmain(int id);
    }
}
