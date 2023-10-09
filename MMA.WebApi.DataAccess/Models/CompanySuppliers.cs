namespace MMA.WebApi.DataAccess.Models
{
    public class CompanySuppliers
    {
        public int CompanyId { get; set; }
        public Company Company { get; set; }
        public string SupplierId { get; set; }
        public ApplicationUser Supplier { get; set; }
    }
}
