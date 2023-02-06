using System;
using System.Linq;
using Stenn.AppData.Contracts;
using Stenn.EntityFrameworkCore;

namespace Stenn.AppData
{
    public abstract class AppDataServiceMock<TBaseEntity> : IAppDataService<TBaseEntity>
        where TBaseEntity : class, IAppDataEntity
    {
        protected AppDataServiceMock(AppDataServiceMockOptions<TBaseEntity> options)
        {
            Strategy = options.Strategy;
        }

        protected MockStrategy Strategy { get; }


        /// <inheritdoc />
        public IQueryable<T> Query<T>()
            where T : class, TBaseEntity
        {
            return Strategy switch
            {
                MockStrategy.Empty => EmptyQuery<T>(),
                MockStrategy.NotImplemented => NotImplementedQuery<T>(),
                MockStrategy.Custom => CustomQuery<T>(),
                MockStrategy.None => throw new ArgumentOutOfRangeException(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        /// <summary>
        /// <see cref="MockStrategy.Empty"/> mock strategy implemetation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected virtual IQueryable<T> EmptyQuery<T>()
            where T : class, TBaseEntity
        {
            return Array.Empty<T>().AsQueryableFixed();
        }

        /// <summary>
        /// <see cref="MockStrategy.NotImplemented"/> mock strategy implemetation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected virtual IQueryable<T> NotImplementedQuery<T>()
            where T : class, TBaseEntity
        {
            throw new NotImplementedException("Not implemented app data service mock call");
        }

        /// <summary>
        /// <see cref="MockStrategy.Custom"/> mock strategy implemetation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected virtual IQueryable<T> CustomQuery<T>()
            where T : class, TBaseEntity
        {
            throw new NotImplementedException("Custom mock strategy doesn't implemented");
        }
    }
}