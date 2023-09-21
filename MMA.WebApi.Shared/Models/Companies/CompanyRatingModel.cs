using MMA.WebApi.Shared.Models.Comments;
using System.Collections.Generic;

namespace MMA.WebApi.Shared.Models.Companies
{
    public class CompanyRatingModel
    {
        public decimal AverageRating { get; set; }
        public int TotalRatings { get; set; }
        public int TotalSupplierRatings { get; set; }
        public decimal AverageSupplierRating { get; set; }
        public virtual IList<CommentModel> Comments { get; set; }
    }
}
