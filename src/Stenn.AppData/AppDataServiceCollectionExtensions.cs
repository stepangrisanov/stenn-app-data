using System;
using Microsoft.Extensions.DependencyInjection;
using Stenn.AppData.Contracts;

namespace Stenn.AppData
{
    public static class AppDataServiceCollectionExtensions
    {
        public static IServiceCollection AddAppDataService<TBaseEntity, TServiceContract, TServiceImplementation>(this IServiceCollection services,
            Action<AppDataServiceBuilder<TBaseEntity>>? initProjections = null,
            Action<IServiceProvider>? beforeCreate = null)
            where TBaseEntity : class, IAppDataEntity
            where TServiceContract : class, IAppDataService<TBaseEntity>
            where TServiceImplementation : class, TServiceContract
        {
            services.AddScoped<TServiceImplementation>();
            services.AddScoped<TServiceContract>(provider =>
            {
                beforeCreate?.Invoke(provider);
                return provider.GetRequiredService<TServiceImplementation>();
            });

            if (initProjections != null)
            {
                var appServiceBuilder = new DependencyInjectionAppDataServiceBuilder<TBaseEntity>(services);
                initProjections(appServiceBuilder);
            }

            return services;
        }

        public static IServiceCollection AddAppDataServiceWithMock<TBaseEntity, TServiceContract, TServiceImplementation, TServiceMockImplementation>(
            this IServiceCollection services, MockStrategy mockStrategy,
            Action<AppDataServiceBuilder<TBaseEntity>>? initProjections = null,
            Action<IServiceProvider>? beforeCreate = null)
            where TBaseEntity : class, IAppDataEntity
            where TServiceContract : class, IAppDataService<TBaseEntity>
            where TServiceImplementation : class, TServiceContract
            where TServiceMockImplementation : AppDataServiceMock<TBaseEntity>, TServiceContract
        {
            if (mockStrategy == MockStrategy.None)
            {
                services.AddAppDataService<TBaseEntity, TServiceContract, TServiceImplementation>(initProjections, beforeCreate);
            }
            else
            {
                services.AddSingleton<AppDataServiceMockOptions<TBaseEntity>>(_ => new AppDataServiceMockOptions<TBaseEntity>(mockStrategy));
                services.AddScoped<TServiceContract, TServiceMockImplementation>();

                throw new ArgumentOutOfRangeException(nameof(mockStrategy), mockStrategy, null);
            }
            return services;
        }
    }
}