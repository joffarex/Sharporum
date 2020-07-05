using System.Collections.Generic;
using System.Threading.Tasks;
using Violetum.ApplicationCore.Dtos.Comment;
using Violetum.ApplicationCore.ViewModels.Comment;
using Violetum.Domain.Entities;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.ApplicationCore.Interfaces
{
    public interface ICommentService
    {
        Task<CommentViewModel> GetComment(string commentId);
        Task<Comment> GetCommentEntity(string commentId);
        Task<IEnumerable<CommentViewModel>> GetCommentsAsync(CommentSearchParams searchParams);
        Task<int> GetCommentsCountAsync(CommentSearchParams searchParams);
        Task<string> CreateCommentAsync(string userId, CreateCommentDto createCommentDto);
        Task<CommentViewModel> UpdateCommentAsync(Comment comment, UpdateCommentDto updateCommentDto);
        Task DeleteCommentAsync(Comment comment);
        Task VoteCommentAsync(string commentId, string userId, CommentVoteDto commentVoteDto);
    }
}