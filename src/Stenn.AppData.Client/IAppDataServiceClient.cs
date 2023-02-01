using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Stenn.AppData.Client
{
    public interface IAppDataServiceClient
    {
        internal T? Deserialize<T>(byte[] bytes);

        internal byte[]? ExecuteRemote(string serializedExpression);

        internal TResult Execute<TResult>(Expression expression);
    }
}
