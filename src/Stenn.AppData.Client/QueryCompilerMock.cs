using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Stenn.AppData.Client
{
    /// <summary>
    /// This class only needed to be passed in EntityQueryProvider, no methods should ever be called
    /// </summary>
    internal sealed class QueryCompilerMock : IQueryCompiler
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