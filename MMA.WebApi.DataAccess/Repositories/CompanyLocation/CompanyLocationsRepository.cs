using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Interfaces.CompanyLocations;
using MMA.WebApi.Shared.Models.CompanyLocations;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace MMA.WebApi.DataAccess.Country
{
    public class CompanyLocationsRepository : BaseRepository<CompanyLocationModel>, ICompanyLocationsRepository
    {
        public CompanyLocationsRepository(Func<MMADbContext> contexFactory) : base(contexFactory) { }

        public IQueryable<CompanyLocationModel> Get()
        {
            return GetEntities();
        }

        protected override IQueryable<CompanyLocationModel> GetEntities()
        {
            var context = ContextFactory();
            return from c in context.CompanyLocation

                   select new CompanyLocationModel
                   {
                       Id = c.Id,
                       Address = c.Address,
                       Country = c.Country,
                       Latitude = c.Latitude,
                       Longitude = c.Longitude,
                       Vicinity = c.Vicinity
                   };
        }

        private Expression<Func<CompanyLocation, CompanyLocationModel>> projectToCompanyLocationModel = data =>
          new CompanyLocationModel()
          {
              Address = data.Address,
              CompanyId = data.CompanyId,
              Country = data.Country,
              Id = data.Id,
              Latitude = data.Latitude,
              Longitude = data.Longitude,
              Vicinity = data.Vicinity
          };
    }
}
