#Interceptor.Base

Project for adding support for aspect oriented programming and interceptors for EPiServer. 

##Logging interceptor

Problem: I have 50 repository classes I want detailed logs from like input/output parameters + execution times. 
Now you can easily add detailed logging on any class in your solution with just a few lines of configuration.
Example logs you will get per class you turn it on for:

2016-09-23 14:51:43,658 [110] INFO LoggingInterceptor: Requesting method GetAllNews  
2016-09-23 14:51:43,923 [110] INFO LoggingInterceptor: GetAllNews executed in: 250 ms  
2016-09-23 14:51:43,936 [110] INFO LoggingInterceptor: Responding method GetAllNews has return value [  
  {  
    "Id": "1"  
  },  
  {  
    "Id": "1"  
  }  
]  


###How to use Logging Interceptor:
1. Install nuget package for the interceptor to use e.g. Mogul.Interceptor.Logging
2. Add the interceptor to the interfaces you want to log by using structuremap initialization.
In alloy template project you can use the new extension RegisterInterceptor. Lets try it out on my NewsRepository class:

using Mogul.Interceptor.Base.Infrastructure.IoC;  
using Mogul.Interceptor.Logging;  
...  
``` 
private static void ConfigureContainer(ConfigurationExpression container)  
{  
   container.RegisterInterceptor<INewsRepository>(new LoggingInterceptor()); //Add logging to all methods on the INewsRepository interface...
}  
``` 

-----------------------------------------------------------------------------------  
The interceptors logs on INFO level with log4net default. This means that you won't see messages unless you turn on INFO level logging. 
To turn on the logging you can either turn on INFO lvl for log4net on the entire site. Probably a bad idea. 
Or you can turn it on for the interceptors in EpiserverLog.config like:  
``` 
 <appender name="debugFileLogAppender" type="log4net.Appender.RollingFileAppender" >  
        <!-- Consider moving the log files to a location outside the web application -->  
        <file value="App_Data\Debug.log" />  
        <encoding value="utf-8" />  
        <staticLogFileName value="true"/>  
        <datePattern value=".yyyyMMdd.'log'" />  
        <rollingStyle value="Date" />  
        <lockingModel type="log4net.Appender.FileAppender+MinimalLock" />  
        <appendToFile value="true" />  
        <layout type="log4net.Layout.PatternLayout">  
            <conversionPattern value="%date [%thread] %level %logger: %message%n" />  
        </layout>  
    </appender>  
  <logger name="Mogul.Interceptor" additivity="true">  
    <level value="All" />  
    <appender-ref ref="debugFileLogAppender" />  
  </logger> 
 ```
Remember to turn off logging when you are not using it anymore...  
Keep your solution clean from cross cutting concerns like logging. Use interceptors :)  


##Cache Interceptor

1. **Register the cache interceptor and hook in on to an interface in ioc**  
container.RegisterInterceptor<INewsRepository>(new LoggingInterceptor());  
or chain them  
```
container.RegisterInterceptors<INewsRepository>(new IInterceptor[]{
                new LoggingInterceptor(),
                new CacheInterceptor()
            });
```
2. **Mark what methods on the interface you want to cache with the new cache attribute**
```
public interface INewsRepository
{
    [Cache(10,"News")]
    NewsResponse GetNews(GetItemsRequest request);
}
```
This will automatically construct a unique key based on method name and request parameters. 
It will cache it for 10s and it uses an area in cache called news. This is basically a master key that allows you to clear parts of the cache easily.

3. **To empty cache for a section in the cache user the cache service class**
```
var cacheService = new CacheService();
cacheService.EmptyCacheBucket("News");
```
4. For advanced scenarios you can implement interfaces for both request and reponse from the methods. 
This will allow you to control the caching in detail like whether to ignore cache for a request (good for authenticated users...) or use a specific cache key. 

**The request interface is used like:**
```
public class GetItemsRequest : ICachedRequest
{
    public string CacheKey { get; set; } //Custom cache key that is not auto-generated
    public TimeSpan? CacheDuration { get; set; } //Custom cache duration
    public bool GetFromCache { get; set; } //Use result in cache if it exists?
    public bool StoreInCache { get; set; } //Store result in cache?
    public IEnumerable<string> CacheBuckets { get; set; } //Master keys for caching. Usually the root entity like "Users", "News" or similar. 
	//Useful when emptying cache
	//...your custom parameters for the request go here. Filters, ids etc...
}
```
**The response interface is used like:**
```
public class NewsResponse : ICachedResponse
{
    public IEnumerable<NewsItem> Items { get; set; } //Custom values you want to return
    public bool GotItemFromCache { get; set; } //Did I get this from cache? Useful when debugging.
}
```
