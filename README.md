# Interceptor.Base
Project for adding support for aspect oriented programming and interceptors for EPiServer. 
Starting with logging interceptor. 

Now you can easily add detailed logging on any class in your solution with just a few lines of configuration.

How to use:
1. Install nuget package for the interceptor to use e.g. Mogul.Interceptor.Logging
2. Add the interceptor to the interfaces you want to log by using structuremap initialization.
In alloy template project you can use the new extension RegisterInterceptor. Lets try it out on my NewsRepository class:

using Mogul.Interceptor.Base.Infrastructure.IoC;
using Mogul.Interceptor.Logging;
...

private static void ConfigureContainer(ConfigurationExpression container)
{
//   container.Scan(x =>
//   {
//       x.TheCallingAssembly();
//       x.WithDefaultConventions();
//   });
container.RegisterInterceptor<INewsRepository>(new LoggingInterceptor());

-----------------------------------------------------------------------------------
To turn on the logging you can either turn on INFO lvl for log4net on the entire site. Probably a bad idea. 
Or you can turn it on for the interceptors

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

Keep your solution clean from cross cutting concerns like logging. Use interceptors :)
More interceptors incoming...