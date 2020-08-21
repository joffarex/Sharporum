using System;
using System.Threading.Tasks;

namespace Sharporum.API.Cache
{
    public interface IResponseCacheService
    {
        Task CacheResponse(string cacheKey, object response, TimeSpan timeToLive);
        Task<string> GetCachedResponse(string cacheKey);
    }
}