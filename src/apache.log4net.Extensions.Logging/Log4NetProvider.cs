﻿using System;
using System.Collections;
using System.IO;
using System.Linq;
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
        private ILoggerRepository _loggerRepository;

        public void Initialize(Log4NetSettings settings)
        {
            _loggerRepository = CreateRepository(settings);
        }

        private ILoggerRepository CreateRepository(Log4NetSettings settings)
        {
            var repo = LogManager.CreateRepository(settings.RootRepository);
            var configFile = Path.GetFullPath(settings.ConfigFile);

            if (File.Exists(configFile))
            {
                ConfigureRepositoryFromXml(repo, configFile);
            }
            else
            {
                BasicConfigurator.Configure(repo);
            }

            return repo;
        }

        private void ConfigureRepositoryFromXml(ILoggerRepository repo, string configFile)
        {
            var xDocument = XDocument.Load(configFile);

            ReplaceEnvironmentVariables(xDocument);

            XmlConfigurator.Configure(repo, AsXmlElement(xDocument));
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
        }

    }
}