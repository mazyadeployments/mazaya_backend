
namespace MMA.WebApi.Shared.Models.Companies
{
    public class CompanyExcelModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int CategoryId { get; set; }
        public string MobileNumber { get; set; }
        public string LandNumber { get; set; }
        public string OfficialEmail { get; set; }
        public int OfferNumber { get; set; }
        public int ApprovedOfferNumber { get; set; }
        public int ExpiredOfferNumber { get; set; }
        public int DraftOfferNumber { get; set; }
        public string[] FocalPointMails { get; set; }
    }
}
