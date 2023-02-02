using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

#nullable disable

namespace Stenn.AppData.Client
{
    /// <summary>
    /// Need to inherit from EntityQueryProvider, as EF extension methdos (such as .Include()) make checks like that "source.Provider is EntityQueryProvider"
    /// </summary>
    public class QueryProvider : EntityQueryProvider, IQueryProvider
    {
        private readonly IAppDataServiceClient _client;

        public QueryProvider(IAppDataServiceClient client) : base(new QueryCompilerMock())
        {
            _client = client;
        }

        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            Type elementType = TypeSystem.GetElementType(expression.Type);

            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(Query<>).MakeGenericType(elementType), new object[] { this, expression });
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException!;
            }
        }

        public override IQueryable<T> CreateQuery<T>(Expression expression)
        {
            return new Query<T>(this, expression);
        }

        public override object Execute(Expression expression)
        {
            //TODO: проверить откуда это может вызываться. найти тип который возвращает expression и передать его вместо object?
            return Execute<object>(expression);
        }

        public override TResult Execute<TResult>(Expression expression)
        {
            return _client.Execute<TResult>(expression);
        }
    }

    /// <summary>
    /// This class only needed to be passed in EntityQueryProvider, no methods should ever be called
    /// </summary>
    internal class QueryCompilerMock : IQueryCompiler
    {
        public Func<QueryContext, TResult> CreateCompiledAsyncQuery<TResult>(Expression query)
        {
            throw new NotImplementedException();
        }

        public Func<QueryContext, TResult> CreateCompiledQuery<TResult>(Expression query)
        {
            throw new NotImplementedException();
        }

        public TResult Execute<TResult>(Expression query)
        {
            throw new NotImplementedException();
        }

        public TResult ExecuteAsync<TResult>(Expression query, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
