using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Stenn.AppData.Contracts;

namespace Stenn.AppData.Mock
{
    public static class MockAppDataServiceDependencyInjectionExtensions
    {
        /// <summary>
        /// Register public api data service with db context
        /// </summary>
        /// <param name="services"></param>
        /// <param name="entitiesInit"></param>
        /// <param name="queryTrackingBehavior"></param>
        /// <returns></returns>
        public static IServiceCollection AddMockAppDataService<TBaseEntity, TServiceContract, TServiceImplementation, TDbContext>(
            this IServiceCollection services,
            Action<MockAppDataServiceBuilder<TBaseEntity>> entitiesInit,
            QueryTrackingBehavior queryTrackingBehavior = QueryTrackingBehavior.NoTracking)
            where TBaseEntity : class, IAppDataEntity
            where TServiceContract : class, IAppDataService<TBaseEntity>
            where TServiceImplementation : class, TServiceContract
            where TDbContext : AppDataServiceDbContext<TBaseEntity>
        {
            services.AddMockAppDataDbContext<TBaseEntity, TDbContext>(entitiesInit, queryTrackingBehavior);

            services.AddScoped<TServiceImplementation>();
            services.AddScoped<TServiceContract>(p =>
            {
                var dbContext = p.GetRequiredService<TDbContext>();
                //NOTE: We need it for fill test data from HasData method
                dbContext.Database.EnsureCreated();

                return p.GetRequiredService<TServiceImplementation>();
            });

            return services;
        }

        private static IServiceCollection AddMockAppDataDbContext<TBaseEntity, TDbContext>(this IServiceCollection services,
            Action<MockAppDataServiceBuilder<TBaseEntity>> entitiesInit,
            QueryTrackingBehavior queryTrackingBehavior = QueryTrackingBehavior.NoTracking)
            where TBaseEntity : class, IAppDataEntity
            where TDbContext : AppDataServiceDbContext<TBaseEntity>
        {
            services.RemoveDbContext<TBaseEntity, TDbContext>();

            // Idea: Use EF Core extension. Put entitiesInit as ef extension option and use it in DbContext.OnModelCreating for fill entity via HasData
            services.AddDbContext<TDbContext>(builder =>
            {
                builder.UseInMemoryDatabase(typeof(TDbContext).Name);
                builder.UseQueryTrackingBehavior(queryTrackingBehavior);
                builder.UseMockServiceBuilder(entitiesInit);
            });

            return services;
        }

        private static IServiceCollection RemoveDbContext<TBaseEntity, TDbContext>(this IServiceCollection services)
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

            return services;
        }
    }
}