using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MMA.WebApi.DataAccess.Models
{
    public class MazayaPackageSubscription : IChangeable, IVisitable<IChangeable>
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser Owner { get; set; }
        public int SubCategoryId { get; set; }

        [ForeignKey("SubCategoryId")]
        public virtual MazayaSubcategory MazayaSubCategory { get; set; }
        public decimal Amount { get; set; }
        public DateTime date { get; set; }
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
