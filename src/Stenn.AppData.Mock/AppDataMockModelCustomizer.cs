using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Stenn.AppData.Contracts;

namespace Stenn.AppData.Mock
{
#if NET6_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicConstructors)]
#endif
    internal sealed class AppDataMockModelCustomizer<TBaseEntity> : ModelCustomizer
        where TBaseEntity : IAppDataEntity
    {

        /// <inheritdoc />
        public AppDataMockModelCustomizer(ModelCustomizerDependencies dependencies)
            : base(dependencies)
        {
        }

        /// <inheritdoc />
        public override void Customize(ModelBuilder modelBuilder, DbContext context)
        {
            base.Customize(modelBuilder, context);
            var dbContextOptions = context.GetService<IDbContextOptions>();

            var initializer = dbContextOptions.FindExtension<MockServiceBuilderOptionsExtension<TBaseEntity>>()?.Action ??
                           throw new ApplicationException("Can't find EF extension: MockServiceBuilderOptionsExtension");

            initializer(modelBuilder);
        }
    }
}