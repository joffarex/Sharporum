using System.Collections.Generic;
using System.Threading.Tasks;
using Violetum.ApplicationCore.Dtos.Comment;
using Violetum.ApplicationCore.ViewModels.Comment;
using Violetum.Domain.Entities;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.ApplicationCore.Interfaces.Services
{
    public interface ICommentService
    {
        CommentViewModel GetComment(string commentId);
        Comment GetCommentEntity(string commentId);
        Task<IEnumerable<CommentViewModel>> GetComments(CommentSearchParams searchParams);
        Task<int> GetTotalCommentsCount(CommentSearchParams searchParams);
        Task<string> CreateComment(string userId, CreateCommentDto createCommentDto);
        Task<CommentViewModel> UpdateComment(Comment comment, UpdateCommentDto updateCommentDto);
        Task DeleteComment(Comment comment);
        Task VoteComment(string commentId, string userId, CommentVoteDto commentVoteDto);
    }
}