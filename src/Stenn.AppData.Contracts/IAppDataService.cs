using System.Linq;

namespace Stenn.AppData.Contracts
{
    public interface IAppDataService<in TBaseEntity>
        where TBaseEntity : IAppDataEntity
    {
        IQueryable<T> Query<T>() where T : class, TBaseEntity;

        abstract byte[] ExecuteSerializedQuery(string bonsai);
    }
}