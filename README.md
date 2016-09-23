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

Keep your solution clean from cross cutting concerns like logging. Use interceptors :)
More interceptors incoming...