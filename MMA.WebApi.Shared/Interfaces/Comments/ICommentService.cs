using MMA.WebApi.Shared.Models.Comments;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Interfaces.Comments
{
    public interface ICommentService
    {
        Task<IEnumerable<CommentModel>> GetCommentsForOffer(int offerId);
    }
}
