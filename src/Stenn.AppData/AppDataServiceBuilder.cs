using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Stenn.AppData
{
    public sealed class AppDataServiceBuilder<TBaseEntity>
    {
        private readonly IServiceCollection _services;

        public AppDataServiceBuilder(IServiceCollection services)
        {
            _services = services;
        }

        public void AddProjection<T>()
            where T : class, IAppDataProjection<TBaseEntity>
        {
            _services.TryAddEnumerable(ServiceDescriptor.Scoped<IAppDataProjection<TBaseEntity>, T>());
        }
    }
}