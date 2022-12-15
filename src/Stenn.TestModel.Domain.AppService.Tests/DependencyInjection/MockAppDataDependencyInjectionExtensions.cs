using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Stenn.AppData.Mock;
using Stenn.TestModel.Domain.AppService.Tests.Entities;

namespace Stenn.TestModel.Domain.AppService.Tests.DependencyInjection
{
    public static class MockAppDataDependencyInjectionExtensions
    {
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
                TestModelDataService, TestModelAppDataServiceDbContext>(entitiesInit, queryTrackingBehavior);

            return services;
        }
    }
}