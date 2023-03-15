using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Stenn.AppData.Contracts;
using Stenn.AppData.EntityFrameworkCore;

namespace Stenn.AppData.Mock
{
    [Obsolete]
    public static class MockAppDataServiceDependencyInjectionExtensions
    {
        /// <summary>
        /// Register public api data service with db context
        /// </summary>
        /// <param name="services"></param>
        /// <param name="entitiesInit"></param>
        /// <param name="initProjections"></param>
        /// <param name="queryTrackingBehavior"></param>
        /// <returns></returns>
        public static IServiceCollection AddMockAppDataService<TBaseEntity, TServiceContract, TServiceImplementation, TDbContext>(
            this IServiceCollection services,
            Action<MockAppDataServiceBuilder<TBaseEntity>>? entitiesInit,
            Action<AppDataServiceBuilder<TBaseEntity>>? initProjections = null,
            QueryTrackingBehavior queryTrackingBehavior = QueryTrackingBehavior.NoTracking)
            where TBaseEntity : class, IAppDataEntity
            where TServiceContract : class, IAppDataService<TBaseEntity>
            where TServiceImplementation : class, TServiceContract
            where TDbContext : AppDataServiceDbContext<TBaseEntity>
        {
            services.RemoveDbContext<TBaseEntity, TDbContext>();

            var projections = services.Where(r => r.ServiceType == typeof(IAppDataProjection<TBaseEntity>)).ToArray();
            foreach (var projection in projections)
            {
                services.Remove(projection);
            }

            services.AddScoped<MockDataStorage<TBaseEntity>>(_ =>
            {
                var builder = new MockAppDataServiceBuilder<TBaseEntity>();
                entitiesInit?.Invoke(builder);
                return new MockDataStorage<TBaseEntity>(builder.Dictionary);
            });
            
            if (initProjections != null)
            {
                var appServiceBuilder = new MockProjectionsAppDataServiceBuilder<TBaseEntity>(services);
                initProjections(appServiceBuilder);
            }

            services.AddAppDataService<TBaseEntity, TServiceContract, TServiceImplementation, TDbContext>((_, builder) =>
            {
                builder.UseInMemoryDatabase(typeof(TDbContext).Name);
                builder.UseQueryTrackingBehavior(queryTrackingBehavior);
                builder.UseMockServiceBuilder<TBaseEntity>();
            });
            
            services.AddScoped<TServiceContract>(p =>
            {
                var dbContext = p.GetRequiredService<TDbContext>();
                //NOTE: We need it for fill test data from HasData method
                dbContext.Database.EnsureCreated();

                return p.GetRequiredService<TServiceImplementation>();
            });

            return services;
        }

        private static void RemoveDbContext<TBaseEntity, TDbContext>(this IServiceCollection services)
            where TBaseEntity : class, IAppDataEntity
            where TDbContext : AppDataServiceDbContext<TBaseEntity>
        {
            var serviceDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(TDbContext));
            if (serviceDescriptor != null)
            {
                services.Remove(serviceDescriptor);

                var options = services.Where(r => r.ServiceType == typeof(DbContextOptions<TDbContext>)).ToArray();
                foreach (var option in options)
                {
                    services.Remove(option);
                }
            }
        }
    }
}