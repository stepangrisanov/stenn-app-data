using System.Collections.Generic;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;

namespace Stenn.TestModel.AppService.IntegrationTests.Logging
{
    internal static class LoggerSinkConfigurationExtensions
    {
        internal static LoggerConfiguration TestLogger(this LoggerSinkConfiguration configuration,
            ICollection<LogEvent> logAccumulator) =>
            configuration?.Sink(new TestSink(logAccumulator));
    }
}