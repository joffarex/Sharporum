using System;
using System.Net;
using Violetum.ApplicationCore.Interfaces.Validators;
using Violetum.Domain.CustomExceptions;
using Violetum.Domain.Entities;
using Violetum.Domain.Infrastructure;

namespace Violetum.ApplicationCore.Validators
{
    public class PostValidators : IPostValidators
    {
        private readonly IPostRepository _postRepository;

        public PostValidators(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public TResult GetReturnedPostOrThrow<TResult>(string postId, Func<Post, TResult> selector)
        {
            TResult post = _postRepository.GetPost(x => x.Id == postId, selector);
            if (post == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound,
                    $"{nameof(Post)}:{postId} not found");
            }

            return post;
        }
    }
}