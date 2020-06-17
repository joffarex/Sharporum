using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Violetum.Domain.Entities;
using Violetum.Domain.Models;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.Domain.Infrastructure
{
    public interface ICommentRepository
    {
        TResult GetComment<TResult>(Expression<Func<TResult, bool>> condition,
            IConfigurationProvider configurationProvider)
            where TResult : class;

        Comment GetComment(Expression<Func<Comment, bool>> condition);

        IEnumerable<TResult> GetComments<TResult>(CommentSearchParams searchParams,
            IConfigurationProvider configurationProvider) where TResult : class;

        int GetCommentsCount(CommentSearchParams searchParams);

        Task CreateCommentAsync(Comment comment);
        Task UpdateCommentAsync(Comment comment);
        Task DeleteCommentAsync(Comment comment);

        List<Ranks> GetCommentRanks();

        int GetUserCommentRank(string userId);
    }
}