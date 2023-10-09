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
    public class MazayaEcardmain : IChangeable, IVisitable<IChangeable>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string invoice_number { get; set; }
        public string date { get; set; }
        public DateTime date_expire { get; set; }
        public decimal amount { get; set; }
        public string vat { get; set; }
        public decimal grandtotal { get; set; }
        public string currency { get; set; }
        public string status { get; set; }
        public int additionalcount { get; set; }
        public string subcategoryids { get; set; }
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
