using System;
using System.Threading.Tasks;

namespace Violetum.API.Cache
{
    public interface IResponseCacheService
    {
        Task CacheResponse(string cacheKey, object response, TimeSpan timeToLive);
        Task<string> GetCachedResponse(string cacheKey);
    }
}