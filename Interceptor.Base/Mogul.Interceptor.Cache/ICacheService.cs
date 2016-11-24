using System;
using System.Collections.Generic;
using EPiServer.Framework.Cache;

namespace Mogul.Interceptor.Cache
{
    public interface ICacheService
    {
        void EmptyCacheBucket(string name);
        string GenerateCacheKey(object parameters);
        CacheEvictionPolicy GetCachePolicy(IEnumerable<string> dependencies, TimeSpan duration);
        void RemoveItem(string key);
        void Add(string key, object item, CacheEvictionPolicy cacheItemPolicy);
        object Get(string key);
    }
}