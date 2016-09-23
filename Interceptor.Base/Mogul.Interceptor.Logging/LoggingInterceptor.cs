using System.Diagnostics;
using System.Linq;
using Castle.DynamicProxy;
using EPiServer.Logging;
using EPiServer.Personalization;
using Mogul.Interceptor.Base.Infrastructure.Logging;

namespace Mogul.Interceptor.Logging
{
    /// <summary>
    /// Useful interceptor for logging all values to and from a method.
    /// Especiallly useful for calls to external data sources to locate errors with integrations...
    /// </summary>
    public class LoggingInterceptor : IInterceptor
    {
        private readonly ILogger _log;
        private readonly IObjectLogger _objectLogger;
        private readonly LoggingInterceptorSettings _settings;

        public LoggingInterceptor()
        {
            _log = LogManager.GetLogger(typeof(LoggingInterceptor));
            _objectLogger = new ObjectLogger();
            _settings = new LoggingInterceptorSettings();
        }
        public LoggingInterceptor(ILogger log, IObjectLogger objectLogger, LoggingInterceptorSettings settings)
        {
            _log = log;
            _objectLogger = objectLogger;
            _settings = settings;
        }
        public void Intercept(IInvocation invocation)
        {
            try
            {
                if (_log.IsInformationEnabled() && _settings.LogInputParameters)
                {
                    var parameters = invocation.Arguments.Select(_objectLogger.Dump).ToArray();
                    if (parameters.Any())
                    {
                        _log.Information($"Requesting method {invocation.Method.Name} with parameters {string.Join(", ", parameters)} ");
                    }
                    else
                    {
                        _log.Information($"Requesting method {invocation.Method.Name}");
                    }
                }
            }
            catch
            {
                //This should never raise an exception...just die silently please...
            }
            Stopwatch watch = null;
            if (_settings.LogExecutionTime)
            {
                watch = new Stopwatch();
                watch.Start();
            }
            invocation.Proceed();
            if (_settings.LogExecutionTime)
            {
                watch.Stop();
                if (_log.IsInformationEnabled())
                {
                    _log.Information($"{invocation.Method.Name} executed in: {watch.ElapsedMilliseconds} ms");
                }
            }
            try
            {
                if (_log.IsInformationEnabled() && _settings.LogOutputParameters)
                {
                    _log.Information($"Responding method {invocation.Method.Name} has return value {_objectLogger.Dump(invocation.ReturnValue)}");
                }
            }
            catch
            {
                //This should never raise an exception...just die silently please...
            }
        }
    }
    public class LoggingInterceptorSettings
    {
        public bool LogExecutionTime { get; set; }
        public bool LogInputParameters { get; set; }
        public bool LogOutputParameters { get; set; }

        public LoggingInterceptorSettings()
        {
            LogExecutionTime = true;
            LogInputParameters = true;
            LogOutputParameters = true;
        }
    }
}
