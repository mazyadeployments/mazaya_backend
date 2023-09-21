using MMA.WebApi.Shared.Interfaces.Comments;
using MMA.WebApi.Shared.Models.Comments;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Core.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        public CommentService(ICommentRepository CategoryRepository)
        {
            _commentRepository = CategoryRepository;
        }

        public Task<IEnumerable<CommentModel>> GetCommentsForOffer(int offerId)
        {
            throw new System.NotImplementedException();
        }
    }
}
