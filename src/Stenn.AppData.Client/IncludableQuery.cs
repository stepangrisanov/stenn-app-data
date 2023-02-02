using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace Stenn.AppData.Client
{
    internal sealed class IncludableQuery<TEntity, TProperty> : IIncludableQueryable<TEntity, TProperty>, IAsyncEnumerable<TEntity>
    {
        private readonly IQueryable<TEntity> _queryable;

        public IncludableQuery(IQueryable<TEntity> queryable)
        {
            _queryable = queryable;
        }

        public Expression Expression
            => _queryable.Expression;

        public Type ElementType
            => _queryable.ElementType;

        public IQueryProvider Provider
            => _queryable.Provider;

        public IAsyncEnumerator<TEntity> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => ((IAsyncEnumerable<TEntity>)_queryable).GetAsyncEnumerator(cancellationToken);

        public IEnumerator<TEntity> GetEnumerator()
            => _queryable.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
