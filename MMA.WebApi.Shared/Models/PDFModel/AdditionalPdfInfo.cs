using MMA.WebApi.Shared.Models.ApplicationUser;
using MMA.WebApi.Shared.Models.Companies;
using System.Collections.Generic;

namespace MMA.WebApi.Shared.Models.PDFModel
{
    public class AdditionalPdfInfo
    {
        public int Id { get; set; }
        public IEnumerable<ApplicationUserModel> FocalPoints { get; set; }
        public IEnumerable<CompanyLocationModel> CompanyLocations { get; set; }
    }
}
