using Castle.DynamicProxy;
using EPiServer.ServiceLocation;
using System;
using System.Collections.Generic;

namespace Mogul.Interceptor.Base.Infrastructure.IoC
{
    public static class InterceptorExtensions
    {
        /// <summary>
        /// Adds interceptors above the actual instance. 
        /// The first interceptor in the array will be added on the top and the last will be the one that calls the actual instance of the class you are adding interceptors to.
        /// </summary>
        /// <typeparam name="T">The interface to add interceptors to</typeparam>
        /// <param name="interceptors">These should implement the IInterceptor from Castle.DynamicProxy</param>
        public static void AddInterceptors<T>(this IRegisteredService provider, IInterceptor[] interceptors) where T : class
        {
            if (interceptors != null && interceptors.Length > 0)
            {
                var generator = new Castle.DynamicProxy.ProxyGenerator();
                provider.Intercept<T>((loc, s) =>
                {
                    var interceptor = generator.CreateInterfaceProxyWithTargetInterface<T>(s, interceptors);
                    return interceptor;
                });
            }
        }
        /// <summary>
        /// Adds interceptors above the actual instance. 
        /// The first interceptor in the array will be added on the top and the last will be the one that calls the actual instance of the class you are adding interceptors to.
        /// </summary>
        /// <typeparam name="T">The interface to add interceptors to</typeparam>
        /// <param name="interceptors">These should implement the IInterceptor from Castle.DynamicProxy</param>
        public static void AddInterceptors<T>(this IRegisteredService provider, Type[] interceptors) where T : class
        {
            List<IInterceptor> interceptorInstances = new List<IInterceptor>();
            foreach (var type in interceptors)
            {
                var instance = Activator.CreateInstance(type) as IInterceptor;
                if (instance != null)
                {
                    interceptorInstances.Add(instance);
                }
            }
            AddInterceptors<T>(provider, interceptorInstances.ToArray());
        }
    }
}