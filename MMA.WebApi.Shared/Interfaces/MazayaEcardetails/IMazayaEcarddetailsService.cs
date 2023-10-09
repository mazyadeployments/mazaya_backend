using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.MazayaCategory;
using MMA.WebApi.Shared.Models.MazayaEcarddetail;
using MMA.WebApi.Shared.Monads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.MazayaEcardetails
{
    public interface IMazayaEcarddetailsService
    {
        Task<IEnumerable<MazayaEcarddetailsModel>> GetMazayaEcarddetails();

        Task<Maybe<MazayaEcarddetailsModel>> GetMazayaEcarddetails(int id);
        Task<PaginationListModel<MazayaEcarddetailsModel>> GetMazayaEcarddetailsPage(QueryModel queryModel);

        Task<Maybe<MazayaEcarddetailsModel>> CreateOrUpdateAsync(MazayaEcarddetailsModel model, string userId);

        Task DeleteMazayaEcarddetails(int id);
    }
}
