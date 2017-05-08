namespace apache.log4net.Extensions.Logging
{
    public class Log4NetSettings
    {
        public string ConfigFile { get; set; } = "log4net.config";
        public string RootRepository { get; set; } = "Root";

        public bool Watch { get; set; } = true;
    }
}
