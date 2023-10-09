using MMA.WebApi.Shared.Models.MazayaEcarddetail;
using MMA.WebApi.Shared.Models.MazayaSubCategory;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Models.MazayaEcardmain
{
    public class MazayaEcardmainModel
    {
        public int id { get; set; }
        public string invoice_number { get; set; }
        public string date { get; set; }
        public DateTime date_expire { get; set; }
        public decimal amount { get; set; }
        public string vat { get; set; }
        public decimal grandtotal { get; set; }
        public string currency { get; set; }
        public string status { get; set; }
        public string subcategoryids { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
    }

    public class MazayaEcardModel
    {
        public MazayaEcardmainModel mazayaecardmain { get; set;}
        public List<MazayaEcarddetailsModel> mazayaecarddetails { get; set;}
        public int additional_count { get; set; }   
    }
}
