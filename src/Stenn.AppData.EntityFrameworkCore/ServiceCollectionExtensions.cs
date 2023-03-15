using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Stenn.AppData.Contracts;

namespace Stenn.AppData.EntityFrameworkCore
{
    [Obsolete]
    public static class AppDataServiceCollectionExtensions
    {
        /// <summary>
        /// Register public api data service with db context
        /// </summary>
        /// <param name="services"></param>
        /// <param name="initDbContext"></param>
        /// <param name="initProjections"></param>
        /// <returns></returns>
        public static IServiceCollection AddAppDataService<TBaseEntity, TServiceContract, TServiceImplementation, TDbContext>(
            this IServiceCollection services,
            Action<IServiceProvider, DbContextOptionsBuilder> initDbContext,
            Action<AppDataServiceBuilder<TBaseEntity>>? initProjections = null)
            where TBaseEntity : class, IAppDataEntity
            where TServiceContract : class, IAppDataService<TBaseEntity>
            where TServiceImplementation : class, TServiceContract
            where TDbContext : AppDataServiceDbContext<TBaseEntity>
        {
            if (initDbContext == null)
            {
                throw new ArgumentNullException(nameof(initDbContext));
            }

            services.AddDbContext<TDbContext>((p, b) => InitDbContext(p, b, initDbContext));
            return services.AddAppDataService<TBaseEntity, TServiceContract, TServiceImplementation>(initProjections);
        }

        private static void InitDbContext(IServiceProvider provider, DbContextOptionsBuilder builder,
            Action<IServiceProvider, DbContextOptionsBuilder>? initDbContext)
        {
            builder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            initDbContext?.Invoke(provider, builder);
        }
    }
}