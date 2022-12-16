using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Stenn.AppData.Contracts;

namespace Stenn.AppData.EntityFrameworkCore
{
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

            services.AddDbContext<TDbContext>(initDbContext);
            return services.AddAppDataService<TBaseEntity, TServiceContract, TServiceImplementation>(initProjections);
        }
    }
}
