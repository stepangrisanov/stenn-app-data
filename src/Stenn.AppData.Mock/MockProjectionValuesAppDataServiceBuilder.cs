using System.Collections.Generic;
using System.Reflection;

namespace Stenn.AppData.Mock
{
    public sealed class MockProjectionValuesAppDataServiceBuilder<TBaseEntity> : MockAppDataServiceBuilder<TBaseEntity>
        where TBaseEntity : class
    {
        private readonly Dictionary<TypeInfo, MockAppDataProjection<TBaseEntity>> _mockAppDataProjections;

        internal MockProjectionValuesAppDataServiceBuilder(Dictionary<TypeInfo, MockAppDataProjection<TBaseEntity>> mockAppDataProjections)
        {
            _mockAppDataProjections = mockAppDataProjections;
        }

        public override T Add<T>(T entity)
        {
            if (_mockAppDataProjections.TryGetValue(typeof(T).GetTypeInfo(), out var projection))
            {
                projection.Add(entity);
            }
            return entity;
        }
    }
}