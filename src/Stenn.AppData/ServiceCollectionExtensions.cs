using System;
using Microsoft.Extensions.DependencyInjection;
using Stenn.AppData.Contracts;

namespace Stenn.AppData
{
    public static class AppDataServiceCollectionExtensions
    {
        public static IServiceCollection AddAppDataService<TBaseEntity, TServiceContract, TServiceImplementation>(IServiceCollection services,
            Action<AppDataServiceBuilder<TBaseEntity>>? initProjections = null)
            where TBaseEntity : class, IAppDataEntity
            where TServiceContract : class, IAppDataService<TBaseEntity>
            where TServiceImplementation : class, TServiceContract
        {
            services.AddScoped<TServiceContract, TServiceImplementation>();
            if (initProjections != null)
            {
                var appServiceBuilder = new AppDataServiceBuilder<TBaseEntity>(services);
                initProjections(appServiceBuilder);
            }

            return services;
        }
    }
}
