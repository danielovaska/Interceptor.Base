using System;
using Mogul.Interceptor.Base.Infrastructure.Logging;
using StructureMap.Configuration.DSL;
using Castle.DynamicProxy;
using StructureMap;

namespace Mogul.Interceptor.Base.Infrastructure.IoC
{
    public class InterceptorRegistry : Registry
    {
        public InterceptorRegistry()
        {
            For<IObjectLogger>().Use<ObjectLogger>();
        }
    }
    public static class ContainerExtensions
    {
        public static void RegisterInterceptor<TDependency>(this ConfigurationExpression container, IInterceptor interceptorToAdd)
            where TDependency : class
        {
            RegisterInterceptors<TDependency>(container, new[] {interceptorToAdd});
        }

        public static void RegisterInterceptors<TDependency>(this ConfigurationExpression container, IInterceptor[] interceptorsToAdd)
            where TDependency : class
        {
            if (interceptorsToAdd == null)
                throw new NullReferenceException("interceptor");
            var proxyGenerator = new ProxyGenerator();
            if (typeof (TDependency).IsInterface)
            {
                container.For<TDependency>().DecorateAllWith(instance =>
                    proxyGenerator.CreateInterfaceProxyWithTarget(instance, interceptorsToAdd));
            }
            else
            {
                if (typeof (TDependency).IsAbstract)
                {
                    throw new ArgumentException("Can't add interceptor to abstract/static classes.");
                }
                container.For<TDependency>().Use(i => proxyGenerator.CreateClassProxy<TDependency>(interceptorsToAdd));
            }
        }
    }
}
