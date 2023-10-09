using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Models.CompanyLocations;

namespace MMA.WebApi.Shared.Interfaces.CompanyLocations
{
    public interface ICompanyLocationsRepository : IQueryableRepository<CompanyLocationModel>
    {
    }
}
