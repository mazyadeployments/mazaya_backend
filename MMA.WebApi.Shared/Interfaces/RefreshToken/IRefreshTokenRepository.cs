using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Models.RefreshToken;

namespace MMA.WebApi.Shared.Interfaces.RefreshToken
{
    public interface IRefreshTokenRepository : IInsertableAsync<int, RefreshTokenModel>,
        IEditableAsync<int, RefreshTokenModel>,
        ISearchableAsync<RefreshTokenModel>,
        IQueryableRepository<RefreshTokenModel>
    {
    }
}
