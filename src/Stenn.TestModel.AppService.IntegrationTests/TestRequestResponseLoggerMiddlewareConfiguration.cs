using Seedwork.HttpClientHelpers;

namespace Stenn.TestModel.AppService.IntegrationTests
{
    public class TestRequestResponseLoggerMiddlewareConfiguration : IRequestResponseLoggerMiddlewareConfiguration
    {
        public RequestResponseLoggerMiddlewareConfig RequestResponseLoggerMiddlewareConfig { get; set; } =
            new RequestResponseLoggerMiddlewareConfig() { LoggingBodyTextLengthLimit = 1024 * 10 };
    }
}
