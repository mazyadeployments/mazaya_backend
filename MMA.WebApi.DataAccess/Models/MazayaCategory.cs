using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MMA.WebApi.DataAccess.Models
{
    public class MazayaCategory : IChangeable, IVisitable<IChangeable>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string description { get; set; } 
        public string facilities { get; set; }
        public string image { get; set; }
        public bool status { get; set; }
        public virtual ICollection<MazayacategoryDocument> mazayaCategoryDocuments { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public int sort_order { get; set; }

        public void Accept(IVisitor<IChangeable> visitor)
        {
            if (visitor != null)
            {
                visitor.Visit(this);
            }
        }
    }
}
