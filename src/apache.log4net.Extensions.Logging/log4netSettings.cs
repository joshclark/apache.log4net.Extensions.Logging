namespace apache.log4net.Extensions.Logging
{
    /// <summary>
    /// Defines the settings for configuring log4net
    /// </summary>
    public class Log4NetSettings
    {
        /// <summary>
        /// The path to the log4net.config file.  Defaults to "log4net.config"
        /// </summary>
        public string ConfigFile { get; set; } = "log4net.config";

        /// <summary>
        /// The name of the root log4net repository.  The default is fine for most use cases. Defaults to "apache.log4net.Extensions.Logging".
        /// </summary>
        public string RootRepository { get; set; } = "apache.log4net.Extensions.Logging";

        /// <summary>
        /// Determines if the config file should be watched and the settings reloaded when the file changes.
        /// </summary>
        public bool Watch { get; set; } = true;
    }
}
