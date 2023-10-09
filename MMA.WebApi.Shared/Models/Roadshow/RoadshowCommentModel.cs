using MMA.WebApi.Shared.Models.Comments;
using System;

namespace MMA.WebApi.Shared.Models.Roadshow
{
    public class RoadshowCommentModel : CommentBaseModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? RoadshowId { get; set; }
    }
}