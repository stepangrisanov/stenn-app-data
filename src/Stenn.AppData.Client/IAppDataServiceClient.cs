using System.Linq.Expressions;
using Stenn.AppData.Contracts;

namespace Stenn.AppData.Client
{
    public interface IAppDataServiceClient<in TBaseEntity> : IAppDataServiceClient
        where TBaseEntity : IAppDataEntity
    {
    }

    public interface IAppDataServiceClient
    {
        internal TResult Execute<TResult>(Expression expression);
    }
}