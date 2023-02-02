using System.Linq.Expressions;

namespace Stenn.AppData.Client
{
    public interface IAppDataServiceClient
    {
        internal T? Deserialize<T>(byte[] bytes);

        internal byte[]? ExecuteRemote(string serializedExpression);

        internal TResult Execute<TResult>(Expression expression);
    }
}
