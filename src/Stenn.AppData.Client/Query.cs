using System.Collections;
using System.Linq.Expressions;
using Stenn.EntityFrameworkCore;

namespace Stenn.AppData.Client
{
    public class Query<T> : IOrderedQueryable<T>, IAsyncEnumerable<T>
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
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _expression = Expression.Constant(this);
        }

        public Query(QueryProvider provider, Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            if (!typeof(IQueryable<T>).IsAssignableFrom(expression.Type))
            {
                throw new ArgumentOutOfRangeException(nameof(expression));
            }

            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _expression = expression;
        }

        Expression IQueryable.Expression => _expression;

        Type IQueryable.ElementType => typeof(T);

        IQueryProvider IQueryable.Provider => _provider;

        public IEnumerator<T> GetEnumerator()
        {
            return Execute().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc />
        IAsyncEnumerator<T> IAsyncEnumerable<T>.GetAsyncEnumerator(CancellationToken cancellationToken)
        {
            return ((IAsyncEnumerable<T>)Execute().AsQueryableFixed()).GetAsyncEnumerator(cancellationToken);
        }

        private IEnumerable<T> Execute()
        {
            return _provider.Execute<IEnumerable<T>>(_expression);
        }
    }
}