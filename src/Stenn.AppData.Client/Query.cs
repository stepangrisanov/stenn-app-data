using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Stenn.AppData.Client
{
    public class Query<T> : IQueryable<T>, IEnumerable<T>, IQueryable, IEnumerable, IOrderedQueryable<T>, IOrderedQueryable//, IIncludableQueryable<T>
    {
        private readonly QueryProvider _provider;
        private readonly Expression _expression;

        public Query(IAppDataServiceClient client)
        {
            _provider = new QueryProvider(client);
            _expression = Expression.Constant(this);
        }

        public Query(QueryProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            _provider = provider;
            _expression = Expression.Constant(this);
        }

        public Query(QueryProvider provider, Expression expression)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            if (!typeof(IQueryable<T>).IsAssignableFrom(expression.Type))
            {
                throw new ArgumentOutOfRangeException("expression");
            }

            _provider = provider;
            _expression = expression;
        }

        Expression IQueryable.Expression { get { return _expression; } }

        Type IQueryable.ElementType { get { return typeof(T); } }

        IQueryProvider IQueryable.Provider { get { return _provider; } }

        public IEnumerator<T> GetEnumerator()
        {
            return (_provider.Execute<IEnumerable<T>>(_expression)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_provider.Execute(_expression)).GetEnumerator();
        }
    }
}
