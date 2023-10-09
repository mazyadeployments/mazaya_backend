using Microsoft.Extensions.Logging;
using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Models.ExpiredToken;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.ExpiredToken
{
    public interface IExpiredTokenRepository : ICrudAsync<int, ExpiredTokenModel>
    {
        Task DeleteExpiredTokens(ILogger logger);
    }
}
