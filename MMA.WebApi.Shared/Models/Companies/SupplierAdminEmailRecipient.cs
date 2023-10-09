using MMA.WebApi.Shared.Models.ApplicationUser;

namespace MMA.WebApi.Shared.Models.Companies
{
    public class SupplierAdminEmailRecipient
    {
        public ApplicationUserModel SupplierAdmin { get; set; }
        public bool OffersForRoadshowAccepted { get; set; }
    }
}
