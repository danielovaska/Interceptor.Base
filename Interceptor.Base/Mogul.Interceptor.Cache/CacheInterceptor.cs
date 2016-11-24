using System;
using System.Collections.Generic;
using System.Linq;
using Castle.DynamicProxy;
using EPiServer.Framework.Cache;
using EPiServer.Logging;
using EPiServer.ServiceLocation;
using Mogul.Interceptor.Base;
using Mogul.Interceptor.Base.Infrastructure.Logging;

namespace Mogul.Interceptor.Cache
{
    /// <summary>
    /// Useful interceptor for caching calls
    /// </summary>
    public class CacheInterceptor : IInterceptor
    {
        private readonly ILogger _log;
        private readonly IObjectLogger _objectLogger;
        private readonly ICacheService _cacheService;
        
        public CacheInterceptor() 
        {
            _log = LogManager.GetLogger(typeof(CacheInterceptor));
            _objectLogger = new ObjectLogger();
            _cacheService = new CacheService(_log, _objectLogger);
        }
        public void Intercept(IInvocation invocation)
        {
            _log.Information("Entering caching interceptor");
            object cachedItem = null;
            var gotItemFromCache = false;
            var cacheAttribute = InterceptorUtil.GetMethodAttribute<CacheAttribute>(invocation);
            var methodCalledIsResolved = false;
            if (cacheAttribute != null)
            {

                var methodCacheSettings = InterceptorUtil.GetArgumentByType<ICachedRequest>(invocation);
                var cacheSettings = GetMergedCacheSettings(methodCacheSettings, cacheAttribute, invocation);
                _objectLogger.Dump(cacheSettings);
                if (cacheSettings.GetFromCache) //Get item from cache...
                {
                    cachedItem = _cacheService.Get(cacheSettings.CacheKey);
                    if (cachedItem != null)
                    {
                        gotItemFromCache = true;
                        var cachedResponse = cachedItem as ICachedResponse;
                        if (cachedResponse != null)
                        {
                            cachedResponse.GotItemFromCache = true;
                        }
                        invocation.ReturnValue = cachedItem;
                        methodCalledIsResolved = true;
                        if (_log.IsInformationEnabled())
                        {
                            _log.Information(string.Format("Got item from cache with key {1} for method:{0}",
                                invocation.Method.Name, cacheSettings.CacheKey));
                        }
                    }
                }
                if (!gotItemFromCache) //Get item from data source...
                {
                    invocation.Proceed();
                    _log.Information(string.Format("Got item from datasource with key {1} for method:{0}",
                        invocation.Method.Name, cacheSettings.CacheKey));
                    methodCalledIsResolved = true;
                    cachedItem = invocation.ReturnValue;
                }
                else
                {
                    var cacheResponse = invocation.ReturnValue as ICachedResponse;
                    if (cacheResponse != null)
                    {
                        cacheResponse.GotItemFromCache = true;
                    }
                } 
                if (cacheSettings.StoreInCache && !gotItemFromCache) //Store in cache...
                {
                    _objectLogger.Dump(cacheSettings);
                    _cacheService.Add(cacheSettings.CacheKey, cachedItem,
                        _cacheService.GetCachePolicy(cacheSettings.CacheBuckets,
                            cacheSettings.Duration ?? new TimeSpan(0, 0, 10, 0)));
                    if (_log.IsInformationEnabled())
                    {
                        _log.Information(
                            string.Format("Stored item in cachebuckets: {2}\n with key: {1}\n for method:{0}\n",
                                invocation.Method.Name, cacheSettings.CacheKey,
                                string.Join(",", cacheSettings.CacheBuckets)));
                    } 
                }
            }
            else
            {
                _log.Information("No cache attribute found. Proceeding without cache");
            }
            if (!methodCalledIsResolved) //If something went wrong...just proceed to the method and run it without cache...
            {
                invocation.Proceed();
            }
        }
        private CacheSettings GetMergedCacheSettings(ICachedRequest methodCacheSettings, CacheAttribute attribute,IInvocation invocation)
        {
            var cacheSettings = new CacheSettings();
            if (methodCacheSettings != null)
            {
                cacheSettings.CacheBuckets = methodCacheSettings.CacheBuckets;
                cacheSettings.CacheKey = methodCacheSettings.CacheKey;
                if (methodCacheSettings.CacheDuration != null)
                {
                    cacheSettings.Duration = methodCacheSettings.CacheDuration.Value;
                  
                }
                cacheSettings.GetFromCache = methodCacheSettings.GetFromCache;
                cacheSettings.StoreInCache = methodCacheSettings.StoreInCache;
               
            }
            if (cacheSettings.CacheBuckets == null || !cacheSettings.CacheBuckets.Any())
            {
                cacheSettings.CacheBuckets = attribute.GetCacheBuckets();
            }
            if (cacheSettings.Duration == null && attribute.GetDuration() != null)
            {
                cacheSettings.Duration = attribute.GetDuration();
              
            }
            if (cacheSettings.Duration == null)
            {
                cacheSettings.Duration = new TimeSpan(0,0,10,0);
              
            }
            if (cacheSettings.CacheKey == null && attribute.GetCacheKey() != null)
            {
                cacheSettings.CacheKey = attribute.GetCacheKey();
            }
            if (cacheSettings.CacheKey == null)
            {
                cacheSettings.CacheKey = _objectLogger.Dump(new { Method = string.Format("{0}.{1}", invocation.TargetType.FullName, invocation.MethodInvocationTarget.Name), Params = invocation.Arguments });
            }
         
            return cacheSettings;
        }
    }
    internal class CacheSettings
    {
        public string CacheKey { get; set; }
        public TimeSpan? Duration { get; set; }
        public IEnumerable<string> CacheBuckets { get; set; }
        public bool GetFromCache { get; set; }
        public bool StoreInCache { get; set; }
        public CacheSettings()
        {
            GetFromCache = true;
            StoreInCache = true;
        }
    }

}