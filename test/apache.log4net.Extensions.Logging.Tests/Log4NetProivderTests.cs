using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace apache.log4net.Extensions.Logging.Tests
{
    public class Log4NetProivderTests
    {

        [Fact]
        public void InitializingTwoProvidersDoesNotThrow()
        {
            var settings = new Log4NetSettings();
            var providerA = new Log4NetProvider();
            var providerB = new Log4NetProvider();

            providerA.Initialize(settings);
            providerB.Initialize(settings);
        }
    }
}
