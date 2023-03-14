#if !NETFRAMEWORK
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Seedwork.Logging.DependencyInjection;
using Seedwork.Logging.Enrichers;
using Serilog;
using Serilog.Events;

namespace Stenn.TestModel.AppService.IntegrationTests.Logging
{
    /// <summary>
    /// Service collection extensions.
    /// </summary>
    internal static class ServiceCollectionExtensions
    {
        internal static IServiceCollection AddMockLogger(this IServiceCollection services, ICollection<LogEvent> logAccumulator)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.TestLogger(logAccumulator)
                //.Enrich.With(new ScopedLogContextEnricher())
                .Enrich.FromLogContext()
                .Enrich.WithProperty("TestsRun", true)
                .CreateLogger();

            services.AddSeedworkLoggingServices();

            return services;
        }
    }
}
#endif