using System.Collections.Generic;
using Serilog.Core;
using Serilog.Events;

namespace Stenn.TestModel.AppService.IntegrationTests.Logging
{
    internal class TestSink : ILogEventSink
    {
        private readonly ICollection<LogEvent> _logAccumulator;

        internal TestSink(ICollection<LogEvent> logAccumulator)
        {
            _logAccumulator = logAccumulator;
        }

        public void Emit(LogEvent logEvent)
        {
            _logAccumulator.Add(logEvent);
        }
    }
}