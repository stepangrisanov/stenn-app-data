using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Stenn.AppData.Mock
{
    internal sealed class MockProjectionsAppDataServiceBuilder<TBaseEntity> : AppDataServiceBuilder<TBaseEntity> 
        where TBaseEntity : class
    {
        private readonly IServiceCollection _services;
        public Dictionary<TypeInfo, MockAppDataProjection<TBaseEntity>> MockProjections { get; } = new();

        public MockProjectionsAppDataServiceBuilder(IServiceCollection services)
        {
            _services = services;
        }

        public override void AddProjection<T, TProjection>()
        {
            var projection = new MockAppDataProjection<T, TBaseEntity>();
            MockProjections.Add(typeof(T).GetTypeInfo(), projection);
            _services.TryAddEnumerable(ServiceDescriptor.Scoped<IAppDataProjection<TBaseEntity>, MockAppDataProjection<T, TBaseEntity>>(_ => projection));
        }
    }
}

