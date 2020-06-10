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
    public class PostValidators : IPostValidators
    {
        private readonly IPostRepository _postRepository;

        public PostValidators(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public TResult GetPostOrThrow<TResult>(Expression<Func<TResult, bool>> condition) where TResult : class
        {
            TResult post = _postRepository.GetPost(condition, PostHelpers.GetPostMapperConfiguration());
            if (post == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, $"{nameof(Post)} not found");
            }

            return post;
        }
    }
}