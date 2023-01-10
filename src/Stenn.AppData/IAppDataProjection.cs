using System;
using System.Linq;

namespace Stenn.AppData
{
    /// <summary>
    /// Data projection of T entity
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TBaseEntity"></typeparam>
    public interface IAppDataProjection<out T, out TBaseEntity> : IAppDataProjection<TBaseEntity>
        where T : class, TBaseEntity
        where TBaseEntity : class
    {
        /// <inheritdoc />
        Type IAppDataProjection<TBaseEntity>.GetEntityType()
        {
            return GetEntityType();
        }

        new Type GetEntityType()
        {
            return typeof(T);
        }

        /// <inheritdoc />
        IQueryable<TBaseEntity> IAppDataProjection<TBaseEntity>.Query()
        {
            return Query();
        }

        new IQueryable<T> Query();
    }

    public interface IAppDataProjection<out TBaseEntity>
        where TBaseEntity : class
    {
        Type GetEntityType();

        IQueryable<TBaseEntity> Query();
    }
}