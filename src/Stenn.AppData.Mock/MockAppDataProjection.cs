using System.Collections.Generic;
using System.Linq;
using Stenn.EntityFrameworkCore;

namespace Stenn.AppData.Mock
{
    internal sealed class MockAppDataProjection<T, TBaseEntity> : MockAppDataProjection<TBaseEntity>, IAppDataProjection<T, TBaseEntity>
        where T : class, TBaseEntity
        where TBaseEntity : class
    {
        private readonly List<T> _list = new();

        /// <inheritdoc />
        public IQueryable<T> Query()
        {
            return _list.AsQueryableFixed();
        }

        /// <inheritdoc />
        public override void Add(TBaseEntity entity)
        {
            _list.Add((T)entity);
        }
    }

    internal abstract class MockAppDataProjection<TBaseEntity>
        where TBaseEntity : class
    {
        public abstract void Add(TBaseEntity entity);
    }
}