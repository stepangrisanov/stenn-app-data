using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Stenn.AppData.Contracts;
using Stenn.AppData.Expressions;

namespace Stenn.AppData.Client
{
    public static class AppDataClientServiceCollectionExtensions
    {
        /// <summary>
        /// Register public api data service client as <see cref="IAppDataService{TBaseEntity}"/>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="initClient">Init <see cref="HttpClient"/> method for <see cref="HttpClientFactoryServiceCollectionExtensions.AddHttpClient{TClient, TImplementation}(IServiceCollection, Action{IServiceProvider, HttpClient})"/>.
        /// If it setted to null then you must explicit register <see cref="HttpClient"/> via AddHttpClient for type parameters <see cref="IAppDataServiceClient{TBaseEntity}"/> and  <see cref="AppDataServiceClient{TBaseEntity}"/>.
        /// Remarks: <see cref="HttpClient.BaseAddress"/> must contains full path to Query endpoint</param>
        /// <param name="expressionValidationFunc"></param>
        /// <returns></returns>
        public static IServiceCollection AddAppDataServiceClient<TBaseEntity>(this IServiceCollection services,
            Action<IServiceProvider, HttpClient>? initClient, Func<MethodInfo, bool>? expressionValidationFunc = null)
            where TBaseEntity : class, IAppDataEntity
        {
            services.AddAppDataServiceExpressionValidator<TBaseEntity>(expressionValidationFunc);

            services.AddScoped<IAppDataServiceClient<TBaseEntity>, AppDataServiceClient<TBaseEntity>>();
            services.AddScoped<IAppDataService<TBaseEntity>, AppDataServiceClientWrapper<TBaseEntity>>();

            services.AddScoped<IAppDataSerializer, TextJsonAppDataSerializer>();
            services.AddScoped<IAppDataSerializerFactory, AppDataSerializerFactory>();

            if (initClient != null)
            {
                services.AddHttpClient<IAppDataServiceClient<TBaseEntity>, AppDataServiceClient<TBaseEntity>>(initClient);
            }

            return services;
        }
    }
}