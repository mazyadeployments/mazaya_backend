using MMA.WebApi.Shared.Models.MazayaSubCategory;
using System;
using System.Collections.Generic;
using System.Text;

namespace MMA.WebApi.Shared.Models.MazayaPackagesubscriptionsModel
{
    public class MazayaPackageSubscriptionsModel
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public int SubCategoryId { get; set; }
        public virtual MazayaSubCategoryModel MazayaSubCategory { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
    }
}
