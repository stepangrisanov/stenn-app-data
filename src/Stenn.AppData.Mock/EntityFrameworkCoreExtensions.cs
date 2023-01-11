using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using Stenn.AppData.Contracts;

namespace Stenn.AppData.Mock
{
    public static class EntityFrameworkCoreExtensions
    {
        internal static DbContextOptionsBuilder UseMockServiceBuilder<TBaseEntity>(this DbContextOptionsBuilder optionsBuilder)
            where TBaseEntity : class, IAppDataEntity
        {
            if (optionsBuilder == null)
            {
                throw new ArgumentNullException(nameof(optionsBuilder));
            }

            optionsBuilder.ReplaceService<IModelCustomizer, ModelCustomizer, AppDataMockModelCustomizer<TBaseEntity>>();
            return optionsBuilder;
        }
    }
}