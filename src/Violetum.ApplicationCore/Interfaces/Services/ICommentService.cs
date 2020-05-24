using System.Collections.Generic;
using System.Threading.Tasks;
using Violetum.ApplicationCore.Dtos.Comment;
using Violetum.ApplicationCore.ViewModels.Comment;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.ApplicationCore.Interfaces.Services
{
    public interface ICommentService
    {
        CommentViewModel GetComment(string commentId);
        Task<IEnumerable<CommentViewModel>> GetComments(CommentSearchParams searchParams);
        Task<int> GetTotalCommentsCount(CommentSearchParams searchParams);
        Task<CommentViewModel> CreateComment(CommentDto commentDto);
        Task<CommentViewModel> UpdateComment(string commentId, string userId, UpdateCommentDto updateCommentDto);
        Task<bool> DeleteComment(string commentId, string userId);
        Task VoteComment(string commentId, string userId, CommentVoteDto commentVoteDto);
    }
}