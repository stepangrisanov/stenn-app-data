using System;
using Microsoft.Extensions.DependencyInjection;
using Stenn.AppData.Contracts;

namespace Stenn.AppData
{
    [Obsolete]
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
    }
}
