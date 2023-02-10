using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Stenn.AppData.Contracts;

namespace Stenn.AppData
{
    public static class AppDataServiceCollectionExtensions
    {
        public static IServiceCollection AddAppDataService<TBaseEntity, TServiceContract, TServiceImplementation>(this IServiceCollection services,
            Action<AppDataServiceBuilder<TBaseEntity>>? initProjections = null,
            Action<IServiceProvider>? beforeCreate = null,
            Func<MethodInfo, bool>? expressionValidationFunc = null)
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

            if (expressionValidationFunc != null)
            {
                services.AddSingleton(new ExpressionValidationOptions<TBaseEntity> { validationFunc = expressionValidationFunc });
            }

            return services;
        }
    }
}
