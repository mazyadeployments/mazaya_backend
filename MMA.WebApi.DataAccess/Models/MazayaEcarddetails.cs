using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMA.WebApi.DataAccess.Models
{
    public class MazayaEcarddetails : IChangeable, IVisitable<IChangeable>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public DateTime dob { get; set; }
        public string relation { get; set; }
        public string card_number { get; set; }
        public string status { get; set; }
        public string profile_img { get; set; }

        [ForeignKey("MazayaEcardmainid")]
        public int MazayaEcardmainid { get; set; }
        public virtual MazayaEcardmain MazayaEcardmain { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public void Accept(IVisitor<IChangeable> visitor)
        {
            if (visitor != null)
            {
                visitor.Visit(this);
            }
        }

    }
}
