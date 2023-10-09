using MMA.WebApi.Shared.Models.CompanyLocations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.Companies
{
    public interface ICompanyLocationsService
    {
        Task<IEnumerable<CompanyLocationModel>> GetAll();
    }
}
