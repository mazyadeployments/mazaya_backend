using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MMA.WebApi.DataAccess.Models
{
    public class Company : IChangeable, IVisitable<IChangeable>
    {
        public IEnumerable<CompanyActivity> CompanyActivities { get; set; } =
            new List<CompanyActivity>();
        public IEnumerable<CompanyLocation> CompanyLocations { get; set; } =
            new List<CompanyLocation>();
        public IEnumerable<CompanyPartner> CompanyPartners { get; set; } =
            new List<CompanyPartner>();
        public ICollection<CompanySuppliers> CompanySuppliers { get; set; } =
            new List<CompanySuppliers>();
        public ICollection<CompanyCategory> CompanyCategories { get; set; } =
            new List<CompanyCategory>();
        public ICollection<Offer> Offers { get; set; } = new List<Offer>();
        public int Id { get; set; }
        public string NameEnglish { get; set; }
        public string NameArabic { get; set; }
        public string Website { get; set; }
        public string OfficialEmail { get; set; }
        public string CompanyDescription { get; set; }
        public string MobileCountryCode { get; set; }
        public string MobileE164Number { get; set; }
        public string Mobile { get; set; }
        public string MobileNumber { get; set; }
        public string LandCountryCode { get; set; }
        public string LandE164Number { get; set; }
        public string Land { get; set; }
        public string LandNumber { get; set; }
        public string FaxCountryCode { get; set; }
        public string FaxE164Number { get; set; }
        public string Fax { get; set; }
        public string FaxNumber { get; set; }
        public CompanyDocument Logo { get; set; }
        public string LegalForm { get; set; }
        public string POBox { get; set; }
        public string Facebook { get; set; }
        public string Instagram { get; set; }
        public string Twitter { get; set; }
        public string IDforADCCI { get; set; }
        public string LicenseNo { get; set; }
        public string CompanyNationality { get; set; }
        public DateTime EstablishDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }

        // enum SupplierStatus
        public string ApproveStatus { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime ApprovedOn { get; set; }
        public DateTime TradeLicenseExpDate { get; set; }
        public Guid? TradeLicenceId { get; set; }

        [ForeignKey("TradeLicenceId")]
        public virtual Document TradeLicence { get; set; }

        public void Accept(IVisitor<IChangeable> visitor)
        {
            if (visitor != null)
            {
                visitor.Visit(this);
            }
        }
    }
}
