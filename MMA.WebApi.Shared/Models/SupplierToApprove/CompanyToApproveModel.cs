namespace MMA.WebApi.Shared.Models.SupplierToApprove
{
    public class CompanyToApproveModel
    {
        public int CompanyId { get; set; }
        public string SupplierId { get; set; }
        public bool Approved { get; set; }
    }
}
