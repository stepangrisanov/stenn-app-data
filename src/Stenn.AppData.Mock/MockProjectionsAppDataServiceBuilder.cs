using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Stenn.AppData.Mock
{
    internal sealed class MockProjectionsAppDataServiceBuilder<TBaseEntity> : AppDataServiceBuilder<TBaseEntity> 
        where TBaseEntity : class
    {
        private readonly IServiceCollection _services;

        public MockProjectionsAppDataServiceBuilder(IServiceCollection services)
        {
            _services = services;
        }

        public override void AddProjection<T, TProjection>()
        {
            _services.TryAddEnumerable(ServiceDescriptor.Scoped<IAppDataProjection<TBaseEntity>, MockAppDataProjection<T, TBaseEntity>>());
        }
    }
}

