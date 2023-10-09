using MMA.WebApi.Shared.Models.MazayaEcardmain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Models.MazayaEcarddetail
{
    public class MazayaEcarddetailsModel
    {
        public int id { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string dob { get; set; }
        public string relation { get; set; }
        public string card_number { get; set; }
        public string status { get; set; }
        public string profile_image { get; set; }
        public int MazayaEcardmainId { get; set; }
        public virtual MazayaEcardmainModel MazayaEcardmainModel { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
    }
}
