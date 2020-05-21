using System.Collections.Generic;
using System.Threading.Tasks;
using Violetum.ApplicationCore.Dtos.Comment;
using Violetum.ApplicationCore.Dtos.Post;
using Violetum.ApplicationCore.ViewModels;
using Violetum.Domain.Entities;

namespace Violetum.ApplicationCore.Interfaces
{
    public interface ICommentService
    {
        CommentViewModel GetComment(string commentId);
        Task<IEnumerable<CommentViewModel>> GetComments(SearchParams searchParams, Paginator paginator);
        Task<CommentViewModel> CreateComment(CommentDto commentDto);
        Task<CommentViewModel> UpdateComment(string commentId, string userId, UpdateCommentDto updateCommentDto);
        Task<bool> DeleteComment(string commentId, string userId, DeleteCommentDto deleteCommentDto);
        Task VoteComment(string commentId, string userId, CommentVoteDto commentVoteDto);
        int GetCommentVoteSum(string commentId);
    }
}