using MMA.WebApi.Shared.Models.Image;

namespace MMA.WebApi.Shared.Models.Companies
{
    public class SupplierModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string CompanyName { get; set; }
        public int CompanyId { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsAdmin { get; set; }
        public ImageUrlsModel ImageUrls { get; set; }
    }
}
