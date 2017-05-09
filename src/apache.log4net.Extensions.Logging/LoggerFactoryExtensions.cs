using Microsoft.Extensions.Configuration;
using apache.log4net.Extensions.Logging;
    

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// Extension methods for adding log4net support to Microsoft.Extensions.Logging
    /// </summary>
    public static class LoggerFactoryExtensions
    {
        /// <summary>
        /// Adds log4net support using the default settings from <see cref="Log4NetSettings"/>
        /// </summary>
        /// <param name="self">The logger factory to update for log4net support</param>
        /// <returns>The passed in logger factory</returns>
        public static ILoggerFactory AddLog4Net(this ILoggerFactory self)
        {
            return AddLog4Net(self, new Log4NetSettings());
        }

        /// <summary>
        /// Adds log4net support using the settings defined in the passed in <see cref="IConfiguration"/> section
        /// </summary>
        /// <param name="self">The logger factory to update for log4net support</param>
        /// <param name="configuration">The settings to use for configuring log4net.  
        /// These settings are defined in <see cref="Log4NetSettings"/></param>
        /// <returns>The passed in logger factory</returns>
        public static ILoggerFactory AddLog4Net(this ILoggerFactory self, IConfiguration configuration)
        {
            var settings = new Log4NetSettings();
            configuration.Bind(settings);
            return AddLog4Net(self, settings);
        }

        /// <summary>
        /// Adds log4net support using the provided <see cref="Log4NetSettings"/>
        /// </summary>
        /// <param name="self">The logger factory to update for log4net support</param>
        /// <param name="settings">The settings to use for configuring log4net.</param>
        /// <returns>The passed in logger factory</returns>
        public static ILoggerFactory AddLog4Net(this ILoggerFactory self, Log4NetSettings settings)
        {
            var provider = new Log4NetProvider();
            provider.Initialize(settings);

            self.AddProvider(provider);
            return self;
        }

    }
}
