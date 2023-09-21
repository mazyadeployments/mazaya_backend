using MMA.WebApi.Shared.Models.ApplicationUser;
using MMA.WebApi.Shared.Models.Attachment;
using MMA.WebApi.Shared.Models.Category;
using System;
using System.Collections.Generic;

namespace MMA.WebApi.Shared.Models.Companies
{
    public class CompanyModel
    {
        public IEnumerable<CompanyPartnerModel> CompanyPartners { get; set; } =
            new List<CompanyPartnerModel>();
        public IEnumerable<CompanyActivityModel> CompanyActivities { get; set; } =
            new List<CompanyActivityModel>();
        public IEnumerable<CompanyLocationModel> CompanyLocations { get; set; } =
            new List<CompanyLocationModel>();
        public IEnumerable<ApplicationUserModel> Suppliers { get; set; } =
            new List<ApplicationUserModel>();
        public IEnumerable<ApplicationUserModel> SupplierAdmins { get; set; } =
            new List<ApplicationUserModel>();
        public IEnumerable<string> CompanyPartnersList { get; set; } = new List<string>();
        public IEnumerable<string> CompanyActivitiesList { get; set; } = new List<string>();
        public int Id { get; set; }
        public string NameEnglish { get; set; }
        public string NameArabic { get; set; }
        public string Website { get; set; }
        public string OfficialEmail { get; set; }

        //public string MobileCountryCode { get; set; }
        //public string MobileE164Number { get; set; }
        //public string MobileInternationalNumber { get; set; }
        //public string LandCountryCode { get; set; }
        //public string LandE164Number { get; set; }
        //public string LandInternationalNumber { get; set; }
        //public string FaxCountryCode { get; set; }
        //public string FaxE164Number { get; set; }
        //public string FaxInternationalNumber { get; set; }
        public PhoneNumberModel Mobile { get; set; }
        public PhoneNumberModel Fax { get; set; }
        public PhoneNumberModel Land { get; set; }
        public string Logo { get; set; }
        public string CompanyDescription { get; set; }
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
        public string ApproveStatus { get; set; }
        public DateTime TradeLicenceExpiryDate { get; set; }
        public AttachmentModel TradeLicence { get; set; }
        public IEnumerable<CategoryModel> Categories { get; set; }
    }

    public class PhoneNumberModel
    {
        public string CountryCode { get; set; }
        public string E164Number { get; set; }
        public string InternationalNumber { get; set; }
        public string Number { get; set; }
    }
}
