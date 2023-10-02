using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MMA.WebApi.DataAccess.Models
{
    public class MazayaSubcategory : IChangeable, IVisitable<IChangeable>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public int NoofChildren { get; set; }
        public int NoofAdult { get; set; }
        public int totalcount { get; set; }
        public string optiontype { get; set; }
        public decimal vat { get; set; }
        public string Description { get; set; }
        [ForeignKey("MazayaCategoryId")]
        public int MazayaCategoryId { get; set; }
        public virtual MazayaCategory MazayaCategory { get; set; }
        public string currency { get; set; }
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
