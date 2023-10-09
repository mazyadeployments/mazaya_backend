using MMA.WebApi.Shared.Interfaces.RefreshToken;
using MMA.WebApi.Shared.Models.RefreshToken;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace MMA.WebApi.Core.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRefreshTokenRepository _repo;
        public RefreshTokenService(IRefreshTokenRepository repo)
        {
            _repo = repo;
        }

        public async Task<RefreshTokenModel> GetAsync(int id)
        {
            return await _repo.GetSingleAsync(x => x.Id.Equals(id));
        }

        public async Task<RefreshTokenModel> GetRefreshToken(string refreshToken)
        {
            return await _repo.GetSingleAsync(x => x.Refreshtoken == refreshToken);
        }

        public async Task<RefreshTokenModel> SaveAsync(RefreshTokenModel data)
        {
            var refreshTokenIdExists = await _repo.GetSingleAsync(x => x.Username.Equals(data.Username));

            data.Refreshtoken = GenerateRefreshToken();

            if (refreshTokenIdExists != null)
            {
                data.Id = refreshTokenIdExists.Id;
                data.Id = await _repo.EditAsync(data);
                return data;
            }
            else
            {

                data.Id = await _repo.InsertAsync(data);
                return data;
            }
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}
