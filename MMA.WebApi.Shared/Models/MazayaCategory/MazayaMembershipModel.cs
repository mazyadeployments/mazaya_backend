using MMA.WebApi.Shared.Models.MazayaSubCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Models.MazayaCategory
{
    public class MazayaMembershipModel
    {
        public List<MazayaCategoryModel> mazayaCategories { get; set; }
        public List<MazayaSubCategoryModel> mazayaSubCategories { get; set; }
    }
}
