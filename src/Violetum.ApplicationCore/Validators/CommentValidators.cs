using System;
using System.Linq.Expressions;
using System.Net;
using Violetum.ApplicationCore.Attributes;
using Violetum.ApplicationCore.Helpers;
using Violetum.ApplicationCore.Interfaces.Validators;
using Violetum.Domain.CustomExceptions;
using Violetum.Domain.Entities;
using Violetum.Domain.Infrastructure;

namespace Violetum.ApplicationCore.Validators
{
    [Validator]
    public class CommentValidators : ICommentValidators
    {
        private readonly ICommentRepository _commentRepository;

        public CommentValidators(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        public TResult GetCommentOrThrow<TResult>(Expression<Func<TResult, bool>> condition) where TResult : class
        {
            TResult comment = _commentRepository.GetComment(condition, CommentHelpers.GetCommentMapperConfiguration());
            if (comment == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, $"{nameof(Comment)} not found");
            }

            return comment;
        }
    }
}