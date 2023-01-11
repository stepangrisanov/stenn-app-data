using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Stenn.AppData.Contracts;

namespace Stenn.AppData.Mock
{
#if NET6_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicConstructors)]
#endif
    internal sealed class AppDataMockModelCustomizer<TBaseEntity> : ModelCustomizer
        where TBaseEntity : class, IAppDataEntity
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

            var storage = context.GetService<MockDataStorage<TBaseEntity>>();
            
            foreach (var data in storage.Dictionary.Where(kp => modelBuilder.Model.FindEntityType(kp.Key) is not null))
            {
                modelBuilder.Entity(data.Key).HasData(data.Value);
            }
        }
    }
}