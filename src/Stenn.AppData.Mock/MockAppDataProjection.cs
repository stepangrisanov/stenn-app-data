using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Stenn.EntityFrameworkCore;

namespace Stenn.AppData.Mock
{
    internal sealed class MockAppDataProjection<T, TBaseEntity> : IAppDataProjection<T, TBaseEntity>
        where T : class, TBaseEntity
        where TBaseEntity : class
    {
        private readonly List<T> _list;

        public MockAppDataProjection(MockDataStorage<TBaseEntity> dataStorage)
        {
            if (dataStorage.Dictionary.TryGetValue(typeof(T).GetTypeInfo(), out var list))
            {
                _list = list.Cast<T>().ToList();
            }
            _list ??= new List<T>();
        }

        /// <inheritdoc />
        public IQueryable<T> Query()
        {
            return _list.AsQueryableFixed();
        }
    }
}