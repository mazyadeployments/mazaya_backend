using Microsoft.Extensions.Logging;
using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Models.ExpiredToken;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.ExpiredToken
{
    public interface IExpiredTokenService : ICrudAsync<int, ExpiredTokenModel>
    {
        Task<int> AddTokenToExpired(string token, string userId);
        Task<bool> IsTokenExpired(string token, string userId);
        Task DeleteExpiredTokens(ILogger logger);
    }
}
