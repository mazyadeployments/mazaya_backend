using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Interfaces.Dashboard;
using MMA.WebApi.Shared.Models.Dashboard;
using System;
using System.Linq;

namespace MMA.WebApi.DataAccess.Repositories.Dashboard
{
    public class DashboardRepository : BaseRepository<DashboardModel>, IDashboardRepository
    {
        public DashboardRepository(Func<MMADbContext> contexFactory) : base(contexFactory) { }

        protected override IQueryable<DashboardModel> GetEntities()
        {
            throw new NotImplementedException();
        }
        public IQueryable<DashboardModel> Get()
        {
            throw new NotImplementedException();
        }
    }
}
