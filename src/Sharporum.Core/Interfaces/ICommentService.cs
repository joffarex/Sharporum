using System.Collections.Generic;
using System.Threading.Tasks;
using Sharporum.Core.Dtos.Comment;
using Sharporum.Core.ViewModels.Comment;
using Sharporum.Domain.Entities;
using Sharporum.Domain.Models.SearchParams;

namespace Sharporum.Core.Interfaces
{
    public interface ICommentService
    {
        Task<CommentViewModel> GetCommentByIdAsync(string commentId);
        Task<Comment> GetCommentEntityAsync(string commentId);
        Task<IEnumerable<CommentViewModel>> GetCommentsAsync(CommentSearchParams searchParams);
        Task<int> GetCommentsCountAsync(CommentSearchParams searchParams);
        Task<string> CreateCommentAsync(string userId, CreateCommentDto createCommentDto);
        Task<CommentViewModel> UpdateCommentAsync(Comment comment, UpdateCommentDto updateCommentDto);
        Task DeleteCommentAsync(Comment comment);
        Task VoteCommentAsync(string commentId, string userId, CommentVoteDto commentVoteDto);
    }
}