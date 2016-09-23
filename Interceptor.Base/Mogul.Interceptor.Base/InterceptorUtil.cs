using System.Linq;
using Castle.DynamicProxy;

namespace Mogul.Interceptor.Base
{
    public static class InterceptorUtil
    {
        public static T GetMethodAttribute<T>(IInvocation invocation) where T : class
        {
            T authorizationAttribute = null;
            if (invocation.Method.IsDefined(typeof(T), false))
            {
                var attributes = System.Attribute.GetCustomAttributes(invocation.Method);

                foreach (System.Attribute attr in attributes)
                {
                    authorizationAttribute = attr as T;
                    if (authorizationAttribute != null)
                    {
                        break;
                    }
                }
            }
            return authorizationAttribute;
        }
        public static T GetArgumentByType<T>(IInvocation invocation) where T : class
        {
            var argumentsWithInterface = invocation.Arguments.OfType<T>().ToList();
            return argumentsWithInterface.Any() ? argumentsWithInterface.First() : null;
        }

    }
}
