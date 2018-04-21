using Castle.DynamicProxy;

namespace Mogul.Interceptor.Base
{
    /// <summary>
    ///This interceptor does nothing...for test purposes...
    /// </summary>
    public class NullInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation) 
        {
            invocation.Proceed();
        }
    }
}