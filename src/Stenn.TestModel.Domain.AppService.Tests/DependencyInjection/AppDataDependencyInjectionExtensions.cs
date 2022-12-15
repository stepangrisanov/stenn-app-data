using Microsoft.Extensions.DependencyInjection;
using Stenn.AppData.EntityFrameworkCore.SqlServer;
using Stenn.TestModel.Domain.AppService.Tests.Entities;

namespace Stenn.TestModel.Domain.AppService.Tests.DependencyInjection
{
    public static class TestModelAppDataDependencyInjectionExtensions
    {
        /// <summary>
        /// Register public api data service <see cref="ITestModelDataService"/>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static IServiceCollection AddTestModelAppDataService(this IServiceCollection services, string connectionString)
        {
            services.AddAppDataServiceSqlServer<ITestModelEntity, ITestModelDataService,
                TestModelDataService, TestModelAppDataServiceDbContext>(connectionString);

            return services;
        }
    }
}