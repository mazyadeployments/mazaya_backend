using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Models.RefreshToken;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.RefreshToken
{
    public interface IRefreshTokenService : IReadableAsync<int, RefreshTokenModel>,
        ISaveableAsync<RefreshTokenModel>
    {
        Task<RefreshTokenModel> GetRefreshToken(string refreshToken);


    }
}
