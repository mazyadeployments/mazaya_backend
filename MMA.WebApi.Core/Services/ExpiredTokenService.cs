using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MMA.WebApi.Shared.Interfaces.ExpiredToken;
using MMA.WebApi.Shared.Models.ExpiredToken;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MMA.WebApi.Core.Services
{
    public class ExpiredTokenService : IExpiredTokenService
    {
        private readonly IExpiredTokenRepository _expiredTokenRepository;
        private readonly IConfiguration _config;

        public ExpiredTokenService(
            IConfiguration config,
            IExpiredTokenRepository expiredTokenRepository
        )
        {
            _config = config;
            _expiredTokenRepository = expiredTokenRepository;
        }

        public Task<int> AddTokenToExpired(string token, string userId)
        {
            int tokenExpireInMintues = GetTokenExpireTime();
            return _expiredTokenRepository.InsertAsync(
                new Shared.Models.ExpiredToken.ExpiredTokenModel
                {
                    ExpiredAt = DateTime.UtcNow.AddMinutes(tokenExpireInMintues),
                    UserId = userId,
                    Token = token
                }
            );
        }

        private int GetTokenExpireTime() =>
            _config["AccessTokenExpireInMinutes"] != null
                ? int.Parse(_config["AccessTokenExpireInMinutes"])
                : 0;

        public async Task DeleteAsync(int id)
        {
            await _expiredTokenRepository.DeleteAsync(id);
        }

        public Task<int> EditAsync(ExpiredTokenModel data)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ExpiredTokenModel>> GetListAsync(
            Expression<Func<ExpiredTokenModel, bool>> query = null
        )
        {
            throw new NotImplementedException();
        }

        public Task<ExpiredTokenModel> GetSingleAsync(
            Expression<Func<ExpiredTokenModel, bool>> query
        )
        {
            throw new NotImplementedException();
        }

        public Task<int> InsertAsync(ExpiredTokenModel data)
        {
            return _expiredTokenRepository.InsertAsync(data);
        }

        public async Task DeleteExpiredTokens(ILogger logger)
        {
            logger.LogInformation($"CheckExpiredTokens in service -> before call to repo");
            await _expiredTokenRepository.DeleteExpiredTokens(logger);
            logger.LogInformation($"CheckExpiredTokens in service -> after call to repo");
        }

        public async Task<bool> IsTokenExpired(string token, string userId)
        {
            try
            {
                ExpiredTokenModel expiredToken = await this._expiredTokenRepository.GetSingleAsync(
                    expToken => expToken.UserId == userId && expToken.Token == token
                );
                if (expiredToken != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return true;
            }
        }
    }
}
