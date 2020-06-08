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
        Task<CommentViewModel> CreateComment(string userId, CreateCommentDto createCommentDto);
        Task<CommentViewModel> UpdateComment(CommentViewModel commentViewModel, UpdateCommentDto updateCommentDto);
        Task<bool> DeleteComment(CommentViewModel commentViewModel);
        Task VoteComment(string commentId, string userId, CommentVoteDto commentVoteDto);
    }
}