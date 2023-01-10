using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Stenn.AppData
{
    internal sealed class DependencyInjectionAppDataServiceBuilder<TBaseEntity>: AppDataServiceBuilder<TBaseEntity> 
        where TBaseEntity : class
    {
        private readonly IServiceCollection _services;

        public DependencyInjectionAppDataServiceBuilder(IServiceCollection services)
        {
            _services = services;
        }

        public override void AddProjection<T, TProjection>()
        {
            _services.TryAddEnumerable(ServiceDescriptor.Scoped<IAppDataProjection<TBaseEntity>, TProjection>());
        }
    }
}