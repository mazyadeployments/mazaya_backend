using MMA.WebApi.Shared.Models.MazayaCategory;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MMA.WebApi.Shared.Models.MazayaSubCategory
{
    public class MazayaSubCategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public int NoofChildren { get; set; }
        public int NoofAdult { get; set; }
        public int totalcount { get; set; }
        public string optiontype { get; set; }
        public decimal vat { get; set; }
        public string Description { get; set; }
        public int MazayaCategoryId { get; set; }
        public virtual MazayaCategoryModel MazayaCategory { get; set; }
        public int sort_order { get; set; }
        public string currency { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }


    }

    public enum subcatgorytype
    {
      FAMILYMEMBERSHIP =0,
      SINGLEMEMBERSHIP =1,
      ADDMEMBERSHIP =2
    }
}
