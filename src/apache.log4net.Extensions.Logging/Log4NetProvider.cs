using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using log4net;
using log4net.Config;
using log4net.Core;
using log4net.Repository;
using log4net.Util;
using Microsoft.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace apache.log4net.Extensions.Logging
{
    internal class Log4NetProvider : ILoggerProvider
    {
        private static readonly LazyConcurrentDictionary<string, ILoggerRepository> _repositoryCache;
        private ILoggerRepository _loggerRepository;
        private FileWatcher _fileWatcher;

        static Log4NetProvider()
        {
            _repositoryCache = new LazyConcurrentDictionary<string, ILoggerRepository>(StringComparer.Ordinal);
        }

        public void Initialize(Log4NetSettings settings)
        {
            _loggerRepository = CreateRepository(settings);
        }

        private ILoggerRepository CreateRepository(Log4NetSettings settings)
        {
            ILoggerRepository CreateAndInitializeRepo(string repoName)
            {
                var repo = LogManager.CreateRepository(repoName);
                var configFile = GetConfigFileFullPath(settings.ConfigFile);

                if (File.Exists(configFile))
                {
                    ConfigureRepositoryFromXml(repo, configFile, settings.Watch);
                }
                else
                {
                    BasicConfigurator.Configure(repo);
                }

                return repo;
            }

            return _repositoryCache.GetOrAdd(settings.RootRepository, CreateAndInitializeRepo);
        }

        private static string GetConfigFileFullPath(string filePath)
        {
            var assemblyPath = new Uri(typeof(Log4NetProvider).GetTypeInfo().Assembly.CodeBase).LocalPath;
            var assemblyDir = Path.GetDirectoryName(assemblyPath) ?? String.Empty;
            var configFile = Path.Combine(assemblyDir, filePath);
            return Path.GetFullPath(configFile);
        }

        private void ConfigureRepositoryFromXml(ILoggerRepository repo, string configFile, bool watchConfigFileForChanges)
        {
            void ConfigureFromFile(string filename)
            {
                var xDocument = XDocument.Load(filename);

                ReplaceEnvironmentVariables(xDocument);

                XmlConfigurator.Configure(repo, AsXmlElement(xDocument));
            }

            ConfigureFromFile(configFile);

            if (watchConfigFileForChanges)
            {
                // We can't just use XmlConfigurator.Configure since we need to modify the file when it changes.
                _fileWatcher = new FileWatcher(configFile, ConfigureFromFile);
            }
        }


        /// <summary>
        /// The current version of the NetStandard build of log4net does not replace environment variables.
        /// We will use the same functionality to replace the environment variables before we pass the xml
        /// into the standard XmlConfigurator
        /// </summary>
        /// <param name="xDocument"></param>
        private void ReplaceEnvironmentVariables(XDocument xDocument)
        {
            var environmentVariables = new Hashtable(Environment.GetEnvironmentVariables(), StringComparer.OrdinalIgnoreCase);

            var valueAttributes = xDocument
                .Descendants()
                .Select(x => x.Attribute("value"))
                .Where(x => x != null);

            foreach (var attribute in valueAttributes)
            {
                attribute.Value = OptionConverter.SubstituteVariables(attribute.Value, environmentVariables);
            }
        }

        private XmlElement AsXmlElement(XDocument xDocument)
        {
            var xmlDoc = new XmlDocument();
            using (var reader = xDocument.CreateReader())
            {
                xmlDoc.Load(reader);
            }

            return xmlDoc.DocumentElement;
        }


        public ILogger CreateLogger(string categoryName)
        {
            var logger = _loggerRepository.GetLogger(categoryName);
            var impl = new LogImpl(logger);
            return new Log4NetLogger(impl);
        }

        public void Dispose()
        {
            _fileWatcher?.Dispose();
        }

    }
}
