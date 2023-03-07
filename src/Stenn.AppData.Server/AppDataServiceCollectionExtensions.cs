using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Stenn.AppData.Contracts;
using Stenn.AppData.Expressions;

namespace Stenn.AppData.Server
{
    public static class AppDataServerServiceCollectionExtensions
    {
        /// <summary>
        /// Add <see cref="IAppDataServiceServer{TBaseEntity}"/> for specific AppData service
        /// </summary>
        /// <param name="services"></param>
        /// <param name="expressionValidationFunc"></param>
        /// <typeparam name="TBaseEntity"></typeparam>
        /// <typeparam name="TServiceContract"></typeparam>
        /// <returns></returns>
        public static IServiceCollection AddAppDataServiceServer<TBaseEntity, TServiceContract>(this IServiceCollection services,
            Func<MethodInfo, bool>? expressionValidationFunc = null)
            where TBaseEntity : class, IAppDataEntity
            where TServiceContract : class, IAppDataService<TBaseEntity>
        {
            services.AddAppDataServiceExpressionValidator<TBaseEntity>(expressionValidationFunc);
            services.AddScoped<IAppDataServiceServer<TBaseEntity>, AppDataServiceServer<TBaseEntity, TServiceContract>>();

            services.AddScoped<IAppDataSerializer, TextJsonAppDataSerializer>();
            services.AddScoped<IAppDataSerializerFactory, AppDataSerializerFactory>();

            return services;
        }
    }
}