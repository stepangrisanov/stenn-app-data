using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Linq.Expressions;
using System.Reflection;

namespace Stenn.AppData.Client
{
    /// <summary>
    /// Need to inherit from EntityQueryProvider, as EF extension methdos (such as .Include()) make checks like that "source.Provider is EntityQueryProvider"
    /// </summary>
    public class QueryProvider : EntityQueryProvider, IQueryProvider
    {
        private readonly IAppDataServiceClient _client;

        public QueryProvider(IAppDataServiceClient client) 
            : base(new QueryCompilerMock())
        {
            _client = client;
        }

        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            var elementType = TypeSystem.GetElementType(expression.Type);

            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(Query<>).MakeGenericType(elementType), this, expression)!;
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
}
