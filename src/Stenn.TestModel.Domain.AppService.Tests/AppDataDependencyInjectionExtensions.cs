using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Stenn.AppData;
using Stenn.AppData.EntityFrameworkCore.SqlServer;
using Stenn.AppData.Mock;
using Stenn.TestModel.Domain.AppService.Tests.Entities;
using Stenn.TestModel.Domain.AppService.Tests.Projections;

namespace Stenn.TestModel.Domain.AppService.Tests
{
    [Obsolete]
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
                TestModelDataService, TestModelAppDataServiceDbContext>(connectionString, InitProjections);

            return services;
        }


        /// <summary>
        /// Register mock of public api data service <see cref="ITestModelDataService"/>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="entitiesInit"></param>
        /// <param name="queryTrackingBehavior"></param>
        /// <returns></returns>
        public static IServiceCollection AddMockTestModelAppDataService(this IServiceCollection services,
            Action<MockAppDataServiceBuilder<ITestModelEntity>> entitiesInit,
            QueryTrackingBehavior queryTrackingBehavior)
        {
            services.AddMockAppDataService<ITestModelEntity, ITestModelDataService,
                TestModelDataService, TestModelAppDataServiceDbContext>(entitiesInit, InitProjections, queryTrackingBehavior);

            return services;
        }

        /// <summary>
        /// Projections' registrations
        /// </summary>
        /// <param name="builder"></param>
        private static void InitProjections(AppDataServiceBuilder<ITestModelEntity> builder)
        {
            builder.AddProjection<TestModelConstantView, TestModelConstantViewDataProjection>();
            builder.AddProjection<TestModelCountryStateView, TestModelCountryStateViewDataProjection>();
        }
    }
}