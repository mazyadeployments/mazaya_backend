using MMA.WebApi.Shared.Models.ServiceNowModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces
{
    public interface IServiceNow
    {
        Task<IEnumerable<ServiceNowUserModel>> GetDataByMail(string email);
        Task<IEnumerable<ServiceNowUserModel>> GetDataByMailwithOAuth(string email);
    }
}
