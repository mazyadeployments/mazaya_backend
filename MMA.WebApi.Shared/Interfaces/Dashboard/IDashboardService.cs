using MMA.WebApi.Shared.Models.Dashboard;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.Dashboard
{
    public interface IDashboardService
    {
        Task<IEnumerable<DashboardModel>> GetDashboardItems(string userId);
    }
}
