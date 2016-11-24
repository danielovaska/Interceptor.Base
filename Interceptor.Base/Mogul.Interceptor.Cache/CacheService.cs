using System;
using System.Collections.Generic;
using EPiServer.Framework.Cache;
using EPiServer.Logging;
using EPiServer.ServiceLocation;
using Mogul.Interceptor.Base.Infrastructure.Logging;

namespace Mogul.Interceptor.Cache
{ 
    public class CacheService : ICacheService
    {
        private readonly ILogger _log;
        private readonly IObjectLogger _objectLogger;
        private Injected<ISynchronizedObjectInstanceCache> _cache;

        public Injected<ISynchronizedObjectInstanceCache> Cache
        {
            get { return _cache; }
            set { _cache = value; }
        }

        public CacheService()
        {
            _log = LogManager.GetLogger(typeof (CacheService));
            _objectLogger = new ObjectLogger();
        }
        public CacheService(ILogger logger, IObjectLogger objectLogger)
        {
            _log = logger;
            _objectLogger = objectLogger;
        }
        public object Get(string key)
        {
            return Cache.Service.Get(key);
        }
        public virtual void Add(string key, object item, CacheEvictionPolicy cacheItemPolicy)
        {
            Cache.Service.Insert(key, item, cacheItemPolicy);
        }
        public virtual void EmptyCacheBucket(string name)
        {
            _log.Information(string.Format("Emptying cache bucket: {0}", name));
            Cache.Service.Remove(name);
        }
        public virtual string GenerateCacheKey(object parameters)
        {
            return _objectLogger.Dump(parameters);
        }

        public virtual CacheEvictionPolicy GetCachePolicy(IEnumerable<string> masterKeys, TimeSpan duration)
        {
            var cip = new CacheEvictionPolicy(duration, CacheTimeoutType.Absolute, new List<string>(), masterKeys);
            return cip;
        }
        public virtual void RemoveItem(string key)
        {
            _log.Information($"Removing single item from cache with key: {key}");
            Cache.Service.Remove(key);
        }
    }
}