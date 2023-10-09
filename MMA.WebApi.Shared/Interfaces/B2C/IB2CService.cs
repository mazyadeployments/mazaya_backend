using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Models.B2C;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.B2C
{
    public interface IB2CService
    {
        Task<JwtSecurityToken> GetJwtSecurityToken(string code, Declares.Roles role);
        JwtSecurityToken DecodeJWTToken(string token);
        Task<B2CLoginModel> RegisterBuyer(string email, string firstName, string lastName, string mobileNumber);
        Task<B2CLoginModel> LoginBuyer(string username);
        Task<B2CLoginModel> RegisterSupplier(string email, string companyDescription, string companyName, string mobileNumber);
        Task<B2CLoginModel> LoginSupplier(string username);
    }
}
