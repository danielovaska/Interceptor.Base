using System;
using System.Collections.Generic;

namespace Mogul.Interceptor.Cache
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CacheAttribute : Attribute
    {
        private readonly string[] _cacheBuckets;
        private readonly TimeSpan? _duration;
        private readonly string _cacheKey;
        public CacheAttribute()
        {
            
        }
        public CacheAttribute(int durationInSeconds,params string[] cacheBuckets)
        {
            _cacheBuckets = cacheBuckets;
            _duration = new TimeSpan(0,0,0,durationInSeconds);
        }
        public CacheAttribute(string cacheKey, int duration, params string[] cacheBuckets):this(duration,cacheBuckets)
        {
            _cacheKey = cacheKey;
        }
        public string GetCacheKey()
        {
            return _cacheKey;
        }
        public IEnumerable<string> GetCacheBuckets()
        {
            return _cacheBuckets;
        }
        public TimeSpan? GetDuration()
        {
            return _duration;
        }
    }
}