using log4net;
using System;
using Microsoft.Extensions.Logging;

namespace apache.log4net.Extensions.Logging
{
    internal class Log4NetLogger : ILogger
    {
        private readonly ILog _log;

        public Log4NetLogger(ILog log)
        {
            _log = log;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                case LogLevel.Debug:
                    return _log.IsDebugEnabled;

                case LogLevel.Information:
                    return _log.IsInfoEnabled;

                case LogLevel.Warning:
                    return _log.IsWarnEnabled;

                case LogLevel.Error:
                    return _log.IsErrorEnabled;

                case LogLevel.Critical:
                    return _log.IsFatalEnabled;

                case LogLevel.None:
                   return false;

                default:
                    throw new ArgumentException($"Unknown log level {logLevel}.", nameof(logLevel));
            }
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            if (formatter == null)
                throw new ArgumentNullException(nameof(formatter));

            var message = formatter(state, exception);
            if (string.IsNullOrEmpty(message))
                return;

            switch (logLevel)
            {
                case LogLevel.Trace:
                case LogLevel.Debug:
                    _log.Debug(message, exception);
                    break;

                case LogLevel.Information:
                    _log.Info(message, exception);
                    break;

                case LogLevel.Warning:
                    _log.Warn(message, exception);
                    break;

                case LogLevel.Error:
                    _log.Error(message, exception);
                    break;

                case LogLevel.Critical:
                    _log.Fatal(message, exception);
                    break;

                case LogLevel.None:
                    break;
                    
                default:
                    throw new ArgumentException($"Unknown log level {logLevel}.", nameof(logLevel));
            }

        }
    }
}
