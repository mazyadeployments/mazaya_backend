using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Models.RefreshToken;
using System.Threading.Tasks;

namespace MMA.WebApi.Services
{
    public interface IJwtService
    {
        Task<RefreshTokenModel> GetRefreshTokenModel(string refreshToken);
        Task<(string token, string refresh)> GenerateToken(ApplicationUser user, string logoutLink);
    }
}
