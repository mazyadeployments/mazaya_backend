using MMA.WebApi.Shared.Interfaces.Companies;
using MMA.WebApi.Shared.Interfaces.CompanyLocations;
using MMA.WebApi.Shared.Models.CompanyLocations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMA.WebApi.Core.Services
{
    public class CompanyLocationsService : ICompanyLocationsService
    {
        private readonly ICompanyLocationsRepository _companyLocationsRepository;

        public CompanyLocationsService(ICompanyLocationsRepository companyLocationsRepository)
        {
            _companyLocationsRepository = companyLocationsRepository;
        }

        public async Task<IEnumerable<CompanyLocationModel>> GetAll()
        {
            var companyModels = await Task.FromResult(_companyLocationsRepository.Get().ToList());
            return companyModels;
        }
    }
}
