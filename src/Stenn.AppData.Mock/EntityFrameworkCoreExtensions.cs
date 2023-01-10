using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using Stenn.AppData.Contracts;

namespace Stenn.AppData.Mock
{
    public static class EntityFrameworkCoreExtensions
    {
        internal static DbContextOptionsBuilder UseMockServiceBuilder<TBaseEntity>(this DbContextOptionsBuilder optionsBuilder,
            Action<MockAppDataServiceBuilder<TBaseEntity>> entitiesInit)
            where TBaseEntity : class, IAppDataEntity
        {
            if (optionsBuilder == null)
            {
                throw new ArgumentNullException(nameof(optionsBuilder));
            }

            if (entitiesInit == null)
            {
                throw new ArgumentNullException(nameof(entitiesInit));
            }

            optionsBuilder.ReplaceService<IModelCustomizer, ModelCustomizer, AppDataMockModelCustomizer<TBaseEntity>>();

            var extension = optionsBuilder.Options.FindExtension<MockServiceBuilderOptionsExtension<TBaseEntity>>();
            if (extension != null)
            {
                throw new InvalidOperationException("Mock service builder is already registered");
            }

            extension = new MockServiceBuilderOptionsExtension<TBaseEntity>(
                builder => entitiesInit(new MockEntityValuesAppDataServiceBuilder<TBaseEntity>(builder)));
            
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

            return optionsBuilder;
        }
    }
}