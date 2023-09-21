using MMA.WebApi.Shared.Models.Companies;
using MMA.WebApi.Shared.Models.Document;
using System;

namespace MMA.WebApi.Shared.Models.CompaniesDocument
{
    public class CompanyDocumentModel
    {
        public int Id { get; set; }

        public int CompanyId { get; set; }

        public virtual CompanyModel CompanyModel { get; set; }

        public Guid DocumentId { get; set; }

        public virtual DocumentFileModel Document { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime UpdatedOn { get; set; }

        public string UpdatedBy { get; set; }
    }
}
