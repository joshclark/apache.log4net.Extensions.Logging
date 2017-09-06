using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace apache.log4net.Extensions.Logging.Tests
{
    public class Log4NetProviderTests
    {
        [Fact]
        public void InitializingTwoProvidersDoesNotThrow()
        {
            var settings = new Log4NetSettings { RootRepository = Guid.NewGuid().ToString() };
            var providerA = new Log4NetProvider();
            var providerB = new Log4NetProvider();

            providerA.Initialize(settings);
            providerB.Initialize(settings);
        }

        [Fact]
        public void GetLogger_CategoryLogLevelOverridesRootLogLevel()
        {
            string categoryName = Guid.NewGuid().ToString();
            var configuration = CreateConfiguration("DEBUG", new Dictionary<string, string>
            {
                [categoryName] = "INFO"
            });
            var provider = CreateProvider(configuration);

            var logger = provider.CreateLogger(categoryName);

            logger.IsEnabled(LogLevel.Information).Should().BeTrue("INFO log level should be enabled");
            logger.IsEnabled(LogLevel.Debug).Should().BeFalse("DEBUG log level should be disabled");
        }

        [Fact]
        public void GetLogger_RootLogLevelIsUsedByDefault()
        {
            string categoryName = Guid.NewGuid().ToString();
            var configuration = CreateConfiguration("DEBUG", new Dictionary<string, string>());
            var provider = CreateProvider(configuration);

            var logger = provider.CreateLogger(categoryName);

            logger.IsEnabled(LogLevel.Information).Should().BeTrue("INFO log level should be enabled");
            logger.IsEnabled(LogLevel.Debug).Should().BeTrue("DEBUG log level should be enabled");
            logger.IsEnabled(LogLevel.Trace).Should().BeFalse("TRACE log level should be disabled");
        }

        [Fact]
        public void EnvironmentVariablesInConfig_AreReplaced()
        {
            string categoryName = Guid.NewGuid().ToString();
            string variable = "TEST_LOG_LEVEL";
            Environment.SetEnvironmentVariable(variable, "WARN");
            var configuration = CreateConfiguration($"${{{variable}}}", new Dictionary<string, string>());
            var provider = CreateProvider(configuration);

            var logger = provider.CreateLogger(categoryName);

            logger.IsEnabled(LogLevel.Warning).Should().BeTrue("WARN log level should be enabled");
            logger.IsEnabled(LogLevel.Information).Should().BeFalse("INFO log level should be disabled");
        }

        private static Log4NetProvider CreateProvider(XDocument configuration)
        {
            var settings = new Log4NetSettings { RootRepository = Guid.NewGuid().ToString() };
            var provider = new Log4NetProvider(x => configuration, x => true);
            provider.Initialize(settings);
            return provider;
        }

        private XDocument CreateConfiguration(string rootLogLevel, Dictionary<string, string> extraLogLevels)
        {
            string template = @"<?xml version=""1.0"" encoding=""utf-8""?>
<log4net>
  {1}

  <root>
    <level value=""{0}"" />
  </root>
</log4net>";

            StringBuilder extraLoggers = new StringBuilder();
            foreach (KeyValuePair<string, string> pair in extraLogLevels)
            {
                extraLoggers.AppendFormat("<logger name=\"{0}\">", pair.Key);
                extraLoggers.AppendFormat("<level value=\"{0}\"></level>", pair.Value);
                extraLoggers.AppendLine("</logger>");
            }

            string config = string.Format(template, rootLogLevel, extraLoggers);
            return XDocument.Parse(config);
        }
    }
}
