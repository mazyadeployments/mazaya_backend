using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Models.Comments;

namespace MMA.WebApi.Shared.Interfaces.Comments
{
    public interface ICommentRepository : IQueryableRepository<CommentModel>
    {
    }
}
