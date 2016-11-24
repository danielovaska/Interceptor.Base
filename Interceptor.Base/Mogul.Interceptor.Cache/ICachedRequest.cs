using System;
using System.Collections.Generic;

namespace Mogul.Interceptor.Cache
{
    public interface ICachedRequest
    {
        string CacheKey { get; }
        TimeSpan? CacheDuration { get; }
        bool GetFromCache { get; }
        bool StoreInCache { get; }
        IEnumerable<string> CacheBuckets { get; }
    }
}