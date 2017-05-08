using Microsoft.Extensions.Configuration;
using apache.log4net.Extensions.Logging;
    

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Logging
{
    public static class LoggerFactoryExtensions
    {
        public static ILoggerFactory AddLog4Net(this ILoggerFactory self)
        {
            return AddLog4Net(self, new Log4NetSettings());
        }

        public static ILoggerFactory AddLog4Net(this ILoggerFactory self, IConfiguration configuration)
        {
            var settings = new Log4NetSettings();
            configuration.Bind(settings);
            return AddLog4Net(self, settings);
        }

        public static ILoggerFactory AddLog4Net(this ILoggerFactory self, Log4NetSettings settings)
        {
            var provider = new Log4NetProvider();
            provider.Initialize(settings);

            self.AddProvider(provider);
            return self;
        }

    }
}
