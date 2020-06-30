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
    public class CommunityValidators : ICommunityValidators
    {
        private readonly ICommunityRepository _communityRepository;

        public CommunityValidators(ICommunityRepository communityRepository)
        {
            _communityRepository = communityRepository;
        }

        public TResult GetCommunityOrThrow<TResult>(Expression<Func<TResult, bool>> condition) where TResult : class
        {
            TResult community =
                _communityRepository.GetCommunity(condition, CommunityHelpers.GetCommunityMapperConfiguration());
            if (community == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, $"{nameof(Community)} not found");
            }

            return community;
        }

        public Community GetCommunityOrThrow(Expression<Func<Community, bool>> condition)
        {
            Community community = _communityRepository.GetCommunity(condition);
            if (community == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, $"{nameof(Community)} not found");
            }

            return community;
        }
    }
}