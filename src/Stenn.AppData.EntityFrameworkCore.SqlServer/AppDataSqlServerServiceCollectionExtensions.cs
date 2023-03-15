using System;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Stenn.AppData.Contracts;

namespace Stenn.AppData.EntityFrameworkCore.SqlServer
{
    [Obsolete]
    public static class AppDataSqlServerServiceCollectionExtensions
    {
        /// <summary>
        /// Register public api data service with db context
        /// </summary>
        /// <param name="services"></param>
        /// <param name="connection"></param>
        /// <param name="initSqlServer"></param>
        /// <param name="initProjections"></param>
        /// <param name="initAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddAppDataServiceSqlServer<TBaseEntity, TServiceContract, TServiceImplementation, TDbContext>(
            this IServiceCollection services,
            DbConnection connection,
            Action<AppDataServiceBuilder<TBaseEntity>>? initProjections = null,
            Action<IServiceProvider, DbContextOptionsBuilder>? initAction = null,
            Action<SqlServerDbContextOptionsBuilder>? initSqlServer = null)
            where TBaseEntity : class, IAppDataEntity
            where TServiceContract : class, IAppDataService<TBaseEntity>
            where TServiceImplementation : class, TServiceContract
            where TDbContext : AppDataServiceDbContext<TBaseEntity>
        {
            return AddAppDataServiceSqlServer<TBaseEntity, TServiceContract, TServiceImplementation, TDbContext>(services,
                _ => connection, initProjections,
                initAction,
                initSqlServer);
        }

        /// <summary>
        /// Register public api data service with db context
        /// </summary>
        /// <param name="services"></param>
        /// <param name="connectionFunc"></param>
        /// <param name="initSqlServer"></param>
        /// <param name="initProjections"></param>
        /// <param name="initAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddAppDataServiceSqlServer<TBaseEntity, TServiceContract, TServiceImplementation, TDbContext>(
            this IServiceCollection services,
            Func<IServiceProvider, DbConnection> connectionFunc,
            Action<AppDataServiceBuilder<TBaseEntity>>? initProjections = null,
            Action<IServiceProvider, DbContextOptionsBuilder>? initAction = null,
            Action<SqlServerDbContextOptionsBuilder>? initSqlServer = null)
            where TBaseEntity : class, IAppDataEntity
            where TServiceContract : class, IAppDataService<TBaseEntity>
            where TServiceImplementation : class, TServiceContract
            where TDbContext : AppDataServiceDbContext<TBaseEntity>
        {
            return services.AddAppDataService<TBaseEntity, TServiceContract, TServiceImplementation, TDbContext>((provider, builder) =>
            {
                var connection = connectionFunc(provider);
                builder.UseSqlServer(connection, initSqlServer);
                initAction?.Invoke(provider, builder);
            }, initProjections);
        }

        /// <summary>
        /// Register public api data service with db context
        /// </summary>
        /// <param name="services"></param>
        /// <param name="connectionString"></param>
        /// <param name="initSqlServer"></param>
        /// <param name="initProjections"></param>
        /// <param name="initAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddAppDataServiceSqlServer<TBaseEntity, TServiceContract, TServiceImplementation, TDbContext>(
            this IServiceCollection services,
            string connectionString,
            Action<AppDataServiceBuilder<TBaseEntity>>? initProjections = null,
            Action<IServiceProvider, DbContextOptionsBuilder>? initAction = null,
            Action<SqlServerDbContextOptionsBuilder>? initSqlServer = null)
            where TBaseEntity : class, IAppDataEntity
            where TServiceContract : class, IAppDataService<TBaseEntity>
            where TServiceImplementation : class, TServiceContract
            where TDbContext : AppDataServiceDbContext<TBaseEntity>
        {
            return AddAppDataServiceSqlServer<TBaseEntity, TServiceContract, TServiceImplementation, TDbContext>(services, _ => connectionString,
                initProjections, initAction,
                initSqlServer);
        }

        /// <summary>
        /// Register public api data service with db context
        /// </summary>
        /// <param name="services"></param>
        /// <param name="connectionStringFunc"></param>
        /// <param name="initSqlServer"></param>
        /// <param name="initProjections"></param>
        /// <param name="initAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddAppDataServiceSqlServer<TBaseEntity, TServiceContract, TServiceImplementation, TDbContext>(
            this IServiceCollection services,
            Func<IServiceProvider, string> connectionStringFunc,
            Action<AppDataServiceBuilder<TBaseEntity>>? initProjections = null,
            Action<IServiceProvider, DbContextOptionsBuilder>? initAction = null,
            Action<SqlServerDbContextOptionsBuilder>? initSqlServer = null)
            where TBaseEntity : class, IAppDataEntity
            where TServiceContract : class, IAppDataService<TBaseEntity>
            where TServiceImplementation : class, TServiceContract
            where TDbContext : AppDataServiceDbContext<TBaseEntity>
        {
            return services.AddAppDataService<TBaseEntity, TServiceContract, TServiceImplementation, TDbContext>((provider, builder) =>
            {
                var connectionString = connectionStringFunc(provider);
                builder.UseSqlServer(connectionString, initSqlServer);
                initAction?.Invoke(provider, builder);
            }, initProjections);
        }
    }
}