using System;

namespace MMA.WebApi.Shared.Models.Comments
{
    public class CommentModel : CommentBaseModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime CreatedOn { get; set; }
        public int OfferId { get; set; }
        public decimal? Rating { get; set; }
    }
}
