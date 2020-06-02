using System;
using System.Net;
using Violetum.ApplicationCore.Attributes;
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

        public TResult GetCommentByIdOrThrow<TResult>(string commentId, Func<Comment, TResult> selector)
        {
            TResult comment = _commentRepository.GetComment(x => x.Id == commentId, selector);
            if (comment == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound,
                    $"{nameof(Comment)}:{commentId} not found");
            }

            return comment;
        }
    }
}